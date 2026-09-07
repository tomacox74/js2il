namespace Jroc.Test262.Tests.built_ins.Number.prototype.toPrecision;

public partial class ExecutionTests
{
    [Fact(DisplayName = "nan.js")]
    public Task nan()
        => ExecutionTestFromFile("nan");

    [Fact(DisplayName = "precision-cannot-be-coerced-to-a-number-in-range.js")]
    public Task precision_cannot_be_coerced_to_a_number_in_range()
        => ExecutionTestFromFile("precision-cannot-be-coerced-to-a-number-in-range");

    [Fact(DisplayName = "this-is-0-precision-is-1.js")]
    public Task this_is_0_precision_is_1()
        => ExecutionTestFromFile("this-is-0-precision-is-1");

    [Fact(DisplayName = "this-is-0-precision-is-gter-than-1.js")]
    public Task this_is_0_precision_is_gter_than_1()
        => ExecutionTestFromFile("this-is-0-precision-is-gter-than-1");
}
