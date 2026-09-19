using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.values;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.TypedArray.prototype.values") { }

    [Fact(DisplayName = "make-in-bounds-after-exhausted")]
    public Task make_in_bounds_after_exhausted()
        => ExecutionTestFromFile("make-in-bounds-after-exhausted");

    [Fact(DisplayName = "make-out-of-bounds-after-exhausted")]
    public Task make_out_of_bounds_after_exhausted()
        => ExecutionTestFromFile("make-out-of-bounds-after-exhausted");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

}
