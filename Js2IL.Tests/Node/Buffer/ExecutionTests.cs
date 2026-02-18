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
    }
}