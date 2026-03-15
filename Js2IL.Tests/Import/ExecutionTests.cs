using Xunit;

namespace Js2IL.Tests.Import
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Import")
        {
        }

        [Fact]
        public Task Import_BasicImport()
        {
            var testName = nameof(Import_BasicImport);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_StaticImport_FromCjs()
        {
            var testName = nameof(Import_StaticImport_FromCjs);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_RequireEsmModule()
        {
            var testName = nameof(Import_RequireEsmModule);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_ExportNamedFrom()
        {
            var testName = nameof(Import_ExportNamedFrom);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_ExportStarFrom()
        {
            var testName = nameof(Import_ExportStarFrom);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_LiveBindings_Named()
        {
            var testName = nameof(Import_LiveBindings_Named);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_LiveBindings_Cycle()
        {
            var testName = nameof(Import_LiveBindings_Cycle);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_Namespace_Esm_Basic()
        {
            var testName = nameof(Import_Namespace_Esm_Basic);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_Namespace_FromCjs_Stable()
        {
            var testName = nameof(Import_Namespace_FromCjs_Stable);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_DynamicImport_Esm_Namespace()
        {
            var testName = nameof(Import_DynamicImport_Esm_Namespace);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_DynamicImport_Cjs_Namespace()
        {
            var testName = nameof(Import_DynamicImport_Cjs_Namespace);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Import_ImportMeta_Url()
        {
            var testName = nameof(Import_ImportMeta_Url);
            return ExecutionTest(testName);
        }
    }
}
