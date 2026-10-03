using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Jroc.Tests;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

// Streamed records are newline-delimited: consumers import one stdout line at a
// time, so a record must never be serialized across multiple lines.
var recordOptions = new JsonSerializerOptions(options) { WriteIndented = false };

if (args.Length == 1 && args[0] == "--capabilities")
{
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        schema_version = 1,
        flags = new
        {
            async = true,
            onlyStrict = true,
            noStrict = true,
            raw = false,
            module = "static-syntax-only"
        },
        includes = new[]
        {
            "assertRelativeDateMs.js", "asyncHelpers.js", "atomicsHelper.js",
            "byteConversionValues.js", "compareArray.js", "compareIterator.js",
            "dateConstants.js", "decimalToHexString.js", "deepEqual.js",
            "detachArrayBuffer.js", "isConstructor.js", "iteratorZipUtils.js",
            "nans.js", "nativeFunctionMatcher.js", "promiseHelper.js",
            "propertyHelper.js", "proxyTrapsHelper.js", "regExpUtils.js",
            "resizableArrayBufferUtils.js", "temporalHelpers.js", "testAtomics.js",
            "testTypedArray.js", "tcoHelper.js", "wellKnownIntrinsicObjects.js"
        },
        dependencies = new { sibling_files = true, harness_files = true },
        isolation = new { worker_process = true, timeout = true, agent_cleanup = true }
    }, recordOptions));
    return 0;
}

if (args.Length == 2 && args[0] == "--worker")
{
    try
    {
        var request = JsonSerializer.Deserialize<WorkerRequest>(
            File.ReadAllText(args[1]), options)
            ?? throw new InvalidOperationException("The worker request is empty.");
        var result = Screen(
            request.Root, request.Candidate, request.Variant, request.TimeoutMs);
        Console.WriteLine(JsonSerializer.Serialize(result, recordOptions));
        return 0;
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine(exception);
        return 2;
    }
}

if (args.Length != 2 || args[0] != "--plan")
{
    Console.Error.WriteLine("Usage: NativeScreeningHost --plan <plan.json>");
    return 2;
}

try
{
    var plan = JsonSerializer.Deserialize<ScreeningPlan>(
        File.ReadAllText(args[1]), options)
        ?? throw new InvalidOperationException("The screening plan is empty.");
    var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(plan.UpstreamRoot));
    var started = DateTimeOffset.UtcNow;
    var screened = 0;
    using var output = Console.Out;
    output.Flush();

    foreach (var candidate in plan.Candidates)
    {
        foreach (var variant in candidate.Variants)
        {
            if (screened >= plan.VariantLimit
                || DateTimeOffset.UtcNow - started >= TimeSpan.FromSeconds(plan.TimeLimitSeconds))
            {
                break;
            }

            output.WriteLine(JsonSerializer.Serialize(
                RunWorker(root, candidate, variant, plan.TimeoutMs, options),
                recordOptions));
            output.Flush();
            screened++;
        }
        if (screened >= plan.VariantLimit
            || DateTimeOffset.UtcNow - started >= TimeSpan.FromSeconds(plan.TimeLimitSeconds))
        {
            break;
        }
    }

    static ScreeningResult RunWorker(
        string root,
        ScreeningCandidate candidate,
        string variant,
        int timeoutMs,
        JsonSerializerOptions options)
    {
        var started = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d;
        var requestPath = Path.Combine(
            Path.GetTempPath(), $"jroc-test262-screen-{Guid.NewGuid():N}.json");
        try
        {
            File.WriteAllText(
                requestPath,
                JsonSerializer.Serialize(
                    new WorkerRequest(root, candidate, variant, timeoutMs), options));
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.ProcessPath
                        ?? throw new InvalidOperationException("Unable to resolve the dotnet host."),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.StartInfo.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
            process.StartInfo.ArgumentList.Add("--worker");
            process.StartInfo.ArgumentList.Add(requestPath);
            process.Start();
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(timeoutMs + 5000))
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit();
                return new ScreeningResult(
                    candidate.Path, variant, candidate.Sha256,
                    "infrastructure-error", "infrastructure-error", "timeout",
                    $"Native fixture worker exceeded {timeoutMs} ms and was terminated.",
                    started, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d);
            }

            Task.WaitAll(output, error);
            if (process.ExitCode != 0)
            {
                var diagnostic = error.Result;
                return new ScreeningResult(
                    candidate.Path, variant, candidate.Sha256,
                    "infrastructure-error", "infrastructure-error", "worker",
                    diagnostic.Length <= 8000 ? diagnostic : diagnostic[^8000..],
                    started, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d);
            }

            return JsonSerializer.Deserialize<ScreeningResult>(output.Result, options)
                ?? throw new InvalidOperationException("The fixture worker returned no result.");
        }
        finally
        {
            File.Delete(requestPath);
        }
    }

    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 2;
}

static ScreeningResult Screen(string root, ScreeningCandidate candidate, string variant, int timeoutMs)
{
    root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
    var started = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d;
    var sourcePath = Path.GetFullPath(Path.Combine(root, candidate.Path));
    if (!sourcePath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal)
        || !File.Exists(sourcePath))
    {
        return Result("infrastructure-error", "infrastructure-error", "load",
            $"Fixture does not exist beneath the pinned root: {candidate.Path}");
    }

    var bytes = File.ReadAllBytes(sourcePath);
    var actualHash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if (!string.Equals(actualHash, candidate.Sha256, StringComparison.Ordinal))
    {
        return Result("infrastructure-error", "infrastructure-error", "load",
            $"Fixture hash mismatch: expected {candidate.Sha256}, got {actualHash}.");
    }

    var source = Encoding.UTF8.GetString(bytes);
    var metadata = Metadata.Parse(source);
    if (metadata.NegativePhase is "parse" or "early" or "resolution")
    {
        return Result("unsupported", "harness-gap", metadata.NegativePhase,
            $"Native compile-negative type verification for {metadata.NegativeType ?? "an unspecified error"} is not available.");
    }

    if (metadata.Flags.Contains("raw", StringComparer.Ordinal))
    {
        return Result("unsupported", "harness-gap", "capability",
            "Raw fixtures bypass the native harness injection contract.");
    }

    if ((metadata.Flags.Contains("module", StringComparer.Ordinal)
            || string.Equals(variant, "module", StringComparison.Ordinal))
        && !Regex.IsMatch(source, @"(?m)^\s*(?:import|export)\b"))
    {
        return Result("unsupported", "harness-gap", "capability",
            "Module-goal screening currently requires static import/export syntax.");
    }

    var missingDependencies = FindMissingRelativeDependencies(sourcePath, source);
    if (missingDependencies.Count > 0)
    {
        return Result("unsupported", "harness-gap", "capability",
            $"Native screening dependencies were not found: {string.Join(", ", missingDependencies)}");
    }

    var unsupportedIncludes = metadata.Includes
        .Where(include => !IsSupportedInclude(include))
        .ToArray();
    if (unsupportedIncludes.Length > 0)
    {
        return Result("unsupported", "harness-gap", "capability",
            $"Native harness does not implement includes: {string.Join(", ", unsupportedIncludes)}");
    }

    try
    {
        var variantSource = string.Equals(variant, "strict", StringComparison.Ordinal)
            && !metadata.Flags.Contains("onlyStrict", StringComparer.Ordinal)
                ? "\"use strict\";\n" + source
                : source;
        var runtimeNegative = string.Equals(
            metadata.NegativePhase, "runtime", StringComparison.OrdinalIgnoreCase);
        var result = Test262SharedAssertHarness.CompileAndExecute(
            Path.GetFileNameWithoutExtension(candidate.Path),
            "NativeScreening",
            requestedName => ResolveFixture(
                root, sourcePath, candidate.Path, requestedName, variantSource),
            allowUnhandledException: runtimeNegative,
            timeoutMs: timeoutMs);
        Test262SharedAssertHarness.AssertNoOutput(candidate.Path, result.Output);
        return Result("pass", null, runtimeNegative ? "runtime" : "execution", "");
    }

    catch (Exception exception)
    {
        var diagnostic = exception.ToString();
        var harnessGap = exception is FileNotFoundException;
        return Result(
            "fail",
            harnessGap ? "harness-gap" : "unresolved",
            metadata.NegativePhase ?? "execution",
            diagnostic.Length <= 8000 ? diagnostic : diagnostic[^8000..]);
    }

    ScreeningResult Result(
        string outcome, string? failureClass, string phase, string diagnostic)
        => new(
            candidate.Path, variant, candidate.Sha256, outcome, failureClass,
            phase, diagnostic, started,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d);
}

static bool IsSupportedInclude(string include)
{
    return include is "assertRelativeDateMs.js" or "asyncHelpers.js"
        or "atomicsHelper.js" or "byteConversionValues.js" or "compareArray.js"
        or "compareIterator.js" or "dateConstants.js" or "decimalToHexString.js"
        or "deepEqual.js" or "detachArrayBuffer.js" or "isConstructor.js"
        or "iteratorZipUtils.js" or "nans.js" or "nativeFunctionMatcher.js"
        or "promiseHelper.js" or "propertyHelper.js" or "proxyTrapsHelper.js"
        or "regExpUtils.js" or "resizableArrayBufferUtils.js"
        or "temporalHelpers.js" or "testAtomics.js" or "testTypedArray.js"
        or "tcoHelper.js" or "wellKnownIntrinsicObjects.js";
}

static IReadOnlyList<string> FindMissingRelativeDependencies(
    string sourcePath, string source)
{
    var directory = Path.GetDirectoryName(sourcePath)
        ?? throw new InvalidOperationException("The candidate has no parent directory.");
    var missing = new List<string>();
    var matches = Regex.Matches(
        source,
        @"(?m)(?:^\s*(?:import|export)\s+(?:[^'""\r\n]*?\s+from\s+)?|import\s*\()\s*['""](?<specifier>\.[^'""]+)['""]");
    foreach (Match match in matches)
    {
        var specifier = match.Groups["specifier"].Value;
        var dependency = Path.GetFullPath(Path.Combine(directory, specifier));
        if (Path.GetExtension(dependency).Length == 0)
        {
            dependency += ".js";
        }
        if (!File.Exists(dependency))
        {
            missing.Add(specifier);
        }
    }
    return missing;
}

static (string Script, string? SourcePath) ResolveFixture(
    string root,
    string sourcePath,
    string candidatePath,
    string requestedName,
    string preparedEntryScript)
{
    root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
    if (string.Equals(
        requestedName,
        Path.GetFileNameWithoutExtension(candidatePath),
        StringComparison.Ordinal)
        || string.Equals(requestedName, candidatePath, StringComparison.Ordinal))
    {
        return (preparedEntryScript, sourcePath);
    }

    var candidateDirectory = Path.GetDirectoryName(sourcePath)
        ?? throw new InvalidOperationException("The candidate has no parent directory.");
    var searchPaths = new[]
    {
        Path.Combine(candidateDirectory, requestedName),
        Path.Combine(root, "harness", requestedName),
        Path.Combine(root, "test", requestedName),
    };
    foreach (var path in searchPaths)
    {
        var fullPath = Path.GetFullPath(path);
        if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || !File.Exists(fullPath))
        {
            continue;
        }

        return (File.ReadAllText(fullPath), fullPath);
    }

    throw new FileNotFoundException(
        $"Native screening dependency '{requestedName}' was not found for '{candidatePath}'.");
}

sealed record ScreeningPlan(
    string UpstreamRoot,
    int TimeoutMs,
    int VariantLimit,
    int TimeLimitSeconds,
    IReadOnlyList<ScreeningCandidate> Candidates);

sealed record ScreeningCandidate(
    string Path,
    string Sha256,
    IReadOnlyList<string> Variants);

sealed record WorkerRequest(
    string Root,
    ScreeningCandidate Candidate,
    string Variant,
    int TimeoutMs);

sealed record ScreeningResult(
    string Path,
    string Variant,
    string FixtureSha256,
    string Outcome,
    string? FailureClass,
    string Phase,
    string Diagnostic,
    double StartedAt,
    double FinishedAt);

sealed record Metadata(
    IReadOnlyList<string> Flags,
    IReadOnlyList<string> Includes,
    string? NegativePhase,
    string? NegativeType)
{
    public static Metadata Parse(string source)
    {
        var match = Regex.Match(source, @"/\*---(?<body>.*?)---\*/", RegexOptions.Singleline);
        if (!match.Success)
        {
            return new Metadata([], [], null, null);
        }

        var body = match.Groups["body"].Value.ReplaceLineEndings("\n");
        return new Metadata(
            ParseArray(body, "flags"),
            ParseArray(body, "includes"),
            ParseScalar(body, "phase"),
            ParseScalar(body, "type"));
    }

    private static string? ParseScalar(string body, string key)
    {
        var match = Regex.Match(
            body, @"(?m)^\s*" + Regex.Escape(key) + @"\s*:\s*(?<value>[^\s#]+)");
        return match.Success ? match.Groups["value"].Value.Trim('\'', '"') : null;
    }

    private static IReadOnlyList<string> ParseArray(string body, string key)
    {
        var match = Regex.Match(
            body, @"(?m)^\s*" + Regex.Escape(key) + @"\s*:\s*\[(?<value>[^\]]*)\]");
        if (!match.Success)
        {
            var block = Regex.Match(
                body,
                @"(?m)^[ \t]*" + Regex.Escape(key)
                    + @"[ \t]*:[ \t]*\n(?<value>(?:[ \t]*-[ \t]+[^\n]*(?:\n|$))+)");
            if (!block.Success)
            {
                return [];
            }

            return block.Groups["value"].Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => value[1..].Split('#', 2)[0].Trim().Trim('\'', '"'))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
        }

        return match.Groups["value"].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.Trim('\'', '"'))
            .ToArray();
    }
}
