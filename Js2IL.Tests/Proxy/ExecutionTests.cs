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

        [Fact]
        public Task Proxy_DeletePropertyTrap_And_Fallback() { var testName = nameof(Proxy_DeletePropertyTrap_And_Fallback); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_OwnKeys_And_PrototypeTraps_WithFallback() { var testName = nameof(Proxy_OwnKeys_And_PrototypeTraps_WithFallback); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_ApplyAndConstructTraps_WithFallback() { var testName = nameof(Proxy_ApplyAndConstructTraps_WithFallback); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_Revocable_ThrowsAfterRevoke() { var testName = nameof(Proxy_Revocable_ThrowsAfterRevoke); return ExecutionTest(testName); }

        [Fact]
        public Task Proxy_Validation_EdgeCases() { var testName = nameof(Proxy_Validation_EdgeCases); return ExecutionTest(testName); }
    }
}
