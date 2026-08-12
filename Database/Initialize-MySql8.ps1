param(
    [string]$ConfigPath = (Join-Path $PSScriptRoot '..\Runtime\GlobalConfig.ini'),
    [string]$MysqlPath = '',
    [string]$SourceDump = '',
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

function Get-IniSection {
    param(
        [string]$Path,
        [string]$Section
    )

    $values = @{}
    $insideSection = $false

    foreach ($line in Get-Content -LiteralPath $Path) {
        $trimmed = $line.Trim()
        if ($trimmed -match '^\[(.+)\]$') {
            $insideSection = $Matches[1] -eq $Section
            continue
        }

        if ($insideSection -and $trimmed -match '^([^=]+)=(.*)$') {
            $values[$Matches[1].Trim()] = $Matches[2].Trim()
        }
    }

    return $values
}

function Invoke-MySql {
    param(
        [string]$Sql
    )

    $arguments = @(
        '--batch',
        '--skip-column-names',
        '--default-character-set=utf8mb4',
        '--protocol=tcp',
        "--host=$($mysqlConfig.IP)",
        "--port=$($mysqlConfig.Port)",
        "--user=$($mysqlConfig.User)"
    )

    $startInfo = New-Object Diagnostics.ProcessStartInfo
    $startInfo.FileName = $MysqlPath
    $startInfo.Arguments = $arguments -join ' '
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardInput = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.StandardOutputEncoding = [Text.Encoding]::UTF8
    $startInfo.StandardErrorEncoding = [Text.Encoding]::UTF8
    $startInfo.EnvironmentVariables['MYSQL_PWD'] = $mysqlConfig.Passwd

    $process = New-Object Diagnostics.Process
    $process.StartInfo = $startInfo
    $null = $process.Start()
    $writeError = $null
    try {
        $inputBytes = [Text.Encoding]::UTF8.GetBytes($Sql)
        $process.StandardInput.BaseStream.Write($inputBytes, 0, $inputBytes.Length)
        $process.StandardInput.BaseStream.Flush()
    }
    catch {
        $writeError = $_.Exception.Message
    }
    finally {
        $process.StandardInput.Close()
    }
    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $process.WaitForExit()

    if ($process.ExitCode -ne 0) {
        $details = $stderr.Trim()
        if ($writeError) {
            $details = "$details Input error: $writeError"
        }
        throw "MySQL exited with code $($process.ExitCode): $details"
    }

    return $stdout.Trim()
}

if ([string]::IsNullOrWhiteSpace($MysqlPath)) {
    $mysqlCommand = Get-Command mysql.exe -ErrorAction SilentlyContinue
    if ($null -ne $mysqlCommand) {
        $MysqlPath = $mysqlCommand.Source
    }
    else {
        $programFiles = [Environment]::GetFolderPath('ProgramFiles')
        $standardMysqlPath = Join-Path $programFiles 'MySQL\MySQL Server 8.0\bin\mysql.exe'
        if (Test-Path -LiteralPath $standardMysqlPath -PathType Leaf) {
            $MysqlPath = $standardMysqlPath
        }
    }
}

if ([string]::IsNullOrWhiteSpace($MysqlPath) -or
    -not (Test-Path -LiteralPath $MysqlPath -PathType Leaf)) {
    throw 'MySQL 8 client not found. Add mysql.exe to PATH or pass -MysqlPath explicitly.'
}

$mysqlConfig = Get-IniSection -Path $ConfigPath -Section 'Mysql'
$requiredKeys = @('IP', 'Port', 'User', 'Passwd', 'database')
foreach ($key in $requiredKeys) {
    if (-not $mysqlConfig.ContainsKey($key) -or [string]::IsNullOrWhiteSpace($mysqlConfig[$key])) {
        throw "Missing Mysql.$key in $ConfigPath"
    }
}

if ($mysqlConfig.database -notmatch '^[A-Za-z0-9_]+$') {
    throw "Invalid database name in runtime configuration: $($mysqlConfig.database)"
}

$version = Invoke-MySql -Sql 'SELECT VERSION();'
if ($version -notmatch '^8\.') {
    throw "This initializer requires MySQL 8. Connected server reports: $version"
}

$escapedDatabase = $mysqlConfig.database.Replace("'", "''")
$tableCount = [int](Invoke-MySql -Sql "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='$escapedDatabase';")
if ($tableCount -gt 0 -and -not $Force) {
    throw "Database '$($mysqlConfig.database)' already has $tableCount tables. Re-run with -Force only if replacing those tables is intentional."
}

$schemaPath = Join-Path $PSScriptRoot 'MySQL8\soul.mysql8.sql'
if (-not [string]::IsNullOrWhiteSpace($SourceDump)) {
    & (Join-Path $PSScriptRoot 'Convert-SoulSchemaForMySql8.ps1') `
        -SourceDump $SourceDump `
        -OutputPath $schemaPath `
        -Database $mysqlConfig.database
}
if (-not (Test-Path -LiteralPath $schemaPath -PathType Leaf)) {
    throw "Packaged MySQL 8 schema not found: $schemaPath"
}

$schemaSql = [IO.File]::ReadAllText($schemaPath, [Text.Encoding]::UTF8)
$databaseHeader = @"
CREATE DATABASE IF NOT EXISTS ``$($mysqlConfig.database)``
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;
USE ``$($mysqlConfig.database)``;
"@
$null = Invoke-MySql -Sql ($databaseHeader + $schemaSql)

if (-not [string]::IsNullOrWhiteSpace($SourceDump)) {
    $migrationNames = @(
        'family.mysql8.migration.sql',
        'legion.mysql8.migration.sql',
        'friend.mysql8.migration.sql'
    )
    foreach ($migrationName in $migrationNames) {
        $migrationPath = Join-Path $PSScriptRoot "MySQL8\$migrationName"
        $migrationSql = [IO.File]::ReadAllText(
            $migrationPath,
            [Text.Encoding]::UTF8)
        $null = Invoke-MySql -Sql ($databaseHeader + $migrationSql)
    }
}

$validation = Invoke-MySql -Sql @"
SELECT CONCAT('version=', VERSION());
SELECT CONCAT('database=', '$escapedDatabase');
SELECT CONCAT('tables=', COUNT(*))
FROM information_schema.tables
WHERE table_schema='$escapedDatabase';
SELECT CONCAT(table_name, '=', table_rows)
FROM information_schema.tables
WHERE table_schema='$escapedDatabase'
ORDER BY table_name;
"@

Write-Output $validation
