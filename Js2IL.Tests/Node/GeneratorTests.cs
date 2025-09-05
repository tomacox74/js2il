using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Js2IL.Tests.Node
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(
            nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_Path_Join_NestedFunction() => GenerateTest(
            nameof(Require_Path_Join_NestedFunction));

    [Fact]
        public Task Global___dirname_PrintsDirectory() => GenerateTest(
            nameof(Global___dirname_PrintsDirectory));

        [Fact]
        public Task Environment_EnumerateProcessArgV() => GenerateTest(
            nameof(Environment_EnumerateProcessArgV));

        [Fact]
        public Task FS_ReadWrite_Utf8() => GenerateTest(
            nameof(FS_ReadWrite_Utf8));

        [Fact]
        public Task PerfHooks_PerformanceNow_Basic() => GenerateTest(
            nameof(PerfHooks_PerformanceNow_Basic));
    }
}
