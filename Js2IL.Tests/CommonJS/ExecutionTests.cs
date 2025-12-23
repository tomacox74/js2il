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

        [Fact]
        public Task CommonJS_Module_Exports_Object()
        {
            // Test that exports and module.exports are aliases to the same object
            return ExecutionTest(nameof(CommonJS_Module_Exports_Object));
        }

        [Fact]
        public Task CommonJS_Module_Exports_Reassign()
        {
            // Test that reassigning exports does NOT change module.exports
            return ExecutionTest(nameof(CommonJS_Module_Exports_Reassign));
        }

        [Fact]
        public Task CommonJS_Module_Exports_Function()
        {
            // Test assigning a function to module.exports
            return ExecutionTest(nameof(CommonJS_Module_Exports_Function));
        }

        [Fact]
        public Task CommonJS_Module_Identity()
        {
            // Test module.id, module.filename, and module.path properties
            return ExecutionTest(nameof(CommonJS_Module_Identity));
        }

        [Fact]
        public Task CommonJS_Module_Loaded()
        {
            // Test module.loaded property
            return ExecutionTest(nameof(CommonJS_Module_Loaded));
        }

        [Fact]
        public Task CommonJS_Module_Require()
        {
            // Test module.require() method
            return ExecutionTest(nameof(CommonJS_Module_Require));
        }

        [Fact]
        public Task CommonJS_Module_Paths()
        {
            // Test module.paths array
            return ExecutionTest(nameof(CommonJS_Module_Paths));
        }

        [Fact]
        public Task CommonJS_Module_ParentChildren()
        {
            // Test module.parent and module.children relationships
            return ExecutionTest(
                "CommonJS_Module_ParentChildren_Main",
                additionalScripts: new[]
                {
                    "CommonJS_Module_ParentChildren_Child1",
                    "CommonJS_Module_ParentChildren_Child2"
                });
        }
    }
}
