using System.Threading.Tasks;

namespace Js2IL.Tests.Node.TimersPromises
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/TimersPromises") { }

        [Fact]
        public Task TimersPromises_SetTimeout_AwaitsValue()
            => GenerateTest(nameof(TimersPromises_SetTimeout_AwaitsValue));

        [Fact]
        public Task TimersPromises_SetImmediate_AwaitsValue()
            => GenerateTest(nameof(TimersPromises_SetImmediate_AwaitsValue));

        [Fact]
        public Task TimersPromises_Abort_RejectsSupportedOneShotApis()
            => GenerateTest(nameof(TimersPromises_Abort_RejectsSupportedOneShotApis));

        [Fact]
        public Task TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks()
            => GenerateTest(nameof(TimersPromises_Ordering_WithNextTick_AndPromiseMicrotasks));

        [Fact]
        public Task TimersPromises_SetInterval_RejectsClearly()
            => GenerateTest(nameof(TimersPromises_SetInterval_RejectsClearly));
    }
}
