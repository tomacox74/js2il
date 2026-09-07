namespace Jroc.Test262.Tests.built_ins.Number.prototype.toExponential;

public partial class ExecutionTests
{
    [Fact(DisplayName = "return-abrupt-tointeger-fractiondigits-symbol.js")]
    public Task return_abrupt_tointeger_fractiondigits_symbol()
        => ExecutionTestFromFile("return-abrupt-tointeger-fractiondigits-symbol");

    [Fact(DisplayName = "return-abrupt-tointeger-fractiondigits.js")]
    public Task return_abrupt_tointeger_fractiondigits()
        => ExecutionTestFromFile("return-abrupt-tointeger-fractiondigits");

    [Fact(DisplayName = "this-is-0-fractiondigits-is-0.js")]
    public Task this_is_0_fractiondigits_is_0()
        => ExecutionTestFromFile("this-is-0-fractiondigits-is-0");

    [Fact(DisplayName = "this-is-0-fractiondigits-is-not-0.js")]
    public Task this_is_0_fractiondigits_is_not_0()
        => ExecutionTestFromFile("this-is-0-fractiondigits-is-not-0");

    [Fact(DisplayName = "undefined-fractiondigits.js")]
    public Task undefined_fractiondigits()
        => ExecutionTestFromFile("undefined-fractiondigits");
}
