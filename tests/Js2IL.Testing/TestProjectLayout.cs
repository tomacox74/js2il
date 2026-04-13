namespace Js2IL.Tests;

internal static class TestProjectLayout
{
    public static string GetResourceRoot(Type testType)
    {
        var resourceRoot = testType.Assembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(resourceRoot))
        {
            throw new InvalidOperationException($"Could not determine assembly name for test type '{testType.FullName}'.");
        }

        return resourceRoot;
    }

    public static string GetCategoryFromNamespace(Type testType)
    {
        var ns = testType.Namespace ?? string.Empty;
        var rootNs = GetResourceRoot(testType) + ".";
        if (ns.StartsWith(rootNs, StringComparison.Ordinal))
        {
            var category = ns.Substring(rootNs.Length);
            if (!string.IsNullOrWhiteSpace(category))
            {
                return category;
            }
        }

        return ns.Split('.').LastOrDefault() ?? string.Empty;
    }

    public static string? FindProjectRoot(Type testType, string callerSourceFilePath)
        => FindDirectoryContainingFile(Path.GetDirectoryName(callerSourceFilePath) ?? string.Empty, GetProjectFileName(testType));

    private static string GetProjectFileName(Type testType) => GetResourceRoot(testType) + ".csproj";

    private static string? FindDirectoryContainingFile(string startDirectory, string fileName)
    {
        var current = startDirectory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            var candidate = Path.Combine(current, fileName);
            if (File.Exists(candidate))
            {
                return current;
            }

            var parent = Directory.GetParent(current);
            if (parent == null)
            {
                break;
            }

            current = parent.FullName;
        }

        return null;
    }
}
