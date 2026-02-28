using System.Threading.Tasks;

namespace Js2IL.Tests.Integration
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Integration") { }

        [Fact]
        public Task Compile_Performance_Dromaeo_Object_Regexp() => ExecutionTest(nameof(Compile_Performance_Dromaeo_Object_Regexp));
    }
}
