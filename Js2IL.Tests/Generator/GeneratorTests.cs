using System.Threading.Tasks;

namespace Js2IL.Tests.Generator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Generator")
        {
        }

        [Fact]
        public Task Generator_BasicNext() { var testName = nameof(Generator_BasicNext); return GenerateTest(testName); }
    }
}
