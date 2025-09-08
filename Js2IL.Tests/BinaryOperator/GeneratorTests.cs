using System.Threading.Tasks;

namespace Js2IL.Tests.BinaryOperator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("BinaryOperator")
        {
        }

        // Binary Operator Tests
        [Fact]
        public Task BinaryOperator_AddNumberNumber() { var testName = nameof(BinaryOperator_AddNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringNumber() { var testName = nameof(BinaryOperator_AddStringNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringString() { var testName = nameof(BinaryOperator_AddStringString); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber() { var testName = nameof(BinaryOperator_BitwiseAndNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber() { var testName = nameof(BinaryOperator_BitwiseOrNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber() { var testName = nameof(BinaryOperator_BitwiseXorNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_DivNumberNumber() { var testName = nameof(BinaryOperator_DivNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_Equal() { var testName = nameof(BinaryOperator_Equal); return GenerateTest(testName); }

    [Fact]
    public Task BinaryOperator_EqualBoolean() { var testName = nameof(BinaryOperator_EqualBoolean); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber() { var testName = nameof(BinaryOperator_ExpNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThan() { var testName = nameof(BinaryOperator_GreaterThan); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThanOrEqual() { var testName = nameof(BinaryOperator_GreaterThanOrEqual); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber() { var testName = nameof(BinaryOperator_LeftShiftNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThan() { var testName = nameof(BinaryOperator_LessThan); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThanOrEqual() { var testName = nameof(BinaryOperator_LessThanOrEqual); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_ModNumberNumber() { var testName = nameof(BinaryOperator_ModNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_MulNumberNumber() { var testName = nameof(BinaryOperator_MulNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber() { var testName = nameof(BinaryOperator_RightShiftNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_SubNumberNumber() { var testName = nameof(BinaryOperator_SubNumberNumber); return GenerateTest(testName); }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber() { var testName = nameof(BinaryOperator_UnsignedRightShiftNumberNumber); return GenerateTest(testName); }

    // Logical operator generator tests
    [Fact]
    public Task BinaryOperator_LogicalOr_Value() { var testName = nameof(BinaryOperator_LogicalOr_Value); return GenerateTest(testName); }

    [Fact]
    public Task BinaryOperator_LogicalAnd_Value() { var testName = nameof(BinaryOperator_LogicalAnd_Value); return GenerateTest(testName); }

    // 'in' operator generator snapshot
    [Fact]
    public Task BinaryOperator_In_Object_OwnAndMissing() { var testName = nameof(BinaryOperator_In_Object_OwnAndMissing); return GenerateTest(testName); }
    }
}
