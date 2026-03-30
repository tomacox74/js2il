using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CoercionOptimization
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CoercionOptimization") { }

        [Fact]
        public Task CoercionCSE_BoxedDoubleToNumber_NoCse()
        {
            return GenerateTest(nameof(CoercionCSE_BoxedDoubleToNumber_NoCse));
        }

        [Fact]
        public Task CoercionCSE_ObjectValueOf()
        {
            return GenerateTest(nameof(CoercionCSE_ObjectValueOf));
        }
    }
}
