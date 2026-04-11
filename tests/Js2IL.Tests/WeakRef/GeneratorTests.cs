using System.Threading.Tasks;

namespace Js2IL.Tests.WeakRef
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("WeakRef") { }

        [Fact]
        public Task WeakRef_Deref_KeptObjects() { var testName = nameof(WeakRef_Deref_KeptObjects); return GenerateTest(testName); }
    }
}
