namespace Benchmarks;

internal sealed record BenchmarkScenario(string Key, string ScriptName, string Content);

internal static class BenchmarkScenarioCatalog
{
    public static IReadOnlyList<BenchmarkScenario> LoadScenarios(string scenariosDir)
    {
        if (!Directory.Exists(scenariosDir))
        {
            return Array.Empty<BenchmarkScenario>();
        }

        return Directory.GetFiles(scenariosDir, "*.js", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path =>
            {
                var scriptName = Path.GetFileNameWithoutExtension(path);
                return new BenchmarkScenario(
                    Key: scriptName,
                    ScriptName: scriptName,
                    Content: File.ReadAllText(path));
            })
            .ToArray();
    }
}
