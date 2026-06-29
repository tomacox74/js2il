using System.Threading.Tasks;

namespace Jroc.Tests.Node.StringDecoder
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/StringDecoder") { }

        [Fact]
        public Task Require_StringDecoder_Utf8_Basic()
            => ExecutionTest(nameof(Require_StringDecoder_Utf8_Basic));

        [Fact]
        public Task Require_StringDecoder_NodePrefix()
            => ExecutionTest(nameof(Require_StringDecoder_NodePrefix));

        [Fact]
        public Task Require_StringDecoder_MemoryStreamStyle()
            => ExecutionTest(nameof(Require_StringDecoder_MemoryStreamStyle));
    }
}
