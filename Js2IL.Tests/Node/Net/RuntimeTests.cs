using JavaScriptRuntime.Node;
using Xunit;

namespace Js2IL.Tests.Node.Net
{
    public class RuntimeTests
    {
        [Fact]
        public void Utf8ChunkDecoder_CombinesSplitMultibyteSequences()
        {
            var decoder = new Utf8ChunkDecoder();

            Assert.Equal(string.Empty, decoder.Decode(new byte[] { 0xF0, 0x9F }, 2, flush: false));
            Assert.Equal("🙂", decoder.Decode(new byte[] { 0x99, 0x82 }, 2, flush: false));
            Assert.Equal(string.Empty, decoder.Flush());
        }
    }
}
