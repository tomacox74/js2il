using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Classes") { }

        // Classes generator tests scaffold (skipped until class codegen is finalized and snapshots added)
        [Fact(Skip = "snapshots pending; class codegen evolving")] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return GenerateTest(testName); }
        [Fact(Skip = "snapshots pending; class codegen evolving")] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return GenerateTest(testName); }
    }
}
