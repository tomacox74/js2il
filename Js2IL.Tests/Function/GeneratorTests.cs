using System.Threading.Tasks;

namespace Js2IL.Tests.Function
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Function")
        {
        }

        [Fact]
        public Task Function_DefaultParameterExpression() { var testName = nameof(Function_DefaultParameterExpression); return GenerateTest(testName); }

        [Fact]
        public Task Function_DefaultParameterValue() { var testName = nameof(Function_DefaultParameterValue); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionCallsGlobalFunction() { var testName = nameof(Function_GlobalFunctionCallsGlobalFunction); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionChangesGlobalVariableValue() { var testName = nameof(Function_GlobalFunctionChangesGlobalVariableValue); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionDeclaresAndCallsNestedFunction() { var testName = nameof(Function_GlobalFunctionDeclaresAndCallsNestedFunction); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionLogsGlobalVariable() { var testName = nameof(Function_GlobalFunctionLogsGlobalVariable); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithArrayIteration() { var testName = nameof(Function_GlobalFunctionWithArrayIteration); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithMultipleParameters() { var testName = nameof(Function_GlobalFunctionWithMultipleParameters); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithParameter() { var testName = nameof(Function_GlobalFunctionWithParameter); return GenerateTest(testName); }

        [Fact]
        public Task Function_HelloWorld()
        {
            var testName = nameof(Function_HelloWorld);
            // Ensure GenerateTest uses the correct input and expected output for HelloWorld
            // If GenerateTest is implemented in GeneratorTestsBase, verify its logic and the source files it uses.
            return GenerateTest(testName);
        }

        [Fact]
        public Task Function_IIFE_Classic() { var testName = nameof(Function_IIFE_Classic); return GenerateTest(testName); }

        [Fact]
        public Task Function_IIFE_Recursive() { var testName = nameof(Function_IIFE_Recursive); return GenerateTest(testName); }

        [Fact]
        public Task Function_IsEven_CompareResultToTrue() { var testName = nameof(Function_IsEven_CompareResultToTrue); return GenerateTest(testName); }

        [Fact]
        public Task Function_NestedFunctionAccessesMultipleScopes() { var testName = nameof(Function_NestedFunctionAccessesMultipleScopes); return GenerateTest(testName); }

        [Fact]
        public Task Function_NestedFunctionLogsOuterParameter() { var testName = nameof(Function_NestedFunctionLogsOuterParameter); return GenerateTest(testName); }

        [Fact]
        public Task Function_ParameterDestructuring_Object() { var testName = nameof(Function_ParameterDestructuring_Object); return GenerateTest(testName); }

        [Fact]
        public Task Function_ReturnsStaticValueAndLogs() { var testName = nameof(Function_ReturnsStaticValueAndLogs); return GenerateTest(testName); }

        [Fact]
        public Task Function_ReturnObjectWithClosure() { var testName = nameof(Function_ReturnObjectWithClosure); return GenerateTest(testName); }

        [Fact]
        public Task Function_ClosureMutatesOuterVariable() { var testName = nameof(Function_ClosureMutatesOuterVariable); return GenerateTest(testName); }

        [Fact]
        public Task Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter() { var testName = nameof(Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter); return GenerateTest(testName); }

        [Fact]
        public Task Function_Arguments_Basics() { var testName = nameof(Function_Arguments_Basics); return GenerateTest(testName); }

        [Fact]
        public Task Function_Arguments_NoFalsePositive_ObjectLiteralKey() { var testName = nameof(Function_Arguments_NoFalsePositive_ObjectLiteralKey); return GenerateTest(testName); }

        [Fact]
        public Task Function_Arguments_ComputedKey_TriggersBinding() { var testName = nameof(Function_Arguments_ComputedKey_TriggersBinding); return GenerateTest(testName); }

        [Fact]
        public Task Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter() { var testName = nameof(Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter); return GenerateTest(testName); }

        [Fact]
        public Task Function_ClosureEscapesScope_ObjectLiteralProperty() { var testName = nameof(Function_ClosureEscapesScope_ObjectLiteralProperty); return GenerateTest(testName); }

        [Fact]
        public Task Function_ObjectLiteralMethod_ThisBinding() { var testName = nameof(Function_ObjectLiteralMethod_ThisBinding); return GenerateTest(testName); }

        [Fact]
        public Task Function_Apply_Basic() { var testName = nameof(Function_Apply_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Function_Apply_ThisArg() { var testName = nameof(Function_Apply_ThisArg); return GenerateTest(testName); }

        [Fact]
        public Task Function_Apply_NullArgArray_TreatedAsEmpty() { var testName = nameof(Function_Apply_NullArgArray_TreatedAsEmpty); return GenerateTest(testName); }

        // Regression: storing a captured boolean into a typed scope field must emit the correct value type.
        [Fact]
        public Task Function_Closure_CapturedBoolean_AssignAndRead() { var testName = nameof(Function_Closure_CapturedBoolean_AssignAndRead); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Basic() { var testName = nameof(Function_Call_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Basic() { var testName = nameof(Function_Call_Spread_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Middle() { var testName = nameof(Function_Call_Spread_Middle); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_Multiple() { var testName = nameof(Function_Call_Spread_Multiple); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_EvaluationOrder() { var testName = nameof(Function_Call_Spread_EvaluationOrder); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_StringIterable() { var testName = nameof(Function_Call_Spread_StringIterable); return GenerateTest(testName); }

        [Fact]
        public Task Function_Call_Spread_MemberCall_ConsoleLog() { var testName = nameof(Function_Call_Spread_MemberCall_ConsoleLog); return GenerateTest(testName); }

        [Fact]
        public Task Function_Bind_Basic_PartialApplication() { var testName = nameof(Function_Bind_Basic_PartialApplication); return GenerateTest(testName); }

        [Fact]
        public Task Function_Bind_ThisBinding_IgnoresCallReceiver() { var testName = nameof(Function_Bind_ThisBinding_IgnoresCallReceiver); return GenerateTest(testName); }

        [Fact]
        public Task Function_ApplyBind_DominoPushAll() { var testName = nameof(Function_ApplyBind_DominoPushAll); return GenerateTest(testName); }

        [Fact]
        public Task Function_Prototype_ObjectCreate_ObjectPrototype() { var testName = nameof(Function_Prototype_ObjectCreate_ObjectPrototype); return GenerateTest(testName); }

        [Fact]
        public Task Function_MaxParameters_16() { var testName = nameof(Function_MaxParameters_16); return GenerateTest(testName); }

        [Fact]
        public Task Function_MaxParameters_32_CallViaVariable() { var testName = nameof(Function_MaxParameters_32_CallViaVariable); return GenerateTest(testName); }

        [Fact]
        public Task Function_NewExpression_CapturesOuterCtor() { var testName = nameof(Function_NewExpression_CapturesOuterCtor); return GenerateTest(testName); }

        [Fact]
        public Task Function_NewExpression_MemberCallee_Compiles() { var testName = nameof(Function_NewExpression_MemberCallee_Compiles); return GenerateTest(testName); }

        [Fact]
        public Task Function_NewTarget_NewVsCall() { var testName = nameof(Function_NewTarget_NewVsCall); return GenerateTest(testName); }

        [Fact]
        public Task Function_NewTarget_Arrow_Inherits() { var testName = nameof(Function_NewTarget_Arrow_Inherits); return GenerateTest(testName); }

        [Fact]
        public Task Function_RestParameters_Basic() { var testName = nameof(Function_RestParameters_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Function_RestParameters_WithNamedParams() { var testName = nameof(Function_RestParameters_WithNamedParams); return GenerateTest(testName); }

        [Fact]
        public Task Function_RestParameters_Empty() { var testName = nameof(Function_RestParameters_Empty); return GenerateTest(testName); }

        [Fact]
        public Task Function_RestParameters_MultipleNamed() { var testName = nameof(Function_RestParameters_MultipleNamed); return GenerateTest(testName); }
    }
}
