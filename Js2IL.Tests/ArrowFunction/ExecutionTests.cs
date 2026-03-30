using System.Threading.Tasks;

namespace Js2IL.Tests.ArrowFunction
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ArrowFunction") { }

        // Arrow Function tests (initial scaffolding)
        [Fact]
        public Task ArrowFunction_BlockBody_Return() { var testName = nameof(ArrowFunction_BlockBody_Return); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_CapturesOuterVariable() { var testName = nameof(ArrowFunction_CapturesOuterVariable); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_DefaultParameterExpression() { var testName = nameof(ArrowFunction_DefaultParameterExpression); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_DefaultParameterValue() { var testName = nameof(ArrowFunction_DefaultParameterValue); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionCallsGlobalFunction() { var testName = nameof(ArrowFunction_GlobalFunctionCallsGlobalFunction); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionWithMultipleParameters() { var testName = nameof(ArrowFunction_GlobalFunctionWithMultipleParameters); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_NestedFunctionAccessesMultipleScopes() { var testName = nameof(ArrowFunction_NestedFunctionAccessesMultipleScopes); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_LexicalThis_ConstructorAssigned() { var testName = nameof(ArrowFunction_LexicalThis_ConstructorAssigned); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_LexicalThis_CreatedInMethod() { var testName = nameof(ArrowFunction_LexicalThis_CreatedInMethod); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_LexicalThis_ObjectLiteralProperty() { var testName = nameof(ArrowFunction_LexicalThis_ObjectLiteralProperty); return ExecutionTest(testName); }

        // New: parameter destructuring (object)
        [Fact]
        public Task ArrowFunction_ParameterDestructuring_Object() { var testName = nameof(ArrowFunction_ParameterDestructuring_Object); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_SimpleExpression() { var testName = nameof(ArrowFunction_SimpleExpression); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_ClosureMutatesOuterVariable() { var testName = nameof(ArrowFunction_ClosureMutatesOuterVariable); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_MaxParameters_32() { var testName = nameof(ArrowFunction_MaxParameters_32); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_RestParameters_Basic() { var testName = nameof(ArrowFunction_RestParameters_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ArrowFunction_RestParameters_WithNamedParams() { var testName = nameof(ArrowFunction_RestParameters_WithNamedParams); return ExecutionTest(testName); }
    }
}
