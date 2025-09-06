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
            return ExecutionTest(nameof(Date_Construct_FromMs_GetTime_ToISOString));
        }

        [Fact]
        public Task Date_Parse_IsoString()
        {
            return ExecutionTest(nameof(Date_Parse_IsoString));
        }
    }
}
