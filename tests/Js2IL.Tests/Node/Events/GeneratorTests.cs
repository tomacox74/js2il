using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Events
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Events") { }

        [Fact]
        public Task Events_EventEmitter_On_Off_Once()
            => GenerateTest(nameof(Events_EventEmitter_On_Off_Once));

        [Fact]
        public Task Events_EventEmitter_Emit_Args()
            => GenerateTest(nameof(Events_EventEmitter_Emit_Args));

        [Fact]
        public Task Events_EventEmitter_Complete()
            => GenerateTest(nameof(Events_EventEmitter_Complete));

        [Fact]
        public Task Events_EventEmitter_SetMaxListeners_Validation()
            => GenerateTest(nameof(Events_EventEmitter_SetMaxListeners_Validation));

        [Fact]
        public Task Events_EventEmitter_ErrorMonitor()
            => GenerateTest(nameof(Events_EventEmitter_ErrorMonitor));

        [Fact]
        public Task Events_AsyncHelpers_On_Once()
            => GenerateTest(nameof(Events_AsyncHelpers_On_Once));
    }
}
