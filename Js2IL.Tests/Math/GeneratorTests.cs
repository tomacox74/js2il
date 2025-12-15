using System.Threading.Tasks;

namespace Js2IL.Tests.Math
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Math") { }

        [Fact]
        public Task Math_Cbrt_Negative() { var testName = nameof(Math_Cbrt_Negative); return GenerateTest(testName); }

        [Fact]
        public Task Math_Ceil_Sqrt_Basic() { var testName = nameof(Math_Ceil_Sqrt_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Math_Fround_SignedZero() { var testName = nameof(Math_Fround_SignedZero); return GenerateTest(testName); }

        [Fact]
        public Task Math_Hypot_Infinity_NaN() { var testName = nameof(Math_Hypot_Infinity_NaN); return GenerateTest(testName); }

        [Fact]
        public Task Math_Imul_Clz32_Basics() { var testName = nameof(Math_Imul_Clz32_Basics); return GenerateTest(testName); }

        [Fact]
        public Task Math_Log_Exp_Identity() { var testName = nameof(Math_Log_Exp_Identity); return GenerateTest(testName); }

        [Fact]
        public Task Math_Min_Max_NaN_EmptyArgs() { var testName = nameof(Math_Min_Max_NaN_EmptyArgs); return GenerateTest(testName); }

        [Fact]
        public Task Math_Round_Trunc_NegativeHalves() { var testName = nameof(Math_Round_Trunc_NegativeHalves); return GenerateTest(testName); }

        [Fact]
        public Task Math_Sign_ZeroVariants() { var testName = nameof(Math_Sign_ZeroVariants); return GenerateTest(testName); }
    }
}
