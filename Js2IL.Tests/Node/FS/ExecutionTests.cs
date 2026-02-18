using System.Threading.Tasks;

namespace Js2IL.Tests.Node.FS
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/FS") { }

        [Fact]
        public Task FS_ReadWrite_Utf8()
            => ExecutionTest(
                nameof(FS_ReadWrite_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_ExistsSync_File_And_Directory()
            => ExecutionTest(
                nameof(FS_ExistsSync_File_And_Directory),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_ReaddirSync_Basic_Names()
            => ExecutionTest(
                nameof(FS_ReaddirSync_Basic_Names),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_ReaddirSync_WithFileTypes()
            => ExecutionTest(
                nameof(FS_ReaddirSync_WithFileTypes),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_StatSync_FileSize()
            => ExecutionTest(
                nameof(FS_StatSync_FileSize),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_RmSync_Removes_File_And_Directory()
            => ExecutionTest(
                nameof(FS_RmSync_Removes_File_And_Directory),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_ExistsSync_EmptyPath_ReturnsFalse()
            => ExecutionTest(
                nameof(FS_ExistsSync_EmptyPath_ReturnsFalse),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_StatSync_NonExistentPath_ReturnsZero()
            => ExecutionTest(
                nameof(FS_StatSync_NonExistentPath_ReturnsZero),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });

        [Fact]
        public Task FS_ReaddirSync_NonExistent_ReturnsEmpty()
            => ExecutionTest(
                nameof(FS_ReaddirSync_NonExistent_ReturnsEmpty),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                });
    }
}
