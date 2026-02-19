using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Util
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Util") { }

        [Fact]
        public Task Require_Util_Promisify_Basic()
            => ExecutionTest(nameof(Require_Util_Promisify_Basic));

        [Fact]
        public Task Require_Util_Promisify_ErrorHandling()
            => ExecutionTest(nameof(Require_Util_Promisify_ErrorHandling));

        [Fact]
        public Task Require_Util_Inherits_Basic()
            => ExecutionTest(nameof(Require_Util_Inherits_Basic));

        [Fact]
        public Task Require_Util_Types_IsPromise()
            => ExecutionTest(nameof(Require_Util_Types_IsPromise));

        [Fact]
        public Task Require_Util_Types_IsArray()
            => ExecutionTest(nameof(Require_Util_Types_IsArray));

        [Fact]
        public Task Require_Util_Types_IsFunction()
            => ExecutionTest(nameof(Require_Util_Types_IsFunction));

        [Fact]
        public Task Require_Util_Inspect_Basic()
            => ExecutionTest(nameof(Require_Util_Inspect_Basic));

        [Fact]
        public Task Require_Util_Inspect_Object()
            => ExecutionTest(nameof(Require_Util_Inspect_Object));
    }
}
