using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.join.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.join.BigInt") { }

    [Fact(DisplayName = "return-abrupt-from-separator-symbol")]
    public Task return_abrupt_from_separator_symbol()
        => ExecutionTestFromFile("return-abrupt-from-separator-symbol");

    [Fact(DisplayName = "return-abrupt-from-separator")]
    public Task return_abrupt_from_separator()
        => ExecutionTestFromFile("return-abrupt-from-separator");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

}
