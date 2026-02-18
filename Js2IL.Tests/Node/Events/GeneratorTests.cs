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
    }
}
