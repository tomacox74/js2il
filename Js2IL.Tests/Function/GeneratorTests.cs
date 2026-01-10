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
        public Task Function_GlobalFunctionChangesGlobalVariableValue() { var testName = nameof(Function_GlobalFunctionChangesGlobalVariableValue); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_GlobalFunctionDeclaresAndCallsNestedFunction() { var testName = nameof(Function_GlobalFunctionDeclaresAndCallsNestedFunction); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_GlobalFunctionLogsGlobalVariable() { var testName = nameof(Function_GlobalFunctionLogsGlobalVariable); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(Function_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithArrayIteration() { var testName = nameof(Function_GlobalFunctionWithArrayIteration); return GenerateTest(testName); }

        [Fact]
        public Task Function_GlobalFunctionWithMultipleParameters() { var testName = nameof(Function_GlobalFunctionWithMultipleParameters); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_GlobalFunctionWithParameter() { var testName = nameof(Function_GlobalFunctionWithParameter); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_HelloWorld()
        {
            var testName = nameof(Function_HelloWorld);
            // Ensure GenerateTest uses the correct input and expected output for HelloWorld
            // If GenerateTest is implemented in GeneratorTestsBase, verify its logic and the source files it uses.
            return GenerateTest(testName, assertOnIRPipelineFailure: true);
        }

        [Fact]
        public Task Function_IIFE_Classic() { var testName = nameof(Function_IIFE_Classic); return GenerateTest(testName); }

        [Fact]
        public Task Function_IIFE_Recursive() { var testName = nameof(Function_IIFE_Recursive); return GenerateTest(testName); }

        [Fact]
        public Task Function_IsEven_CompareResultToTrue() { var testName = nameof(Function_IsEven_CompareResultToTrue); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_NestedFunctionAccessesMultipleScopes() { var testName = nameof(Function_NestedFunctionAccessesMultipleScopes); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_NestedFunctionLogsOuterParameter() { var testName = nameof(Function_NestedFunctionLogsOuterParameter); return GenerateTest(testName); }

        [Fact]
        public Task Function_ParameterDestructuring_Object() { var testName = nameof(Function_ParameterDestructuring_Object); return GenerateTest(testName); }

        [Fact]
        public Task Function_ReturnsStaticValueAndLogs() { var testName = nameof(Function_ReturnsStaticValueAndLogs); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_ClosureMutatesOuterVariable() { var testName = nameof(Function_ClosureMutatesOuterVariable); return GenerateTest(testName); }

        [Fact]
        public Task Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter() { var testName = nameof(Function_ArrowFunctionExpression_ConciseBody_ForEachCapturesOuter); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter() { var testName = nameof(Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter); return GenerateTest(testName, assertOnIRPipelineFailure: true); }
    }
}
