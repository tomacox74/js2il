using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Classes") { }

        // Classes tests scaffold (skipped until class semantics like new/this are implemented)
        [Fact(Skip = "class semantics (new/this) not fully implemented yet")] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return ExecutionTest(testName); }
        [Fact(Skip = "class semantics (methods) not fully implemented yet")] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return ExecutionTest(testName); }
    }
}
