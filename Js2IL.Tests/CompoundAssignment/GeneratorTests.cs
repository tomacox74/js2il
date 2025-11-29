using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CompoundAssignment
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CompoundAssignment")
        {
        }

        // Bitwise compound assignments
        [Fact]
        public Task CompoundAssignment_BitwiseOrAssignment() { var testName = nameof(CompoundAssignment_BitwiseOrAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_BitwiseAndAssignment() { var testName = nameof(CompoundAssignment_BitwiseAndAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_ArrayIndexBitwiseOr() { var testName = nameof(CompoundAssignment_ArrayIndexBitwiseOr); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_BitwiseXorAssignment() { var testName = nameof(CompoundAssignment_BitwiseXorAssignment); return GenerateTest(testName); }

        // Shift compound assignments
        [Fact]
        public Task CompoundAssignment_LeftShiftAssignment() { var testName = nameof(CompoundAssignment_LeftShiftAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_RightShiftAssignment() { var testName = nameof(CompoundAssignment_RightShiftAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_UnsignedRightShiftAssignment() { var testName = nameof(CompoundAssignment_UnsignedRightShiftAssignment); return GenerateTest(testName); }

        // Arithmetic compound assignments
        [Fact]
        public Task CompoundAssignment_SubtractionAssignment() { var testName = nameof(CompoundAssignment_SubtractionAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_MultiplicationAssignment() { var testName = nameof(CompoundAssignment_MultiplicationAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_DivisionAssignment() { var testName = nameof(CompoundAssignment_DivisionAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_RemainderAssignment() { var testName = nameof(CompoundAssignment_RemainderAssignment); return GenerateTest(testName); }

        [Fact]
        public Task CompoundAssignment_ExponentiationAssignment() { var testName = nameof(CompoundAssignment_ExponentiationAssignment); return GenerateTest(testName); }
    }
}
