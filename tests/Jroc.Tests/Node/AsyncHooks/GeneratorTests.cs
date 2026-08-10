using System.Threading.Tasks;

namespace Jroc.Tests.Node.AsyncHooks
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/AsyncHooks") { }

        [Fact]
        public Task Require_AsyncHooks_AsyncResource()
            => GenerateTest(nameof(Require_AsyncHooks_AsyncResource));

        [Fact]
        public Task Require_AsyncHooks_AsyncResource_Subclass()
            => GenerateTest(nameof(Require_AsyncHooks_AsyncResource_Subclass));

        [Fact]
        public Task Require_AsyncHooks_AsyncLocalStorage()
            => GenerateTest(nameof(Require_AsyncHooks_AsyncLocalStorage));

        [Fact]
        public Task Require_AsyncHooks_CreateHook()
            => GenerateTest(nameof(Require_AsyncHooks_CreateHook));

        [Fact]
        public Task Require_AsyncHooks_AsyncLocalStorage_Propagation()
            => GenerateTest(nameof(Require_AsyncHooks_AsyncLocalStorage_Propagation));
    }
}
