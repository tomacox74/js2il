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
            // Test that reassigning module.exports replaces the entire exports object
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
                nameof(CommonJS_Module_ParentChildren),
                additionalScripts: new[]
                {
                    "CommonJS_Module_ParentChildren_Child1",
                    "CommonJS_Module_ParentChildren_Child2"
                });
        }

        [Fact]
        public Task CommonJS_Export_Function()
        {
            // Test importing and calling a function exported from another module
            return ExecutionTest(
                nameof(CommonJS_Export_Function),
                additionalScripts: new[] { "CommonJS_Export_Function_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ObjectWithFunctions()
        {
            // Test importing an object with function properties (issue #156 repro)
            return ExecutionTest(
                nameof(CommonJS_Export_ObjectWithFunctions),
                additionalScripts: new[] { "CommonJS_Export_ObjectWithFunctions_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_Class()
        {
            // Test importing and instantiating a class from another module
            return ExecutionTest(
                nameof(CommonJS_Export_Class),
                additionalScripts: new[] { "CommonJS_Export_Class_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ClassWithConstructor()
        {
            // Test importing a class with constructor parameters from another module
            return ExecutionTest(
                nameof(CommonJS_Export_ClassWithConstructor),
                additionalScripts: new[] { "CommonJS_Export_ClassWithConstructor_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_NestedObjects()
        {
            // Test importing nested literal objects with fields and methods
            return ExecutionTest(
                nameof(CommonJS_Export_NestedObjects),
                additionalScripts: new[] { "CommonJS_Export_NestedObjects_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ObjectWithClosure()
        {
            // Issue #167 repro: imported object contains functions that capture module/function scope.
            return ExecutionTest(
                nameof(CommonJS_Export_ObjectWithClosure),
                additionalScripts: new[] { "CommonJS_Export_ObjectWithClosure_Lib" });
        }

        [Fact]
        public Task CommonJS_Global_ErrorPrototype_Read()
        {
            // Issue #550 repro: IR pipeline crash lowering `Error.prototype` property access in a CommonJS module.
            return ExecutionTest(
                nameof(CommonJS_Global_ErrorPrototype_Read),
                additionalScripts: new[] { "CommonJS_Global_ErrorPrototype_Read_Lib" });
        }
    }
}
