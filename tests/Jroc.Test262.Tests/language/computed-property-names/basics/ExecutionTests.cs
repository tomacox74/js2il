using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.basics;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.basics")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/basics/number.js")]
    public Task test_number() => ExecutionTest("number");

    [Fact(DisplayName = "language/computed-property-names/basics/string.js")]
    public Task test_string() => ExecutionTest("string");

    [Fact(DisplayName = "language/computed-property-names/basics/symbol.js")]
    public Task test_symbol() => ExecutionTest("symbol");
}
