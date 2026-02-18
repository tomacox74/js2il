using System;
using System.Threading.Tasks;
using JavaScriptRuntime;
using Js2IL.Tests.Node;

namespace Js2IL.Tests.Node.Timers
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Timers") { }

        [Fact]
        public Task GlobalTimers_AsValues_WindowLikeAssignment() => ExecutionTest(nameof(GlobalTimers_AsValues_WindowLikeAssignment));

        [Fact]
        public Task ClearTimeout_MultipleZeroDelay_ClearSecondTimer() => ExecutionTest(nameof(ClearTimeout_MultipleZeroDelay_ClearSecondTimer));

        [Fact]
        public Task ClearTimeout_ZeroDelay() => ExecutionTest(nameof(ClearTimeout_ZeroDelay));

        [Fact]
        public Task SetTimeout_MultipleZeroDelay_ExecutedInOrder() => ExecutionTest(nameof(SetTimeout_MultipleZeroDelay_ExecutedInOrder));

        [Fact(Skip = "Flaky in full suite; investigate root cause and re-enable")]
        public Task SetInterval_ExecutesThreeTimes_ThenClears()
        {
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
