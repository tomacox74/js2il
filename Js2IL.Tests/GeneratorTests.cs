using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Main")
        {
        }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() { var testName = nameof(UnaryOperator_PlusPlusPostfix); return GenerateTest(testName); }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() { var testName = nameof(UnaryOperator_MinusMinusPostfix); return GenerateTest(testName); }

        [Fact(Skip = "process/argv not yet supported")]
        public Task Environment_EnumerateProcessArgV() { var testName = nameof(Environment_EnumerateProcessArgV); return GenerateTest(testName); }
        
    // moved to Node.GeneratorTests
        
    }
}
