using System.Text.Json;
using System.Text.RegularExpressions;
using Jroc;
using Jroc.Services;

namespace Jroc.Tests;

public static class Test262CompilationAssertions
{
    public static void AssertResolutionSyntaxError(string testName, string script, string sourcePath)
    {
        var prepared = Test262SharedAssertHarness.PrepareEntryScript(script);
        var parser = new JavaScriptParser();
        parser.ParseJavaScriptModule(prepared, sourcePath);
        InvalidOperationException? failure = null;
        try
        {
            JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(sourcePath)
            {
                SourceText = prepared,
                ParseAsModule = true
            });
        }
        catch (InvalidOperationException error) when (error.Message.StartsWith("Compilation failed.", StringComparison.Ordinal))
        {
            failure = error;
        }
        if (failure is null)
            throw new InvalidOperationException($"Expected a resolution SyntaxError for test '{testName}', but compilation succeeded.");

        // AOT syntax/link errors are diagnostics. Reject unrelated compiler, validation and I/O failures.
        if (failure.Message.Contains("Module Link Errors:", StringComparison.Ordinal)
            && Regex.IsMatch(failure.Message,
                @"Module '[^'\r\n]+' (?:does not export|exports '[^'\r\n]+' ambiguously)|Ambiguous export '|Export '[^'\r\n]+' in module '[^'\r\n]+' could not be resolved during module linking"))
            return;
        foreach (Match match in Regex.Matches(failure.Message,
            @"(?m)^Error: \[[^\]\r\n]+\] (?<path>[^\r\n]+): Failed to parse (?<kind>JSON module|JavaScript(?: module)?):?"))
        {
            var dependency = match.Groups["path"].Value;
            if (!Path.IsPathRooted(dependency) || !File.Exists(dependency)
                || string.Equals(Path.GetFullPath(dependency), Path.GetFullPath(sourcePath), StringComparison.Ordinal))
                continue;
            try
            {
                if (match.Groups["kind"].Value == "JSON module")
                {
                    using var document = JsonDocument.Parse(File.ReadAllText(dependency).TrimStart('\uFEFF'));
                }
                else
                    parser.ParseJavaScriptModule(File.ReadAllText(dependency), dependency);
            }
            catch (JsonException)
            {
                return;
            }
            catch (Exception error) when (error.InnerException is Acornima.ParseErrorException)
            {
                return;
            }
        }
        throw new InvalidOperationException(
            $"Compilation failed for '{testName}', but did not establish a resolution SyntaxError.\n{failure.Message}", failure);
    }
}
