using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.String
{
    public class GeneratorTests : GeneratorTestsBase
    {
    public GeneratorTests() : base("String") {}
        [Fact]
        public Task String_Replace_Regex_Global()
        {
            var testName = nameof(String_Replace_Regex_Global);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_TemplateLiteral_Basic()
        {
            var testName = nameof(String_TemplateLiteral_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_LocaleCompare_Numeric()
        {
            var testName = nameof(String_LocaleCompare_Numeric);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_StartsWith_Basic()
        {
            var testName = nameof(String_StartsWith_Basic);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_PlusEquals_Append()
        {
            var testName = nameof(String_PlusEquals_Append);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Replace_CallOnExpression()
        {
            var testName = nameof(String_Replace_CallOnExpression);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_StartsWith_NestedParam()
        {
            var testName = nameof(String_StartsWith_NestedParam);
            return GenerateTest(testName);
        }

        [Fact]
        public Task String_Split_Basic()
        {
            var testName = nameof(String_Split_Basic);
            return GenerateTest(testName);
        }
    }
}
