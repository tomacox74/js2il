using System.Threading.Tasks;

namespace Js2IL.Tests.Function
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Function")
        {
        }

        [Fact]
        public Task Function_DefaultParameterExpression() { var testName = nameof(Function_DefaultParameterExpression); return ExecutionTest(testName); }

        [Fact]
        public Task Function_DefaultParameterValue() { var testName = nameof(Function_DefaultParameterValue); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionCallsGlobalFunction() { var testName = nameof(Function_GlobalFunctionCallsGlobalFunction); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionChangesGlobalVariableValue() { var testName = nameof(Function_GlobalFunctionChangesGlobalVariableValue); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionDeclaresAndCallsNestedFunction() { var testName = nameof(Function_GlobalFunctionDeclaresAndCallsNestedFunction); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionLogsGlobalVariable() { var testName = nameof(Function_GlobalFunctionLogsGlobalVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithArrayIteration() { var testName = nameof(Function_GlobalFunctionWithArrayIteration); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithMultipleParameters() { var testName = nameof(Function_GlobalFunctionWithMultipleParameters); return ExecutionTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithParameter() { var testName = nameof(Function_GlobalFunctionWithParameter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_HelloWorld() { var testName = nameof(Function_HelloWorld); return ExecutionTest(testName); }

        [Fact]
        public Task Function_IIFE_Classic() { var testName = nameof(Function_IIFE_Classic); return ExecutionTest(testName); }

        [Fact]
        public Task Function_IIFE_Recursive() { var testName = nameof(Function_IIFE_Recursive); return ExecutionTest(testName); }

        [Fact]
        public Task Function_IsEven_CompareResultToTrue() { var testName = nameof(Function_IsEven_CompareResultToTrue); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionAccessesMultipleScopes() { var testName = nameof(Function_NestedFunctionAccessesMultipleScopes); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionLogsOuterParameter() { var testName = nameof(Function_NestedFunctionLogsOuterParameter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ParameterDestructuring_Object() { var testName = nameof(Function_ParameterDestructuring_Object); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ReturnsStaticValueAndLogs() { var testName = nameof(Function_ReturnsStaticValueAndLogs); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ReturnObjectWithClosure() { var testName = nameof(Function_ReturnObjectWithClosure); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ClosureEscapesScope_ObjectLiteralProperty() { var testName = nameof(Function_ClosureEscapesScope_ObjectLiteralProperty); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ClosureMutatesOuterVariable() { var testName = nameof(Function_ClosureMutatesOuterVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Closure_OuterNeedsLeafScopeInstanceToCallInner() { var testName = nameof(Function_Closure_OuterNeedsLeafScopeInstanceToCallInner); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Closure_MultiLevel_ReadWriteAcrossScopes() { var testName = nameof(Function_Closure_MultiLevel_ReadWriteAcrossScopes); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter() { var testName = nameof(Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Arguments_Basics() { var testName = nameof(Function_Arguments_Basics); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Arguments_NoFalsePositive_ObjectLiteralKey() { var testName = nameof(Function_Arguments_NoFalsePositive_ObjectLiteralKey); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Arguments_ComputedKey_TriggersBinding() { var testName = nameof(Function_Arguments_ComputedKey_TriggersBinding); return ExecutionTest(testName); }

        [Fact]
        public Task Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter() { var testName = nameof(Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Reassignment() { var testName = nameof(Function_CallViaVariable_Reassignment); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionExpression_ReturnedAndCalledViaVariable() { var testName = nameof(Function_NestedFunctionExpression_ReturnedAndCalledViaVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NestedFunctionDeclaration_AssignedAndCalledViaVariable() { var testName = nameof(Function_NestedFunctionDeclaration_AssignedAndCalledViaVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Reassignment_ClosureValues() { var testName = nameof(Function_CallViaVariable_Reassignment_ClosureValues); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralMethod_ThisBinding() { var testName = nameof(Function_ObjectLiteralMethod_ThisBinding); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralMethod_ThisBinding_AsyncAwait() { var testName = nameof(Function_ObjectLiteralMethod_ThisBinding_AsyncAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Function_MaxParameters_16() { var testName = nameof(Function_MaxParameters_16); return ExecutionTest(testName); }

        [Fact]
        public Task Function_MaxParameters_32_CallViaVariable() { var testName = nameof(Function_MaxParameters_32_CallViaVariable); return ExecutionTest(testName); }
    }
}
