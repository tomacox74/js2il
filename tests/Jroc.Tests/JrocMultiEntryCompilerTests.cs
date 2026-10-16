using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using Jroc.Runtime;

namespace Jroc.Tests;

public sealed class JrocMultiEntryCompilerTests
{
    [Fact]
    public void VirtualEntries_CompileIntoOneLoadableArtifactAndRunIndependently()
    {
        var directory = NewVirtualDirectory();
        var firstPath = Path.Combine(directory, "first.js");
        var secondPath = Path.Combine(directory, "second.js");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(firstPath, "const value = 1; globalThis.marker = 'first'; exports.value = value;"),
            new(secondPath, "const value = 2; exports.value = value; exports.marker = typeof globalThis.marker;")
        ])
        {
            AssemblyName = "TwoVirtualEntries",
            DefaultEntryFilePath = secondPath,
            EmitPdb = true
        });

        Assert.Equal(["first", "second"], artifact.EntryModules.Select(entry => entry.ModuleId));
        Assert.Equal([firstPath, secondPath], artifact.EntryModules.Select(entry => entry.SourcePath));
        Assert.Equal(["first", "second"], artifact.ModuleIds);
        Assert.Equal("second", artifact.EntryModuleId);
        Assert.NotEmpty(artifact.PeBytes);
        Assert.NotEmpty(artifact.PdbBytes!);
        Assert.False(Directory.Exists(directory));

        using var pdbStream = new MemoryStream(artifact.PdbBytes!, writable: false);
        using var pdbProvider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdb = pdbProvider.GetMetadataReader();
        var documents = pdb.Documents.Select(handle => pdb.GetString(pdb.GetDocument(handle).Name)).ToArray();
        Assert.Contains(firstPath, documents);
        Assert.Contains(secondPath, documents);

        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        Assert.Equal(string.Empty, loaded.Assembly.Location);
        Assert.Equal("second", JsEngine.GetEntryModuleId(loaded.Assembly));
        RunFacade(loaded.Assembly, artifact, "first");
        RunFacade(loaded.Assembly, artifact, "second");
        RunFacade(loaded.Assembly, artifact, null);

        using var first = JsEngine.LoadDynamicModule(loaded.Assembly, artifact.EntryModules[0].ModuleId);
        using var second = JsEngine.LoadDynamicModule(loaded.Assembly, artifact.EntryModules[1].ModuleId);
        Assert.Equal(1d, first.Get("value"));
        Assert.Equal(2d, second.Get("value"));
        Assert.Equal("undefined", second.Get("marker"));
    }

    [Fact]
    public void FileEntries_IncludeSharedDependencyOnceAndOverlayAnExistingFile()
    {
        var directory = NewVirtualDirectory();
        var firstDirectory = Path.Combine(directory, "a");
        var secondDirectory = Path.Combine(directory, "b");
        Directory.CreateDirectory(firstDirectory);
        Directory.CreateDirectory(secondDirectory);

        try
        {
            var firstPath = Path.Combine(firstDirectory, "entry.js");
            var secondPath = Path.Combine(secondDirectory, "entry.js");
            File.WriteAllText(firstPath, "const value = require('../shared.js'); exports.value = value + 1;");
            File.WriteAllText(secondPath, "throw new Error('this must be overlaid');");
            File.WriteAllText(Path.Combine(directory, "shared.js"), "module.exports = 40;");

            var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath),
                new(secondPath, "const value = require('../shared.js'); exports.value = value + 2;")
            ]));

            Assert.Equal("entry", artifact.AssemblyName);
            Assert.Equal(["a/entry", "b/entry"], artifact.EntryModules.Select(entry => entry.ModuleId));
            Assert.Equal(3, artifact.ModuleIds.Count);
            Assert.Equal(1, artifact.ModuleIds.Count(id => id == "shared"));
            using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
            using var first = JsEngine.LoadDynamicModule(loaded.Assembly, "a/entry");
            using var second = JsEngine.LoadDynamicModule(loaded.Assembly, "b/entry");
            Assert.Equal(41d, first.Get("value"));
            Assert.Equal(42d, second.Get("value"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void EntryModuleOverrides_ArePublishedWithoutChangingCanonicalEntryIds()
    {
        var directory = NewVirtualDirectory();
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(Path.Combine(directory, "a.js"), "exports.value = 1;", "host/a"),
            new(Path.Combine(directory, "b.js"), "exports.value = 2;", "host/b")
        ]));

        Assert.Equal(["a", "b"], artifact.EntryModules.Select(entry => entry.ModuleId));
        Assert.Contains("host/a", artifact.ModuleIds);
        Assert.Contains("host/b", artifact.ModuleIds);
        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        using var second = JsEngine.LoadDynamicModule(loaded.Assembly, "host/b");
        Assert.Equal(2d, second.Get("value"));
    }

    [Fact]
    public void UnreachableEntry_IsCompiledButDoesNotRunWithDefaultEntry()
    {
        var directory = NewVirtualDirectory();
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(Path.Combine(directory, "safe.js"), "const value = 1; exports.value = value;"),
            new(Path.Combine(directory, "unreachable.js"), "throw new Error('unreachable entry was executed');")
        ]));

        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        RunFacade(loaded.Assembly, artifact, null);
        RunFacade(loaded.Assembly, artifact, "safe");
        Assert.Contains("unreachable", Assert.Throws<TargetInvocationException>(
            () => RunFacade(loaded.Assembly, artifact, "unreachable")).InnerException!.Message);
    }

    [Fact]
    public void SharedFileSystem_ResolvesVirtualDependenciesRelativeToEachEntry()
    {
        var directory = NewVirtualDirectory();
        var firstPath = Path.Combine(directory, "first", "entry.js");
        var secondPath = Path.Combine(directory, "second", "entry.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(firstPath, "exports.value = require('./dependency.js');");
        fileSystem.AddFile(Path.Combine(directory, "first", "dependency.js"), "module.exports = 10;");
        fileSystem.AddFile(Path.Combine(directory, "second", "dependency.js"), "module.exports = 20;");

        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(firstPath),
            new(secondPath, "exports.value = require('./dependency.js');")
        ])
        {
            FileSystem = fileSystem
        });

        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        using var first = JsEngine.LoadDynamicModule(loaded.Assembly, "first/entry");
        using var second = JsEngine.LoadDynamicModule(loaded.Assembly, "second/entry");
        Assert.Equal(10d, first.Get("value"));
        Assert.Equal(20d, second.Get("value"));
        Assert.False(Directory.Exists(directory));
    }

    [Fact]
    public void IndependentEsmEntries_LinkToOneSharedDependency()
    {
        var directory = NewVirtualDirectory();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(Path.Combine(directory, "shared.js"), "export const value = 40;");

        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(Path.Combine(directory, "a", "entry.js"),
                "import { value } from '../shared.js'; export const result = value + 1;"),
            new(Path.Combine(directory, "b", "entry.js"),
                "import { value } from '../shared.js'; export const result = value + 2;")
        ])
        {
            FileSystem = fileSystem
        });

        Assert.Equal(1, artifact.ModuleIds.Count(id => id == "shared"));
        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        using var first = JsEngine.LoadDynamicModule(loaded.Assembly, "a/entry");
        using var second = JsEngine.LoadDynamicModule(loaded.Assembly, "b/entry");
        Assert.Equal(41d, first.Get("result"));
        Assert.Equal(42d, second.Get("result"));
    }

    [Fact]
    public async Task SelectedDefaultEntry_RunsWhenAssemblyIsLaunched()
    {
        var directory = NewVirtualDirectory();
        var outputDirectory = Path.Combine(directory, "output");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
        [
            new(Path.Combine(directory, "first.js"), "console.log('first');"),
            new(Path.Combine(directory, "second.js"), "console.log('second');")
        ])
        {
            AssemblyName = "MultiEntryLaunch",
            DefaultEntryFilePath = Path.Combine(directory, "second.js")
        });

        try
        {
            var materialized = artifact.Materialize(outputDirectory);
            var startInfo = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(materialized.AssemblyPath);

            using var process = Process.Start(startInfo)!;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
                var error = process.StandardError.ReadToEndAsync(timeout.Token);
                await process.WaitForExitAsync(timeout.Token);
                Assert.Equal(0, process.ExitCode);
                Assert.Equal("second", (await output).Trim());
                Assert.Equal(string.Empty, await error);
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void DuplicatePathsAndModuleIds_FailWithBothSourceIdentities()
    {
        var directory = NewVirtualDirectory();
        var firstPath = Path.Combine(directory, "one.js");
        var secondPath = Path.Combine(directory, "two.js");

        var duplicatePath = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath),
                new(Path.Combine(directory, ".", "one.js"))
            ])));
        Assert.Contains("duplicates", duplicatePath.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(firstPath, duplicatePath.Message);

        var duplicateId = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath, "exports.value = 1;", "shared-id"),
                new(secondPath, "exports.value = 2;", "shared-id")
            ])));
        Assert.Contains(firstPath, duplicateId.Message);
        Assert.Contains(secondPath, duplicateId.Message);
        Assert.Contains("shared-id", duplicateId.Message);

        var invalidOverride = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath, "exports.value = 1;", "./relative"),
                new(secondPath, "exports.value = 2;")
            ])));
        Assert.Contains(firstPath, invalidOverride.Message);
        Assert.Contains("./relative", invalidOverride.Message);
    }

    [Fact]
    public void InvalidDefaultMissingEntryAndInvalidSecondSource_FailWithoutPartialArtifact()
    {
        var directory = NewVirtualDirectory();
        var firstPath = Path.Combine(directory, "first.js");
        var secondPath = Path.Combine(directory, "second.js");

        Assert.Throws<ArgumentException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest([])));

        var invalidDefault = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
                [new(firstPath, "exports.value = 1;")])
            {
                DefaultEntryFilePath = secondPath
            }));
        Assert.Contains(secondPath, invalidDefault.Message);

        var missing = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath, "exports.value = 1;"),
                new(secondPath)
            ])));
        Assert.Contains(secondPath, missing.Message);

        var invalidSource = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath, "exports.value = 1;"),
                new(secondPath, "exports.value = ;")
            ])));
        Assert.Contains(secondPath, invalidSource.Message);
        Assert.Contains("Parse Errors:", invalidSource.Message);

        var invalidFeature = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(firstPath, "exports.value = 1;"),
                new(secondPath, "eval('1');")
            ])));
        Assert.Contains(secondPath, invalidFeature.Message);
        Assert.Contains("eval", invalidFeature.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FacadeNameCollision_ReportsBothConflictingModuleIds()
    {
        var directory = NewVirtualDirectory();
        var error = Assert.Throws<InvalidOperationException>(() =>
            JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
            [
                new(Path.Combine(directory, "a-b.js"), "exports.value = 1;"),
                new(Path.Combine(directory, "a_b.js"), "exports.value = 2;")
            ])));

        Assert.Contains("a-b", error.Message);
        Assert.Contains("a_b", error.Message);
        Assert.Contains("collid", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AdditionalModulePaths_BundlesComputedTargetsWithoutRunningUnrequestedModules(bool multiEntry)
    {
        var directory = NewVirtualDirectory();
        var entryPath = Path.Combine(directory, "entry.js");
        var targetPath = Path.Combine(directory, "target.js");
        var unusedPath = Path.Combine(directory, "unused.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(targetPath, "globalThis.additionalMarker = 42; export const value = 42;");
        fileSystem.AddFile(unusedPath, "throw new Error('unrequested module executed');");
        const string source = """
            export const before = typeof globalThis.additionalMarker;
            const specifier = './target.js';
            export const value = (await import(specifier)).value;
            export const after = globalThis.additionalMarker;
            """;
        var artifact = multiEntry
            ? JrocInMemoryCompiler.Compile(new JrocInMemoryMultiEntryCompileRequest(
                [new(entryPath, source) { ParseAsModule = true }])
            {
                FileSystem = fileSystem,
                AdditionalModulePaths = [targetPath, unusedPath]
            })
            : JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = source,
                ParseAsModule = true,
                FileSystem = fileSystem,
                AdditionalModulePaths = [targetPath, unusedPath]
            });

        Assert.Single(artifact.EntryModules);
        Assert.Contains("target", artifact.ModuleIds);
        Assert.Contains("unused", artifact.ModuleIds);
        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        using var exports = JsEngine.LoadDynamicModule(loaded.Assembly, artifact.EntryModuleId);
        Assert.Equal("undefined", exports.Get("before"));
        Assert.Equal(42d, exports.Get("value"));
        Assert.Equal(42d, exports.Get("after"));
    }

    [Theory]
    [InlineData("export const = ;", "SyntaxError", "target.js")]
    [InlineData("import { missing } from './dependency.js';", "SyntaxError", "target.js")]
    [InlineData("import './missing.js';", "ReferenceError", "target.js")]
    [InlineData("{ \"invalid\":", "SyntaxError", "target.json")]
    public void AdditionalModulePaths_DefersModuleErrorsUntilImport(
        string targetSource, string expectedName, string targetFileName)
    {
        var directory = NewVirtualDirectory();
        var targetPath = Path.Combine(directory, targetFileName);
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(targetPath, targetSource);
        fileSystem.AddFile(Path.Combine(directory, "dependency.js"), "export const value = 1;");
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(Path.Combine(directory, "entry.js"))
        {
            SourceText = $"""
                const specifier = './{targetFileName}';
                export const errorName = await import(specifier).then(
                    () => 'unexpected success', error => error.name);
                """,
            ParseAsModule = true,
            FileSystem = fileSystem,
            AdditionalModulePaths = [targetPath]
        });

        using var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        using var exports = JsEngine.LoadDynamicModule(loaded.Assembly, artifact.EntryModuleId);
        Assert.Equal(expectedName, exports.Get("errorName"));

        Assert.Throws<InvalidOperationException>(() => JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest(Path.Combine(directory, "eager.js"))
            {
                SourceText = $"import './{targetFileName}';",
                ParseAsModule = true,
                FileSystem = fileSystem
            }));
    }

    [Theory]
    [InlineData(" \n{\n \"value\": 262\n}\n", true)]
    [InlineData("not JSON\n", false)]
    public void AdditionalModulePaths_PreservesRawTextAndDistinctJsonIdentity(string jsonSource, bool validJson)
    {
        var directory = NewVirtualDirectory();
        var jsonPath = Path.Combine(directory, "data.json");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(jsonPath, jsonSource);
        fileSystem.AddFile(Path.Combine(directory, "data.js"), "export default 'JavaScript';");
        using var module = JrocInMemoryCompiler.CompileAndLoadModule(
            new JrocInMemoryCompileRequest(Path.Combine(directory, "entry.js"))
            {
                SourceText = """
                    const specifier = './data.json';
                    const text = await import(specifier, { with: { type: 'text' } });
                    export const content = text.default;
                    export const cached = text === await import(specifier, { with: { type: 'text' } });
                    export const javascript = (await import('./data.js')).default;
                    export const json = await import(specifier, { with: { type: 'json' } }).then(
                        ns => ns.default.value, error => error.name);
                    """,
                ParseAsModule = true,
                FileSystem = fileSystem,
                AdditionalModulePaths = [jsonPath]
            });

        var exports = Assert.IsType<JsDynamicExports>(module.Exports);
        Assert.Equal(jsonSource, exports.Get("content"));
        Assert.Equal(true, exports.Get("cached"));
        Assert.Equal("JavaScript", exports.Get("javascript"));
        if (validJson)
            Assert.Equal(262d, exports.Get("json"));
        else
            Assert.Equal("SyntaxError", exports.Get("json"));
    }

    [Fact]
    public void AdditionalModulePaths_PreservesCommonJsWrapperParsing()
    {
        var directory = NewVirtualDirectory();
        var targetPath = Path.Combine(directory, "target.cjs");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(targetPath, "module.exports = { value: 42 }; return;");
        using var module = JrocInMemoryCompiler.CompileAndLoadModule(
            new JrocInMemoryCompileRequest(Path.Combine(directory, "entry.mjs"))
            {
                SourceText = "const path = './target.cjs'; export const value = (await import(path)).default.value;",
                FileSystem = fileSystem,
                AdditionalModulePaths = [targetPath]
            });
        Assert.Equal(42d, Assert.IsType<JsDynamicExports>(module.Exports).Get("value"));
    }

    private static string NewVirtualDirectory() =>
        Path.Combine(Path.GetTempPath(), "Jroc.Tests", "MultiEntry", Guid.NewGuid().ToString("N"));

    private static void RunFacade(Assembly assembly, JrocCompiledAssemblyArtifact artifact, string? moduleId)
    {
        var type = assembly.GetType(artifact.FacadeNames!.RootTypeName, throwOnError: true)!;
        if (moduleId is not null)
        {
            type = type.GetNestedType("Scripts", BindingFlags.Public)!;
            var module = Assert.Single(artifact.FacadeNames.Modules, candidate => candidate.ModuleId == moduleId);
            foreach (var name in module.TypePath)
            {
                type = type.GetNestedType(name, BindingFlags.Public)!;
            }
        }

        type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [System.Array.Empty<string>()]);
    }
}
