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
    }
}
