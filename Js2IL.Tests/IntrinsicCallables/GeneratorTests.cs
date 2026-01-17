using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.IntrinsicCallables
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("IntrinsicCallables") { }

        [Fact]
        public Task IntrinsicCallables_Date_Callable_ReturnsString()
        {
            return GenerateTest(nameof(IntrinsicCallables_Date_Callable_ReturnsString));
        }

        [Fact]
        public Task IntrinsicCallables_RegExp_Callable_CreatesRegex()
        {
            return GenerateTest(nameof(IntrinsicCallables_RegExp_Callable_CreatesRegex));
        }

        [Fact]
        public Task IntrinsicCallables_Error_Callable_CreatesInstances()
        {
            return GenerateTest(nameof(IntrinsicCallables_Error_Callable_CreatesInstances));
        }

        [Fact]
        public Task IntrinsicCallables_Object_Callable_ReturnsObject()
        {
            return GenerateTest(nameof(IntrinsicCallables_Object_Callable_ReturnsObject));
        }

        [Fact]
        public Task IntrinsicCallables_ParseInt_Basic()
        {
            return GenerateTest(nameof(IntrinsicCallables_ParseInt_Basic));
        }
    }
}
