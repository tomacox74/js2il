using System.Threading.Tasks;

namespace Js2IL.Tests.Integration
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Integration") { }

        [Fact]
        public Task Compile_Scripts_GenerateFeatureCoverage() => GenerateTest(nameof(Compile_Scripts_GenerateFeatureCoverage));

        [Fact]
        public Task Compile_Performance_PrimeJavaScript() => GenerateTest(nameof(Compile_Performance_PrimeJavaScript));
    }
}
