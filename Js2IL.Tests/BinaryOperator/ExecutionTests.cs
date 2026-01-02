using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Js2IL.Tests.BinaryOperator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("BinaryOperator")
        {
        }

        // Binary Operator Tests (sorted alphabetically)
        [Fact]
        public Task BinaryOperator_AddNumberNumber() { var testName = nameof(BinaryOperator_AddNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddObjectObject() { var testName = nameof(BinaryOperator_AddObjectObject); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringNumber() { var testName = nameof(BinaryOperator_AddStringNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_AddStringString() { var testName = nameof(BinaryOperator_AddStringString); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseAndNumberNumber() { var testName = nameof(BinaryOperator_BitwiseAndNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseOrNumberNumber() { var testName = nameof(BinaryOperator_BitwiseOrNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_BitwiseXorNumberNumber() { var testName = nameof(BinaryOperator_BitwiseXorNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_DivNumberNumber() { var testName = nameof(BinaryOperator_DivNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_Equal() { var testName = nameof(BinaryOperator_Equal); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_EqualBoolean() { var testName = nameof(BinaryOperator_EqualBoolean); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_EqualMethodReturn() { var testName = nameof(BinaryOperator_EqualMethodReturn); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_EqualObjectPropertyVsMethodReturn() { var testName = nameof(BinaryOperator_EqualObjectPropertyVsMethodReturn); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_EqualParameter() { var testName = nameof(BinaryOperator_EqualParameter); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_ExpNumberNumber() { var testName = nameof(BinaryOperator_ExpNumberNumber); return ExecutionTest(testName); }

        // 'in' operator tests
        [Fact]
        public Task BinaryOperator_In_Object_OwnAndMissing() { var testName = nameof(BinaryOperator_In_Object_OwnAndMissing); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThan() { var testName = nameof(BinaryOperator_GreaterThan); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_GreaterThanOrEqual() { var testName = nameof(BinaryOperator_GreaterThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LeftShiftBit31() { var testName = nameof(BinaryOperator_LeftShiftBit31); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LeftShiftNumberNumber() { var testName = nameof(BinaryOperator_LeftShiftNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThan() { var testName = nameof(BinaryOperator_LessThan); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LessThanOrEqual() { var testName = nameof(BinaryOperator_LessThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LogicalAnd_Value() { var testName = nameof(BinaryOperator_LogicalAnd_Value); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LogicalOr_ArrayHasData() { var testName = nameof(BinaryOperator_LogicalOr_ArrayHasData); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_LogicalOr_Value() { var testName = nameof(BinaryOperator_LogicalOr_Value); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_ModNumberNumber() { var testName = nameof(BinaryOperator_ModNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_MulNumberNumber() { var testName = nameof(BinaryOperator_MulNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_MulObjectObject() { var testName = nameof(BinaryOperator_MulObjectObject); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_NotEqual() { var testName = nameof(BinaryOperator_NotEqual); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_RightShiftNumberNumber() { var testName = nameof(BinaryOperator_RightShiftNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_StrictEqualCapturedVariable() { var testName = nameof(BinaryOperator_StrictEqualCapturedVariable); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_SubNumberNumber() { var testName = nameof(BinaryOperator_SubNumberNumber); return ExecutionTest(testName); }

        [Fact]
        public Task BinaryOperator_UnsignedRightShiftNumberNumber() { var testName = nameof(BinaryOperator_UnsignedRightShiftNumberNumber); return ExecutionTest(testName); }
    }
}
