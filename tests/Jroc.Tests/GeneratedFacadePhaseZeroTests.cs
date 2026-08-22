using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Jroc.Runtime;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseZeroTests
{
    [Fact]
    public void ExplicitAssemblyIdentity_DrivesMetadataArtifactsPdbAndFacadeRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "PhaseZero", Guid.NewGuid().ToString("N"));
        try
        {
            var artifact = JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest(Path.Combine(root, "source-file.js"))
                {
                    AssemblyName = "Host-Friendly.Package",
                    SourceText = "console.log('hello');",
                    EmitPdb = true
                });

            Assert.Equal("Host-Friendly.Package", artifact.AssemblyName);
            Assert.Equal("Host_Friendly_Package", artifact.FacadeNames!.RootTypeName);

            using (var peReader = new PEReader(new MemoryStream(artifact.PeBytes)))
            {
                var metadata = peReader.GetMetadataReader();
                Assert.Equal(
                    "Host-Friendly.Package",
                    metadata.GetString(metadata.GetAssemblyDefinition().Name));
                var codeViewEntry = Assert.Single(
                    peReader.ReadDebugDirectory(),
                    entry => entry.Type == DebugDirectoryEntryType.CodeView);
                Assert.Equal(
                    "Host-Friendly.Package.pdb",
                    peReader.ReadCodeViewDebugDirectoryData(codeViewEntry).Path);
            }

            var materialized = artifact.Materialize(root);
            Assert.Equal("Host-Friendly.Package.dll", Path.GetFileName(materialized.AssemblyPath));
            Assert.Equal("Host-Friendly.Package.pdb", Path.GetFileName(materialized.PdbPath));
            Assert.Equal(
                "Host-Friendly.Package.runtimeconfig.json",
                Path.GetFileName(materialized.RuntimeConfigPath));
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); } catch { }
        }
    }

    [Fact]
    public void OmittedAssemblyIdentity_PreservesSourceFilenameDefault()
    {
        var artifact = JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest("/virtual/legacy-name.js")
            {
                SourceText = "console.log('hello');"
            });

        Assert.Equal("legacy-name", artifact.AssemblyName);
        Assert.Equal("legacy_name", artifact.FacadeNames!.RootTypeName);
    }

    [Theory]
    [InlineData("../escape")]
    [InlineData("trailing.")]
    [InlineData("trailing ")]
    [InlineData("line\nbreak")]
    public void InvalidAssemblyIdentity_IsRejected(string assemblyName)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest("/virtual/entry.js")
                {
                    AssemblyName = assemblyName,
                    SourceText = "console.log('hello');"
                }));

        Assert.Contains("not a valid portable assembly", exception.Message);
    }

    [Fact]
    public void EntryModuleMetadata_UsesCanonicalIdentityAndPreservesAliases()
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "PhaseZero", Guid.NewGuid().ToString("N"));
        var entryPath = Path.Combine(root, "entry.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, "require('./dependency.js'); module.exports = 42;");
        fileSystem.AddFile(Path.Combine(root, "dependency.js"), "module.exports = 'dependency';");

        var artifact = JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest(entryPath)
            {
                FileSystem = fileSystem,
                RootModuleIdOverride = "sample-package"
            });

        Assert.Equal("entry", artifact.EntryModuleId);
        Assert.Contains("sample-package", artifact.EntryModuleAliases!);
        Assert.Contains("dependency", artifact.ModuleIds);

        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        Assert.Equal("entry", loaded.EntryModuleId);
        Assert.Equal("entry", JsEngine.GetEntryModuleId(loaded.Assembly));
        Assert.Single(loaded.Assembly.GetCustomAttributes<JsCompiledEntryModuleAttribute>());
        var alias = Assert.Single(
            loaded.Assembly.GetCustomAttributes<JsCompiledModuleTypeAttribute>(),
            attribute => attribute.ModuleId == "sample-package");
        Assert.Equal("entry", alias.CanonicalModuleId);
    }

    [Fact]
    public void CompileAndLoad_WithMultipleModules_DefaultsToRecordedEntryModule()
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "PhaseZero", Guid.NewGuid().ToString("N"));
        var entryPath = Path.Combine(root, "entry.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(
            entryPath,
            "const dependency = require('./dependency.js'); exports.value = dependency + 1;");
        fileSystem.AddFile(Path.Combine(root, "dependency.js"), "module.exports = 41;");

        using var module = JrocInMemoryCompiler.CompileAndLoadModule(
            new JrocInMemoryCompileRequest(entryPath) { FileSystem = fileSystem });

        dynamic exports = module.Exports;
        Assert.Equal(42d, (double)exports.value);
        Assert.Equal("entry", module.EntryModuleId);
    }

    [Fact]
    public void HostingDiscovery_RejectsMissingOrAmbiguousEntryModuleMetadata()
    {
        Assert.Throws<InvalidOperationException>(
            () => JsEngine.GetEntryModuleId(typeof(GeneratedFacadePhaseZeroTests).Assembly));

        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("AmbiguousEntryModules"),
            AssemblyBuilderAccess.Run);
        var constructor = typeof(JsCompiledEntryModuleAttribute).GetConstructor([typeof(string)])!;
        assembly.SetCustomAttribute(new CustomAttributeBuilder(constructor, ["first"]));
        assembly.SetCustomAttribute(new CustomAttributeBuilder(constructor, ["second"]));

        var exception = Assert.Throws<InvalidOperationException>(
            () => JsEngine.GetEntryModuleId(assembly));
        Assert.Contains("exactly one", exception.Message);
    }

    [Fact]
    public void FacadeNaming_MapsScopedPackagesKeywordsExtensionsAndDeepPaths()
    {
        var plan = JrocFacadeNamePlanner.Create(
            "mixmark-io.domino",
            "@mixmark-io/domino/index.js",
            [
                "@mixmark-io/domino/index.js",
                "@mixmark-io/domino/api.js",
                "@mixmark-io/domino/api/css.mjs",
                "@mixmark-io/domino/class.cjs",
                "@mixmark-io/domino/123 tools.js"
            ],
            "@mixmark-io/domino");

        Assert.Equal("mixmark_io_domino", plan.RootTypeName);
        Assert.Equal(
            "mixmark_io_domino",
            JrocFacadeNamePlanner.NormalizeIdentifier("@mixmark-io/domino", stripLeadingAtSign: true));
        Assert.Equal(["index"], FindPath(plan, "@mixmark-io/domino/index.js"));
        Assert.Equal(["api"], FindPath(plan, "@mixmark-io/domino/api.js"));
        Assert.Equal(["api", "css"], FindPath(plan, "@mixmark-io/domino/api/css.mjs"));
        Assert.Equal(["_class"], FindPath(plan, "@mixmark-io/domino/class.cjs"));
        Assert.Equal(["_123_tools"], FindPath(plan, "@mixmark-io/domino/123 tools.js"));
    }

    [Fact]
    public void FacadeNaming_AllowsModuleAndDirectoryDuality()
    {
        var plan = JrocFacadeNamePlanner.Create(
            "DualityAssembly",
            "api.js",
            ["api.js", "api/css.js"]);

        Assert.Equal(["api"], FindPath(plan, "api.js"));
        Assert.Equal(["api", "css"], FindPath(plan, "api/css.js"));
    }

    [Theory]
    [InlineData("api-one.js", "api_one.js")]
    [InlineData("api.js", "API.js")]
    [InlineData("some path/a.js", "some-path/b.js")]
    public void FacadeNaming_RejectsOrdinalAndCaseInsensitiveCollisions(
        string firstModuleId,
        string secondModuleId)
    {
        var exception = Assert.Throws<JrocFacadeNameCollisionException>(
            () => JrocFacadeNamePlanner.Create(
                "CollisionAssembly",
                firstModuleId,
                [firstModuleId, secondModuleId]));

        Assert.Contains(exception.FirstModuleId, new[] { firstModuleId, secondModuleId });
        Assert.Contains(exception.SecondModuleId, new[] { firstModuleId, secondModuleId });
        Assert.NotEqual(exception.FirstModuleId, exception.SecondModuleId);
        Assert.Contains("CollisionAssembly.Scripts", exception.ProposedClrPath);
    }

    [Fact]
    public void ConsumerHarness_BuildsReferencesReflectsAndRunsGeneratedAssembly()
    {
        string workingDirectory;
        using (var harness = new GeneratedAssemblyConsumerHarness(
                   "console.log('hello from generated assembly');",
                   "ConsumerFixture"))
        {
            workingDirectory = harness.WorkingDirectory;
            Assert.Equal("entry", harness.Artifact.EntryModuleId);

            var result = harness.Build(
                """
                using System.Reflection;

                var assembly = Assembly.Load("ConsumerFixture");
                Console.WriteLine($"assembly={assembly.GetName().Name}");
                assembly.EntryPoint!.Invoke(null, null);
                """,
                run: true);

            Assert.Equal(0, result.BuildExitCode);
            Assert.True(
                result.RunExitCode == 0,
                $"Consumer failed.{Environment.NewLine}{result.RunStandardOutput}{Environment.NewLine}{result.RunStandardError}");
            Assert.Contains("assembly=ConsumerFixture", result.RunStandardOutput);
            Assert.Contains("hello from generated assembly", result.RunStandardOutput);
        }

        Assert.False(Directory.Exists(workingDirectory));
    }

    [Fact]
    public void ConsumerHarness_CapturesCompilerRejectionsAndCleansFailureArtifacts()
    {
        string workingDirectory;
        using (var harness = new GeneratedAssemblyConsumerHarness(
                   "console.log('fixture');"))
        {
            workingDirectory = harness.WorkingDirectory;
            var result = harness.Build("MissingGeneratedFacade.Run();");

            Assert.NotEqual(0, result.BuildExitCode);
            Assert.Contains("CS0103", result.BuildDiagnostics);
        }

        Assert.False(Directory.Exists(workingDirectory));
    }

    [Theory]
    [InlineData("using Jroc.Runtime;")]
    [InlineData("var runtime = typeof(JavaScriptRuntime.ObjectRuntime);")]
    public void ConsumerHarness_RejectsDirectRuntimeReferences(string consumerSource)
    {
        using var harness = new GeneratedAssemblyConsumerHarness("console.log('fixture');");

        var exception = Assert.Throws<InvalidOperationException>(
            () => harness.Build(consumerSource));

        Assert.Contains("must not directly reference", exception.Message);
    }

    private static IReadOnlyList<string> FindPath(JrocFacadeNamePlan plan, string moduleId) =>
        Assert.Single(plan.Modules, module => module.ModuleId == moduleId).TypePath;
}
