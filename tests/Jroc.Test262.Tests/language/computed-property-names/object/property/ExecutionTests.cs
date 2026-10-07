using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@object.property;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.object.property")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/object/property/number-duplicates.js")]
    public Task test_number_duplicates() => ExecutionTest("number-duplicates");
}
