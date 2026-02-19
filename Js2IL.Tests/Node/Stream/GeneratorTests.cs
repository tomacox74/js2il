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
    }
}
