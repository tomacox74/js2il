using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

public static class Test262SharedAssertHarness
{
    private const string AssertModuleId = "node_modules/assert/index";

    public static InMemoryTestExecutionResult CompileAndExecute(
        string testName,
        string testCategory,
        Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath,
        string callerSourceFilePath,
        bool enableIRMetrics = false,
        bool allowUnhandledException = false,
        Action<ServiceContainer>? addMocks = null,
        int timeoutMs = 30000)
    {
        return InMemoryTestCompiler.CompileAndExecute(
            testName,
            testCategory,
            name => ResolveJavaScriptAndSourcePath(name, callerSourceFilePath, getJavaScriptAndSourcePath),
            additionalScripts: [AssertModuleId],
            executeAdditionalScriptsBeforeEntry: true,
            enableIRMetrics: enableIRMetrics,
            allowUnhandledException: allowUnhandledException,
            addMocks: addMocks,
            timeoutMs: timeoutMs);
    }

    private static (string Script, string? SourcePath) ResolveJavaScriptAndSourcePath(
        string testName,
        string callerSourceFilePath,
        Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath)
    {
        if (string.Equals(testName, AssertModuleId, StringComparison.Ordinal)
            || string.Equals(testName, "node_modules\\assert\\index", StringComparison.Ordinal))
        {
            return (File.ReadAllText(GetAssertHarnessSourcePath(callerSourceFilePath)), null);
        }

        return getJavaScriptAndSourcePath(testName);
    }

    private static string GetAssertHarnessSourcePath(string callerSourceFilePath)
    {
        var directory = Path.GetDirectoryName(callerSourceFilePath)
            ?? throw new InvalidOperationException("Unable to determine caller directory.");

        while (!string.IsNullOrWhiteSpace(directory))
        {
            var candidate = Path.Combine(directory, "tests", "Jroc.Test262.Tests", "Harness", "assert.js");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new InvalidOperationException("Unable to locate tests\\Jroc.Test262.Tests\\Harness\\assert.js.");
    }
}
