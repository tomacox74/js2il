using System.Threading.Tasks;

namespace Jroc.Tests.JSON
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("JSON") { }

        [Fact]
        public Task JSON_Parse_SimpleObject() => GenerateTest(nameof(JSON_Parse_SimpleObject));
    }
}
