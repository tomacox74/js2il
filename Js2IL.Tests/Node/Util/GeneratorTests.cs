using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Util
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Util") { }

        [Fact]
        public Task Require_Util_Promisify_Basic()
            => GenerateTest(nameof(Require_Util_Promisify_Basic));

        [Fact]
        public Task Require_Util_Promisify_ErrorHandling()
            => GenerateTest(nameof(Require_Util_Promisify_ErrorHandling));

        [Fact]
        public Task Require_Util_Inherits_Basic()
            => GenerateTest(nameof(Require_Util_Inherits_Basic));

        [Fact]
        public Task Require_Util_Types_IsPromise()
            => GenerateTest(nameof(Require_Util_Types_IsPromise));

        [Fact]
        public Task Require_Util_Types_IsArray()
            => GenerateTest(nameof(Require_Util_Types_IsArray));

        [Fact]
        public Task Require_Util_Types_IsFunction()
            => GenerateTest(nameof(Require_Util_Types_IsFunction));

        [Fact]
        public Task Require_Util_Inspect_Basic()
            => GenerateTest(nameof(Require_Util_Inspect_Basic));

        [Fact]
        public Task Require_Util_Inspect_Object()
            => GenerateTest(nameof(Require_Util_Inspect_Object));
    }
}
