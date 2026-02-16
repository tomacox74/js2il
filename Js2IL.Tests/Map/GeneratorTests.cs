using System.Threading.Tasks;

namespace Js2IL.Tests.Map
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Map") { }

        [Fact]
        public Task Map_Constructor_Empty() { var testName = nameof(Map_Constructor_Empty); return GenerateTest(testName); }

        [Fact]
        public Task Map_Set_Get_Basic() { var testName = nameof(Map_Set_Get_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Map_Has_Basic() { var testName = nameof(Map_Has_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Map_Delete_Basic() { var testName = nameof(Map_Delete_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Map_Clear_Basic() { var testName = nameof(Map_Clear_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Map_Size_Property() { var testName = nameof(Map_Size_Property); return GenerateTest(testName); }

        [Fact]
        public Task Map_Set_Returns_This() { var testName = nameof(Map_Set_Returns_This); return GenerateTest(testName); }

        [Fact]
        public Task Map_Keys_Values_Entries() { var testName = nameof(Map_Keys_Values_Entries); return GenerateTest(testName); }

        [Fact]
        public Task Map_Multiple_Keys() { var testName = nameof(Map_Multiple_Keys); return GenerateTest(testName); }

        [Fact]
        public Task Map_Update_Existing_Key() { var testName = nameof(Map_Update_Existing_Key); return GenerateTest(testName); }

        [Fact]
        public Task Map_Null_Key() { var testName = nameof(Map_Null_Key); return GenerateTest(testName); }
    }
}
