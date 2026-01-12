using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("TypedArray") { }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Construct_Length() { var testName = nameof(Int32Array_Construct_Length); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_FromArray_CopyAndCoerce() { var testName = nameof(Int32Array_FromArray_CopyAndCoerce); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Set_FromArray_WithOffset() { var testName = nameof(Int32Array_Set_FromArray_WithOffset); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_SmallStep_WordValueOrAssign() { var testName = nameof(Prime_SetBitsTrue_SmallStep_WordValueOrAssign); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_SmallStep_WordOffsetChangeCheck() { var testName = nameof(Prime_SetBitsTrue_SmallStep_WordOffsetChangeCheck); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_LargeStep_DoWhileCounter() { var testName = nameof(Prime_LargeStep_DoWhileCounter); return ExecutionTest(testName); }

        [Fact]
        public Task Prime_SetBitsTrue_LargeStep_OptimizedVsNaive() { var testName = nameof(Prime_SetBitsTrue_LargeStep_OptimizedVsNaive); return ExecutionTest(testName); }
    }
}
