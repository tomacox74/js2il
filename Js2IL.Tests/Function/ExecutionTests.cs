using System.Threading.Tasks;

namespace Js2IL.Tests.Function
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Function")
        {
        }

        // Function Tests
        [Fact]
        public Task Function_HelloWorld() { var testName = nameof(Function_HelloWorld); return ExecutionTest(testName); }

        [Fact]
        public Task Function_TwoFunctionsInMain() { var testName = nameof(Function_TwoFunctionsInMain); return ExecutionTest(testName); }
    }
}
