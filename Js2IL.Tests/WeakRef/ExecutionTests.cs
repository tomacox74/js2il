using System.Threading.Tasks;

namespace Js2IL.Tests.WeakRef
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("WeakRef") { }

        [Fact]
        public Task WeakRef_Deref_KeptObjects() { var testName = nameof(WeakRef_Deref_KeptObjects); return ExecutionTest(testName); }
    }
}
