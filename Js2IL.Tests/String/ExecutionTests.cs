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
    }
}
