using System.Threading.Tasks;

namespace Js2IL.Tests.WeakRef
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

        public ExecutionTests() : base("WeakRef") { }

        [Fact]
        public Task WeakRef_Deref_KeptObjects() { var testName = nameof(WeakRef_Deref_KeptObjects); return ExecutionTest(testName, addMocks: EnableGc); }
    }
}
