using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CoercionOptimization
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("CoercionOptimization") { }

        [Fact]
        public Task CoercionCSE_DoubleToNumber()
        {
            return ExecutionTest(nameof(CoercionCSE_DoubleToNumber));
        }

        [Fact]
        public Task CoercionCSE_ObjectValueOf()
        {
            return ExecutionTest(nameof(CoercionCSE_ObjectValueOf));
        }
    }
}
