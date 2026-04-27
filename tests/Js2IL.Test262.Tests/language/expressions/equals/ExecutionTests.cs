using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.equals;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.equals") { }

    [Fact(DisplayName = "bigint-and-boolean", Skip = "Known JS2IL defect")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-incomparable-primitive", Skip = "Known JS2IL defect")]
    public Task bigint_and_incomparable_primitive()
        => ExecutionTest("bigint-and-incomparable-primitive");

    [Fact(DisplayName = "bigint-and-non-finite", Skip = "Known JS2IL defect")]
    public Task bigint_and_non_finite()
        => ExecutionTest("bigint-and-non-finite");
}
