using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jroc.Test262.Tests.Integration;

public class RunnerCliTests
{
    private readonly VerifySettings _verifySettings = new();

    public RunnerCliTests()
    {
        _verifySettings.DisableDiff();
    }

    [Fact]
    public Task RunMvp_ExecutesStableSubset()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: false);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");

        RunnerResult result = RunRunner(test262Root, outputRoot);
        Assert.Equal(0, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    [Fact]
    public Task RunMvp_ReportsFailureWithRepro()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: true);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");

        RunnerResult result = RunRunner(
            test262Root,
            outputRoot,
            " --file \"test/language/mvp/fail-positive.js\"");

        Assert.Equal(1, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    [Fact]
    public Task RunMvp_ClassifiesPolicyWrongPhaseAndWrongErrorKind()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: false, includeClassificationEdges: true);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");
        string pinPath = CreatePinFile(tempDirectory.Path, ["test/language/mvp/classification-policy-skip.js"]);

        RunnerResult result = RunRunner(
            test262Root,
            outputRoot,
            $" --pin \"{pinPath}\" --filter \"classification-\"");

        Assert.Equal(1, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    [Fact]
    public Task RunMvp_UsesNamedSuiteSelection()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: false);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");
        string suiteConfigPath = CreateSuiteConfigFile(
            tempDirectory.Path,
            [
                "test/language/mvp/basic-pass.js",
                "test/language/mvp/runtime-negative.js",
            ]);

        RunnerResult result = RunRunner(
            test262Root,
            outputRoot,
            $" --suite pr --suite-config \"{suiteConfigPath}\"");

        Assert.Equal(0, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    [Fact]
    public Task RunMvp_RejectsSuiteWhenCombinedWithAdHocSelection()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: false);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");
        string suiteConfigPath = CreateSuiteConfigFile(
            tempDirectory.Path,
            [
                "test/language/mvp/basic-pass.js",
                "test/language/mvp/runtime-negative.js",
            ]);

        RunnerResult result = RunRunner(
            test262Root,
            outputRoot,
            $" --suite pr --suite-config \"{suiteConfigPath}\" --limit 1");

        Assert.Equal(1, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    [Fact]
    public Task RunMvp_AnnotatesSummaryWithEcmaLinkage()
    {
        using var tempDirectory = new TemporaryDirectory();
        string test262Root = CreateTest262Fixture(tempDirectory.Path, includeFailingPositive: false);
        string outputRoot = Path.Combine(tempDirectory.Path, "runner-output");
        string suiteConfigPath = CreateSuiteConfigFile(
            tempDirectory.Path,
            [
                "test/language/mvp/basic-pass.js",
                "test/language/mvp/runtime-negative.js",
            ]);
        string linkageConfigPath = CreateLinkageConfigFile(tempDirectory.Path);

        RunnerResult result = RunRunner(
            test262Root,
            outputRoot,
            $" --suite pr --suite-config \"{suiteConfigPath}\" --linkage-config \"{linkageConfigPath}\"");

        Assert.Equal(0, result.ExitCode);

        return VerifyWithSnapshot(FormatResult(result, tempDirectory.Path, outputRoot));
    }

    private static RunnerResult RunRunner(string test262Root, string outputRoot, string extraArguments = "", int timeoutMilliseconds = 180000)
    {
        string repoRoot = FindRepoRoot();
        string runnerPath = Path.Combine(repoRoot, "scripts", "test262", "runMvp.js");

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"\"{runnerPath}\" --root \"{test262Root}\" --output \"{outputRoot}\"{extraArguments}",
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start the test262 MVP runner.");
        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(timeoutMilliseconds))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"test262 MVP runner timed out after {timeoutMilliseconds}ms.");
        }

        Task.WaitAll(stdoutTask, stderrTask);
        return new RunnerResult(process.ExitCode, stdoutTask.Result, stderrTask.Result);
    }

    private static string CreatePinFile(string root, IReadOnlyList<string> excludedFromMvp)
    {
        string pinPath = Path.Combine(root, "fixture.pin.json");
        string exclusionsJson = string.Join(
            ",\n",
            excludedFromMvp.Select(entry => $"                \"{entry.Replace("\\", "\\\\")}\""));

        File.WriteAllText(
            pinPath,
            $$"""
            {
              "upstream": {
                "owner": "tc39",
                "repo": "test262",
                "cloneUrl": "https://github.com/tc39/test262.git",
                "commit": "0123456789abcdef0123456789abcdef01234567",
                "packageVersion": "5.0.0"
              },
              "localOverrideEnvVar": "JROC_TEST262_ROOT",
              "managedRoot": "./managed-cache",
              "lineEndings": "lf",
              "updateStrategy": "manual-pinned-sha",
              "includeFiles": [
                "LICENSE",
                "INTERPRETING.md",
                "features.txt",
                "package.json"
              ],
              "includeDirectories": [
                "harness",
                "test/language",
                "test/built-ins"
              ],
              "requiredFiles": [
                "LICENSE",
                "INTERPRETING.md",
                "features.txt",
                "package.json",
                "harness/assert.js",
                "harness/sta.js"
              ],
              "requiredDirectories": [
                "harness",
                "test/language",
                "test/built-ins"
              ],
              "defaultHarnessFiles": [
                "assert.js",
                "sta.js"
              ],
              "excludedFromMvp": [
            {{exclusionsJson}}
              ],
              "attributionFiles": [
                "LICENSE",
                "INTERPRETING.md"
              ]
            }
            """.ReplaceLineEndings("\n"));

        return pinPath;
    }

    private static string CreateSuiteConfigFile(string root, IReadOnlyList<string> prFiles)
    {
        string suitePath = Path.Combine(root, "fixture.suites.json");
        string filesJson = string.Join(
            ",\n",
            prFiles.Select(entry => $"      \"{entry.Replace("\\", "\\\\")}\""));

        File.WriteAllText(
            suitePath,
            $$"""
            {
              "pr": {
                "description": "fixture PR suite",
                "files": [
            {{filesJson}}
                ]
              },
              "nightly": {
                "description": "fixture nightly suite",
                "filter": "test/language/mvp",
                "limit": 3
              }
            }
            """.ReplaceLineEndings("\n"));

        return suitePath;
    }

    private static string CreateLinkageConfigFile(string root)
    {
        string linkagePath = Path.Combine(root, "fixture.linkage.json");
        File.WriteAllText(
            linkagePath,
            """
            {
              "schemaVersion": 1,
              "guidance": {
                "updateDocsWhen": "Update the ECMA docs when the linked evidence changes the support claim.",
                "attachToIssueWhen": "Attach to an existing issue when one already tracks the same feature.",
                "createIssueWhen": "Create a new issue when no existing issue tracks the same feature."
              },
              "groups": [
                {
                  "id": "fixture-basic-pass",
                  "title": "fixture basic pass",
                  "filePaths": [
                    "test/language/mvp/basic-pass.js"
                  ],
                  "clauses": [
                    "15.3"
                  ],
                  "docSections": [
                    "docs/ECMA262/15/Section15_3.json"
                  ],
                  "supportEntries": [
                    {
                      "clause": "15.3",
                      "feature": "fixture basic pass"
                    }
                  ],
                  "backlogDocs": [
                    "docs/tracking-issues/ECMA262TopMissingBacklog.md"
                  ],
                  "existingIssues": []
                },
                {
                  "id": "fixture-runtime-negative",
                  "title": "fixture runtime negative",
                  "filePaths": [
                    "test/language/mvp/runtime-negative.js"
                  ],
                  "clauses": [
                    "10.4.4"
                  ],
                  "docSections": [
                    "docs/ECMA262/10/Section10_4.json"
                  ],
                  "supportEntries": [
                    {
                      "clause": "10.4.4",
                      "feature": "fixture runtime negative"
                    }
                  ],
                  "backlogDocs": [
                    "docs/tracking-issues/ECMA262TopMissingBacklog.md"
                  ],
                  "existingIssues": [
                    933
                  ]
                }
              ]
            }
            """.ReplaceLineEndings("\n"));

        return linkagePath;
    }

    private static string CreateTest262Fixture(string root, bool includeFailingPositive, bool includeClassificationEdges = false)
    {
        string test262Root = Path.Combine(root, "test262-root");
        Directory.CreateDirectory(test262Root);
        Directory.CreateDirectory(Path.Combine(test262Root, "test", "built-ins"));

        WriteFile(test262Root, "LICENSE", "fixture license\n");
        WriteFile(test262Root, "INTERPRETING.md", "fixture instructions\n");
        WriteFile(test262Root, "features.txt", "fixture-feature\n");
        WriteFile(test262Root, "package.json", "{ \"name\": \"fixture-test262\", \"version\": \"5.0.0\" }\n");

        WriteFile(test262Root, @"harness\assert.js", """
            function Test262Error(message) {
              this.name = 'Test262Error';
              this.message = message || '';
            }

            Test262Error.prototype = Object.create(Error.prototype);
            Test262Error.prototype.constructor = Test262Error;

            var assert = {
              sameValue: function(actual, expected, message) {
                if (actual !== expected) {
                  throw new Test262Error(message || ('Expected SameValue but got ' + actual + ' and ' + expected));
                }
              }
            };
            """);
        WriteFile(test262Root, @"harness\sta.js", "// no-op fixture sta.js\n");
        WriteFile(test262Root, @"harness\mvpHelper.js", "var helperValue = 2;\n");

        WriteFile(test262Root, @"test\language\mvp\basic-pass.js", """
            /*---
            description: basic pass
            includes: [mvpHelper.js]
            ---*/
            assert.sameValue(helperValue, 2);
            """);
        WriteFile(test262Root, @"test\language\mvp\strict-only-pass.js", """
            /*---
            description: strict-only pass
            flags: [onlyStrict]
            ---*/
            var value = 1;
            assert.sameValue(value, 1);
            """);
        WriteFile(test262Root, @"test\language\mvp\parse-negative.js", """
            /*---
            description: parse negative
            flags: [noStrict]
            negative:
              phase: parse
              type: SyntaxError
            ---*/
            function () {}
            """);
        WriteFile(test262Root, @"test\language\mvp\runtime-negative.js", """
            /*---
            description: runtime negative
            flags: [noStrict]
            negative:
              phase: runtime
              type: TypeError
            ---*/
            throw new TypeError('runtime boom');
            """);
        WriteFile(test262Root, @"test\language\mvp\skip-module.js", """
            /*---
            description: skipped module test
            flags: [module]
            ---*/
            export default 1;
            """);
        WriteFile(test262Root, @"test\language\mvp\unsupported-flag.js", """
            /*---
            description: skipped unsupported flag
            flags: [mysteryFlag]
            ---*/
            0;
            """);

        if (includeFailingPositive)
        {
            WriteFile(test262Root, @"test\language\mvp\fail-positive.js", """
                /*---
                description: failing positive
                flags: [noStrict]
                ---*/
                assert.sameValue(1, 2, 'fixture failure');
                """);
        }

        if (includeClassificationEdges)
        {
            WriteFile(test262Root, @"test\language\mvp\classification-policy-skip.js", """
                /*---
                description: skipped by policy
                flags: [noStrict]
                ---*/
                assert.sameValue(1, 1);
                """);
            WriteFile(test262Root, @"test\language\mvp\classification-parse-wrong-phase.js", """
                /*---
                description: parse negative wrong phase
                flags: [noStrict]
                negative:
                  phase: parse
                  type: SyntaxError
                ---*/
                throw new SyntaxError('late parse-like boom');
                """);
            WriteFile(test262Root, @"test\language\mvp\classification-runtime-wrong-error.js", """
                /*---
                description: runtime negative wrong error kind
                flags: [noStrict]
                negative:
                  phase: runtime
                  type: TypeError
                ---*/
                throw new RangeError('wrong kind');
                """);
        }

        return test262Root;
    }

    private static void WriteFile(string root, string relativePath, string content)
    {
        string normalizedRelativePath = relativePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        string filePath = Path.Combine(root, normalizedRelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, content.ReplaceLineEndings("\n"));
    }

    private static string FormatResult(RunnerResult result, string tempRoot, string outputRoot)
    {
        string normalizedTempRoot = NormalizePath(tempRoot);
        string normalizedRepoRoot = NormalizePath(FindRepoRoot());

        var builder = new StringBuilder();
        builder.AppendLine($"exit: {result.ExitCode}");
        builder.AppendLine("stdout:");
        builder.AppendLine(NormalizeText(result.StdOut, normalizedTempRoot, normalizedRepoRoot));

        if (!string.IsNullOrWhiteSpace(result.StdErr))
        {
            builder.AppendLine("stderr:");
            builder.AppendLine(NormalizeText(result.StdErr, normalizedTempRoot, normalizedRepoRoot));
        }

        string summaryPath = Path.Combine(outputRoot, "summary.json");
        if (File.Exists(summaryPath))
        {
            builder.AppendLine("summary.json:");
            builder.AppendLine(NormalizeText(File.ReadAllText(summaryPath), normalizedTempRoot, normalizedRepoRoot));
        }

        return builder.ToString().TrimEnd();
    }

    private static string NormalizeText(string value, string normalizedTempRoot, string normalizedRepoRoot)
    {
        return value
            .ReplaceLineEndings("\n")
            .Replace('\\', '/')
            .Replace(normalizedRepoRoot, "<repo>")
            .Replace(normalizedTempRoot, "<temp>")
            .Replace("/bin/Release/net10.0/Jroc.dll", "/bin/<configuration>/net10.0/Jroc.dll")
            .Replace("/bin/Debug/net10.0/Jroc.dll", "/bin/<configuration>/net10.0/Jroc.dll")
            .TrimEnd();
    }

    private static string NormalizePath(string value)
    {
        return Path.GetFullPath(value).Replace('\\', '/');
    }

    private static string FindRepoRoot()
    {
        string? current = Path.GetDirectoryName(typeof(RunnerCliTests).Assembly.Location);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current, "jroc.sln")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the test assembly output.");
    }

    private Task VerifyWithSnapshot(string value, [CallerFilePath] string sourceFilePath = "")
    {
        var settings = new VerifySettings(_verifySettings);
        string directory = Path.GetDirectoryName(sourceFilePath)!;
        string snapshotsDirectory = Path.Combine(directory, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        return Verify(value, settings);
    }

    private sealed record RunnerResult(int ExitCode, string StdOut, string StdErr);

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "jroc-test262-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, recursive: true);
            }
            catch
            {
                // Best-effort cleanup for temporary integration test fixtures.
            }
        }
    }
}
