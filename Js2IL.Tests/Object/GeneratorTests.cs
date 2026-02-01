using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Object
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Object")
        {
        }

        [Fact]
        public Task ObjectLiteral_Spread_Basic() { var testName = nameof(ObjectLiteral_Spread_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_Basic() { var testName = nameof(ObjectLiteral_ComputedKey_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_EvaluationOrder() { var testName = nameof(ObjectLiteral_ComputedKey_EvaluationOrder); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ShorthandAndMethod() { var testName = nameof(ObjectLiteral_ShorthandAndMethod); return GenerateTest(testName); }

        [Fact]
        public Task PrototypeChain_Basic() { var testName = nameof(PrototypeChain_Basic); return GenerateTest(testName); }
    }
}
