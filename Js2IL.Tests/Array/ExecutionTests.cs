using System.Threading.Tasks;

namespace Js2IL.Tests.Array
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Array") { }

        // Array execution tests
        [Fact]
        public Task Array_EmptyLength_IsZero() { var testName = nameof(Array_EmptyLength_IsZero); return ExecutionTest(testName); }

        [Fact]
        public Task Array_IsArray_Basic() { var testName = nameof(Array_IsArray_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Join_Basic() { var testName = nameof(Array_Join_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LengthProperty_ReturnsCount() { var testName = nameof(Array_LengthProperty_ReturnsCount); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Map_Basic() { var testName = nameof(Array_Map_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Map_NestedParam() { var testName = nameof(Array_Map_NestedParam); return ExecutionTest(testName); }

        [Fact]
        public Task Array_New_Empty() { var testName = nameof(Array_New_Empty); return ExecutionTest(testName); }

        [Fact]
        public Task Array_New_Length() { var testName = nameof(Array_New_Length); return ExecutionTest(testName); }

        [Fact]
        public Task Array_New_MultipleArgs() { var testName = nameof(Array_New_MultipleArgs); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Pop_Basic() { var testName = nameof(Array_Pop_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Push_Basic() { var testName = nameof(Array_Push_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Slice_Basic() { var testName = nameof(Array_Slice_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Slice_FromCharCode_Apply() { var testName = nameof(Array_Slice_FromCharCode_Apply); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Sort_Basic() { var testName = nameof(Array_Sort_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Sort_WithComparatorArrow() { var testName = nameof(Array_Sort_WithComparatorArrow); return ExecutionTest(testName); }
    
        [Fact]
        public Task Array_Splice_Basic() { var testName = nameof(Array_Splice_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Splice_InsertAndDelete() { var testName = nameof(Array_Splice_InsertAndDelete); return ExecutionTest(testName); }

        [Fact]
        public Task Array_AsArray_Ternary() { var testName = nameof(Array_AsArray_Ternary); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Find_Basic() { var testName = nameof(Array_Find_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_CallbackOps_Basic() { var testName = nameof(Array_CallbackOps_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Some_Basic() { var testName = nameof(Array_Some_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_SearchOps_Basic() { var testName = nameof(Array_SearchOps_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_MutationOps_Basic() { var testName = nameof(Array_MutationOps_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_NonMutatingOps_Basic() { var testName = nameof(Array_NonMutatingOps_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Static_Basic() { var testName = nameof(Array_Static_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Stringification_Basic() { var testName = nameof(Array_Stringification_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Callable_Construct() { var testName = nameof(Array_Callable_Construct); return ExecutionTest(testName); }

        // Repro: computed index using (arr.length - 1) should be treated as numeric
        [Fact]
        public Task Array_Index_UsingLengthMinusOne_Read() { var testName = nameof(Array_Index_UsingLengthMinusOne_Read); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Canonical_Index_StringKeys() { var testName = nameof(Array_Canonical_Index_StringKeys); return ExecutionTest(testName); }

        [Fact]
        public Task Array_Length_Set_Fractional_ThrowsRangeError() { var testName = nameof(Array_Length_Set_Fractional_ThrowsRangeError); return ExecutionTest(testName); }

        [Fact]
        public Task Array_PrototypeMethods_ArrayLike_Call() { var testName = nameof(Array_PrototypeMethods_ArrayLike_Call); return ExecutionTest(testName); }

        [Fact]
        public Task Array_PrototypeMethods_ArrayLike_EdgeCases() { var testName = nameof(Array_PrototypeMethods_ArrayLike_EdgeCases); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LiteralSpread_Basic() { var testName = nameof(Array_LiteralSpread_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LiteralSpread_Multiple() { var testName = nameof(Array_LiteralSpread_Multiple); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LiteralSpread_Mixed() { var testName = nameof(Array_LiteralSpread_Mixed); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LiteralSpread_Empty() { var testName = nameof(Array_LiteralSpread_Empty); return ExecutionTest(testName); }

        [Fact]
        public Task Array_LiteralSpread_Nested() { var testName = nameof(Array_LiteralSpread_Nested); return ExecutionTest(testName); }
    }
}
