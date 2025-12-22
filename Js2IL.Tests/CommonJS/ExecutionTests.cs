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

        [Fact]
        public Task CommonJS_Require_NestedNameConflict()
        {
            // Two different modules share the same basename "b":
            //   require('./b');
            //   require('./helpers/b');
            // Ensure module resolution and generated type names do not collide.
            return ExecutionTest(
                "CommonJS_Require_NestedNameConflict/a",
                additionalScripts: new[]
                {
                    "CommonJS_Require_NestedNameConflict/b",
                    "CommonJS_Require_NestedNameConflict/helpers/b"
                });
        }

        [Fact]
        public Task CommonJS_Require_RelativeFromModule()
        {
            // module a -> require('./helpers/b')
            // module helpers/b -> require('./c')  (full path is ./helpers/c)
            // Ensure relative resolution is based on the requiring module's directory.
            return ExecutionTest(
                "CommonJS_Require_RelativeFromModule/a",
                additionalScripts: new[]
                {
                    "CommonJS_Require_RelativeFromModule/helpers/b",
                    "CommonJS_Require_RelativeFromModule/helpers/c"
                });
        }

        [Fact]
        public Task CommonJS_Require_SharedDependency_ExecutedOnce()
        {
            // a requires b and c
            // b requires d
            // c requires d
            // d should only execute once (CommonJS caching semantics)
            return ExecutionTest(
                "CommonJS_Require_SharedDependency_ExecutedOnce/a",
                additionalScripts: new[]
                {
                    "CommonJS_Require_SharedDependency_ExecutedOnce/b",
                    "CommonJS_Require_SharedDependency_ExecutedOnce/c",
                    "CommonJS_Require_SharedDependency_ExecutedOnce/d"
                });
        }
    }
}
