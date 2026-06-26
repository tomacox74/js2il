using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
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
