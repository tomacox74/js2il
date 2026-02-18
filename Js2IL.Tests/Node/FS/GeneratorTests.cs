using System.Threading.Tasks;

namespace Js2IL.Tests.Node.FS
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/FS") { }

        [Fact]
        public Task FS_ReadWrite_Utf8() => GenerateTest(
            nameof(FS_ReadWrite_Utf8));
    }
}
