using System.Threading.Tasks;

namespace Jroc.Tests.Node.Path
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Path") { }

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(
            nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_NodePath_Join_Basic() => GenerateTest(
            nameof(Require_NodePath_Join_Basic));

        [Fact]
        public Task Require_Path_MemberOverrides() => GenerateTest(
            nameof(Require_Path_MemberOverrides));

        [Fact]
        public Task Require_Path_AliasOverride() => GenerateTest(
            nameof(Require_Path_AliasOverride));

        [Fact]
        public Task Require_Path_RequireAliasOverride() => GenerateTest(
            nameof(Require_Path_RequireAliasOverride));

        [Fact]
        public Task Require_Path_ModuleRequireOverride() => GenerateTest(
            nameof(Require_Path_ModuleRequireOverride));

        [Fact]
        public Task Require_Path_DefaultParameterOverride() => GenerateTest(
            nameof(Require_Path_DefaultParameterOverride));

        [Fact]
        public Task Require_Path_UpdateOverride() => GenerateTest(
            nameof(Require_Path_UpdateOverride));

        [Fact]
        public Task Require_Path_DynamicImportOverride() => GenerateTest(
            nameof(Require_Path_DynamicImportOverride));

        [Fact]
        public Task Require_Path_Join_NestedFunction() => GenerateTest(
            nameof(Require_Path_Join_NestedFunction));

        [Fact]
        public Task Require_Path_Extname_And_IsAbsolute() => GenerateTest(
            nameof(Require_Path_Extname_And_IsAbsolute));

        [Fact]
        public Task Require_Path_Parse_And_Format() => GenerateTest(
            nameof(Require_Path_Parse_And_Format));

        [Fact]
        public Task Require_Path_Normalize_And_Sep() => GenerateTest(
            nameof(Require_Path_Normalize_And_Sep));

        [Fact]
        public Task Require_Path_Delimiter() => GenerateTest(
            nameof(Require_Path_Delimiter));

        [Fact]
        public Task Require_Path_ToNamespacedPath() => GenerateTest(
            nameof(Require_Path_ToNamespacedPath));

        [Fact]
        public Task Require_Path_Relative_SamePath_EmptyString() => GenerateTest(
            nameof(Require_Path_Relative_SamePath_EmptyString));
    }
}
