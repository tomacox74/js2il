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
        public Task FS_ReadWrite_Buffer()
            => ExecutionTest(
                nameof(FS_ReadWrite_Buffer),
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

        [Fact]
        public Task FSPromises_ReadFile_Utf8()
            => ExecutionTest(
                nameof(FSPromises_ReadFile_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_WriteFile_Utf8()
            => ExecutionTest(
                nameof(FSPromises_WriteFile_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_WriteFile_NullRejects()
            => ExecutionTest(
                nameof(FSPromises_WriteFile_NullRejects),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Stat_FileSize()
            => ExecutionTest(
                nameof(FSPromises_Stat_FileSize),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Realpath()
            => ExecutionTest(
                nameof(FSPromises_Realpath),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_ReadFile_Buffer()
            => ExecutionTest(
                nameof(FSPromises_ReadFile_Buffer),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_ReadFile_MissingFile_Rejects()
            => ExecutionTest(
                nameof(FSPromises_ReadFile_MissingFile_Rejects),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_ReadFile_Directory_RejectsEISDIR()
            => ExecutionTest(
                nameof(FSPromises_ReadFile_Directory_RejectsEISDIR),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });
    }
}
