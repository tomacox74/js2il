using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Classes") { }

        // Classes tests
        [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithStaticMethod_HelloWorld() { var testName = nameof(Classes_ClassWithStaticMethod_HelloWorld); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassProperty_DefaultAndLog() { var testName = nameof(Classes_ClassProperty_DefaultAndLog); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_Param_Field_Log() { var testName = nameof(Classes_ClassConstructor_Param_Field_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_WithMultipleParameters() { var testName = nameof(Classes_ClassConstructor_WithMultipleParameters); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_AddMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_AddMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_SubtractMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_SubtractMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassWithStaticProperty_DefaultAndLog() { var testName = nameof(Classes_ClassWithStaticProperty_DefaultAndLog); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassPrivateField_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateField_HelperMethod_Log); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassPrivateProperty_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateProperty_HelperMethod_Log); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_CallsAnotherMethod); return ExecutionTest(testName); }
        [Fact] public Task Classes_ClassMethod_ForLoop_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_ForLoop_CallsAnotherMethod); return ExecutionTest(testName); }

    // Repro (fixed): previously surfaced a TypeLoadException when new-ing a class inside an arrow function.
    // Bug fixed by sharing ClassRegistry with nested generators; test is active.
        [Fact]
        public Task Classes_ClassConstructor_New_In_ArrowFunction()
        {
            var testName = nameof(Classes_ClassConstructor_New_In_ArrowFunction);
            // Allow unhandled exception so we can snapshot stdout if any; test now passes
            return ExecutionTest(testName, allowUnhandledException: true);
        }

    // While increment variants to mirror generator tests
    [Fact] public Task Classes_ClassMethod_While_Increment_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Postfix); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassMethod_While_Increment_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Prefix); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassMethod_While_Increment_Param_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Postfix); return ExecutionTest(testName); }
    [Fact] public Task Classes_ClassMethod_While_Increment_Param_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Prefix); return ExecutionTest(testName); }

    // Minimal repro: bit-shift and Int32Array length in a class constructor
    // Allow unhandled exception so we capture stdout even if the runtime faults
    [Fact]
    public Task Classes_BitShiftInCtor_Int32Array()
    {
        var testName = nameof(Classes_BitShiftInCtor_Int32Array);
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
    }
}
