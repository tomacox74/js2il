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
    }
}
