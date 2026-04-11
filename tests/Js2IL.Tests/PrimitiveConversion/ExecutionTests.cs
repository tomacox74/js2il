using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.PrimitiveConversion
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("PrimitiveConversion") { }

        [Fact]
        public Task PrimitiveConversion_String_Callable()
        {
            return ExecutionTest(nameof(PrimitiveConversion_String_Callable));
        }

        [Fact]
        public Task PrimitiveConversion_Number_Callable()
        {
            return ExecutionTest(nameof(PrimitiveConversion_Number_Callable));
        }

        [Fact]
        public Task PrimitiveConversion_Boolean_Callable()
        {
            return ExecutionTest(nameof(PrimitiveConversion_Boolean_Callable));
        }
    }
}
