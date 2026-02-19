using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Buffer
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Buffer") { }

        [Fact]
        public Task Buffer_From_And_IsBuffer() => GenerateTest(
            nameof(Buffer_From_And_IsBuffer));

        [Fact]
        public Task Buffer_Alloc_ByteLength_Concat() => GenerateTest(
            nameof(Buffer_Alloc_ByteLength_Concat));

        [Fact]
        public Task Buffer_Slice_Copy_IndexAccess() => GenerateTest(
            nameof(Buffer_Slice_Copy_IndexAccess));

        [Fact]
        public Task Buffer_AllocUnsafe_Compare() => GenerateTest(
            nameof(Buffer_AllocUnsafe_Compare));

        [Fact]
        public Task Buffer_ReadWrite_Methods() => GenerateTest(
            nameof(Buffer_ReadWrite_Methods));
    }
}