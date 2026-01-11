using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.ArrowFunction
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ArrowFunction") { }

        [Fact]
        public Task ArrowFunction_BlockBody_Return() { var testName = nameof(ArrowFunction_BlockBody_Return); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_CapturesOuterVariable() { var testName = nameof(ArrowFunction_CapturesOuterVariable); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_DefaultParameterExpression() { var testName = nameof(ArrowFunction_DefaultParameterExpression); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_DefaultParameterValue() { var testName = nameof(ArrowFunction_DefaultParameterValue); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionCallsGlobalFunction() { var testName = nameof(ArrowFunction_GlobalFunctionCallsGlobalFunction); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionWithMultipleParameters() { var testName = nameof(ArrowFunction_GlobalFunctionWithMultipleParameters); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_NestedFunctionAccessesMultipleScopes() { var testName = nameof(ArrowFunction_NestedFunctionAccessesMultipleScopes); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        // New: parameter destructuring (object)
        [Fact]
        public Task ArrowFunction_ParameterDestructuring_Object() { var testName = nameof(ArrowFunction_ParameterDestructuring_Object); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_SimpleExpression() { var testName = nameof(ArrowFunction_SimpleExpression); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task ArrowFunction_ClosureMutatesOuterVariable() { var testName = nameof(ArrowFunction_ClosureMutatesOuterVariable); return GenerateTest(testName, assertOnIRPipelineFailure: true); }
    }
}
