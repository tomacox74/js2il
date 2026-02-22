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
    }
}
