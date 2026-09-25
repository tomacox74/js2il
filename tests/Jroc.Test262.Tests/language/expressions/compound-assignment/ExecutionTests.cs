using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.compound_assignment;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.compound-assignment") { }

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-add.js")]
    public Task ported_data_add() => ExecutionTest("left-hand-side-private-reference-data-property-add");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-sub.js")]
    public Task ported_data_sub() => ExecutionTest("left-hand-side-private-reference-data-property-sub");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-mult.js")]
    public Task ported_data_mult() => ExecutionTest("left-hand-side-private-reference-data-property-mult");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-div.js")]
    public Task ported_data_div() => ExecutionTest("left-hand-side-private-reference-data-property-div");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-mod.js")]
    public Task ported_data_mod() => ExecutionTest("left-hand-side-private-reference-data-property-mod");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-exp.js")]
    public Task ported_data_exp() => ExecutionTest("left-hand-side-private-reference-data-property-exp");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-bitand.js")]
    public Task ported_data_bitand() => ExecutionTest("left-hand-side-private-reference-data-property-bitand");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-bitor.js")]
    public Task ported_data_bitor() => ExecutionTest("left-hand-side-private-reference-data-property-bitor");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-bitxor.js")]
    public Task ported_data_bitxor() => ExecutionTest("left-hand-side-private-reference-data-property-bitxor");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-lshift.js")]
    public Task ported_data_lshift() => ExecutionTest("left-hand-side-private-reference-data-property-lshift");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-rshift.js")]
    public Task ported_data_rshift() => ExecutionTest("left-hand-side-private-reference-data-property-rshift");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-srshift.js")]
    public Task ported_data_srshift() => ExecutionTest("left-hand-side-private-reference-data-property-srshift");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-add.js")]
    public Task ported_accessor_add() => ExecutionTest("left-hand-side-private-reference-accessor-property-add");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-sub.js")]
    public Task ported_accessor_sub() => ExecutionTest("left-hand-side-private-reference-accessor-property-sub");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-mult.js")]
    public Task ported_accessor_mult() => ExecutionTest("left-hand-side-private-reference-accessor-property-mult");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-div.js")]
    public Task ported_accessor_div() => ExecutionTest("left-hand-side-private-reference-accessor-property-div");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-mod.js")]
    public Task ported_accessor_mod() => ExecutionTest("left-hand-side-private-reference-accessor-property-mod");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-exp.js")]
    public Task ported_accessor_exp() => ExecutionTest("left-hand-side-private-reference-accessor-property-exp");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-bitand.js")]
    public Task ported_accessor_bitand() => ExecutionTest("left-hand-side-private-reference-accessor-property-bitand");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-bitor.js")]
    public Task ported_accessor_bitor() => ExecutionTest("left-hand-side-private-reference-accessor-property-bitor");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-bitxor.js")]
    public Task ported_accessor_bitxor() => ExecutionTest("left-hand-side-private-reference-accessor-property-bitxor");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-lshift.js")]
    public Task ported_accessor_lshift() => ExecutionTest("left-hand-side-private-reference-accessor-property-lshift");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-rshift.js")]
    public Task ported_accessor_rshift() => ExecutionTest("left-hand-side-private-reference-accessor-property-rshift");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-srshift.js")]
    public Task ported_accessor_srshift() => ExecutionTest("left-hand-side-private-reference-accessor-property-srshift");
}
