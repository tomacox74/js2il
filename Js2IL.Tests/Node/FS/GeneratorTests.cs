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
        public Task FSPromises_Realpath() => GenerateTest(
            nameof(FSPromises_Realpath));

        [Fact]
        public Task FSPromises_ReadFile_Buffer() => GenerateTest(
            nameof(FSPromises_ReadFile_Buffer));
    }
}
