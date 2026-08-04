using System.IO;
using System.Threading.Tasks;

namespace Jroc.Tests.Node.Path
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Path") { }

        [Fact]
        public Task Require_Path_Join_Basic()
            => ExecutionTest(nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_NodePath_Join_Basic()
            => ExecutionTest(nameof(Require_NodePath_Join_Basic));

        [Fact]
        public Task Require_Path_MemberOverrides()
            => ExecutionTest(nameof(Require_Path_MemberOverrides));

        [Fact]
        public Task Require_Path_AliasOverride()
            => ExecutionTest(nameof(Require_Path_AliasOverride));

        [Fact]
        public Task Require_Path_RequireAliasOverride()
            => ExecutionTest(nameof(Require_Path_RequireAliasOverride));

        [Fact]
        public Task Require_Path_ModuleRequireOverride()
            => ExecutionTest(nameof(Require_Path_ModuleRequireOverride));

        [Fact]
        public Task Require_Path_DefaultParameterOverride()
            => ExecutionTest(nameof(Require_Path_DefaultParameterOverride));

        [Fact]
        public Task Require_Path_UpdateOverride()
            => ExecutionTest(nameof(Require_Path_UpdateOverride));

        [Fact]
        public Task Require_Path_DynamicImportOverride()
            => ExecutionTest(nameof(Require_Path_DynamicImportOverride));

        [Fact]
        public Task Require_Path_DynamicImportThenOverride()
            => ExecutionTest(nameof(Require_Path_DynamicImportThenOverride));

        [Fact]
        public Task Require_Path_DynamicImportThenPrototypeOverride()
            => ExecutionTest(nameof(Require_Path_DynamicImportThenPrototypeOverride));

        [Fact]
        public Task Require_Path_Join_NestedFunction()
            => ExecutionTest(nameof(Require_Path_Join_NestedFunction));

        [Fact]
        public Task Require_Path_Resolve_Relative_To_Absolute()
            => ExecutionTest(
                nameof(Require_Path_Resolve_Relative_To_Absolute),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
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
        public Task Require_Path_Relative_SamePath_EmptyString()
            => ExecutionTest(
                nameof(Require_Path_Relative_SamePath_EmptyString),
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
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task Require_Path_Join_Normalizes_DotDot()
            => ExecutionTest(
                nameof(Require_Path_Join_Normalizes_DotDot),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Extname_And_IsAbsolute()
            => ExecutionTest(
                nameof(Require_Path_Extname_And_IsAbsolute),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Parse_And_Format()
            => ExecutionTest(
                nameof(Require_Path_Parse_And_Format),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Normalize_And_Sep()
            => ExecutionTest(
                nameof(Require_Path_Normalize_And_Sep),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Delimiter()
            => ExecutionTest(nameof(Require_Path_Delimiter));

        [Fact]
        public Task Require_Path_ToNamespacedPath()
            => ExecutionTest(
                nameof(Require_Path_ToNamespacedPath),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task Require_Path_Posix_Win32_Basics()
            => ExecutionTest(nameof(Require_Path_Posix_Win32_Basics));
    }
}
