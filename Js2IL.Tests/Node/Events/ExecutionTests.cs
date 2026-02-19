using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Events
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Events") { }

        [Fact]
        public Task Events_EventEmitter_On_Off_Once()
            => ExecutionTest(nameof(Events_EventEmitter_On_Off_Once));

        [Fact]
        public Task Events_EventEmitter_Emit_Args()
            => ExecutionTest(nameof(Events_EventEmitter_Emit_Args));

        [Fact]
        public Task Events_EventEmitter_Complete()
            => ExecutionTest(nameof(Events_EventEmitter_Complete));
    }
}
