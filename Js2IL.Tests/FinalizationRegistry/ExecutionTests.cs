using System.Threading.Tasks;

namespace Js2IL.Tests.FinalizationRegistry
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("FinalizationRegistry") { }

        [Fact]
        public Task FinalizationRegistry_Cleanup_Order() { var testName = nameof(FinalizationRegistry_Cleanup_Order); return ExecutionTest(testName); }

        [Fact]
        public Task FinalizationRegistry_Unregister_Basic() { var testName = nameof(FinalizationRegistry_Unregister_Basic); return ExecutionTest(testName); }
    }
}
