using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CoercionOptimization
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CoercionOptimization") { }

        [Fact]
        public Task CoercionCSE_DoubleToNumber()
        {
            return GenerateTest(nameof(CoercionCSE_DoubleToNumber));
        }

        [Fact]
        public Task CoercionCSE_ObjectValueOf()
        {
            return GenerateTest(nameof(CoercionCSE_ObjectValueOf));
        }
    }
}
