using System.IO;
using System.Threading.Tasks;

namespace Js2IL.Tests.Node
{
    public class PathAdditionalTests : ExecutionTestsBase
    {
        public PathAdditionalTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Resolve_Relative_To_Absolute()
            => ExecutionTest(
                nameof(Require_Path_Resolve_Relative_To_Absolute),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task Require_Path_Relative_Between_Two_Paths()
            => ExecutionTest(
                nameof(Require_Path_Relative_Between_Two_Paths),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Basename_And_Dirname()
            => ExecutionTest(
                nameof(Require_Path_Basename_And_Dirname),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });
    }
}
