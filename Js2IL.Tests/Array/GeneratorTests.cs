using System.Threading.Tasks;

namespace Js2IL.Tests.Array
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Array") { }

        [Fact]
        public Task Array_AsArray_Ternary() { var testName = nameof(Array_AsArray_Ternary); return GenerateTest(testName); }

        [Fact]
        public Task Array_EmptyLength_IsZero() { var testName = nameof(Array_EmptyLength_IsZero); return GenerateTest(testName); }

        [Fact]
        public Task Array_IsArray_Basic() { var testName = nameof(Array_IsArray_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Join_Basic() { var testName = nameof(Array_Join_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_LengthProperty_ReturnsCount() { var testName = nameof(Array_LengthProperty_ReturnsCount); return GenerateTest(testName); }

        [Fact]
        public Task Array_Map_Basic() { var testName = nameof(Array_Map_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Map_NestedParam() { var testName = nameof(Array_Map_NestedParam); return GenerateTest(testName); }

        [Fact]
        public Task Array_New_Empty() { var testName = nameof(Array_New_Empty); return GenerateTest(testName); }

        [Fact]
        public Task Array_New_Length() { var testName = nameof(Array_New_Length); return GenerateTest(testName); }

        [Fact]
        public Task Array_New_MultipleArgs() { var testName = nameof(Array_New_MultipleArgs); return GenerateTest(testName); }

        [Fact]
        public Task Array_Pop_Basic() { var testName = nameof(Array_Pop_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Push_Basic() { var testName = nameof(Array_Push_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Slice_Basic() { var testName = nameof(Array_Slice_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Sort_Basic() { var testName = nameof(Array_Sort_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Sort_WithComparatorArrow() { var testName = nameof(Array_Sort_WithComparatorArrow); return GenerateTest(testName); }

        [Fact]
        public Task Array_Splice_Basic() { var testName = nameof(Array_Splice_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Splice_InsertAndDelete() { var testName = nameof(Array_Splice_InsertAndDelete); return GenerateTest(testName); }

    // Repro: computed index using (arr.length - 1)
    [Fact]
    public Task Array_Index_UsingLengthMinusOne_Read() { var testName = nameof(Array_Index_UsingLengthMinusOne_Read); return GenerateTest(testName); }
    }
}
