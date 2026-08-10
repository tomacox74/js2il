using System.Threading.Tasks;

namespace Jroc.Tests.Node.AsyncHooks
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/AsyncHooks") { }

        [Fact]
        public Task Require_AsyncHooks_AsyncResource()
            => ExecutionTest(nameof(Require_AsyncHooks_AsyncResource));

        [Fact]
        public Task Require_AsyncHooks_AsyncResource_Subclass()
            => ExecutionTest(nameof(Require_AsyncHooks_AsyncResource_Subclass));

        [Fact]
        public Task Require_AsyncHooks_AsyncLocalStorage()
            => ExecutionTest(nameof(Require_AsyncHooks_AsyncLocalStorage));

        [Fact]
        public Task Require_AsyncHooks_CreateHook()
            => ExecutionTest(nameof(Require_AsyncHooks_CreateHook));

        [Fact]
        public Task Require_AsyncHooks_AsyncLocalStorage_Propagation()
            => ExecutionTest(nameof(Require_AsyncHooks_AsyncLocalStorage_Propagation));
    }
}
