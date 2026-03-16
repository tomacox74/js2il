using System.Collections.Generic;
using System.Linq;
using System.Text;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Xunit;

namespace Js2IL.Tests.Node.Http
{
    public class RuntimeTests
    {
        [Fact]
        public void HttpWireParser_TryParseRequest_ContentLengthCountsUtf8Bytes()
        {
            var bodyBytes = Encoding.UTF8.GetBytes("🙂");
            var headerBytes = Encoding.ASCII.GetBytes(
                "POST /submit HTTP/1.1\r\n"
                + $"Content-Length: {bodyBytes.Length}\r\n"
                + "\r\n");
            var raw = headerBytes.Concat(bodyBytes).ToArray();

            var parsed = default(HttpParsedRequest);
            var ok = HttpWireParser.TryParseRequest(raw, isEndOfStream: false, out parsed);

            Assert.True(ok);
            Assert.NotNull(parsed);
            Assert.Equal("🙂", parsed!.Body);
        }

        [Fact]
        public void HttpWireParser_TryParseRequest_IncompleteUtf8BodyAtEndOfStream_Fails()
        {
            var partialBodyBytes = new byte[] { 0xF0, 0x9F, 0x99 };
            var headerBytes = Encoding.ASCII.GetBytes(
                "POST /submit HTTP/1.1\r\n"
                + "Content-Length: 4\r\n"
                + "\r\n");
            var raw = headerBytes.Concat(partialBodyBytes).ToArray();

            var ok = HttpWireParser.TryParseRequest(raw, isEndOfStream: true, out _);

            Assert.False(ok);
        }

        [Fact]
        public void HttpWireParser_TryParseRequest_ChunkedBody_DecodesChunks()
        {
            var raw = Encoding.ASCII.GetBytes(
                "POST /submit HTTP/1.1\r\n"
                + "Transfer-Encoding: chunked\r\n"
                + "\r\n"
                + "5\r\nhello\r\n"
                + "1\r\n \r\n"
                + "5\r\nworld\r\n"
                + "0\r\n"
                + "\r\n");

            var parsed = default(HttpParsedRequest);
            var ok = HttpWireParser.TryParseRequest(raw, isEndOfStream: false, out parsed);

            Assert.True(ok);
            Assert.NotNull(parsed);
            Assert.Equal("hello world", parsed!.Body);
            Assert.Equal("chunked", parsed.Headers["transfer-encoding"]);
        }

        [Fact]
        public void HttpWireParser_TryParseResponse_ChunkedBody_DecodesChunks()
        {
            var raw = Encoding.ASCII.GetBytes(
                "HTTP/1.1 200 OK\r\n"
                + "Transfer-Encoding: chunked\r\n"
                + "\r\n"
                + "5\r\nalpha\r\n"
                + "4\r\nbeta\r\n"
                + "0\r\n"
                + "\r\n");

            var parsed = default(HttpParsedResponse);
            var ok = HttpWireParser.TryParseResponse(raw, isEndOfStream: false, out parsed);

            Assert.True(ok);
            Assert.NotNull(parsed);
            Assert.Equal("alphabeta", parsed!.Body);
            Assert.Equal(200, parsed.StatusCode);
        }

        [Fact]
        public void HttpRequestOptions_TryGetUnsupportedFeatureMessage_ExpectHeader_ReturnsTrue()
        {
            var hasUnsupportedFeature = JavaScriptRuntime.Node.HttpRequestOptions.TryGetUnsupportedFeatureMessage(
                "POST",
                new Dictionary<string, string>
                {
                    ["expect"] = "100-continue",
                },
                "node:http",
                out var message);

            Assert.True(hasUnsupportedFeature);
            Assert.Contains("100-continue", message);
        }

        [Fact]
        public void HttpRequestOptions_Parse_InvalidUrl_ThrowsTypeError()
        {
            var ex = Assert.Throws<TypeError>(() => JavaScriptRuntime.Node.HttpRequestOptions.Parse(new object[] { "http://[" }, "GET", "http", 80, "node:http"));
            Assert.Contains("Invalid URL", ex.Message);
        }

        [Fact]
        public void CoerceHttpStatusCode_FractionalValue_ThrowsRangeError()
        {
            var ex = Assert.Throws<RangeError>(() => NodeNetworkingCommon.CoerceHttpStatusCode(200.5));
            Assert.Contains("integer", ex.Message);
        }
    }
}
