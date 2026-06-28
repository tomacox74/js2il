using System.Threading.Tasks;
using Xunit;

namespace Jroc.Tests.CoercionOptimization
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("CoercionOptimization") { }

        [Fact]
        public Task CoercionCSE_BoxedDoubleToNumber_NoCse()
        {
            return ExecutionTest(nameof(CoercionCSE_BoxedDoubleToNumber_NoCse));
        }

        [Fact]
        public Task CoercionCSE_ObjectValueOf()
        {
            return ExecutionTest(nameof(CoercionCSE_ObjectValueOf));
        }

        [Fact]
        public Task NumericInference_ExponentiationLoop_NoBoxing()
        {
            return ExecutionTest(nameof(NumericInference_ExponentiationLoop_NoBoxing));
        }
    }
}
