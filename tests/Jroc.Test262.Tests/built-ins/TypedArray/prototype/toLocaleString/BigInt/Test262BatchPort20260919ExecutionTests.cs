using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toLocaleString.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.toLocaleString.BigInt") { }

    [Fact(DisplayName = "calls-tolocalestring-from-each-value")]
    public Task calls_tolocalestring_from_each_value()
        => ExecutionTestFromFile("calls-tolocalestring-from-each-value");

    [Fact(DisplayName = "calls-tostring-from-each-value")]
    public Task calls_tostring_from_each_value()
        => ExecutionTestFromFile("calls-tostring-from-each-value");

    [Fact(DisplayName = "calls-valueof-from-each-value")]
    public Task calls_valueof_from_each_value()
        => ExecutionTestFromFile("calls-valueof-from-each-value");

    [Fact(DisplayName = "return-abrupt-from-firstelement-tolocalestring")]
    public Task return_abrupt_from_firstelement_tolocalestring()
        => ExecutionTestFromFile("return-abrupt-from-firstelement-tolocalestring");

    [Fact(DisplayName = "return-abrupt-from-firstelement-tostring")]
    public Task return_abrupt_from_firstelement_tostring()
        => ExecutionTestFromFile("return-abrupt-from-firstelement-tostring");

    [Fact(DisplayName = "return-abrupt-from-firstelement-valueof")]
    public Task return_abrupt_from_firstelement_valueof()
        => ExecutionTestFromFile("return-abrupt-from-firstelement-valueof");

    [Fact(DisplayName = "return-abrupt-from-nextelement-tolocalestring")]
    public Task return_abrupt_from_nextelement_tolocalestring()
        => ExecutionTestFromFile("return-abrupt-from-nextelement-tolocalestring");

    [Fact(DisplayName = "return-abrupt-from-nextelement-tostring")]
    public Task return_abrupt_from_nextelement_tostring()
        => ExecutionTestFromFile("return-abrupt-from-nextelement-tostring");

    [Fact(DisplayName = "return-abrupt-from-nextelement-valueof")]
    public Task return_abrupt_from_nextelement_valueof()
        => ExecutionTestFromFile("return-abrupt-from-nextelement-valueof");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

}
