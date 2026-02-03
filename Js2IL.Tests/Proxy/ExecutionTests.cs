using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Proxy
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Proxy")
        {
        }

        [Fact]
        public Task Proxy_GetTrap_OverridesProperty() { var testName = nameof(Proxy_GetTrap_OverridesProperty); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_SetTrap_InterceptsWrites() { var testName = nameof(Proxy_SetTrap_InterceptsWrites); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_HasTrap_AffectsInOperator() { var testName = nameof(Proxy_HasTrap_AffectsInOperator); return ExecutionTest(testName); }
    }
}
