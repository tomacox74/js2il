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

        [Fact]
        public Task FSPromises_CopyFile()
            => ExecutionTest(
                nameof(FSPromises_CopyFile),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Readdir_Names()
            => ExecutionTest(
                nameof(FSPromises_Readdir_Names),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Readdir_WithFileTypes()
            => ExecutionTest(
                nameof(FSPromises_Readdir_WithFileTypes),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Readdir_MissingDir_Rejects()
            => ExecutionTest(
                nameof(FSPromises_Readdir_MissingDir_Rejects),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Open_Read_Write_Close()
            => ExecutionTest(
                nameof(FSPromises_Open_Read_Write_Close),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Open_ExplicitPosition_DoesNotMoveOffset()
            => ExecutionTest(
                nameof(FSPromises_Open_ExplicitPosition_DoesNotMoveOffset),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Open_Callback_FileHandle()
            => ExecutionTest(
                nameof(FS_Open_Callback_FileHandle),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_CreateReadStream_Basic()
            => ExecutionTest(
                nameof(FS_CreateReadStream_Basic),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_CreateReadStream_Missing_Error()
            => ExecutionTest(
                nameof(FS_CreateReadStream_Missing_Error),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_CreateWriteStream_Basic()
            => ExecutionTest(
                nameof(FS_CreateWriteStream_Basic),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_CreateWriteStream_Missing_Error()
            => ExecutionTest(
                nameof(FS_CreateWriteStream_Missing_Error),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Append_Rename_Unlink()
            => ExecutionTest(
                nameof(FSPromises_Append_Rename_Unlink),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FSPromises_Rename_ExistingDirectory_Rejects()
            => ExecutionTest(
                nameof(FSPromises_Rename_ExistingDirectory_Rejects),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Append_Rename_Unlink_Callback()
            => ExecutionTest(
                nameof(FS_Append_Rename_Unlink_Callback),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_ReadFile_Callback_Utf8()
            => ExecutionTest(
                nameof(FS_ReadFile_Callback_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_ReadFile_Callback_MissingFile_ENOENT()
            => ExecutionTest(
                nameof(FS_ReadFile_Callback_MissingFile_ENOENT),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_WriteFile_Callback_Utf8()
            => ExecutionTest(
                nameof(FS_WriteFile_Callback_Utf8),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_CopyFile_Callback()
            => ExecutionTest(
                nameof(FS_CopyFile_Callback),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Readdir_Callback_Names()
            => ExecutionTest(
                nameof(FS_Readdir_Callback_Names),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Stat_Callback_FileSize()
            => ExecutionTest(
                nameof(FS_Stat_Callback_FileSize),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Mkdir_Callback_Recursive()
            => ExecutionTest(
                nameof(FS_Mkdir_Callback_Recursive),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Rm_Callback_Removes_File()
            => ExecutionTest(
                nameof(FS_Rm_Callback_Removes_File),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Access_Callback()
            => ExecutionTest(
                nameof(FS_Access_Callback),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Realpath_Callback()
            => ExecutionTest(
                nameof(FS_Realpath_Callback),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });

        [Fact]
        public Task FS_Promises_Property()
            => ExecutionTest(
                nameof(FS_Promises_Property),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                });
    }
}
