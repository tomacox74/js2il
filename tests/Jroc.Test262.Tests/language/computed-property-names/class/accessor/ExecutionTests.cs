using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@class.accessor;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.class.accessor")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/class/accessor/getter-duplicates.js")]
    public Task test_getter_duplicates() => ExecutionTest("getter-duplicates");

    [Fact(DisplayName = "language/computed-property-names/class/accessor/getter.js")]
    public Task test_getter() => ExecutionTest("getter");

    [Fact(DisplayName = "language/computed-property-names/class/accessor/setter-duplicates.js")]
    public Task test_setter_duplicates() => ExecutionTest("setter-duplicates");

    [Fact(DisplayName = "language/computed-property-names/class/accessor/setter.js")]
    public Task test_setter() => ExecutionTest("setter");
}
