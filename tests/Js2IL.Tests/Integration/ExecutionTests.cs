using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Js2IL.Tests.Integration
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Integration") { }

        [Fact]
        public Task Compile_Performance_Dromaeo_Object_Regexp() => ExecutionTest(nameof(Compile_Performance_Dromaeo_Object_Regexp));

        [Fact]
        public async Task Compile_Scripts_ExtractEcma262SectionHtml_UrlMode()
        {
            await using var server = await LoopbackEcma262Server.StartAsync();
            using var currentDirectory = new TemporaryCurrentDirectory();

            await ExecutionTest(
                nameof(Compile_Scripts_ExtractEcma262SectionHtml_UrlMode),
                additionalScripts:
                [
                    "Compile_Scripts_ExtractEcma262SectionHtml_TestHarness",
                ],
                addMocks: services => services.RegisterInstance<IEnvironment>(
                    new FixedCommandLineEnvironment(
                        "dotnet",
                        "extract-url-smoke.dll",
                        "--section",
                        "27.3",
                        "--url",
                        server.RedirectUrl,
                        "--id",
                        "sec-generatorfunction-objects",
                        "--out",
                        "section-url.html")));

            Assert.Equal(
                new[]
                {
                    LoopbackEcma262Server.RedirectRequestPath,
                    LoopbackEcma262Server.FinalRequestPath,
                },
                server.RequestPaths);
        }

        [Fact]
        public async Task Compile_Scripts_ExtractEcma262SectionHtml_AutoMode()
        {
            await using var server = await LoopbackEcma262Server.StartAsync();
            using var currentDirectory = new TemporaryCurrentDirectory();

            await ExecutionTest(
                nameof(Compile_Scripts_ExtractEcma262SectionHtml_AutoMode),
                additionalScripts:
                [
                    "Compile_Scripts_ExtractEcma262SectionHtml_TestHarness",
                ],
                addMocks: services => services.RegisterInstance<IEnvironment>(
                    new FixedCommandLineEnvironment(
                        "dotnet",
                        "extract-auto-smoke.dll",
                        "--section",
                        "27.3",
                        "--auto",
                        "--index-url",
                        server.IndexUrl,
                        "--out",
                        "section-auto.html")));

            Assert.Equal(
                new[]
                {
                    LoopbackEcma262Server.IndexRequestPath,
                    LoopbackEcma262Server.RedirectRequestPath,
                    LoopbackEcma262Server.FinalRequestPath,
                },
                server.RequestPaths);
        }

        private sealed class LoopbackEcma262Server : IAsyncDisposable
        {
            internal const string IndexRequestPath = "/multipage/";
            internal const string RedirectRequestPath = "/multipage/control-abstraction-objects.html";
            internal const string FinalRequestPath = "/pages/control-abstraction-objects.html";

            private readonly TcpListener _listener;
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly Task _acceptLoopTask;
            private readonly List<string> _requestPaths = new();
            private readonly object _gate = new();

            private LoopbackEcma262Server(TcpListener listener)
            {
                _listener = listener;
                _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_cancellationTokenSource.Token));
            }

            public static Task<LoopbackEcma262Server> StartAsync()
            {
                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                return Task.FromResult(new LoopbackEcma262Server(listener));
            }

            public string IndexUrl => $"http://127.0.0.1:{Port}{IndexRequestPath}";

            public string RedirectUrl => $"http://127.0.0.1:{Port}{RedirectRequestPath}";

            public IReadOnlyList<string> RequestPaths
            {
                get
                {
                    lock (_gate)
                    {
                        return _requestPaths.ToArray();
                    }
                }
            }

            private int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

            public async ValueTask DisposeAsync()
            {
                _cancellationTokenSource.Cancel();
                _listener.Stop();

                try
                {
                    await _acceptLoopTask;
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                }
            }

            private async Task AcceptLoopAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client;
                    try
                    {
                        client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    await HandleClientAsync(client, cancellationToken);
                }
            }

            private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
            {
                using var _ = client;
                using var stream = client.GetStream();

                var requestText = await ReadRequestAsync(stream, cancellationToken);
                var requestPath = ParseRequestPath(requestText);
                lock (_gate)
                {
                    _requestPaths.Add(requestPath);
                }

                var response = CreateResponse(requestPath);
                var bodyBytes = Encoding.UTF8.GetBytes(response.Body);

                var headerBuilder = new StringBuilder();
                headerBuilder.Append(response.StatusLine).Append("\r\n");
                headerBuilder.Append("Connection: close\r\n");
                headerBuilder.Append("Content-Length: ").Append(bodyBytes.Length).Append("\r\n");
                foreach (var header in response.Headers)
                {
                    headerBuilder.Append(header.Key).Append(": ").Append(header.Value).Append("\r\n");
                }

                headerBuilder.Append("\r\n");

                var headerBytes = Encoding.ASCII.GetBytes(headerBuilder.ToString());
                await stream.WriteAsync(headerBytes, cancellationToken);
                if (bodyBytes.Length > 0)
                {
                    await stream.WriteAsync(bodyBytes, cancellationToken);
                }

                await stream.FlushAsync(cancellationToken);
            }

            private static async Task<string> ReadRequestAsync(NetworkStream stream, CancellationToken cancellationToken)
            {
                var bytes = new List<byte>();
                var buffer = new byte[1];
                while (true)
                {
                    var read = await stream.ReadAsync(buffer, cancellationToken);
                    if (read == 0)
                    {
                        break;
                    }

                    bytes.Add(buffer[0]);
                    var count = bytes.Count;
                    if (count >= 4
                        && bytes[count - 4] == '\r'
                        && bytes[count - 3] == '\n'
                        && bytes[count - 2] == '\r'
                        && bytes[count - 1] == '\n')
                    {
                        break;
                    }
                }

                return Encoding.ASCII.GetString(bytes.ToArray());
            }

            private static string ParseRequestPath(string requestText)
            {
                var requestLine = requestText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
                var parts = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    return string.Empty;
                }

                var rawTarget = parts[1];
                if (Uri.TryCreate(rawTarget, UriKind.Absolute, out var absoluteUri))
                {
                    return absoluteUri.PathAndQuery;
                }

                return rawTarget;
            }

            private static (string StatusLine, IReadOnlyDictionary<string, string> Headers, string Body) CreateResponse(string requestPath)
            {
                return requestPath switch
                {
                    IndexRequestPath => (
                        "HTTP/1.1 200 OK",
                        new Dictionary<string, string>
                        {
                            ["Content-Type"] = "text/html; charset=utf-8",
                        },
                        """
                        <!doctype html>
                        <html lang="en">
                        <body>
                          <a href="control-abstraction-objects.html#sec-generatorfunction-objects">
                            <span class="secnum">27.3</span>
                            Generator Function Objects
                          </a>
                        </body>
                        </html>
                        """),
                    RedirectRequestPath => (
                        "HTTP/1.1 302 Found",
                        new Dictionary<string, string>
                        {
                            ["Content-Type"] = "text/plain; charset=utf-8",
                            ["Location"] = FinalRequestPath,
                        },
                        "redirect"),
                    FinalRequestPath => (
                        "HTTP/1.1 200 OK",
                        new Dictionary<string, string>
                        {
                            ["Content-Type"] = "text/html; charset=utf-8",
                        },
                        """
                        <!doctype html>
                        <html lang="en">
                        <body>
                          <emu-clause id="sec-generatorfunction-objects">
                            <h1><span class="secnum">27.3</span> Generator Function Objects</h1>
                            <p>Loopback extractor body.</p>
                          </emu-clause>
                        </body>
                        </html>
                        """),
                    _ => (
                        "HTTP/1.1 404 Not Found",
                        new Dictionary<string, string>
                        {
                            ["Content-Type"] = "text/plain; charset=utf-8",
                        },
                        "not found"),
                };
            }
        }
    }
}
