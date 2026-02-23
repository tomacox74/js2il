using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("TypedArray") { }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Construct_Length() { var testName = nameof(Int32Array_Construct_Length); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_FromArray_CopyAndCoerce() { var testName = nameof(Int32Array_FromArray_CopyAndCoerce); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Set_FromArray_WithOffset() { var testName = nameof(Int32Array_Set_FromArray_WithOffset); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return GenerateTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_SmallStep_WordValueOrAssign() { var testName = nameof(Prime_SetBitsTrue_SmallStep_WordValueOrAssign); return GenerateTest(testName); }

        [Fact]
        public Task Prime_LargeStep_DoWhileCounter() { var testName = nameof(Prime_LargeStep_DoWhileCounter); return GenerateTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_LargeStep_OptimizedVsNaive() { var testName = nameof(Prime_SetBitsTrue_LargeStep_OptimizedVsNaive); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_NaN_Index_NoOp() { var testName = nameof(Int32Array_NaN_Index_NoOp); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Infinity_Index_NoOp() { var testName = nameof(Int32Array_Infinity_Index_NoOp); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Fractional_Index_NoOp() { var testName = nameof(Int32Array_Fractional_Index_NoOp); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Wrapping_Semantics() { var testName = nameof(Int32Array_Wrapping_Semantics); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_GetItemAsNumber_NumericContext() { var testName = nameof(Int32Array_GetItemAsNumber_NumericContext); return GenerateTest(testName); }
    }
}
