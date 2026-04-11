using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.PrimitiveConversion
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("PrimitiveConversion") { }

        [Fact]
        public Task PrimitiveConversion_String_Callable()
        {
            var testName = nameof(PrimitiveConversion_String_Callable);
            return GenerateTest(testName);
        }

        [Fact]
        public Task PrimitiveConversion_Number_Callable()
        {
            var testName = nameof(PrimitiveConversion_Number_Callable);
            return GenerateTest(testName);
        }

        [Fact]
        public Task PrimitiveConversion_Boolean_Callable()
        {
            var testName = nameof(PrimitiveConversion_Boolean_Callable);
            return GenerateTest(testName);
        }
    }
}
