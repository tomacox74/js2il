using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.indexOf.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.indexOf.BigInt") { }

    [Fact(DisplayName = "length-zero-returns-minus-one")]
    public Task length_zero_returns_minus_one()
        => ExecutionTestFromFile("length-zero-returns-minus-one");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

}
