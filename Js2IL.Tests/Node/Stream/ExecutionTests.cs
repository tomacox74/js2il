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

        [Fact]
        public Task Stream_PassThrough_Basic()
            => ExecutionTest(nameof(Stream_PassThrough_Basic));

        [Fact]
        public Task Stream_Transform_Basic()
            => ExecutionTest(nameof(Stream_Transform_Basic));

        [Fact]
        public Task Stream_Pipe_Backpressure_Basic()
            => ExecutionTest(nameof(Stream_Pipe_Backpressure_Basic));

        [Fact]
        public Task Stream_Readable_Pause_Resume()
            => ExecutionTest(nameof(Stream_Readable_Pause_Resume));

        [Fact]
        public Task Stream_Readable_SetEncoding_Utf8()
            => ExecutionTest(nameof(Stream_Readable_SetEncoding_Utf8));

        [Fact]
        public Task Stream_Writable_Drain_Finish_Order()
            => ExecutionTest(nameof(Stream_Writable_Drain_Finish_Order));

        [Fact]
        public Task Stream_Writable_Destroy_Error()
            => ExecutionTest(nameof(Stream_Writable_Destroy_Error));

        [Fact]
        public Task Stream_Finished_Callback_Basic()
            => ExecutionTest(nameof(Stream_Finished_Callback_Basic));

        [Fact]
        public Task Stream_Finished_Callback_DestroyError()
            => ExecutionTest(nameof(Stream_Finished_Callback_DestroyError));

        [Fact]
        public Task Stream_Pipeline_Basic()
            => ExecutionTest(nameof(Stream_Pipeline_Basic));

        [Fact]
        public Task Stream_Pipeline_Error()
            => ExecutionTest(nameof(Stream_Pipeline_Error));

        [Fact]
        public Task Stream_Pipeline_Error_PropagatesToPeers()
            => ExecutionTest(nameof(Stream_Pipeline_Error_PropagatesToPeers));
    }
}
