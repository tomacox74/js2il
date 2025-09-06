using System.Threading.Tasks;

namespace Js2IL.Tests.Array
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Array") { }

        [Fact]
        public Task Array_LengthProperty_ReturnsCount() { var testName = nameof(Array_LengthProperty_ReturnsCount); return GenerateTest(testName); }

        [Fact]
        public Task Array_EmptyLength_IsZero() { var testName = nameof(Array_EmptyLength_IsZero); return GenerateTest(testName); }

        [Fact]
        public Task Array_Sort_Basic() { var testName = nameof(Array_Sort_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Map_Basic() { var testName = nameof(Array_Map_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Map_NestedParam() { var testName = nameof(Array_Map_NestedParam); return GenerateTest(testName); }

        [Fact]
        public Task Array_Sort_WithComparatorArrow() { var testName = nameof(Array_Sort_WithComparatorArrow); return GenerateTest(testName); }

        [Fact]
        public Task Array_Join_Basic() { var testName = nameof(Array_Join_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_IsArray_Basic() { var testName = nameof(Array_IsArray_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Push_Basic() { var testName = nameof(Array_Push_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Pop_Basic() { var testName = nameof(Array_Pop_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Splice_Basic() { var testName = nameof(Array_Splice_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Array_Splice_InsertAndDelete() { var testName = nameof(Array_Splice_InsertAndDelete); return GenerateTest(testName); }
    }
}
