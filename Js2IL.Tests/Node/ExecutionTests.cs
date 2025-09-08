using System.Threading.Tasks;
using System.IO;

namespace Js2IL.Tests.Node
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Join_Basic()
            => ExecutionTest(nameof(Require_Path_Join_Basic), configureSettings: s => s.AddScrubber(sb => sb.Replace('\\', '/')));

        [Fact]
        public Task Require_Path_Join_NestedFunction()
            => ExecutionTest(nameof(Require_Path_Join_NestedFunction), configureSettings: s => s.AddScrubber(sb => sb.Replace('\\', '/')));

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
        public Task PerfHooks_PerformanceNow_Basic()
            => ExecutionTest(
                nameof(PerfHooks_PerformanceNow_Basic),
                configureSettings: s =>
                {
                    // Keep as-is; output is integer ms values which should be stable enough.
                });
    }
}
