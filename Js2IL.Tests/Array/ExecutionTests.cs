using System.Threading.Tasks;

namespace Js2IL.Tests.Array
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Array") { }

        // Array execution tests
        [Fact]
        public Task Array_LengthProperty_ReturnsCount() { var testName = nameof(Array_LengthProperty_ReturnsCount); return ExecutionTest(testName); }

        [Fact]
        public Task Array_EmptyLength_IsZero() { var testName = nameof(Array_EmptyLength_IsZero); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Sort_Basic() { var testName = nameof(Array_Sort_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Map_Basic() { var testName = nameof(Array_Map_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Map_NestedParam() { var testName = nameof(Array_Map_NestedParam); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Sort_WithComparatorArrow() { var testName = nameof(Array_Sort_WithComparatorArrow); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Join_Basic() { var testName = nameof(Array_Join_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_IsArray_Basic() { var testName = nameof(Array_IsArray_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Push_Basic() { var testName = nameof(Array_Push_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Pop_Basic() { var testName = nameof(Array_Pop_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Slice_Basic() { var testName = nameof(Array_Slice_Basic); return ExecutionTest(testName); }
    
        [Fact]
        public Task Array_Splice_Basic() { var testName = nameof(Array_Splice_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Splice_InsertAndDelete() { var testName = nameof(Array_Splice_InsertAndDelete); return ExecutionTest(testName); }

        [Fact]
        public Task Array_New_Empty() { var testName = nameof(Array_New_Empty); return ExecutionTest(testName); }

        [Fact]
        public Task Array_AsArray_Ternary() { var testName = nameof(Array_AsArray_Ternary); return ExecutionTest(testName); }
    }
}
