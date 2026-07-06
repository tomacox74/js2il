using JavaScriptRuntime.DependencyInjection;
using System.Text.RegularExpressions;

namespace Jroc.Tests;

public static class Test262SharedAssertHarness
{
    private const string AssertModuleId = "node_modules/assert/index";
    private const string HarnessModulePrefix = "test262-harness/";

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
        var (entryScript, entrySourcePath) = getJavaScriptAndSourcePath(testName);
        var metadata = ParseFrontmatter(entryScript);
        var preparedEntryScript = metadata.OnlyStrict
            ? "\"use strict\";\n" + entryScript
            : entryScript;
        var additionalScripts = new List<string> { AssertModuleId };
        foreach (var include in metadata.Includes)
        {
            var harnessModuleId = GetHarnessModuleId(include);
            if (harnessModuleId == null)
            {
                continue;
            }

            additionalScripts.Add(harnessModuleId);
        }

        return InMemoryTestCompiler.CompileAndExecute(
            testName,
            testCategory,
            name => ResolveJavaScriptAndSourcePath(
                name,
                testName,
                preparedEntryScript,
                entrySourcePath,
                callerSourceFilePath,
                getJavaScriptAndSourcePath),
            additionalScripts: additionalScripts.ToArray(),
            executeAdditionalScriptsBeforeEntry: true,
            enableIRMetrics: enableIRMetrics,
            allowUnhandledException: allowUnhandledException,
            addMocks: addMocks,
            timeoutMs: timeoutMs);
    }

    private static (string Script, string? SourcePath) ResolveJavaScriptAndSourcePath(
        string requestedScriptName,
        string entryTestName,
        string preparedEntryScript,
        string? entrySourcePath,
        string callerSourceFilePath,
        Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath)
    {
        if (string.Equals(requestedScriptName, entryTestName, StringComparison.Ordinal))
        {
            return (preparedEntryScript, entrySourcePath);
        }

        if (string.Equals(requestedScriptName, AssertModuleId, StringComparison.Ordinal)
            || string.Equals(requestedScriptName, "node_modules\\assert\\index", StringComparison.Ordinal))
        {
            return (File.ReadAllText(GetAssertHarnessSourcePath(callerSourceFilePath)), null);
        }

        var harnessFileName = TryGetHarnessFileName(requestedScriptName);
        if (harnessFileName != null)
        {
            return (File.ReadAllText(GetHarnessSourcePath(callerSourceFilePath, harnessFileName)), null);
        }

        return getJavaScriptAndSourcePath(requestedScriptName);
    }

    private static FrontmatterMetadata ParseFrontmatter(string script)
    {
        var match = Regex.Match(script, @"/\*---(?<body>.*?)---\*/", RegexOptions.Singleline);
        if (!match.Success)
        {
            return FrontmatterMetadata.Empty;
        }

        var body = match.Groups["body"].Value;
        return new FrontmatterMetadata(
            ParseArrayValue(body, "flags").Contains("onlyStrict", StringComparer.Ordinal),
            ParseArrayValue(body, "includes"));
    }

    private static IReadOnlyList<string> ParseArrayValue(string frontmatterBody, string key)
    {
        var match = Regex.Match(
            frontmatterBody,
            @"(?m)^\s*" + Regex.Escape(key) + @"\s*:\s*\[(?<value>[^\]]*)\]");
        if (!match.Success)
        {
            return Array.Empty<string>();
        }

        return match.Groups["value"].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(entry => entry.Trim().Trim('\'', '"'))
            .Where(entry => !string.IsNullOrWhiteSpace(entry))
            .ToArray();
    }

    private static string? GetHarnessModuleId(string includeFileName)
    {
        if (string.Equals(includeFileName, "assert.js", StringComparison.Ordinal)
            || string.Equals(includeFileName, "compareArray.js", StringComparison.Ordinal)
            || string.Equals(includeFileName, "isConstructor.js", StringComparison.Ordinal)
            || string.Equals(includeFileName, "wellKnownIntrinsicObjects.js", StringComparison.Ordinal)
            || string.Equals(includeFileName, "sta.js", StringComparison.Ordinal))
        {
            return null;
        }

        return HarnessModulePrefix + includeFileName.Replace('\\', '/');
    }

    private static string? TryGetHarnessFileName(string requestedScriptName)
    {
        var normalized = requestedScriptName.Replace('\\', '/');
        return normalized.StartsWith(HarnessModulePrefix, StringComparison.Ordinal)
            ? normalized.Substring(HarnessModulePrefix.Length)
            : null;
    }

    private static string GetAssertHarnessSourcePath(string callerSourceFilePath)
        => GetHarnessSourcePath(callerSourceFilePath, "assert.js");

    private static string GetHarnessSourcePath(string callerSourceFilePath, string harnessFileName)
    {
        var directory = Path.GetDirectoryName(callerSourceFilePath)
            ?? throw new InvalidOperationException("Unable to determine caller directory.");

        while (!string.IsNullOrWhiteSpace(directory))
        {
            var candidate = Path.Combine(directory, "tests", "Jroc.Test262.Tests", "Harness", harnessFileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new InvalidOperationException($"Unable to locate tests\\Jroc.Test262.Tests\\Harness\\{harnessFileName}.");
    }

    private sealed record FrontmatterMetadata(bool OnlyStrict, IReadOnlyList<string> Includes)
    {
        public static FrontmatterMetadata Empty { get; } = new(false, Array.Empty<string>());
    }
}
