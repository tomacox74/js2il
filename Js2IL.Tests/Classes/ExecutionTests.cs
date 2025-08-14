using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Classes") { }

    // Classes tests
    [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return ExecutionTest(testName); }
    [Fact(Skip = "new expressions and instance method calls not implemented yet")] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return ExecutionTest(testName); }
    }
}
