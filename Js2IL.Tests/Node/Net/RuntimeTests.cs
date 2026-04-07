using System;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;
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

        [Fact]
        public void NetSocket_FinalizeReadableSide_DeliversQueuedDataBeforeEnd()
        {
            var tickSource = new Node.MockTickSource();
            var waitHandle = new Node.MockWaitHandle(
                onSet: () => { },
                onWaitOne: milliseconds => tickSource.Increment(TimeSpan.FromMilliseconds(milliseconds)));
            var serviceProvider = RuntimeServices.BuildServiceProvider();
            serviceProvider.Replace<ITickSource>(tickSource);
            serviceProvider.Replace<IWaitHandle>(waitHandle);

            try
            {
                GlobalThis.ServiceProvider = serviceProvider;

                var schedulerState = serviceProvider.Resolve<NodeSchedulerState>();
                var eventLoop = new NodeEventLoopPump(schedulerState, tickSource, waitHandle);

                dynamic options = new ExpandoObject();
                options.allowHalfOpen = true;

                var socket = new NetSocket((object)options);
                var events = new System.Collections.Generic.List<string>();
                socket.on("data", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    var chunk = Assert.IsType<JavaScriptRuntime.Node.Buffer>(args[0]);
                    events.Add("data:" + chunk.toString());
                    return null;
                }));
                socket.on("end", (Func<object[], object?[], object?>)((scopes, args) =>
                {
                    events.Add("end");
                    return null;
                }));

                var emitReadChunk = typeof(NetSocket).GetMethod("EmitReadChunk", BindingFlags.Instance | BindingFlags.NonPublic);
                var finalizeReadableSide = typeof(NetSocket).GetMethod("FinalizeReadableSide", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(emitReadChunk);
                Assert.NotNull(finalizeReadableSide);

                emitReadChunk!.Invoke(socket, new object?[] { new JavaScriptRuntime.Node.Buffer(Encoding.UTF8.GetBytes("alpha")), false });
                emitReadChunk.Invoke(socket, new object?[] { new JavaScriptRuntime.Node.Buffer(Encoding.UTF8.GetBytes("beta")), false });
                finalizeReadableSide!.Invoke(socket, new object?[] { false });

                DrainEventLoop(eventLoop);

                Assert.Equal(new[] { "data:alpha", "data:beta", "end" }, events);
            }
            finally
            {
                GlobalThis.ServiceProvider = null;
            }
        }

        private static void DrainEventLoop(NodeEventLoopPump eventLoop, int maxIterations = 10)
        {
            var iterations = 0;
            while (eventLoop.HasPendingWork() && iterations++ < maxIterations)
            {
                eventLoop.RunOneIteration();
                if (eventLoop.HasPendingWork())
                {
                    eventLoop.WaitForWorkOrNextTimer();
                }
            }

            Assert.False(eventLoop.HasPendingWork());
        }
    }
}
