using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.BinaryOperator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("BinaryOperator")
        {
        }

        // Binary Operator Tests (sorted alphabetically)
        [Fact]
        public Task BinaryOperator_AddNumberNumber() { var testName = nameof(BinaryOperator_AddNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_AddObjectObject() { var testName = nameof(BinaryOperator_AddObjectObject); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_AddStringNumber() { var testName = nameof(BinaryOperator_AddStringNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_AddStringString() { var testName = nameof(BinaryOperator_AddStringString); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber() { var testName = nameof(BinaryOperator_BitwiseAndNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber() { var testName = nameof(BinaryOperator_BitwiseOrNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber() { var testName = nameof(BinaryOperator_BitwiseXorNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_DivNumberNumber() { var testName = nameof(BinaryOperator_DivNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_Equal() { var testName = nameof(BinaryOperator_Equal); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_EqualBoolean() { var testName = nameof(BinaryOperator_EqualBoolean); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_EqualMethodReturn() { var testName = nameof(BinaryOperator_EqualMethodReturn); return GenerateTest(testName, assertOnIRPipelineFailure: true); } // function calls not yet in IR pipeline

        [Fact]
        public Task BinaryOperator_EqualObjectPropertyVsMethodReturn() { var testName = nameof(BinaryOperator_EqualObjectPropertyVsMethodReturn); return GenerateTest(testName, assertOnIRPipelineFailure: true); } // function calls not yet in IR pipeline

        [Fact]
        public Task BinaryOperator_EqualParameter() { var testName = nameof(BinaryOperator_EqualParameter); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber() { var testName = nameof(BinaryOperator_ExpNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        // 'in' operator generator snapshot
        [Fact]
        public Task BinaryOperator_In_Object_OwnAndMissing() { var testName = nameof(BinaryOperator_In_Object_OwnAndMissing); return GenerateTest(testName, assertOnIRPipelineFailure: true); } // 'in' operator not yet in IR pipeline

        [Fact]
        public Task BinaryOperator_GreaterThan() { var testName = nameof(BinaryOperator_GreaterThan); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_GreaterThanOrEqual() { var testName = nameof(BinaryOperator_GreaterThanOrEqual); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LeftShiftBit31() { var testName = nameof(BinaryOperator_LeftShiftBit31); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber() { var testName = nameof(BinaryOperator_LeftShiftNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LessThan() { var testName = nameof(BinaryOperator_LessThan); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LessThanOrEqual() { var testName = nameof(BinaryOperator_LessThanOrEqual); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        // Logical operator generator tests
        [Fact]
        public Task BinaryOperator_LogicalAnd_Value() { var testName = nameof(BinaryOperator_LogicalAnd_Value); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LogicalAnd_ShortCircuit() { var testName = nameof(BinaryOperator_LogicalAnd_ShortCircuit); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LogicalOr_ArrayHasData() { var testName = nameof(BinaryOperator_LogicalOr_ArrayHasData); return GenerateTest(testName, assertOnIRPipelineFailure: true); } // array length property not yet in IR pipeline

        [Fact]
        public Task BinaryOperator_LogicalOr_Value() { var testName = nameof(BinaryOperator_LogicalOr_Value); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_LogicalOr_ShortCircuit() { var testName = nameof(BinaryOperator_LogicalOr_ShortCircuit); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_ModNumberNumber() { var testName = nameof(BinaryOperator_ModNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_MulNumberNumber() { var testName = nameof(BinaryOperator_MulNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_MulObjectObject() { var testName = nameof(BinaryOperator_MulObjectObject); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_NotEqual() { var testName = nameof(BinaryOperator_NotEqual); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber() { var testName = nameof(BinaryOperator_RightShiftNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_StrictEqualCapturedVariable() { var testName = nameof(BinaryOperator_StrictEqualCapturedVariable); return GenerateTest(testName, assertOnIRPipelineFailure: true); } // captured variables not yet in IR pipeline

        [Fact]
        public Task BinaryOperator_SubNumberNumber() { var testName = nameof(BinaryOperator_SubNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber() { var testName = nameof(BinaryOperator_UnsignedRightShiftNumberNumber); return GenerateTest(testName, assertOnIRPipelineFailure: true); }
    }
}
