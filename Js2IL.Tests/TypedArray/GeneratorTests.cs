using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("TypedArray") { }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task ArrayBuffer_Construct_ByteLength() { var testName = nameof(ArrayBuffer_Construct_ByteLength); return GenerateTest(testName); }

        [Fact]
        public Task ArrayBuffer_IsView_DataView() { var testName = nameof(ArrayBuffer_IsView_DataView); return GenerateTest(testName); }

        [Fact]
        public Task ArrayBuffer_IsView_TypedArrays() { var testName = nameof(ArrayBuffer_IsView_TypedArrays); return GenerateTest(testName); }

        [Fact]
        public Task DataView_BoundsChecks_RangeError() { var testName = nameof(DataView_BoundsChecks_RangeError); return GenerateTest(testName); }

        [Fact]
        public Task DataView_ByteOffset_ByteLength() { var testName = nameof(DataView_ByteOffset_ByteLength); return GenerateTest(testName); }

        [Fact]
        public Task DataView_Float32_Float64_RoundTrip() { var testName = nameof(DataView_Float32_Float64_RoundTrip); return GenerateTest(testName); }

        [Fact]
        public Task DataView_InvalidByteOffset_ByteLength_Messages() { var testName = nameof(DataView_InvalidByteOffset_ByteLength_Messages); return GenerateTest(testName); }

        [Fact]
        public Task DataView_SetGet_UintAndEndian() { var testName = nameof(DataView_SetGet_UintAndEndian); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Construct_Length() { var testName = nameof(Int32Array_Construct_Length); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_FromArray_CopyAndCoerce() { var testName = nameof(Int32Array_FromArray_CopyAndCoerce); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Construct_ArrayBuffer_ViewProperties() { var testName = nameof(Int32Array_Construct_ArrayBuffer_ViewProperties); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Construct_ArrayBuffer_Alignment_RangeError() { var testName = nameof(Int32Array_Construct_ArrayBuffer_Alignment_RangeError); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Set_FromArray_WithOffset() { var testName = nameof(Int32Array_Set_FromArray_WithOffset); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Set_BoundsChecks() { var testName = nameof(Int32Array_Set_BoundsChecks); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Slice_Basic() { var testName = nameof(Int32Array_Slice_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Slice_RelativeIndices() { var testName = nameof(Int32Array_Slice_RelativeIndices); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Subarray_ViewSemantics() { var testName = nameof(Int32Array_Subarray_ViewSemantics); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return GenerateTest(testName); }

        [Fact]
        public Task Uint8Array_Construct_ArrayLike_Buffer_Search() { var testName = nameof(Uint8Array_Construct_ArrayLike_Buffer_Search); return GenerateTest(testName); }

        [Fact]
        public Task Uint8Array_Values_Iterator() { var testName = nameof(Uint8Array_Values_Iterator); return GenerateTest(testName); }

        [Fact]
        public Task Uint8Array_Iterator_Metadata() { var testName = nameof(Uint8Array_Iterator_Metadata); return GenerateTest(testName); }

        [Fact]
        public Task Float64Array_Construct_ArrayBuffer_Search() { var testName = nameof(Float64Array_Construct_ArrayBuffer_Search); return GenerateTest(testName); }

        [Fact]
        public Task Float64Array_Callback_Methods() { var testName = nameof(Float64Array_Callback_Methods); return GenerateTest(testName); }

        [Fact]
        public Task TypedArray_Static_From_Of() { var testName = nameof(TypedArray_Static_From_Of); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Fill_Reverse_Join_LastIndexOf() { var testName = nameof(Int32Array_Fill_Reverse_Join_LastIndexOf); return GenerateTest(testName); }

        [Fact]
        public Task TypedArray_ConstructorAndSet_Errors() { var testName = nameof(TypedArray_ConstructorAndSet_Errors); return GenerateTest(testName); }

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
