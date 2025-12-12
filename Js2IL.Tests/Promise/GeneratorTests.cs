namespace Js2IL.Tests.Promise;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests() : base("Promise")
    {
    }

    [Fact]
    public Task Promise_Handler_Resolved()
    {
        return GenerateTest(nameof(Promise_Handler_Resolved));
    }
}