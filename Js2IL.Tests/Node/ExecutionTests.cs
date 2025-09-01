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
                });
    }
}
