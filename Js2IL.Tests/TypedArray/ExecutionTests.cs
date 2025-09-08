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
    }
}
