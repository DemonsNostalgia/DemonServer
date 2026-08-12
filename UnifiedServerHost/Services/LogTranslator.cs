using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Eudemons.UnifiedServer.Services;

public static class LogTranslator
{
    private static readonly Lazy<IReadOnlyList<KeyValuePair<string, string>>> Catalog =
        new(LoadCatalog);

    public static string Translate(string message)
    {
        var translated = message;
        foreach (var entry in Catalog.Value)
        {
            translated = translated.Replace(
                entry.Key,
                entry.Value,
                StringComparison.Ordinal);
        }

        return translated;
    }

    private static IReadOnlyList<KeyValuePair<string, string>> LoadCatalog()
    {
        try
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(
                    "Eudemons.UnifiedServer.diagnostic-log-translations.json");
            if (stream is null)
            {
                return [];
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var catalog =
                JsonSerializer.Deserialize<Dictionary<string, string>>(json) ??
                new Dictionary<string, string>();
            return catalog
                .OrderByDescending(entry => entry.Key.Length)
                .ToArray();
        }
        catch
        {
            return [];
        }
    }
}
