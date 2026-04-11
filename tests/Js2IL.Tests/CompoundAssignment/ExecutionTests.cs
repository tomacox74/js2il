using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CompoundAssignment
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("CompoundAssignment")
        {
        }

        // Bitwise compound assignments
        [Fact]
        public Task CompoundAssignment_BitwiseOrAssignment() { var testName = nameof(CompoundAssignment_BitwiseOrAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_BitwiseAndAssignment() { var testName = nameof(CompoundAssignment_BitwiseAndAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_ArrayIndexBitwiseOr() { var testName = nameof(CompoundAssignment_ArrayIndexBitwiseOr); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_BitwiseXorAssignment() { var testName = nameof(CompoundAssignment_BitwiseXorAssignment); return ExecutionTest(testName); }

        // Shift compound assignments
        [Fact]
        public Task CompoundAssignment_LeftShiftAssignment() { var testName = nameof(CompoundAssignment_LeftShiftAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_RightShiftAssignment() { var testName = nameof(CompoundAssignment_RightShiftAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_UnsignedRightShiftAssignment() { var testName = nameof(CompoundAssignment_UnsignedRightShiftAssignment); return ExecutionTest(testName); }

        // Arithmetic compound assignments
        [Fact]
        public Task CompoundAssignment_SubtractionAssignment() { var testName = nameof(CompoundAssignment_SubtractionAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_MultiplicationAssignment() { var testName = nameof(CompoundAssignment_MultiplicationAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_DivisionAssignment() { var testName = nameof(CompoundAssignment_DivisionAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_RemainderAssignment() { var testName = nameof(CompoundAssignment_RemainderAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_ExponentiationAssignment() { var testName = nameof(CompoundAssignment_ExponentiationAssignment); return ExecutionTest(testName); }

        [Fact]
        public Task CompoundAssignment_LocalVarIndex() { var testName = nameof(CompoundAssignment_LocalVarIndex); return ExecutionTest(testName); }
    }
}
