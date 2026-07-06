using JavaScriptRuntime.DependencyInjection;
using System.Text.RegularExpressions;

namespace Jroc.Tests;

public static class Test262SharedAssertHarness
{
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
        var preparedEntryScript = BuildPreparedEntryScript(entryScript, metadata, callerSourceFilePath);

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
            enableIRMetrics: enableIRMetrics,
            allowUnhandledException: allowUnhandledException,
            addMocks: addMocks,
            timeoutMs: timeoutMs);
    }

    private static string BuildPreparedEntryScript(string entryScript, FrontmatterMetadata metadata, string callerSourceFilePath)
    {
        var (prefix, remainder, hasUseStrictDirective) = SplitDirectivePrologue(entryScript);
        var scriptBuilder = new System.Text.StringBuilder();
        scriptBuilder.Append(prefix);

        if (metadata.OnlyStrict && !hasUseStrictDirective)
        {
            scriptBuilder.AppendLine("\"use strict\";");
        }

        var helperFiles = new List<string> { "assert.js" };
        helperFiles.AddRange(GetInlineHarnessFileNames(metadata.Includes));

        foreach (var helperFile in helperFiles.Distinct(StringComparer.Ordinal))
        {
            var helperScript = File.ReadAllText(GetHarnessSourcePath(callerSourceFilePath, helperFile));
            scriptBuilder.AppendLine(BuildInlineHarnessBlock(helperFile, helperScript));
        }

        scriptBuilder.Append(remainder);
        return scriptBuilder.ToString();
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

    private static string BuildInlineHarnessBlock(string harnessFileName, string script)
    {
        var scriptWithoutExports = StripModuleExports(script);
        var defines = ParseDefines(script);
        var assignmentLines = defines.Count == 0
            ? string.Empty
            : string.Join(
                "\n",
                defines.Select(name => $"if (typeof {name} !== 'undefined') globalThis.{name} = {name};"));

        var block = new System.Text.StringBuilder();
        block.AppendLine($"// Inlined test262 harness helper: {harnessFileName}");
        block.AppendLine(";(() => {");
        block.AppendLine(scriptWithoutExports);
        if (!string.IsNullOrWhiteSpace(assignmentLines))
        {
            block.AppendLine(assignmentLines);
        }

        block.AppendLine("})();");
        return block.ToString();
    }

    private static IReadOnlyList<string> ParseDefines(string script)
    {
        var match = Regex.Match(script, @"/\*---(?<body>.*?)---\*/", RegexOptions.Singleline);
        if (!match.Success)
        {
            return Array.Empty<string>();
        }

        var body = match.Groups["body"].Value;
        var inline = ParseArrayValue(body, "defines");
        if (inline.Count > 0)
        {
            return inline;
        }

        var blockMatch = Regex.Match(
            body,
            @"(?ms)^\s*defines\s*:\s*\r?\n(?<value>(?:\s*-\s*[^\r\n]+\r?\n?)*)");
        if (!blockMatch.Success)
        {
            return Array.Empty<string>();
        }

        return blockMatch.Groups["value"].Value
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("- ", StringComparison.Ordinal))
            .Select(line => line.Substring(2).Split('#')[0].Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }

    private static string StripModuleExports(string script)
    {
        return Regex.Replace(
            script,
            @"(?ms)^\s*module\.exports\s*=\s*\{.*?\};\s*$",
            string.Empty);
    }

    private static (string Prefix, string Remainder, bool HasUseStrictDirective) SplitDirectivePrologue(string script)
    {
        var index = 0;
        var prefixEnd = 0;
        var hasUseStrictDirective = false;

        SkipTrivia(script, ref index);
        prefixEnd = index;

        while (TryConsumeDirective(script, ref index, out var directiveValue))
        {
            if (string.Equals(directiveValue, "use strict", StringComparison.Ordinal))
            {
                hasUseStrictDirective = true;
            }

            SkipTrivia(script, ref index);
            prefixEnd = index;
        }

        return (script[..prefixEnd], script[prefixEnd..], hasUseStrictDirective);
    }

    private static void SkipTrivia(string script, ref int index)
    {
        while (index < script.Length)
        {
            if (char.IsWhiteSpace(script[index]))
            {
                index++;
                continue;
            }

            if (index + 1 < script.Length && script[index] == '/' && script[index + 1] == '/')
            {
                index += 2;
                while (index < script.Length && script[index] != '\r' && script[index] != '\n')
                {
                    index++;
                }

                continue;
            }

            if (index + 1 < script.Length && script[index] == '/' && script[index + 1] == '*')
            {
                index += 2;
                while (index + 1 < script.Length && !(script[index] == '*' && script[index + 1] == '/'))
                {
                    index++;
                }

                if (index + 1 < script.Length)
                {
                    index += 2;
                }

                continue;
            }

            break;
        }
    }

    private static bool TryConsumeDirective(string script, ref int index, out string directiveValue)
    {
        directiveValue = string.Empty;
        if (index >= script.Length)
        {
            return false;
        }

        var quote = script[index];
        if (quote != '"' && quote != '\'')
        {
            return false;
        }

        var literalStart = index;
        index++;
        while (index < script.Length)
        {
            if (script[index] == '\\')
            {
                index += 2;
                continue;
            }

            if (script[index] == quote)
            {
                directiveValue = script[(literalStart + 1)..index];
                index++;
                while (index < script.Length && (script[index] == ' ' || script[index] == '\t'))
                {
                    index++;
                }

                if (index < script.Length && script[index] == ';')
                {
                    index++;
                }

                return true;
            }

            if (script[index] == '\r' || script[index] == '\n')
            {
                return false;
            }

            index++;
        }

        return false;
    }

    private static IEnumerable<string> GetInlineHarnessFileNames(IReadOnlyList<string> includeFileNames)
    {
        foreach (var includeFileName in includeFileNames)
        {
            if (string.Equals(includeFileName, "assert.js", StringComparison.Ordinal)
                || string.Equals(includeFileName, "compareArray.js", StringComparison.Ordinal)
                || string.Equals(includeFileName, "isConstructor.js", StringComparison.Ordinal)
                || string.Equals(includeFileName, "wellKnownIntrinsicObjects.js", StringComparison.Ordinal)
                || string.Equals(includeFileName, "sta.js", StringComparison.Ordinal))
            {
                continue;
            }

            yield return includeFileName;
        }
    }

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
