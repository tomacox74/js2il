namespace Js2IL.Tests.Async
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Async") { }

        [Fact]
        public Task Async_HelloWorld() { var testName = nameof(Async_HelloWorld); return GenerateTest(testName); }

        [Fact]
        public Task Async_ReturnValue() { var testName = nameof(Async_ReturnValue); return GenerateTest(testName); }

        [Fact]
        public Task Async_SimpleAwait() { var testName = nameof(Async_SimpleAwait); return GenerateTest(testName); }

        [Fact(Skip = "Pending promise await not implemented yet")]
        public Task Async_PendingPromiseAwait() { var testName = nameof(Async_PendingPromiseAwait); return GenerateTest(testName); }

        [Fact(Skip = "await not fully implemented yet")]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return GenerateTest(testName); }
    }
}