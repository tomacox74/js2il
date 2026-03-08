using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("TypedArray") { }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return ExecutionTest(testName); }

        [Fact]
        public Task ArrayBuffer_Construct_ByteLength() { var testName = nameof(ArrayBuffer_Construct_ByteLength); return ExecutionTest(testName); }

        [Fact]
        public Task ArrayBuffer_IsView_DataView() { var testName = nameof(ArrayBuffer_IsView_DataView); return ExecutionTest(testName); }

        [Fact]
        public Task ArrayBuffer_IsView_TypedArrays() { var testName = nameof(ArrayBuffer_IsView_TypedArrays); return ExecutionTest(testName); }

        [Fact]
        public Task DataView_BoundsChecks_RangeError() { var testName = nameof(DataView_BoundsChecks_RangeError); return ExecutionTest(testName); }

        [Fact]
        public Task DataView_ByteOffset_ByteLength() { var testName = nameof(DataView_ByteOffset_ByteLength); return ExecutionTest(testName); }

        [Fact]
        public Task DataView_Float32_Float64_RoundTrip() { var testName = nameof(DataView_Float32_Float64_RoundTrip); return ExecutionTest(testName); }

        [Fact]
        public Task DataView_InvalidByteOffset_ByteLength_Messages() { var testName = nameof(DataView_InvalidByteOffset_ByteLength_Messages); return ExecutionTest(testName); }

        [Fact]
        public Task DataView_SetGet_UintAndEndian() { var testName = nameof(DataView_SetGet_UintAndEndian); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Construct_Length() { var testName = nameof(Int32Array_Construct_Length); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_FromArray_CopyAndCoerce() { var testName = nameof(Int32Array_FromArray_CopyAndCoerce); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Construct_ArrayBuffer_ViewProperties() { var testName = nameof(Int32Array_Construct_ArrayBuffer_ViewProperties); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Construct_ArrayBuffer_Alignment_RangeError() { var testName = nameof(Int32Array_Construct_ArrayBuffer_Alignment_RangeError); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Set_FromArray_WithOffset() { var testName = nameof(Int32Array_Set_FromArray_WithOffset); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Set_BoundsChecks() { var testName = nameof(Int32Array_Set_BoundsChecks); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Slice_Basic() { var testName = nameof(Int32Array_Slice_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Slice_RelativeIndices() { var testName = nameof(Int32Array_Slice_RelativeIndices); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Subarray_ViewSemantics() { var testName = nameof(Int32Array_Subarray_ViewSemantics); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return ExecutionTest(testName); }

        [Fact]
        public Task Uint8Array_Construct_ArrayLike_Buffer_Search() { var testName = nameof(Uint8Array_Construct_ArrayLike_Buffer_Search); return ExecutionTest(testName); }

        [Fact]
        public Task Uint8Array_Values_Iterator() { var testName = nameof(Uint8Array_Values_Iterator); return ExecutionTest(testName); }

        [Fact]
        public Task Uint8Array_Iterator_Metadata() { var testName = nameof(Uint8Array_Iterator_Metadata); return ExecutionTest(testName); }

        [Fact]
        public Task Float64Array_Construct_ArrayBuffer_Search() { var testName = nameof(Float64Array_Construct_ArrayBuffer_Search); return ExecutionTest(testName); }

        [Fact]
        public Task Float64Array_Callback_Methods() { var testName = nameof(Float64Array_Callback_Methods); return ExecutionTest(testName); }

        [Fact]
        public Task TypedArray_Static_From_Of() { var testName = nameof(TypedArray_Static_From_Of); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Fill_Reverse_Join_LastIndexOf() { var testName = nameof(Int32Array_Fill_Reverse_Join_LastIndexOf); return ExecutionTest(testName); }

        [Fact]
        public Task TypedArray_ConstructorAndSet_Errors() { var testName = nameof(TypedArray_ConstructorAndSet_Errors); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_SmallStep_WordValueOrAssign() { var testName = nameof(Prime_SetBitsTrue_SmallStep_WordValueOrAssign); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_SmallStep_WordOffsetChangeCheck() { var testName = nameof(Prime_SetBitsTrue_SmallStep_WordOffsetChangeCheck); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_LargeStep_DoWhileCounter() { var testName = nameof(Prime_LargeStep_DoWhileCounter); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_LargeStep_OptimizedVsNaive() { var testName = nameof(Prime_SetBitsTrue_LargeStep_OptimizedVsNaive); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_NaN_Index_NoOp() { var testName = nameof(Int32Array_NaN_Index_NoOp); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Infinity_Index_NoOp() { var testName = nameof(Int32Array_Infinity_Index_NoOp); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Fractional_Index_NoOp() { var testName = nameof(Int32Array_Fractional_Index_NoOp); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Wrapping_Semantics() { var testName = nameof(Int32Array_Wrapping_Semantics); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_GetItemAsNumber_NumericContext() { var testName = nameof(Int32Array_GetItemAsNumber_NumericContext); return ExecutionTest(testName); }
    }
}
