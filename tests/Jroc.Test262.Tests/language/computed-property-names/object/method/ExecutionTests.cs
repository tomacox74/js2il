using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@object.method;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.object.method")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/object/method/generator.js")]
    public Task test_generator() => ExecutionTest("generator");

    [Fact(DisplayName = "language/computed-property-names/object/method/number.js")]
    public Task test_number() => ExecutionTest("number");

    [Fact(DisplayName = "language/computed-property-names/object/method/string.js")]
    public Task test_string() => ExecutionTest("string");

    [Fact(DisplayName = "language/computed-property-names/object/method/super.js")]
    public Task test_super() => ExecutionTest("super");

    [Fact(DisplayName = "language/computed-property-names/object/method/symbol.js")]
    public Task test_symbol() => ExecutionTest("symbol");
}
