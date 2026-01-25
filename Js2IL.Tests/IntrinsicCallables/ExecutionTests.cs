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
        public Task IntrinsicCallables_Error_Callable_CreatesInstances()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Error_Callable_CreatesInstances));
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
        public Task IntrinsicCallables_Symbol_Callable_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_Symbol_Callable_Basic));
        }

        [Fact]
        public Task IntrinsicCallables_BigInt_Callable_Basic()
        {
            return ExecutionTest(nameof(IntrinsicCallables_BigInt_Callable_Basic));
        }
    }
}
