using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.FinalizationRegistry.prototype.register;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.FinalizationRegistry.prototype.register") { }

    [Fact(DisplayName = "heldValue-same-as-target")]
    public Task heldValue_same_as_target()
        => ExecutionTestFromFile("heldValue-same-as-target");

    [Fact(DisplayName = "holdings-any-value-type")]
    public Task holdings_any_value_type()
        => ExecutionTestFromFile("holdings-any-value-type");

    [Fact(DisplayName = "return-undefined-register-itself")]
    public Task return_undefined_register_itself()
        => ExecutionTestFromFile("return-undefined-register-itself");

    [Fact(DisplayName = "return-undefined-register-object")]
    public Task return_undefined_register_object()
        => ExecutionTestFromFile("return-undefined-register-object");

    [Fact(DisplayName = "return-undefined-register-symbol")]
    public Task return_undefined_register_symbol()
        => ExecutionTestFromFile("return-undefined-register-symbol");

    [Fact(DisplayName = "throws-when-target-cannot-be-held-weakly")]
    public Task throws_when_target_cannot_be_held_weakly()
        => ExecutionTestFromFile("throws-when-target-cannot-be-held-weakly");

    [Fact(DisplayName = "throws-when-unregisterToken-not-undefined-and-cannot-be-held-weakly")]
    public Task throws_when_unregisterToken_not_undefined_and_cannot_be_held_weakly()
        => ExecutionTestFromFile("throws-when-unregisterToken-not-undefined-and-cannot-be-held-weakly");

    [Fact(DisplayName = "unregisterToken-same-as-holdings-and-target")]
    public Task unregisterToken_same_as_holdings_and_target()
        => ExecutionTestFromFile("unregisterToken-same-as-holdings-and-target");

    [Fact(DisplayName = "unregisterToken-same-as-holdings")]
    public Task unregisterToken_same_as_holdings()
        => ExecutionTestFromFile("unregisterToken-same-as-holdings");

    [Fact(DisplayName = "unregisterToken-same-as-target")]
    public Task unregisterToken_same_as_target()
        => ExecutionTestFromFile("unregisterToken-same-as-target");

}
