using System;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
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

        [Fact]
        public void NetSocket_OptionsConstructor_ParsesAllowHalfOpen()
        {
            dynamic options = new ExpandoObject();
            options.allowHalfOpen = true;

            var socket = new NetSocket((object)options);

            Assert.True(socket.allowHalfOpen);
        }

        [Fact]
        public async Task NetSocket_SetKeepAlive_AppliesUnderlyingSocketOption()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            using var client = new TcpClient(AddressFamily.InterNetwork);
            var acceptTask = listener.AcceptTcpClientAsync();
            await client.ConnectAsync(IPAddress.Loopback, port);
            using var accepted = await acceptTask;

            var socket = new NetSocket();
            var attachClient = typeof(NetSocket).GetMethod("AttachClient", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(attachClient);

            attachClient!.Invoke(socket, new object[] { accepted });
            socket.setKeepAlive(true);

            var keepAliveValue = accepted.Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive);
            Assert.Equal(1, Convert.ToInt32(keepAliveValue));
        }
    }
}
