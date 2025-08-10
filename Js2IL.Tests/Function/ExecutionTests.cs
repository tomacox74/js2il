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

        [Fact]
        public Task Function_ReturnsStaticValueAndLogs() { var testName = nameof(Function_ReturnsStaticValueAndLogs); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithArrayIteration() { var testName = nameof(Function_GlobalFunctionWithArrayIteration); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionLogsGlobalVariable() { var testName = nameof(Function_GlobalFunctionLogsGlobalVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionChangesGlobalVariableValue() { var testName = nameof(Function_GlobalFunctionChangesGlobalVariableValue); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionDeclaresAndCallsNestedFunction() { var testName = nameof(Function_GlobalFunctionDeclaresAndCallsNestedFunction); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionAccessesMultipleScopes() { var testName = nameof(Function_NestedFunctionAccessesMultipleScopes); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionLogsOuterParameter() { var testName = nameof(Function_NestedFunctionLogsOuterParameter); return ExecutionTest(testName); }
    }
}
