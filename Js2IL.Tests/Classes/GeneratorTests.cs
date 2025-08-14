using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Classes") { }

    // Classes generator tests
    [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return GenerateTest(testName); }
    [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return GenerateTest(testName); }
    [Fact] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return GenerateTest(testName); }
    }
}
