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
    public Task Function_GlobalFunctionCallsGlobalFunction() { var testName = nameof(Function_GlobalFunctionCallsGlobalFunction); return ExecutionTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionWithParameter() { var testName = nameof(Function_GlobalFunctionWithParameter); return ExecutionTest(testName); }
    }
}
