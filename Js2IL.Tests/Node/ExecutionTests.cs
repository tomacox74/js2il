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
        public Task GlobalTimers_AsValues_WindowLikeAssignment() => ExecutionTest(nameof(GlobalTimers_AsValues_WindowLikeAssignment));

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
            => ExecutionTest(nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_Path_Join_NestedFunction()
            => ExecutionTest(nameof(Require_Path_Join_NestedFunction));

        [Fact]
        public Task SetTimeout_MultipleZeroDelay_ExecutedInOrder() => ExecutionTest(nameof(SetTimeout_MultipleZeroDelay_ExecutedInOrder));

        [Fact(Skip = "Flaky in full suite; investigate root cause and re-enable")]
        public Task SetInterval_ExecutesThreeTimes_ThenClears()
        {
            // Test uses 50ms intervals, so should complete in ~150ms plus overhead
            // clearInterval should stop the repeating timer after 3 ticks
            return ExecutionTest(nameof(SetInterval_ExecutesThreeTimes_ThenClears));
        }

        [Fact]
        public async Task SetTimeout_OneSecondDelay()
        {
            var mockTickSource = new MockTickSource();
            var mockWaitHandle = new MockWaitHandle(
                onSet: () => { },
                onWaitOne: (msTimeout) =>
                {
                    // Simulate the passage of time by incrementing the mock tick source.
                    mockTickSource.Increment(TimeSpan.FromMilliseconds(msTimeout));
                });


            var startTime = mockTickSource.GetTicks();

            var addMocks = new Action<JavaScriptRuntime.DependencyInjection.ServiceContainer>(container =>
            {
                container.Replace<JavaScriptRuntime.EngineCore.ITickSource>(mockTickSource);
                container.Replace<JavaScriptRuntime.EngineCore.IWaitHandle>(mockWaitHandle);
            });

            var postTestProcessingAction = new Action<IConsoleOutput>(output =>
            {
                var endTime = mockTickSource.GetTicks();
                var elapsedMs = TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds;
                output.WriteLine($"Elapsed simulated time: {elapsedMs} ms");
            });

            await ExecutionTest(nameof(SetTimeout_OneSecondDelay), postTestProcessingAction: postTestProcessingAction, addMocks: addMocks);
        }

        [Fact]
        public Task SetTimeout_ZeroDelay() => ExecutionTest(nameof(SetTimeout_ZeroDelay));

        [Fact]
        public Task SetTimeout_ZeroDelay_WithArgs() => ExecutionTest(nameof(SetTimeout_ZeroDelay_WithArgs));

        [Fact]
        public Task SetImmediate_ExecutesCallback() => ExecutionTest(nameof(SetImmediate_ExecutesCallback));

        [Fact]
        public Task SetImmediate_WithArgs_PassesCorrectly() => ExecutionTest(nameof(SetImmediate_WithArgs_PassesCorrectly));

        [Fact]
        public Task SetImmediate_Multiple_ExecuteInOrder() => ExecutionTest(nameof(SetImmediate_Multiple_ExecuteInOrder));

        [Fact]
        public Task ClearImmediate_CancelsCallback() => ExecutionTest(nameof(ClearImmediate_CancelsCallback));

        [Fact]
        public Task SetImmediate_ExecutesBeforeSetTimeout() => ExecutionTest(nameof(SetImmediate_ExecutesBeforeSetTimeout));

        [Fact]
        public Task SetImmediate_Nested_ExecutesInNextIteration() => ExecutionTest(nameof(SetImmediate_Nested_ExecutesInNextIteration));
    }
}
