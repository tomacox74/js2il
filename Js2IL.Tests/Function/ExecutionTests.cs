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
        public Task Function_CallViaVariable_Arity0() { var testName = nameof(Function_CallViaVariable_Arity0); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Arity1() { var testName = nameof(Function_CallViaVariable_Arity1); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Arity2() { var testName = nameof(Function_CallViaVariable_Arity2); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Arity3() { var testName = nameof(Function_CallViaVariable_Arity3); return ExecutionTest(testName); }

        [Fact]
        public Task Function_CallViaVariable_Arity4() { var testName = nameof(Function_CallViaVariable_Arity4); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralMethod_ThisBinding() { var testName = nameof(Function_ObjectLiteralMethod_ThisBinding); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralMethod_ThisBinding_AsyncAwait() { var testName = nameof(Function_ObjectLiteralMethod_ThisBinding_AsyncAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralValueFunction_ForEachCapturesOuter() { var testName = nameof(Function_ObjectLiteralValueFunction_ForEachCapturesOuter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Apply_Basic() { var testName = nameof(Function_Apply_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Apply_ThisArg() { var testName = nameof(Function_Apply_ThisArg); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Apply_NullArgArray_TreatedAsEmpty() { var testName = nameof(Function_Apply_NullArgArray_TreatedAsEmpty); return ExecutionTest(testName); }

        // Regression: storing a captured boolean into a typed scope field must emit the correct value type.
        [Fact]
        public Task Function_Closure_CapturedBoolean_AssignAndRead() { var testName = nameof(Function_Closure_CapturedBoolean_AssignAndRead); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Basic() { var testName = nameof(Function_Call_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Basic() { var testName = nameof(Function_Call_Spread_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Middle() { var testName = nameof(Function_Call_Spread_Middle); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Multiple() { var testName = nameof(Function_Call_Spread_Multiple); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_EvaluationOrder() { var testName = nameof(Function_Call_Spread_EvaluationOrder); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_StringIterable() { var testName = nameof(Function_Call_Spread_StringIterable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Call_Spread_MemberCall_ConsoleLog() { var testName = nameof(Function_Call_Spread_MemberCall_ConsoleLog); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Bind_Basic_PartialApplication() { var testName = nameof(Function_Bind_Basic_PartialApplication); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Bind_ThisBinding_IgnoresCallReceiver() { var testName = nameof(Function_Bind_ThisBinding_IgnoresCallReceiver); return ExecutionTest(testName); }

        [Fact]
        public Task Function_ApplyBind_DominoPushAll() { var testName = nameof(Function_ApplyBind_DominoPushAll); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Prototype_ObjectCreate_ObjectPrototype() { var testName = nameof(Function_Prototype_ObjectCreate_ObjectPrototype); return ExecutionTest(testName); }

        [Fact]
        public Task Function_MaxParameters_16() { var testName = nameof(Function_MaxParameters_16); return ExecutionTest(testName); }

        [Fact]
        public Task Function_MaxParameters_32_CallViaVariable() { var testName = nameof(Function_MaxParameters_32_CallViaVariable); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NewExpression_CapturesOuterCtor() { var testName = nameof(Function_NewExpression_CapturesOuterCtor); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NewExpression_MemberCallee_Compiles() { var testName = nameof(Function_NewExpression_MemberCallee_Compiles); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NewTarget_NewVsCall() { var testName = nameof(Function_NewTarget_NewVsCall); return ExecutionTest(testName); }

        [Fact]
        public Task Function_NewTarget_Arrow_Inherits() { var testName = nameof(Function_NewTarget_Arrow_Inherits); return ExecutionTest(testName); }

        [Fact]
        public Task Function_RestParameters_Basic() { var testName = nameof(Function_RestParameters_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Function_RestParameters_WithNamedParams() { var testName = nameof(Function_RestParameters_WithNamedParams); return ExecutionTest(testName); }

        [Fact]
        public Task Function_RestParameters_Empty() { var testName = nameof(Function_RestParameters_Empty); return ExecutionTest(testName); }

        [Fact]
        public Task Function_RestParameters_MultipleNamed() { var testName = nameof(Function_RestParameters_MultipleNamed); return ExecutionTest(testName); }

        // ABI optimization tests: non-capturing functions should NOT have scopes parameter
        [Fact]
        public Task Function_NoCapture_NoScopesParameter() { var testName = nameof(Function_NoCapture_NoScopesParameter); return ExecutionTest(testName); }

        [Fact]
        public Task Function_Capture_HasScopesParameter() { var testName = nameof(Function_Capture_HasScopesParameter); return ExecutionTest(testName); }

        [Fact]
        public Task Arrow_NoCapture_NoScopesParameter() { var testName = nameof(Arrow_NoCapture_NoScopesParameter); return ExecutionTest(testName); }

        [Fact]
        public Task Arrow_Capture_HasScopesParameter() { var testName = nameof(Arrow_Capture_HasScopesParameter); return ExecutionTest(testName); }
    }
}
