using System.Threading.Tasks;

namespace Js2IL.Tests.Generator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Generator")
        {
        }

        [Fact]
        public Task Generator_BasicNext() { var testName = nameof(Generator_BasicNext); return ExecutionTest(testName); }
    }
}
