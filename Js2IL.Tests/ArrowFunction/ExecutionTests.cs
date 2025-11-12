using System.Threading.Tasks;

namespace Js2IL.Tests.ArrowFunction
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ArrowFunction") { }

        // Arrow Function tests (initial scaffolding)
        [Fact]
        public Task ArrowFunction_SimpleExpression() { var testName = nameof(ArrowFunction_SimpleExpression); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_BlockBody_Return() { var testName = nameof(ArrowFunction_BlockBody_Return); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_CapturesOuterVariable() { var testName = nameof(ArrowFunction_CapturesOuterVariable); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionWithMultipleParameters() { var testName = nameof(ArrowFunction_GlobalFunctionWithMultipleParameters); return ExecutionTest(testName); }

    [Fact]
    public Task ArrowFunction_GlobalFunctionCallsGlobalFunction() { var testName = nameof(ArrowFunction_GlobalFunctionCallsGlobalFunction); return ExecutionTest(testName); }

    [Fact]
    public Task ArrowFunction_NestedFunctionAccessesMultipleScopes() { var testName = nameof(ArrowFunction_NestedFunctionAccessesMultipleScopes); return ExecutionTest(testName); }

    [Fact]
    public Task ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return ExecutionTest(testName); }

    // New: parameter destructuring (object)
    [Fact]
    public Task ArrowFunction_ParameterDestructuring_Object() { var testName = nameof(ArrowFunction_ParameterDestructuring_Object); return ExecutionTest(testName); }
    }
}
