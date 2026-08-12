using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Eudemons.UnifiedServer.Services;

public static class DatabaseBootstrapper
{
    private static readonly Regex SafeDatabaseName = new(
        "^[A-Za-z0-9_]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static DatabaseBootstrapResult EnsureReady(string runtimeRoot)
    {
        var configPath = Path.Combine(runtimeRoot, "GlobalConfig.ini");
        var config = ReadIniSection(configPath, "Mysql");
        var databaseName = Require(config, "database", configPath);
        if (!SafeDatabaseName.IsMatch(databaseName))
        {
            throw new InvalidDataException(
                $"Mysql.database contains unsupported characters: {databaseName}");
        }

        var builder = new MySqlConnectionStringBuilder
        {
            Server = Require(config, "IP", configPath),
            Port = uint.Parse(
                Require(config, "Port", configPath),
                NumberStyles.None,
                CultureInfo.InvariantCulture),
            UserID = Require(config, "User", configPath),
            Password = Require(config, "Passwd", configPath),
            CharacterSet = "utf8mb4",
            SslMode = MySqlSslMode.Disabled,
            ConnectionTimeout = 5,
            DefaultCommandTimeout = 180,
            AllowUserVariables = true
        };

        using var connection = new MySqlConnection(builder.ConnectionString);
        connection.Open();

        var version = ReadScalarString(connection, "SELECT VERSION();");
        if (!version.StartsWith("8.", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"MySQL 8 is required. The connected server reports {version}.");
        }

        var existingTableCount = CountTables(connection, databaseName);
        if (existingTableCount > 0)
        {
            ValidateRequiredTables(connection, databaseName);
            connection.ChangeDatabase(databaseName);
			EnsureWardrobeSchema(connection);
            return new DatabaseBootstrapResult(
                databaseName,
                existingTableCount,
                false,
                $"MySQL 8 database '{databaseName}' is ready ({existingTableCount} tables).");
        }

        var snapshotPath = FindSnapshot(runtimeRoot);
        var sql = File.ReadAllText(snapshotPath, new UTF8Encoding(false, true));
        using (var createDatabase = connection.CreateCommand())
        {
            createDatabase.CommandText =
                $"CREATE DATABASE IF NOT EXISTS `{databaseName}` " +
                "CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;";
            createDatabase.ExecuteNonQuery();
        }
        connection.ChangeDatabase(databaseName);

        var script = new MySqlScript(connection, sql)
        {
            Delimiter = ";"
        };
        script.Execute();

        var importedTableCount = CountTables(connection, databaseName);
        if (importedTableCount == 0)
        {
            throw new InvalidDataException(
                $"The bundled snapshot did not create the configured database '{databaseName}'.");
        }

        ValidateRequiredTables(connection, databaseName);
		EnsureWardrobeSchema(connection);
        return new DatabaseBootstrapResult(
            databaseName,
            importedTableCount,
            true,
            $"Initialized MySQL 8 database '{databaseName}' from the bundled snapshot ({importedTableCount} tables).");
    }

    private static Dictionary<string, string> ReadIniSection(
        string path,
        string sectionName)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Server configuration was not found: {path}",
                path);
        }

        var values = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);
        var insideSection = false;
        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                insideSection = string.Equals(
                    line[1..^1].Trim(),
                    sectionName,
                    StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!insideSection)
            {
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            values[line[..separator].Trim()] = line[(separator + 1)..].Trim();
        }

        return values;
    }

    private static string Require(
        IReadOnlyDictionary<string, string> values,
        string key,
        string configPath)
    {
        if (!values.TryGetValue(key, out var value) ||
            string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException(
                $"Missing Mysql.{key} in {configPath}.");
        }

        return value;
    }

    private static string FindSnapshot(string runtimeRoot)
    {
        var packageRoot = Directory.GetParent(runtimeRoot)?.FullName;
        var candidates = new[]
        {
            packageRoot is null
                ? null
                : Path.Combine(packageRoot, "Database", "soul.mysql8.sql"),
            packageRoot is null
                ? null
                : Path.Combine(packageRoot, "Database", "MySQL8", "soul.mysql8.sql"),
            Path.Combine(AppContext.BaseDirectory, "Database", "soul.mysql8.sql"),
            Path.Combine(
                AppContext.BaseDirectory,
                "Database",
                "MySQL8",
                "soul.mysql8.sql")
        };

        var snapshot = candidates
            .Where(path => path is not null)
            .FirstOrDefault(File.Exists);
        if (snapshot is null)
        {
            throw new FileNotFoundException(
                "The bundled MySQL 8 snapshot was not found under the Database directory.");
        }

        return snapshot;
    }

    private static int CountTables(MySqlConnection connection, string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM information_schema.tables " +
            "WHERE table_schema = @database;";
        command.Parameters.AddWithValue("@database", databaseName);
        return Convert.ToInt32(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    private static void ValidateRequiredTables(
        MySqlConnection connection,
        string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM information_schema.tables " +
            "WHERE table_schema = @database " +
            "AND table_name IN ('account', 'cq_user');";
        command.Parameters.AddWithValue("@database", databaseName);
        var requiredTableCount = Convert.ToInt32(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
        if (requiredTableCount != 2)
        {
            throw new InvalidDataException(
                $"Database '{databaseName}' is incomplete. Required account tables are missing.");
        }
    }

    private static void EnsureWardrobeSchema(MySqlConnection connection)
    {
        using (var columnQuery = connection.CreateCommand())
        {
            columnQuery.CommandText =
                "SELECT COUNT(*) FROM information_schema.columns " +
                "WHERE table_schema = DATABASE() " +
                "AND table_name = 'cq_user' " +
                "AND column_name = 'wardrobe_hairs';";
            var columnCount = Convert.ToInt32(
                columnQuery.ExecuteScalar(),
                CultureInfo.InvariantCulture);
            if (columnCount == 0)
            {
                using var alter = connection.CreateCommand();
                alter.CommandText =
                    "ALTER TABLE `cq_user` ADD COLUMN `wardrobe_hairs` " +
                    "varchar(2048) NOT NULL DEFAULT '' AFTER `maxeudemon`;";
                alter.ExecuteNonQuery();
            }
        }

        using var seed = connection.CreateCommand();
        seed.CommandText =
            "UPDATE `cq_user` SET `wardrobe_hairs` = CAST(`hair` AS CHAR) " +
            "WHERE `hair` > 0 AND `wardrobe_hairs` = '';";
        seed.ExecuteNonQuery();

        using (var columnQuery = connection.CreateCommand())
        {
            columnQuery.CommandText =
                "SELECT COUNT(*) FROM information_schema.columns " +
                "WHERE table_schema = DATABASE() " +
                "AND table_name = 'cq_user' " +
                "AND column_name = 'wardrobe_avatars';";
            var columnCount = Convert.ToInt32(
                columnQuery.ExecuteScalar(),
                CultureInfo.InvariantCulture);
            if (columnCount == 0)
            {
                using var alter = connection.CreateCommand();
                alter.CommandText =
                    "ALTER TABLE `cq_user` ADD COLUMN `wardrobe_avatars` " +
                    "varchar(2048) NOT NULL DEFAULT '' AFTER `wardrobe_hairs`;";
                alter.ExecuteNonQuery();
            }
        }
    }

	private static string ReadScalarString(
        MySqlConnection connection,
        string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToString(
                   command.ExecuteScalar(),
                   CultureInfo.InvariantCulture) ??
               string.Empty;
    }
}

public sealed record DatabaseBootstrapResult(
    string DatabaseName,
    int TableCount,
    bool Imported,
    string Message);
