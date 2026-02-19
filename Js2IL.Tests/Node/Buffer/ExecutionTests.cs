using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Buffer
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Buffer") { }

        [Fact]
        public Task Buffer_From_And_IsBuffer()
            => ExecutionTest(nameof(Buffer_From_And_IsBuffer));
    
        [Fact]
        public Task Buffer_Alloc_ByteLength_Concat()
            => ExecutionTest(nameof(Buffer_Alloc_ByteLength_Concat));

        [Fact]
        public Task Buffer_Slice_Copy_IndexAccess()
            => ExecutionTest(nameof(Buffer_Slice_Copy_IndexAccess));

        [Fact]
        public Task Buffer_AllocUnsafe_Compare()
            => ExecutionTest(nameof(Buffer_AllocUnsafe_Compare));

        [Fact]
        public Task Buffer_ReadWrite_Methods()
            => ExecutionTest(nameof(Buffer_ReadWrite_Methods));

        [Fact]
        public Task Buffer_Advanced_CoreApis()
            => ExecutionTest(nameof(Buffer_Advanced_CoreApis));
    }
}