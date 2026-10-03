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
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

if (args.Length == 2 && args[0] == "--worker")
{
    try
    {
        var request = JsonSerializer.Deserialize<WorkerRequest>(
            File.ReadAllText(args[1]), options)
            ?? throw new InvalidOperationException("The worker request is empty.");
        var result = Screen(
            request.Root, request.Candidate, request.Variant, request.TimeoutMs);
        Console.WriteLine(JsonSerializer.Serialize(result, options));
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
    var root = Path.GetFullPath(plan.UpstreamRoot);
    var results = new List<ScreeningResult>();
    var started = DateTimeOffset.UtcNow;

    foreach (var candidate in plan.Candidates)
    {
        foreach (var variant in candidate.Variants)
        {
            if (results.Count >= plan.VariantLimit
                || DateTimeOffset.UtcNow - started >= TimeSpan.FromSeconds(plan.TimeLimitSeconds))
            {
                break;
            }

            results.Add(RunWorker(root, candidate, variant, plan.TimeoutMs, options));
        }
        if (results.Count >= plan.VariantLimit
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

    Console.WriteLine(JsonSerializer.Serialize(results, options));
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 2;
}

static ScreeningResult Screen(string root, ScreeningCandidate candidate, string variant, int timeoutMs)
{
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
    if (metadata.Flags.Contains("module", StringComparer.Ordinal)
        || string.Equals(variant, "module", StringComparison.Ordinal))
    {
        return Result("unsupported", "harness-gap", "metadata",
            "The first-release native screening host does not support module fixtures.");
    }

    if (metadata.NegativePhase is "parse" or "early")
    {
        return Result("unsupported", "harness-gap", metadata.NegativePhase,
            $"Native compile-negative type verification for {metadata.NegativeType ?? "an unspecified error"} is not available.");
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
            _ => (variantSource, sourcePath),
            allowUnhandledException: runtimeNegative,
            timeoutMs: timeoutMs);
        Test262SharedAssertHarness.AssertNoOutput(candidate.Path, result.Output);
        return Result("pass", null, runtimeNegative ? "runtime" : "execution", "");
    }
    catch (Exception exception)
    {
        var diagnostic = exception.ToString();
        var harnessGap = diagnostic.Contains("harness", StringComparison.OrdinalIgnoreCase)
            || diagnostic.Contains("include", StringComparison.OrdinalIgnoreCase)
            || diagnostic.Contains("$262", StringComparison.Ordinal);
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
    string? NegativePhase,
    string? NegativeType)
{
    public static Metadata Parse(string source)
    {
        var match = Regex.Match(source, @"/\*---(?<body>.*?)---\*/", RegexOptions.Singleline);
        if (!match.Success)
        {
            return new Metadata([], null, null);
        }

        var body = match.Groups["body"].Value.ReplaceLineEndings("\n");
        return new Metadata(
            ParseArray(body, "flags"),
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
            return [];
        }

        return match.Groups["value"].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.Trim('\'', '"'))
            .ToArray();
    }
}
