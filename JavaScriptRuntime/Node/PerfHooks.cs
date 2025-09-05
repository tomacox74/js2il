using System;
using System.Diagnostics;

namespace JavaScriptRuntime.Node
{
    // Minimal perf_hooks module exposing performance.now()
    [NodeModule("perf_hooks")]
    public sealed class PerfHooks
    {
        private readonly Performance _performance = new Performance();
        public Performance performance => _performance;

        public sealed class Performance
        {
            private static readonly long _origin = Stopwatch.GetTimestamp();

            // Returns milliseconds with fractional precision since an arbitrary time origin (process start).
            // Return type is object (boxed double) to align with JS number boxing semantics.
            public object now()
            {
                long ticks = Stopwatch.GetTimestamp() - _origin;
                double ms = (double)ticks * 1000.0 / Stopwatch.Frequency;
                return ms; // boxed as object
            }
        }
    }
}
