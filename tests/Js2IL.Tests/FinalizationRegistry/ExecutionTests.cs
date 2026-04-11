using System.Threading.Tasks;

namespace Js2IL.Tests.FinalizationRegistry
{
    public class ExecutionTests : ExecutionTestsBase
    {
        private static void EnableGc(JavaScriptRuntime.DependencyInjection.ServiceContainer serviceProvider)
        {
            serviceProvider.Replace(new JavaScriptRuntime.GlobalThisOptions
            {
                ExposeGc = true
            });
        }

        public ExecutionTests() : base("FinalizationRegistry") { }

        [Fact]
        public Task FinalizationRegistry_Cleanup_Order() { var testName = nameof(FinalizationRegistry_Cleanup_Order); return ExecutionTest(testName, addMocks: EnableGc); }

        [Fact]
        public Task FinalizationRegistry_Unregister_Basic() { var testName = nameof(FinalizationRegistry_Unregister_Basic); return ExecutionTest(testName, addMocks: EnableGc); }
    }
}
