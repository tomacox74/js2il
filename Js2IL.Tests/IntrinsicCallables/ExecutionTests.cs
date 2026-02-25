using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.IntrinsicCallables
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("IntrinsicCallables") { }

        [Fact]
        public Task IntrinsicCallables_Date_Callable_ReturnsString()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Date_Callable_ReturnsString));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Callable_CreatesRegex()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_Callable_CreatesRegex));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Prototype_Getters_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_Prototype_Getters_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Test_LastIndex_Global()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_Test_LastIndex_Global));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_ToString_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_ToString_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Flags_Getter()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_Flags_Getter));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Getters_Extended()
        {
            return ExecutionTest(nameof(IntrinsicCallables_RegExp_Getters_Extended));
        }

        [Fact]
        public Task IntrinsicCallables_Error_Callable_CreatesInstances()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Error_Callable_CreatesInstances));
        }

        [Fact]
        public Task IntrinsicCallables_Error_ConstructorSurface()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Error_ConstructorSurface));
        }

        [Fact]
        public Task IntrinsicCallables_Object_Callable_ReturnsObject()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Object_Callable_ReturnsObject));
        }

        [Fact]
        public Task IntrinsicCallables_ParseInt_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_ParseInt_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_ParseInt_Spec()
        {
            return ExecutionTest(nameof(IntrinsicCallables_ParseInt_Spec));
        }

        [Fact]
        public Task IntrinsicCallables_ParseFloat_IsFinite_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_ParseFloat_IsFinite_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_GlobalBuiltins_AsValues_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_GlobalBuiltins_AsValues_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_GlobalFunctions_AsValues_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_GlobalFunctions_AsValues_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_GlobalThis_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_GlobalThis_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_GlobalThis_Enumerability()
        {
            return ExecutionTest(nameof(IntrinsicCallables_GlobalThis_Enumerability));
        }

        [Fact]
        public Task IntrinsicCallables_Symbol_Callable_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Symbol_Callable_Basic));
        }


        [Fact]
        public Task IntrinsicCallables_Symbol_Registry_WellKnown()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Symbol_Registry_WellKnown));
        }

        [Fact]
        public Task IntrinsicCallables_Symbol_Prototype_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Symbol_Prototype_Basic));
        }
        [Fact]
        public Task IntrinsicCallables_BigInt_Callable_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_BigInt_Callable_Basic));
        }
    }
}
