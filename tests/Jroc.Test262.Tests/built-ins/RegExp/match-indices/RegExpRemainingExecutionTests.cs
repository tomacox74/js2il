namespace Jroc.Test262.Tests.built_ins.RegExp.match_indices;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.match_indices") { }

    [Fact(DisplayName = "indices-array-unicode-property-names.js")]
    public Task indices_array_unicode_property_names()
        => ExecutionTest("indices-array-unicode-property-names");
}
