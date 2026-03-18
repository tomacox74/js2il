using System.Threading.Tasks;

namespace Js2IL.Tests.Node.TimersPromises
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/TimersPromises") { }

        [Fact]
        public Task TimersPromises_SetTimeout_AwaitsValue()
            => ExecutionTest(nameof(TimersPromises_SetTimeout_AwaitsValue));

        [Fact]
        public Task TimersPromises_SetImmediate_AwaitsValue()
            => ExecutionTest(nameof(TimersPromises_SetImmediate_AwaitsValue));

        [Fact]
        public Task TimersPromises_Abort_RejectsSupportedOneShotApis()
            => ExecutionTest(nameof(TimersPromises_Abort_RejectsSupportedOneShotApis));

        [Fact]
        public Task TimersPromises_Abort_GetterErrors_SurfaceCorrectly()
            => ExecutionTest(nameof(TimersPromises_Abort_GetterErrors_SurfaceCorrectly));

        [Fact]
        public Task TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks()
            => ExecutionTest(nameof(TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks));

        [Fact]
        public Task TimersPromises_SetInterval_RejectsClearly()
            => ExecutionTest(nameof(TimersPromises_SetInterval_RejectsClearly));
    }
}
