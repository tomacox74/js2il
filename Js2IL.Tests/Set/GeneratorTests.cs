using System.Threading.Tasks;

namespace Js2IL.Tests.Set
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Set") { }

        [Fact]
        public Task Set_Constructor_Prototype_Surface() { var testName = nameof(Set_Constructor_Prototype_Surface); return GenerateTest(testName); }
    }
}
