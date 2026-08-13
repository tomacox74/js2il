using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;
using Jroc.Tests.Utilities;
using Xunit;

namespace Jroc.Tests.Import
{
    /// <summary>
    /// Focused coverage for issue #1796: static ES modules are lowered natively (runtime binding cells
    /// + direct <c>EsModuleLinker</c> calls) instead of by injecting JavaScript helper functions and
    /// export getter closures. These tests assert the generated IL carries no <c>__jroc_esm</c>
    /// helper function/type overhead — for every supported static ESM form — while live-binding runtime
    /// behavior is preserved.
    /// </summary>
    public class NativeEsmOverheadTests
    {
        private const string TestCategory = "Import";

        /// <summary>
        /// Every supported static ESM syntax must compile without any injected <c>__jroc_esm</c>
        /// helper source. Each entry pulls in a module graph that exercises the named form:
        /// local named/default/live exports, named imports, indirect exports
        /// (<c>export { x as y } from</c>), star exports (<c>export * from</c>) including multi-hop,
        /// namespace re-exports (<c>export * as ns from</c>), and named/anonymous <c>export default</c>
        /// function, class, and expression forms. The compiled assembly contains every module in the
        /// graph, so a single scan proves the whole graph is lowered natively.
        /// </summary>
        [Theory]
        [InlineData("Import_LiveBindings_Named")]
        [InlineData("Import_ExportNamedFrom")]
        [InlineData("Import_ExportRenamedFrom")]
        [InlineData("Import_ExportStarFrom")]
        [InlineData("Import_ExportStarFromMultiHop")]
        [InlineData("Import_ExportStarAsNamespace")]
        [InlineData("Import_DefaultAnonFunction")]
        [InlineData("Import_DefaultAnonClass")]
        [InlineData("Import_DefaultNamedClass")]
        [InlineData("Import_SideEffectImport")]
        [InlineData("Import_MixedDeclOrder")]
        [InlineData("Import_ReexportAsDefault")]
        [InlineData("Import_RequireEsmModule")]
        [InlineData("Import_Namespace_Esm_Basic")]
        public void NativeStaticEsm_GeneratesNoInjectedHelperSource(string testName)
        {
            var compiled = CompileImportFixture(testName);
            var il = AssemblyToText.ConvertToText(compiled.Artifact.PeBytes, compiled.Artifact.AssemblyName);

            // No injected JavaScript helper functions/types/state remain anywhere in the module graph.
            Assert.DoesNotContain("__jroc_esm", il);

            // Static ESM linking is realized through the native runtime linker, not getter closures.
            Assert.Contains("EsModuleLinker", il);

            // Reflect over the emitted assembly: no generated type should carry the helper name.
            using var loaded = global::Jroc.JrocInMemoryAssemblyLoader.Load(compiled.Artifact);
            Assert.DoesNotContain(
                loaded.Assembly.GetTypes(),
                type => (type.FullName ?? string.Empty).Contains("__jroc_esm", StringComparison.Ordinal));
        }

        [Fact]
        public void NativeStaticEsm_GeneratesNoInjectedHelperFunctionOrTypeOverhead()
        {
            var compiled = CompileImportFixture("Import_LiveBindings_Named");
            var il = AssemblyToText.ConvertToText(compiled.Artifact.PeBytes, compiled.Artifact.AssemblyName);

            // No injected JavaScript helper functions/types remain in the generated assembly.
            Assert.DoesNotContain("__jroc_esm", il);

            // Static ESM is lowered through native runtime binding cells, not getter closures.
            Assert.Contains("EsModuleLinker::RegisterLocalExport", il);
            Assert.Contains("EsModuleLinker::SetLocalExport", il);
            Assert.Contains("EsModuleLinker::MarkEsModule", il);

            // Reflect over the emitted assembly: no generated type should carry the helper name.
            using var loaded = global::Jroc.JrocInMemoryAssemblyLoader.Load(compiled.Artifact);
            Assert.DoesNotContain(
                loaded.Assembly.GetTypes(),
                type => (type.FullName ?? string.Empty).Contains("__jroc_esm", StringComparison.Ordinal));
        }

        [Fact]
        public void NativeStaticEsm_IndirectExport_LowersToLiveReexport()
        {
            var compiled = CompileImportFixture("Import_ExportRenamedFrom");
            var il = AssemblyToText.ConvertToText(compiled.Artifact.PeBytes, compiled.Artifact.AssemblyName);

            Assert.DoesNotContain("__jroc_esm", il);
            // `export { x as y } from "mod"` installs a live forwarding accessor via RegisterReexport.
            Assert.Contains("EsModuleLinker::RegisterReexport", il);
        }

        [Fact]
        public void NativeStaticEsm_StarExport_LowersToStarReexport()
        {
            var compiled = CompileImportFixture("Import_ExportStarFrom");
            var il = AssemblyToText.ConvertToText(compiled.Artifact.PeBytes, compiled.Artifact.AssemblyName);

            Assert.DoesNotContain("__jroc_esm", il);
            // `export * from "mod"` enumerates + forwards the source module's exports at init.
            Assert.Contains("EsModuleLinker::RegisterStarReexports", il);
        }

        [Fact]
        public void NativeStaticEsm_NamespaceReexport_LowersToNamespaceAccessor()
        {
            var compiled = CompileImportFixture("Import_ExportStarAsNamespace");
            var il = AssemblyToText.ConvertToText(compiled.Artifact.PeBytes, compiled.Artifact.AssemblyName);

            Assert.DoesNotContain("__jroc_esm", il);
            // `export * as ns from "mod"` installs a stable namespace accessor via RegisterNamespaceReexport.
            Assert.Contains("EsModuleLinker::RegisterNamespaceReexport", il);
        }

        [Fact]
        public void NativeStaticEsm_PreservesLiveBindingExecutionBehavior()
        {
            const string testName = "Import_LiveBindings_Named";
            var compiled = CompileImportFixture(testName);

            var result = InMemoryTestCompiler.ExecuteArtifact(
                compiled.Artifact,
                compiled.TestFilePath,
                testName);

            // The imported live binding reflects the exporter's post-increment update.
            Assert.Contains("x0: 1", result.Output);
            Assert.Contains("x1: 2", result.Output);
        }

        [Fact]
        public void ForIn_DoesNotTreatCommonJsEsModuleFlagAsNamespaceMarker()
        {
            var getterCalls = 0;
            var commonJsExports = new JsObject();
            var esModuleDescriptor = new JsObject();
            esModuleDescriptor.SetBoolean("value", true);
            esModuleDescriptor.SetBoolean("enumerable", false);
            ObjectRuntime.defineProperty(commonJsExports, "__esModule", esModuleDescriptor);
            ObjectRuntime.DefineObjectLiteralAccessorProperty(
                commonJsExports,
                "forwarded",
                () =>
                {
                    getterCalls++;
                    return 42d;
                },
                null);

            var iterator = new ForInIterator(commonJsExports);
            var result = iterator.Next();

            Assert.Equal("forwarded", result.value);
            Assert.False(result.done);
            Assert.Equal(0, getterCalls);
        }

        private static CompiledAssembly CompileImportFixture(
            string testName,
            [CallerFilePath] string sourceFilePath = "")
        {
            var testDirectory = Path.GetDirectoryName(sourceFilePath)
                ?? throw new InvalidOperationException("Unable to resolve the test source directory.");
            var javaScriptDirectory = Path.Combine(testDirectory, "JavaScript");

            (string Script, string? SourcePath) ResolveScript(string name)
            {
                var scriptPath = Path.Combine(javaScriptDirectory, name + ".js");
                return (File.ReadAllText(scriptPath), scriptPath);
            }

            // The mock file system falls back to real disk, so sibling module files (e.g. *.mjs libraries)
            // referenced by the entry module resolve automatically without being listed explicitly.
            var logicalOutputDirectory = Path.Combine(testDirectory, "obj", "NativeEsmOverheadProbe", testName);
            return TestCompiler.Compile(
                testName,
                TestCategory,
                logicalOutputDirectory,
                ResolveScript,
                additionalScripts: null,
                enableIRMetrics: true);
        }
    }
}
