using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Path
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Path") { }

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(
            nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_Path_Join_NestedFunction() => GenerateTest(
            nameof(Require_Path_Join_NestedFunction));

        [Fact]
        public Task Require_Path_Extname_And_IsAbsolute() => GenerateTest(
            nameof(Require_Path_Extname_And_IsAbsolute));

        [Fact]
        public Task Require_Path_Parse_And_Format() => GenerateTest(
            nameof(Require_Path_Parse_And_Format),
            configureSettings: s =>
            {
                s.AddScrubber(sb => sb.Replace('\\', '/'));
            });

        [Fact]
        public Task Require_Path_Normalize_And_Sep() => GenerateTest(
            nameof(Require_Path_Normalize_And_Sep),
            configureSettings: s =>
            {
                s.AddScrubber(sb => sb.Replace('\\', '/'));
            });

        [Fact]
        public Task Require_Path_Delimiter() => GenerateTest(
            nameof(Require_Path_Delimiter));

        [Fact]
        public Task Require_Path_ToNamespacedPath() => GenerateTest(
            nameof(Require_Path_ToNamespacedPath),
            configureSettings: s =>
            {
                s.AddScrubber(sb => sb.Replace('\\', '/'));
            });

        [Fact]
        public Task Require_Path_Relative_SamePath_EmptyString() => GenerateTest(
            nameof(Require_Path_Relative_SamePath_EmptyString));
    }
}
