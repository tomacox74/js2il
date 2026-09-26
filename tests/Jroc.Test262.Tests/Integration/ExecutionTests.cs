using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using AssertionError = JavaScriptRuntime.Node.AssertionError;
using Jroc.Tests;
using Jroc.Tests.Integration;

namespace Jroc.Test262.Tests.Integration;

public class ExecutionTests
{
    private readonly VerifySettings _verifySettings = new();

    public ExecutionTests()
    {
        _verifySettings.DisableDiff();
    }

    [Fact]
    public Task Compile_Scripts_Test262MetadataParser()
        => ExecutionTest(nameof(Compile_Scripts_Test262MetadataParser), additionalScripts: ["test262/metadataParser"]);

    [Fact]
    public async Task Compile_Scripts_Test262Bootstrap()
    {
        using var currentDirectory = new TemporaryCurrentDirectory();
        var pinPath = Path.Combine(currentDirectory.Path, "test262.pin.json");

        File.WriteAllText(
            pinPath,
            """
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
                "test/annexB/**",
                "test/intl402/**",
                "test/staging/**"
              ],
              "attributionFiles": [
                "LICENSE",
                "INTERPRETING.md"
              ]
            }
            """.ReplaceLineEndings("\n"));

        await ExecutionTest(
            nameof(Compile_Scripts_Test262Bootstrap),
            addMocks: services => services.RegisterInstance<IEnvironment>(
                new FixedCommandLineEnvironment(
                    "dotnet",
                    "test262-bootstrap.dll",
                    "--describe",
                    "--pin",
                    pinPath)));
    }

    [Fact]
    public Task Compile_Scripts_Test262NativeHostHelpers()
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(Compile_Scripts_Test262NativeHostHelpers),
            "Integration",
            GetJavaScriptAndSourcePath,
            enableIRMetrics: true);

        Test262SharedAssertHarness.AssertNoOutput(
            nameof(Compile_Scripts_Test262NativeHostHelpers),
            result.Output);
        return Task.CompletedTask;
    }

    [Fact]
    public void Compile_Scripts_Test262AgentWorkersShareAtomics()
    {
        const string script = """
            /*--- includes: [atomicsHelper.js] ---*/
            var view = new Int32Array(new SharedArrayBuffer(8));
            for (var i = 0; i < 2; i++) {
              $262.agent.start(`
                $262.agent.receiveBroadcast(function(buffer) {
                  var workerView = new Int32Array(buffer);
                  Atomics.add(workerView, 1, 1);
                  $262.agent.report(Atomics.wait(workerView, 0, 0, 2000));
                  $262.agent.leaving();
                });
              `);
            }
            $262.agent.safeBroadcast(view);
            $262.agent.waitUntil(view, 1, 2);
            $262.agent.tryYield();
            assert.sameValue(Atomics.notify(view, 0, 2), 2);
            assert.sameValue($262.agent.getReport(), 'ok');
            assert.sameValue($262.agent.getReport(), 'ok');
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(Compile_Scripts_Test262AgentWorkersShareAtomics), "Integration",
            _ => (script, null));
        Test262SharedAssertHarness.AssertNoOutput(
            nameof(Compile_Scripts_Test262AgentWorkersShareAtomics), result.Output);
    }

    [Fact]
    public void Compile_Scripts_Test262AgentWorkerFailureIsReported()
    {
        const string script = """
            /*--- includes: [atomicsHelper.js] ---*/
            $262.agent.start("throw new Error('worker failed');");
            $262.agent.getReport();
            """;
        var error = Assert.Throws<InvalidOperationException>(() =>
            Test262SharedAssertHarness.CompileAndExecute(
                nameof(Compile_Scripts_Test262AgentWorkerFailureIsReported),
                "Integration", _ => (script, null)));
        Assert.Contains("worker", error.ToString());
    }

    [Fact]
    public void Compile_Scripts_Test262AgentGetReportWithoutHelperIsNonBlocking()
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(Compile_Scripts_Test262AgentGetReportWithoutHelperIsNonBlocking),
            "Integration",
            _ => ("assert.sameValue($262.agent.getReport(), null);", null));
        Test262SharedAssertHarness.AssertNoOutput(
            nameof(Compile_Scripts_Test262AgentGetReportWithoutHelperIsNonBlocking), result.Output);
    }

    [Fact]
    public void Compile_Scripts_Test262AgentAsyncReport()
    {
        const string script = """
            /*---
            flags: [async]
            includes: [atomicsHelper.js]
            ---*/
            var view = new Int32Array(new SharedArrayBuffer(8));
            $262.agent.start(`
              $262.agent.receiveBroadcast(async function(buffer) {
                var workerView = new Int32Array(buffer);
                Atomics.add(workerView, 1, 1);
                $262.agent.report(await Atomics.waitAsync(workerView, 0, 0, 0).value);
                $262.agent.leaving();
              });
            `);
            $262.agent.safeBroadcastAsync(view, 1, 1)
              .then(async function(count) {
                assert.sameValue(count, 1);
                assert.sameValue(await $262.agent.getReportAsync(), 'timed-out');
              }).then($DONE, $DONE);
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(Compile_Scripts_Test262AgentAsyncReport), "Integration",
            _ => (script, null));
        Test262SharedAssertHarness.AssertNoOutput(
            nameof(Compile_Scripts_Test262AgentAsyncReport), result.Output);
    }

    [Fact]
    public void Compile_Scripts_Test262AgentTimingHelpers()
    {
        const string script = """
            /*---
            flags: [async]
            includes: [atomicsHelper.js]
            ---*/
            assert.sameValue($262.agent.timeouts.yield, 100);
            assert.sameValue($262.agent.timeouts.small, 200);
            assert.sameValue($262.agent.timeouts.long, 1000);
            assert.sameValue($262.agent.timeouts.huge, 10000);
            var before = $262.agent.monotonicNow();
            $262.agent.trySleep(5);
            assert($262.agent.monotonicNow() >= before);
            var called = false;
            $262.agent.setTimeout(function() {
              called = true;
              $DONE();
            }, 0);
            assert.sameValue(called, false);
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            nameof(Compile_Scripts_Test262AgentTimingHelpers), "Integration",
            _ => (script, null));
        Test262SharedAssertHarness.AssertNoOutput(
            nameof(Compile_Scripts_Test262AgentTimingHelpers), result.Output);
    }

    [Theory]
    [InlineData("wait/negative-timeout-agent.js")]
    [InlineData("wait/no-spurious-wakeup-on-exchange.js")]
    [InlineData("waitAsync/negative-timeout-agent.js")]
    [InlineData("notify/notify-one.js")]
    [InlineData("notify/notify-in-order.js")]
    [InlineData("waitAsync/was-woken-before-timeout.js")]
    [InlineData("wait/bigint/negative-timeout-agent.js")]
    [InlineData("notify/bigint/notify-all-on-loc.js")]
    public void Compile_Scripts_Test262PinnedAgentFixtures(string fixture)
    {
        var path = Path.Combine(FindRepositoryRoot(), "artifacts", "test262", "cache",
            "2b2ecead6e828dd9af13a9ec72065e645724a50f", "test", "built-ins",
            "Atomics", fixture);
        if (!File.Exists(path))
            return; // The pinned cache is optional in clean checkouts.
        var result = Test262SharedAssertHarness.CompileAndExecute(
            Path.GetFileNameWithoutExtension(fixture), "Integration",
            _ => (File.ReadAllText(path), path));
        Test262SharedAssertHarness.AssertNoOutput(fixture, result.Output);
    }

    [Fact]
    public void Compile_Scripts_Test262AssertionFailureThrows()
    {
        Assert.Throws<AssertionError>(() =>
        {
            Test262SharedAssertHarness.CompileAndExecute(
                nameof(Compile_Scripts_Test262AssertionFailureThrows),
                "Integration",
                _ => ("assert.sameValue(1, 2, 'intentional failure');", null));
        });
    }

    [Fact]
    public void Compile_Scripts_Test262RuntimeNegativeRejectsWrongErrorType()
    {
        const string script = """
            /*---
            negative:
              phase: runtime
              type: TypeError
            ---*/
            throw new Error('wrong type');
            """;

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            Test262SharedAssertHarness.CompileAndExecute(
                nameof(Compile_Scripts_Test262RuntimeNegativeRejectsWrongErrorType),
                "Integration",
                _ => (script, null),
                allowUnhandledException: true);
        });
        Assert.Contains("expected runtime exception 'TypeError', but got 'Error'", exception.Message);
    }

    private async Task ExecutionTest(
        string testName,
        string[]? additionalScripts = null,
        Action<ServiceContainer>? addMocks = null,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            testName,
            "Integration",
            GetJavaScriptAndSourcePath,
            additionalScripts: additionalScripts,
            enableIRMetrics: true,
            addMocks: addMocks);

        var settings = new VerifySettings(_verifySettings);
        var directory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Could not resolve source directory.");
        var snapshotsDirectory = Path.Combine(directory, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        await Verify(result.Output, settings);
    }

    private static (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName)
    {
        var repoRoot = FindRepositoryRoot();
        var path = testName switch
        {
            nameof(Compile_Scripts_Test262MetadataParser) => Path.Combine(
                repoRoot,
                "tests",
                "Jroc.Test262.Tests",
                "Integration",
                "JavaScript",
                "test262MetadataParser_testHarness.js"),
            nameof(Compile_Scripts_Test262Bootstrap) => Path.Combine(repoRoot, "scripts", "test262", "bootstrap.js"),
            nameof(Compile_Scripts_Test262NativeHostHelpers) => Path.Combine(
                repoRoot,
                "tests",
                "Jroc.Test262.Tests",
                "Integration",
                "JavaScript",
                "test262NativeHostHelpers.js"),
            "test262/metadataParser" => Path.Combine(repoRoot, "scripts", "test262", "metadataParser.js"),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown integration test script.")
        };

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"JavaScript fixture not found at '{path}'.", path);
        }

        return (File.ReadAllText(path), testName == "test262/metadataParser" ? null : path);
    }

    private static string FindRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "jroc.sln")))
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new InvalidOperationException("Unable to locate repository root.");
    }
}
