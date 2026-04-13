using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using Js2IL.Tests;
using Js2IL.Tests.Integration;

namespace Js2IL.Test262.Tests.Integration;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("Integration") { }

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
              "localOverrideEnvVar": "JS2IL_TEST262_ROOT",
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
}
