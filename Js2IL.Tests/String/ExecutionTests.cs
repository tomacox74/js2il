using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.String
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("String") {}

        [Fact]
        public Task String_Replace_Regex_Global()
        {
            // Uses embedded resource JavaScript/String_Replace_Regex_Global.js
            return ExecutionTest(nameof(String_Replace_Regex_Global));
        }

        [Fact]
        public Task String_TemplateLiteral_Basic()
        {
            return ExecutionTest(nameof(String_TemplateLiteral_Basic));
        }

        [Fact]
        public Task String_LocaleCompare_Numeric()
        {
            return ExecutionTest(nameof(String_LocaleCompare_Numeric));
        }

        [Fact]
        public Task String_StartsWith_Basic()
        {
            return ExecutionTest(nameof(String_StartsWith_Basic));
        }

        [Fact]
        public Task String_PlusEquals_Append()
        {
            return ExecutionTest(nameof(String_PlusEquals_Append));
        }
    }
}
