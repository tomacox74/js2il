using System.Threading.Tasks;

namespace Js2IL.Tests.CommonJS
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CommonJS")
        {
        }

        [Fact]
        public Task CommonJS_Require_Basic()
        {
            var testName = nameof(CommonJS_Require_Basic);
            return GenerateTest(testName, new[] { "CommonJS_Require_Dependency" });
        }

        [Fact]
        public Task CommonJS_Require_NestedNameConflict()
        {
            // Two different modules share the same basename "b":
            //   require('./b');
            //   require('./helpers/b');
            // Ensure module resolution and generated type names do not collide.
            return GenerateTest(
                "CommonJS_Require_NestedNameConflict/a",
                new[]
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
            return GenerateTest(
                "CommonJS_Require_RelativeFromModule/a",
                new[]
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
            return GenerateTest(
                "CommonJS_Require_SharedDependency_ExecutedOnce/a",
                new[]
                {
                    "CommonJS_Require_SharedDependency_ExecutedOnce/b",
                    "CommonJS_Require_SharedDependency_ExecutedOnce/c",
                    "CommonJS_Require_SharedDependency_ExecutedOnce/d"
                });
        }

        [Fact]
        public Task CommonJS_Require_Reassigned_Function()
        {
            return GenerateTest(nameof(CommonJS_Require_Reassigned_Function));
        }

        [Fact]
        public Task CommonJS_Require_Reassigned_Number_ThrowsTypeError()
        {
            return GenerateTest(nameof(CommonJS_Require_Reassigned_Number_ThrowsTypeError));
        }

        [Fact]
        public Task CommonJS_Require_Shadowed_Parameter()
        {
            return GenerateTest(nameof(CommonJS_Require_Shadowed_Parameter));
        }

        [Fact]
        public Task CommonJS_Module_Exports_Object()
        {
            // Test that exports and module.exports are aliases to the same object
            return GenerateTest(nameof(CommonJS_Module_Exports_Object));
        }

        [Fact]
        public Task CommonJS_Module_Exports_Reassign()
        {
            // Test that reassigning module.exports replaces the entire exports object
            return GenerateTest(nameof(CommonJS_Module_Exports_Reassign));
        }

        [Fact]
        public Task CommonJS_Module_Exports_ChainedAssignment()
        {
            // Issue #558 repro: chained assignment `exports = module.exports = {...}` must compile and decompile.
            return GenerateTest(
                nameof(CommonJS_Module_Exports_ChainedAssignment),
                new[] { "CommonJS_Module_Exports_ChainedAssignment_Lib" });
        }

        [Fact]
        public Task CommonJS_Module_Exports_Function()
        {
            // Test assigning a function to module.exports
            return GenerateTest(nameof(CommonJS_Module_Exports_Function));
        }

        [Fact]
        public Task CommonJS_Module_Identity()
        {
            // Test module.id, module.filename, and module.path properties
            return GenerateTest(nameof(CommonJS_Module_Identity));
        }

        [Fact]
        public Task CommonJS_Module_Loaded()
        {
            // Test module.loaded property
            return GenerateTest(nameof(CommonJS_Module_Loaded));
        }

        [Fact]
        public Task CommonJS_Module_Require()
        {
            // Test module.require() method
            return GenerateTest(nameof(CommonJS_Module_Require));
        }

        [Fact]
        public Task CommonJS_Module_Paths()
        {
            // Test module.paths array
            return GenerateTest(nameof(CommonJS_Module_Paths));
        }

        [Fact]
        public Task CommonJS_Module_ParentChildren()
        {
            // Test module.parent and module.children relationships
            return GenerateTest(
                nameof(CommonJS_Module_ParentChildren),
                new[]
                {
                    "CommonJS_Module_ParentChildren_Child1",
                    "CommonJS_Module_ParentChildren_Child2"
                });
        }

        [Fact]
        public Task CommonJS_Export_Function()
        {
            // Test importing and calling a function exported from another module
            return GenerateTest(
                nameof(CommonJS_Export_Function),
                new[] { "CommonJS_Export_Function_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ObjectWithFunctions()
        {
            // Test importing an object with function properties (issue #156 repro)
            return GenerateTest(
                nameof(CommonJS_Export_ObjectWithFunctions),
                new[] { "CommonJS_Export_ObjectWithFunctions_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_Class()
        {
            // Test importing and instantiating a class from another module
            return GenerateTest(
                nameof(CommonJS_Export_Class),
                new[] { "CommonJS_Export_Class_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ClassWithConstructor()
        {
            // Test importing a class with constructor parameters from another module
            return GenerateTest(
                nameof(CommonJS_Export_ClassWithConstructor),
                new[] { "CommonJS_Export_ClassWithConstructor_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_NestedObjects()
        {
            // Test importing nested literal objects with fields and methods
            return GenerateTest(
                nameof(CommonJS_Export_NestedObjects),
                new[] { "CommonJS_Export_NestedObjects_Lib" });
        }

        [Fact]
        public Task CommonJS_Export_ObjectWithClosure()
        {
            // Issue #167 repro: imported object contains functions that capture module/function scope.
            return GenerateTest(
                nameof(CommonJS_Export_ObjectWithClosure),
                new[] { "CommonJS_Export_ObjectWithClosure_Lib" });
        }

        [Fact]
        public Task CommonJS_Global_ErrorPrototype_Read()
        {
            // Issue #550 repro: IR pipeline crash lowering `Error.prototype` property access in a CommonJS module.
            return GenerateTest(
                nameof(CommonJS_Global_ErrorPrototype_Read),
                new[] { "CommonJS_Global_ErrorPrototype_Read_Lib" });
        }

        [Fact]
        public Task CommonJS_Module_Exports_ClassExpression_ExtendsArray()
        {
            // Issue #552 repro: IR pipeline crash compiling a CommonJS module that exports a class expression.
            return GenerateTest(nameof(CommonJS_Module_Exports_ClassExpression_ExtendsArray));
        }

        [Fact]
        public Task CommonJS_ImportMeta_Basic()
        {
            return GenerateTest(nameof(CommonJS_ImportMeta_Basic));
        }
    }
}
