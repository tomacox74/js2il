using System.Threading.Tasks;

namespace Js2IL.Tests.Node
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node") { }

        [Fact]
        public Task Global___dirname_PrintsDirectory()
            => ExecutionTest(
                nameof(Global___dirname_PrintsDirectory),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
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

    }
}
