using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Stream
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Stream") { }

        [Fact]
        public Task Stream_Readable_Basic()
            => ExecutionTest(nameof(Stream_Readable_Basic));

        [Fact]
        public Task Stream_Writable_Basic()
            => ExecutionTest(nameof(Stream_Writable_Basic));

        [Fact]
        public Task Stream_Writable_CustomWrite()
            => ExecutionTest(nameof(Stream_Writable_CustomWrite));

        [Fact]
        public Task Stream_Pipe_Basic()
            => ExecutionTest(nameof(Stream_Pipe_Basic));
    }
}
