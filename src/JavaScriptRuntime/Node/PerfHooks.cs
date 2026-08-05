using System;
using System.Diagnostics;

namespace JavaScriptRuntime.Node
{
    // Minimal perf_hooks module exposing performance.now()
    [NodeModule("perf_hooks")]
    public sealed partial class PerfHooks
    {
        private readonly Performance _performance = new Performance();
        public Performance performance => _performance;

        public sealed partial class Performance
        {
            private static readonly long _origin = Stopwatch.GetTimestamp();

            object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

            public double now()
            {
                long ticks = Stopwatch.GetTimestamp() - _origin;
                return (double)ticks * 1000.0 / Stopwatch.Frequency;
            }
        }
    }
}
