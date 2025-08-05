using System.Threading.Tasks;

namespace Js2IL.Tests.Function
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Function")
        {
        }

        // Function Tests
        [Fact]
        public Task Function_HelloWorld() { var testName = nameof(Function_HelloWorld); return GenerateTest(testName); }
    }
}
