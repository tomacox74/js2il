using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Proxy
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Proxy")
        {
        }

        [Fact]
        public Task Proxy_GetTrap_OverridesProperty() { var testName = nameof(Proxy_GetTrap_OverridesProperty); return GenerateTest(testName); }

        [Fact]
        public Task Proxy_SetTrap_InterceptsWrites() { var testName = nameof(Proxy_SetTrap_InterceptsWrites); return GenerateTest(testName); }

        [Fact]
        public Task Proxy_HasTrap_AffectsInOperator() { var testName = nameof(Proxy_HasTrap_AffectsInOperator); return GenerateTest(testName); }
    }
}
