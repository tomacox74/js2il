using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Stream
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Stream") { }

        [Fact]
        public Task Stream_Readable_Basic()
            => GenerateTest(nameof(Stream_Readable_Basic));

        [Fact]
        public Task Stream_Writable_Basic()
            => GenerateTest(nameof(Stream_Writable_Basic));

        [Fact]
        public Task Stream_Writable_CustomWrite()
            => GenerateTest(nameof(Stream_Writable_CustomWrite));

        [Fact]
        public Task Stream_Pipe_Basic()
            => GenerateTest(nameof(Stream_Pipe_Basic));

        [Fact]
        public Task Stream_PassThrough_Basic()
            => GenerateTest(nameof(Stream_PassThrough_Basic));

        [Fact]
        public Task Stream_Transform_Basic()
            => GenerateTest(nameof(Stream_Transform_Basic));

        [Fact]
        public Task Stream_Pipe_Backpressure_Basic()
            => GenerateTest(nameof(Stream_Pipe_Backpressure_Basic));

        [Fact]
        public Task Stream_Readable_Pause_Resume()
            => GenerateTest(nameof(Stream_Readable_Pause_Resume));

        [Fact]
        public Task Stream_Readable_SetEncoding_Utf8()
            => GenerateTest(nameof(Stream_Readable_SetEncoding_Utf8));

        [Fact]
        public Task Stream_Writable_Drain_Finish_Order()
            => GenerateTest(nameof(Stream_Writable_Drain_Finish_Order));

        [Fact]
        public Task Stream_Writable_Destroy_Error()
            => GenerateTest(nameof(Stream_Writable_Destroy_Error));

        [Fact]
        public Task Stream_Finished_Callback_Basic()
            => GenerateTest(nameof(Stream_Finished_Callback_Basic));

        [Fact]
        public Task Stream_Finished_Callback_DestroyError()
            => GenerateTest(nameof(Stream_Finished_Callback_DestroyError));

        [Fact]
        public Task Stream_Pipeline_Basic()
            => GenerateTest(nameof(Stream_Pipeline_Basic));

        [Fact]
        public Task Stream_Pipeline_Error()
            => GenerateTest(nameof(Stream_Pipeline_Error));
    }
}
