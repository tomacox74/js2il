using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@object.accessor;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.object.accessor")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/object/accessor/getter-duplicates.js")]
    public Task test_getter_duplicates() => ExecutionTest("getter-duplicates");

    [Fact(DisplayName = "language/computed-property-names/object/accessor/getter-super.js")]
    public Task test_getter_super() => ExecutionTest("getter-super");

    [Fact(DisplayName = "language/computed-property-names/object/accessor/getter.js")]
    public Task test_getter() => ExecutionTest("getter");

    [Fact(DisplayName = "language/computed-property-names/object/accessor/setter-duplicates.js")]
    public Task test_setter_duplicates() => ExecutionTest("setter-duplicates");

    [Fact(DisplayName = "language/computed-property-names/object/accessor/setter-super.js")]
    public Task test_setter_super() => ExecutionTest("setter-super");

    [Fact(DisplayName = "language/computed-property-names/object/accessor/setter.js")]
    public Task test_setter() => ExecutionTest("setter");
}
