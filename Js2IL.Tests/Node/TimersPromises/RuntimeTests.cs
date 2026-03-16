using System;
using System.Dynamic;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace Js2IL.Tests.Node.TimersPromises
{
    public class RuntimeTests
    {
        [Fact]
        public void TimersPromises_NativeAbortController_PreservesCause_ForOneShotApis()
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
                var timersPromises = new JavaScriptRuntime.Node.TimersPromises();

                var timeoutController = new AbortController();
                timeoutController.abort(new Error("boom-reason"));
                var timeoutPromise = Assert.IsType<JavaScriptRuntime.Promise>(timersPromises.setTimeout(0, "timeout-value", CreateOptions(timeoutController.signal)));
                var timeoutError = Assert.IsType<AbortError>(CaptureRejection(timeoutPromise, eventLoop));
                Assert.Equal("AbortError", timeoutError.name);
                Assert.Equal("ABORT_ERR", timeoutError.code);
                Assert.Equal("The operation was aborted", timeoutError.message);
                var timeoutCause = Assert.IsType<Error>(timeoutError.cause);
                Assert.Equal("Error", timeoutCause.name);
                Assert.Equal("boom-reason", timeoutCause.message);

                var immediateController = new AbortController();
                immediateController.abort();
                var immediatePromise = Assert.IsType<JavaScriptRuntime.Promise>(timersPromises.setImmediate("immediate-value", CreateOptions(immediateController.signal)));
                var immediateError = Assert.IsType<AbortError>(CaptureRejection(immediatePromise, eventLoop));
                Assert.Equal("AbortError", immediateError.name);
                Assert.Equal("ABORT_ERR", immediateError.code);
                Assert.Equal("The operation was aborted", immediateError.message);
                var immediateCause = Assert.IsType<AbortError>(immediateError.cause);
                Assert.Equal("AbortError", immediateCause.name);
                Assert.Equal("This operation was aborted", immediateCause.message);
            }
            finally
            {
                GlobalThis.ServiceProvider = null;
            }
        }

        private static ExpandoObject CreateOptions(object signal)
        {
            dynamic options = new ExpandoObject();
            options.signal = signal;
            return options;
        }

        private static object CaptureRejection(JavaScriptRuntime.Promise promise, NodeEventLoopPump eventLoop)
        {
            object? resolved = null;
            object? rejected = null;

            promise.@then(
                new Func<object?[], object?, object?>((_, value) =>
                {
                    resolved = value;
                    return null;
                }),
                new Func<object?[], object?, object?>((_, reason) =>
                {
                    rejected = reason;
                    return null;
                }));

            DrainEventLoop(eventLoop);

            Assert.Null(resolved);
            return rejected ?? throw new Xunit.Sdk.XunitException("Expected promise rejection.");
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
