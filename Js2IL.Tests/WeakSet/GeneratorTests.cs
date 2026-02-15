using System.Threading.Tasks;

namespace Js2IL.Tests.WeakSet
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("WeakSet") { }

        [Fact]
        public Task WeakSet_Constructor_Empty() { var testName = nameof(WeakSet_Constructor_Empty); return GenerateTest(testName); }

        [Fact]
        public Task WeakSet_Add_Has_Basic() { var testName = nameof(WeakSet_Add_Has_Basic); return GenerateTest(testName); }

        [Fact]
        public Task WeakSet_Delete_Basic() { var testName = nameof(WeakSet_Delete_Basic); return GenerateTest(testName); }

        [Fact]
        public Task WeakSet_Object_Values() { var testName = nameof(WeakSet_Object_Values); return GenerateTest(testName); }
    }
}
