using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Math
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Math") { }

        [Fact]
        public Task Math_Ceil_Sqrt_Basic()
        {
            return ExecutionTest(nameof(Math_Ceil_Sqrt_Basic));
        }
    }
}
