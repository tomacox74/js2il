using System.Threading.Tasks;

namespace Js2IL.Tests.CommonJS
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("CommonJS")
        {
        }

        [Fact]
        public Task CommonJS_Require_Basic()
        {
            var testName = nameof(CommonJS_Require_Basic);
            return ExecutionTest(testName, additionalScripts: new[] { "CommonJS_Require_Dependency" });
        }
    }
}
