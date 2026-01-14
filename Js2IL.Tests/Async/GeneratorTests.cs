namespace Js2IL.Tests.Async
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Async") { }

        [Fact]
        public Task Async_HelloWorld() { var testName = nameof(Async_HelloWorld); return GenerateTest(testName); }

        [Fact]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return GenerateTest(testName); }
    }
}