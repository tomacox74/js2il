using System.Threading.Tasks;

namespace Jroc.Tests.Node.StringDecoder
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/StringDecoder") { }

        [Fact]
        public Task Require_StringDecoder_Utf8_Basic()
            => GenerateTest(nameof(Require_StringDecoder_Utf8_Basic));

        [Fact]
        public Task Require_StringDecoder_NodePrefix()
            => GenerateTest(nameof(Require_StringDecoder_NodePrefix));

        [Fact]
        public Task Require_StringDecoder_MemoryStreamStyle()
            => GenerateTest(nameof(Require_StringDecoder_MemoryStreamStyle));
    }
}
