using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("TypedArray") { }

        [Fact]
        public Task Int32Array_Construct_Length() { var testName = nameof(Int32Array_Construct_Length); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_FromArray_CopyAndCoerce() { var testName = nameof(Int32Array_FromArray_CopyAndCoerce); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Set_FromArray_WithOffset() { var testName = nameof(Int32Array_Set_FromArray_WithOffset); return ExecutionTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return ExecutionTest(testName); }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return ExecutionTest(testName); }

        [Fact(Skip = "Temporarily disabled: investigate Int32Array derived shift index access semantics in follow-up PR")]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return ExecutionTest(testName); }
    }
}
