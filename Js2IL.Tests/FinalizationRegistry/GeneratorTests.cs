using System.Threading.Tasks;

namespace Js2IL.Tests.FinalizationRegistry
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("FinalizationRegistry") { }

        [Fact]
        public Task FinalizationRegistry_Cleanup_Order() { var testName = nameof(FinalizationRegistry_Cleanup_Order); return GenerateTest(testName); }

        [Fact]
        public Task FinalizationRegistry_Unregister_Basic() { var testName = nameof(FinalizationRegistry_Unregister_Basic); return GenerateTest(testName); }
    }
}
