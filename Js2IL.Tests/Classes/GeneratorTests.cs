using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Classes") { }

        // Minimal repro: bit-shift and Int32Array length in a class constructor
        // This triggers invalid IL patterns (conv/add on boxed objects) in current codegen
        [Fact]
        public Task Classes_BitShiftInCtor_Int32Array()
        {
            var testName = nameof(Classes_BitShiftInCtor_Int32Array);
            return GenerateTest(testName, verifyAssembly: assembly =>
            {
                var moduleType = assembly.GetType("Modules.Classes_BitShiftInCtor_Int32Array");
                Assert.NotNull(moduleType);

                // Module root type should be internal (non-public).
                Assert.False(moduleType.IsPublic);
                Assert.True(moduleType.IsNotPublic);

                // Module root should contain the nested module scope type.
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
                var nestedScopeType = moduleType.GetNestedType("Scope", flags);
                Assert.NotNull(nestedScopeType);
            });
        }

        [Fact]
        public Task Classes_ClassConstructor_AccessFunctionVariable_Log()
        {
            var testName = nameof(Classes_ClassConstructor_AccessFunctionVariable_Log);
            return GenerateTest(testName, verifyAssembly: assembly =>
            {
                var moduleType = assembly.GetType($"Modules.{testName}");
                Assert.NotNull(moduleType);

                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
                var functionType = moduleType.GetNestedType("testFunction", flags);
                Assert.NotNull(functionType);

                var nestedClass = functionType.GetNestedType("MyClass", flags);
                Assert.NotNull(nestedClass);
            });
        }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariableAndParameterValue_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndParameterValue_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_AccessGlobalVariable_Log() { var testName = nameof(Classes_ClassConstructor_AccessGlobalVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_AccessGlobalVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessGlobalVariableAndParameterValue_Log); return GenerateTest(testName); }

        // Repro: new ClassName() inside an arrow function failed to resolve the declared class in nested generator context.
        // Bug has been fixed: codegen in arrow/function now shares the parent ClassRegistry and targets the declared class.
        // Test is now active.
        [Fact]
        public Task Classes_ClassConstructor_New_In_ArrowFunction()
        {
            var testName = nameof(Classes_ClassConstructor_New_In_ArrowFunction);
            return GenerateTest(testName);
        }

        // Repro: class constructor instantiates another class that references a global variable
        // This should fail with InvalidOperationException due to missing _scopes field
        [Fact]
        public Task Classes_ClassConstructor_NewClassReferencingGlobal()
        {
            var testName = nameof(Classes_ClassConstructor_NewClassReferencingGlobal);
            return GenerateTest(testName);
        }

        // Test parameter destructuring in class constructors
        [Fact]
        public Task Classes_ClassConstructor_ParameterDestructuring()
        {
            var testName = nameof(Classes_ClassConstructor_ParameterDestructuring);
            return GenerateTest(testName);
        }

        [Fact] public Task Classes_ClassConstructor_Param_Field_Log() { var testName = nameof(Classes_ClassConstructor_Param_Field_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_AddMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_AddMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_SubtractMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_SubtractMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_WithMultipleParameters() { var testName = nameof(Classes_ClassConstructor_WithMultipleParameters); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessArrowFunctionVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessArrowFunctionVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessArrowFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessArrowFunctionVariableAndGlobalVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessFunctionVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessFunctionVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessFunctionVariableAndGlobalVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessGlobalVariable_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_ReturnsThis_IsSelf_Log() { var testName = nameof(Classes_ClassMethod_ReturnsThis_IsSelf_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_CallsAnotherMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_ForLoop_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_ForLoop_CallsAnotherMethod); return GenerateTest(testName); }

        // Test parameter destructuring in class methods
        [Fact]
        public Task Classes_ClassMethod_ParameterDestructuring()
        {
            var testName = nameof(Classes_ClassMethod_ParameterDestructuring);
            return GenerateTest(testName);
        }

        // Repro for UpdateExpression applied to a method parameter inside while-loop
        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Postfix); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Prefix); return GenerateTest(testName); }

        // Repro for UpdateExpression inside a class method while-loop (postfix and prefix)
        [Fact] public Task Classes_ClassMethod_While_Increment_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Postfix); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Prefix); return GenerateTest(testName); }

        [Fact] public Task Classes_ClassPrivateField_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateField_HelperMethod_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassPrivateProperty_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateProperty_HelperMethod_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassProperty_DefaultAndLog() { var testName = nameof(Classes_ClassProperty_DefaultAndLog); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithStaticMethod_HelloWorld() { var testName = nameof(Classes_ClassWithStaticMethod_HelloWorld); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithStaticProperty_DefaultAndLog() { var testName = nameof(Classes_ClassWithStaticProperty_DefaultAndLog); return GenerateTest(testName); }
        [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return GenerateTest(testName); }

        [Fact]
        public Task Classes_DefaultParameterValue_Constructor()
        {
            var testName = nameof(Classes_DefaultParameterValue_Constructor);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Classes_DefaultParameterValue_Method()
        {
            var testName = nameof(Classes_DefaultParameterValue_Method);
            return GenerateTest(testName);
        }

        [Fact]
        public Task Classes_ClassFieldTypeInference_Primitives()
        {
            var testName = nameof(Classes_ClassFieldTypeInference_Primitives);
            return GenerateTest(testName);
        }

        [Fact] public Task Classes_ForLoopMin() { var testName = nameof(Classes_ForLoopMin); return GenerateTest(testName); }

        // Bug repro: class method local variable initialized as number, then reassigned from method call.
        // Type inference marks 'factor' as stable double, but method calls return object - must unbox before storing.
        [Fact]
        public Task Classes_ClassMethod_LocalVar_ReassignedFromMethodCall()
        {
            var testName = nameof(Classes_ClassMethod_LocalVar_ReassignedFromMethodCall);
            return GenerateTest(testName);
        }

        // Minimal repro for InvalidProgramException: invalid boxing order in ctor for expression '1 + this.sieveSizeInBits'
        // Test enabled to validate current behavior.
        [Fact]
        public Task Classes_PrimeCtor_BitArrayAdd()
        {
            var testName = nameof(Classes_PrimeCtor_BitArrayAdd);
            return GenerateTest(testName);
        }

        // PL5.4: Test that constructors implicitly return 'this'
        [Fact]
        public Task Classes_Constructor_ImplicitlyReturnsThis()
        {
            var testName = nameof(Classes_Constructor_ImplicitlyReturnsThis);
            return GenerateTest(testName);
        }

        // PL5.4a: Constructor with explicit 'return this;' - should work correctly
        [Fact]
        public Task Classes_Constructor_ExplicitReturnThis()
        {
            var testName = nameof(Classes_Constructor_ExplicitReturnThis);
            return GenerateTest(testName);
        }

        // PL5.4a: Constructor return override semantics
        // - returning an object overrides the constructed instance
        // - returning a primitive does not override
        [Fact]
        public Task Classes_Constructor_ReturnObjectOverridesThis()
        {
            var testName = nameof(Classes_Constructor_ReturnObjectOverridesThis);
            return GenerateTest(testName);
        }

        // PL5.5: Test that methods without explicit return return 'undefined', not 'this'
        // BUG: Currently methods without explicit return incorrectly return 'this' instead of 'undefined'
        [Fact]
        public Task Classes_Method_DefaultReturnUndefined()
        {
            var testName = nameof(Classes_Method_DefaultReturnUndefined);
            return GenerateTest(testName);
        }
    }
}
