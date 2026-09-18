using System;
using System.Diagnostics;

namespace JavaScriptRuntime.Node
{
    // Minimal perf_hooks module exposing performance.now()
    [NodeModule("perf_hooks")]
    public sealed partial class PerfHooks
    {
        public Performance performance => RuntimeIntrinsics.Current.Performance;

        public sealed partial class Performance : JsObject
        {
            private static readonly long _origin = Stopwatch.GetTimestamp();

            public Performance()
            {
                SetObject("now", (BuiltinFunction0)(static _ => NowCore()));
            }

            object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

            public double now() => NowCore();

            private static double NowCore()
            {
                long ticks = Stopwatch.GetTimestamp() - _origin;
                return (double)ticks * 1000.0 / Stopwatch.Frequency;
            }
        }
    }
}
