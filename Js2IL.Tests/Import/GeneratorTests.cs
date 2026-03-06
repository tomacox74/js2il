using Xunit;

namespace Js2IL.Tests.Import
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Import")
        {
        }

        [Fact]
        public Task Import_BasicImport()
        {
            var testName = nameof(Import_BasicImport);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_StaticImport_FromCjs()
        {
            var testName = nameof(Import_StaticImport_FromCjs);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_RequireEsmModule()
        {
            var testName = nameof(Import_RequireEsmModule);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_ExportNamedFrom()
        {
            var testName = nameof(Import_ExportNamedFrom);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_ExportStarFrom()
        {
            var testName = nameof(Import_ExportStarFrom);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_LiveBindings_Named()
        {
            var testName = nameof(Import_LiveBindings_Named);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_LiveBindings_Cycle()
        {
            var testName = nameof(Import_LiveBindings_Cycle);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_Namespace_Esm_Basic()
        {
            var testName = nameof(Import_Namespace_Esm_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Import_Namespace_FromCjs_Stable()
        {
            var testName = nameof(Import_Namespace_FromCjs_Stable);
            return GenerateTest(testName);
        }
    }
}
