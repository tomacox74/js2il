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

        // NOTE: throw() and return() protocol methods are not yet fully implemented for async generators
        // [Fact]
        // public Task AsyncGenerator_Throw() { var testName = nameof(AsyncGenerator_Throw); return GenerateTest(testName); }

        // [Fact]
        // public Task AsyncGenerator_Return() { var testName = nameof(AsyncGenerator_Return); return GenerateTest(testName); }

        [Fact]
        public Task AsyncGenerator_YieldAwait() { var testName = nameof(AsyncGenerator_YieldAwait); return GenerateTest(testName); }

        // NOTE: try/catch/finally with async generators has known issues
        // [Fact]
        // public Task AsyncGenerator_TryCatchFinally() { var testName = nameof(AsyncGenerator_TryCatchFinally); return GenerateTest(testName); }
    }
}
