using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.CompoundAssignment
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CompoundAssignment")
        {
        }

        [Fact]
        public Task CompoundAssignment_BitwiseOrAssignment() { var testName = nameof(CompoundAssignment_BitwiseOrAssignment); return GenerateTest(testName); }
    }
}
