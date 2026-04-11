using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Date
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Date") { }

        [Fact]
        public Task Date_Construct_FromMs_GetTime_ToISOString()
        {
            // Use the test name that matches the verified output
            return ExecutionTest(nameof(Date_Construct_FromMs_GetTime_ToISOString));
        }

        [Fact]
        public Task Date_Parse_IsoString()
        {
            // Use the test name that matches the verified output
            return ExecutionTest(nameof(Date_Parse_IsoString));
        }
    }
}
