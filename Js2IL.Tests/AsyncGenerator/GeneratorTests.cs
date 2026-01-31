namespace Js2IL.Tests.AsyncGenerator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("AsyncGenerator")
        {
        }

        [Fact]
        public Task AsyncGenerator_BasicNext() { var testName = nameof(AsyncGenerator_BasicNext); return GenerateTest(testName); }

        [Fact]
        public Task AsyncGenerator_ForAwaitOf() { var testName = nameof(AsyncGenerator_ForAwaitOf); return GenerateTest(testName); }
    }
}
