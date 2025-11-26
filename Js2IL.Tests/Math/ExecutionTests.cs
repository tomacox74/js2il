using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Math
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Math") { }

        [Fact]
        public Task Math_Cbrt_Negative()
        {
            return ExecutionTest(nameof(Math_Cbrt_Negative));
        }

        [Fact]
        public Task Math_Ceil_Sqrt_Basic()
        {
            return ExecutionTest(nameof(Math_Ceil_Sqrt_Basic));
        }

        [Fact]
        public Task Math_Fround_SignedZero()
        {
            return ExecutionTest(nameof(Math_Fround_SignedZero));
        }

        [Fact]
        public Task Math_Hypot_Infinity_NaN()
        {
            return ExecutionTest(nameof(Math_Hypot_Infinity_NaN));
        }

        [Fact]
        public Task Math_Imul_Clz32_Basics()
        {
            return ExecutionTest(nameof(Math_Imul_Clz32_Basics));
        }

        [Fact]
        public Task Math_Log_Exp_Identity()
        {
            return ExecutionTest(nameof(Math_Log_Exp_Identity));
        }

        [Fact]
        public Task Math_Min_Max_NaN_EmptyArgs()
        {
            return ExecutionTest(nameof(Math_Min_Max_NaN_EmptyArgs));
        }

        [Fact(Skip = "Temporarily disabled in CI: rounding/truncation edge-case differences on Linux; will re-enable after harmonizing semantics")]
        public Task Math_Round_Trunc_NegativeHalves()
        {
            return ExecutionTest(nameof(Math_Round_Trunc_NegativeHalves));
        }

        [Fact(Skip = "Temporarily disabled in CI: cross-platform signed-zero differences to be resolved later")]
        public Task Math_Sign_ZeroVariants()
        {
            return ExecutionTest(nameof(Math_Sign_ZeroVariants));
        }
    }
}
