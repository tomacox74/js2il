using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.to_name_side_effects;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.to-name-side-effects")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/to-name-side-effects/class.js")]
    public Task test_class() => ExecutionTest("class");

    [Fact(DisplayName = "language/computed-property-names/to-name-side-effects/numbers-class.js")]
    public Task test_numbers_class() => ExecutionTest("numbers-class");

    [Fact(DisplayName = "language/computed-property-names/to-name-side-effects/numbers-object.js")]
    public Task test_numbers_object() => ExecutionTest("numbers-object");

    [Fact(DisplayName = "language/computed-property-names/to-name-side-effects/object.js")]
    public Task test_object() => ExecutionTest("object");
}
