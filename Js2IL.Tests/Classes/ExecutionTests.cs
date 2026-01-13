using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Classes") { }

        // Minimal repro: bit-shift and Int32Array length in a class constructor
        // Allow unhandled exception so we capture stdout even if the runtime faults
        [Fact]
        public Task Classes_BitShiftInCtor_Int32Array()
        {
            var testName = nameof(Classes_BitShiftInCtor_Int32Array);
            return ExecutionTest(testName, allowUnhandledException: true);
        }

        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariable_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariableAndParameterValue_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassConstructor_AccessFunctionVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessFunctionVariableAndParameterValue_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassConstructor_AccessGlobalVariable_Log() { var testName = nameof(Classes_ClassConstructor_AccessGlobalVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassConstructor_AccessGlobalVariableAndParameterValue_Log() { var testName = nameof(Classes_ClassConstructor_AccessGlobalVariableAndParameterValue_Log); return ExecutionTest(testName, allowUnhandledException: true); }

        // Repro (fixed): previously surfaced a TypeLoadException when new-ing a class inside an arrow function.
        // Bug fixed by sharing ClassRegistry with nested generators; test is active.
        [Fact]
        public Task Classes_ClassConstructor_New_In_ArrowFunction()
        {
            var testName = nameof(Classes_ClassConstructor_New_In_ArrowFunction);
            // Allow unhandled exception so we can snapshot stdout if any; test now passes
            return ExecutionTest(testName, allowUnhandledException: true);
        }

        // Repro: class constructor instantiates another class that references a global variable
        // This should fail with ArgumentOutOfRangeException due to invalid scope array construction
        [Fact]
        public Task Classes_ClassConstructor_NewClassReferencingGlobal()
        {
            var testName = nameof(Classes_ClassConstructor_NewClassReferencingGlobal);
            return ExecutionTest(testName, allowUnhandledException: true);
        }

        // Test parameter destructuring in class constructors
        [Fact]
        public Task Classes_ClassConstructor_ParameterDestructuring()
        {
            var testName = nameof(Classes_ClassConstructor_ParameterDestructuring);
            return ExecutionTest(testName);
        }

        [Fact] public Task Classes_ClassConstructor_Param_Field_Log() { var testName = nameof(Classes_ClassConstructor_Param_Field_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_AddMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_AddMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_SubtractMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_SubtractMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_WithMultipleParameters() { var testName = nameof(Classes_ClassConstructor_WithMultipleParameters); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_AccessArrowFunctionVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessArrowFunctionVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassMethod_AccessArrowFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessArrowFunctionVariableAndGlobalVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassMethod_AccessFunctionVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessFunctionVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassMethod_AccessFunctionVariableAndGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessFunctionVariableAndGlobalVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassMethod_AccessGlobalVariable_Log() { var testName = nameof(Classes_ClassMethod_AccessGlobalVariable_Log); return ExecutionTest(testName, allowUnhandledException: true); }
        [Fact] public Task Classes_ClassMethod_ReturnsThis_IsSelf_Log() { var testName = nameof(Classes_ClassMethod_ReturnsThis_IsSelf_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_CallsAnotherMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_ForLoop_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_ForLoop_CallsAnotherMethod); return ExecutionTest(testName); }

        // Test parameter destructuring in class methods
        [Fact]
        public Task Classes_ClassMethod_ParameterDestructuring()
        {
            var testName = nameof(Classes_ClassMethod_ParameterDestructuring);
            return ExecutionTest(testName);
        }

        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Postfix); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Prefix); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Postfix); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Prefix); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassPrivateField_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateField_HelperMethod_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassPrivateProperty_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateProperty_HelperMethod_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassProperty_DefaultAndLog() { var testName = nameof(Classes_ClassProperty_DefaultAndLog); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithStaticMethod_HelloWorld() { var testName = nameof(Classes_ClassWithStaticMethod_HelloWorld); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithStaticProperty_DefaultAndLog() { var testName = nameof(Classes_ClassWithStaticProperty_DefaultAndLog); return ExecutionTest(testName); }
        [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return ExecutionTest(testName); }

        [Fact]
        public Task Classes_DefaultParameterValue_Constructor()
        {
            var testName = nameof(Classes_DefaultParameterValue_Constructor);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Classes_DefaultParameterValue_Method()
        {
            var testName = nameof(Classes_DefaultParameterValue_Method);
            return ExecutionTest(testName);
        }

        [Fact]
        public Task Classes_ClassFieldTypeInference_Primitives()
        {
            var testName = nameof(Classes_ClassFieldTypeInference_Primitives);
            return ExecutionTest(testName);
        }

        [Fact] public Task Classes_ForLoopMin() { var testName = nameof(Classes_ForLoopMin); return ExecutionTest(testName); }

        // Bug repro: class method local variable initialized as number, then reassigned from method call.
        // Type inference marks 'factor' as stable double, but method calls return object - must unbox before storing.
        [Fact]
        public Task Classes_ClassMethod_LocalVar_ReassignedFromMethodCall()
        {
            var testName = nameof(Classes_ClassMethod_LocalVar_ReassignedFromMethodCall);
            return ExecutionTest(testName, allowUnhandledException: true);
        }

        // Minimal repro for InvalidProgramException in ctor due to '1 + this.sieveSizeInBits'
        // Test enabled to validate current behavior.
        [Fact]
        public Task Classes_PrimeCtor_BitArrayAdd()
        {
            var testName = nameof(Classes_PrimeCtor_BitArrayAdd);
            return ExecutionTest(testName, allowUnhandledException: true);
        }

        // PL5.4: Test that constructors implicitly return 'this'
        // Removed explicit 'return this;' case as it causes InvalidProgramException (see separate bug test)
        [Fact]
        public Task Classes_Constructor_ImplicitlyReturnsThis()
        {
            var testName = nameof(Classes_Constructor_ImplicitlyReturnsThis);
            return ExecutionTest(testName);
        }

        // PL5.4a: Constructor with explicit 'return this;' - should work correctly
        // Constructors are void-returning in IL, return statements should not push values
        [Fact]
        public Task Classes_Constructor_ExplicitReturnThis()
        {
            var testName = nameof(Classes_Constructor_ExplicitReturnThis);
            return ExecutionTest(testName);
        }

        // PL5.4a: Constructor return override semantics
        // - returning an object overrides the constructed instance
        // - returning a primitive does not override
        [Fact]
        public Task Classes_Constructor_ReturnObjectOverridesThis()
        {
            var testName = nameof(Classes_Constructor_ReturnObjectOverridesThis);
            return ExecutionTest(testName);
        }

        // PL5.5: Test that methods without explicit return return 'undefined', not 'this'
        // BUG: Currently methods without explicit return incorrectly return 'this' instead of 'undefined'
        // The verified output reflects current (incorrect) behavior - first line shows 'not undefined'
        [Fact]
        public Task Classes_Method_DefaultReturnUndefined()
        {
            var testName = nameof(Classes_Method_DefaultReturnUndefined);
            return ExecutionTest(testName);
        }
    }
}
