using System.Threading.Tasks;

namespace Js2IL.Tests.Node.FS
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/FS") { }

        [Fact]
        public Task FS_ReadWrite_Utf8() => GenerateTest(
            nameof(FS_ReadWrite_Utf8));

        [Fact]
        public Task FS_ReadWrite_Buffer() => GenerateTest(
            nameof(FS_ReadWrite_Buffer));

        [Fact]
        public Task FSPromises_ReadFile_Utf8() => GenerateTest(
            nameof(FSPromises_ReadFile_Utf8));

        [Fact]
        public Task FSPromises_WriteFile_Utf8() => GenerateTest(
            nameof(FSPromises_WriteFile_Utf8));

        [Fact]
        public Task FSPromises_Stat_FileSize() => GenerateTest(
            nameof(FSPromises_Stat_FileSize));

        [Fact]
        public Task FSPromises_Stat_RichMetadata() => GenerateTest(
            nameof(FSPromises_Stat_RichMetadata));

        [Fact]
        public Task FSPromises_Realpath() => GenerateTest(
            nameof(FSPromises_Realpath));

        [Fact]
        public Task FSPromises_ReadFile_Buffer() => GenerateTest(
            nameof(FSPromises_ReadFile_Buffer));

        [Fact]
        public Task FSPromises_Readdir_Names() => GenerateTest(
            nameof(FSPromises_Readdir_Names));

        [Fact]
        public Task FSPromises_Readdir_WithFileTypes() => GenerateTest(
            nameof(FSPromises_Readdir_WithFileTypes));

        [Fact]
        public Task FSPromises_Readdir_MissingDir_Rejects() => GenerateTest(
            nameof(FSPromises_Readdir_MissingDir_Rejects));

        [Fact]
        public Task FSPromises_Open_Read_Write_Close() => GenerateTest(
            nameof(FSPromises_Open_Read_Write_Close));

        [Fact]
        public Task FSPromises_Open_ExplicitPosition_DoesNotMoveOffset() => GenerateTest(
            nameof(FSPromises_Open_ExplicitPosition_DoesNotMoveOffset));

        [Fact]
        public Task FS_Open_Callback_FileHandle() => GenerateTest(
            nameof(FS_Open_Callback_FileHandle));

        [Fact]
        public Task FS_StatSync_RichMetadata() => GenerateTest(
            nameof(FS_StatSync_RichMetadata));

        [Fact]
        public Task FS_CreateReadStream_Basic() => GenerateTest(
            nameof(FS_CreateReadStream_Basic));

        [Fact]
        public Task FS_CreateReadStream_Missing_Error() => GenerateTest(
            nameof(FS_CreateReadStream_Missing_Error));

        [Fact]
        public Task FS_CreateWriteStream_Basic() => GenerateTest(
            nameof(FS_CreateWriteStream_Basic));

        [Fact]
        public Task FS_CreateWriteStream_Missing_Error() => GenerateTest(
            nameof(FS_CreateWriteStream_Missing_Error));

        [Fact]
        public Task FSPromises_Append_Rename_Unlink() => GenerateTest(
            nameof(FSPromises_Append_Rename_Unlink));

        [Fact]
        public Task FSPromises_Rename_ExistingDirectory_Rejects() => GenerateTest(
            nameof(FSPromises_Rename_ExistingDirectory_Rejects));

        [Fact]
        public Task FS_Append_Rename_Unlink_Callback() => GenerateTest(
            nameof(FS_Append_Rename_Unlink_Callback));
    }
}
