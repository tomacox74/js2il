using System.Threading.Tasks;

namespace Jroc.Tests.Node.AssertModule
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/AssertModule") { }

        [Fact]
        public Task Require_Assert_Callable_And_Core_Methods()
            => GenerateTest(nameof(Require_Assert_Callable_And_Core_Methods));

        [Fact]
        public Task Require_Assert_AssertionError_Metadata()
            => GenerateTest(nameof(Require_Assert_AssertionError_Metadata));
    }
}
