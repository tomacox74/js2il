using System.Threading.Tasks;

namespace Js2IL.Tests.TypedArray
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("TypedArray") { }

        [Fact]
        public Task BeanCounter_Class_Index_Assign() { var testName = nameof(BeanCounter_Class_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_Index_Assign() { var testName = nameof(Int32Array_Index_Assign); return GenerateTest(testName); }

        [Fact]
        public Task Int32Array_ShiftDerived_Index_Access() { var testName = nameof(Int32Array_ShiftDerived_Index_Access); return GenerateTest(testName); }
    }
}
