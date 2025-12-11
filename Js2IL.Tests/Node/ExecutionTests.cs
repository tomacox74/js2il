using System.Threading.Tasks;
using System.IO;
using JavaScriptRuntime;

namespace Js2IL.Tests.Node
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node") { }

        [Fact]
        public Task ClearTimeout_MultipleZeroDelay_ClearSecondTimer() => ExecutionTest(nameof(ClearTimeout_MultipleZeroDelay_ClearSecondTimer));

        [Fact]
        public Task ClearTimeout_ZeroDelay() => ExecutionTest(nameof(ClearTimeout_ZeroDelay));

        [Fact]
        public Task Environment_EnumerateProcessArgV()
            => ExecutionTest(
                nameof(Environment_EnumerateProcessArgV),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                    // CI adds dynamic args (e.g., --port, --endpoint). Keep only the first line and normalize the count.
                    s.AddScrubber(sb =>
                    {
                        var text = sb.ToString();
                        var nl = text.IndexOf('\n');
                        var firstLine = nl >= 0 ? text.Substring(0, nl + 1) : text;
                        const string prefix = "argv length is ";
                        sb.Clear();
                        if (firstLine.StartsWith(prefix))
                        {
                            sb.Append(prefix).Append("{N}").Append('\n');
                        }
                        else
                        {
                            sb.Append(firstLine);
                        }
                    });
                });

        [Fact]
        public Task FS_ReadWrite_Utf8()
            => ExecutionTest(
                nameof(FS_ReadWrite_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task Global___dirname_PrintsDirectory()
            => ExecutionTest(
                nameof(Global___dirname_PrintsDirectory),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task PerfHooks_PerformanceNow_Basic()
            => ExecutionTest(
                nameof(PerfHooks_PerformanceNow_Basic),
                configureSettings: s =>
                {
                    // Keep as-is; output is integer ms values which should be stable enough.
                });

        [Fact]
        public Task Require_Path_Join_Basic()
            => ExecutionTest(nameof(Require_Path_Join_Basic), configureSettings: s => s.AddScrubber(sb => sb.Replace('\\', '/')));

        [Fact]
        public Task Require_Path_Join_NestedFunction()
            => ExecutionTest(nameof(Require_Path_Join_NestedFunction), configureSettings: s => s.AddScrubber(sb => sb.Replace('\\', '/')));

        [Fact]
        public Task SetTimeout_MultipleZeroDelay_ExecutedInOrder() => ExecutionTest(nameof(SetTimeout_MultipleZeroDelay_ExecutedInOrder));

        [Fact]
        public async Task SetTimeout_OneSecondDelay()
        {
            var defaultOptionsProvider = JavaScriptRuntime.EngineOptions.DefaultOptionsProvider;
            var mockTickSource = new Js2IL.Tests.Node.MockTickSource();
            var mockWaitHandle = new Js2IL.Tests.Node.MockWaitHandle(
                onSet: () => { },
                onWaitOne: (msTimeout) =>
                {
                    // Simulate the passage of time by incrementing the mock tick source.
                    mockTickSource.Increment(TimeSpan.FromMilliseconds(msTimeout));
                });

            JavaScriptRuntime.EngineOptions.DefaultOptionsProvider = () =>
            {
                return new JavaScriptRuntime.EngineOptions
                {
                    TickSource = mockTickSource,
                    WaitHandle = mockWaitHandle,
                };
            };

            var startTime = mockTickSource.GetTicks();

            var postTestProcessingAction = new Action<IConsoleOutput>(output =>
            {
                var endTime = mockTickSource.GetTicks();
                var elapsedMs = TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds;
                output.WriteLine($"Elapsed simulated time: {elapsedMs} ms");
            });

            try 
            {                
                await ExecutionTest(nameof(SetTimeout_OneSecondDelay), postTestProcessingAction: postTestProcessingAction);
            }
            finally
            {
                JavaScriptRuntime.EngineOptions.DefaultOptionsProvider = defaultOptionsProvider;
            }
        }

        [Fact]
        public Task SetTimeout_ZeroDelay() => ExecutionTest(nameof(SetTimeout_ZeroDelay));

        [Fact]
        public Task SetTimeout_ZeroDelay_WithArgs() => ExecutionTest(nameof(SetTimeout_ZeroDelay_WithArgs));
    }
}
