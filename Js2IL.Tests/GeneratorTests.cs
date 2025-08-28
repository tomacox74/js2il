using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Main")
        {
        }

    // moved to UnaryOperator.GeneratorTests

    // moved to UnaryOperator.GeneratorTests

        [Fact(Skip = "process/argv not yet supported")]
        public Task Environment_EnumerateProcessArgV() { var testName = nameof(Environment_EnumerateProcessArgV); return GenerateTest(testName); }
        
    // moved to Node.GeneratorTests
        
    }
}
