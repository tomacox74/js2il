using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.drop;

public class FailingBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Iterator.prototype.drop") { }

    [Fact(DisplayName = "argument-effect-order")]
    public Task argument_effect_order() => ExecutionTestFromFile("argument-effect-order");

    [Fact(DisplayName = "argument-validation-failure-closes-underlying")]
    public Task argument_validation_failure_closes_underlying() => ExecutionTestFromFile("argument-validation-failure-closes-underlying");

    [Fact(DisplayName = "exhaustion-does-not-call-return")]
    public Task exhaustion_does_not_call_return() => ExecutionTestFromFile("exhaustion-does-not-call-return");

    [Fact(DisplayName = "get-next-method-only-once")]
    public Task get_next_method_only_once() => ExecutionTestFromFile("get-next-method-only-once");

    [Fact(DisplayName = "get-next-method-throws")]
    public Task get_next_method_throws() => ExecutionTestFromFile("get-next-method-throws");

    [Fact(DisplayName = "get-return-method-throws")]
    public Task get_return_method_throws() => ExecutionTestFromFile("get-return-method-throws");

    [Fact(DisplayName = "limit-rangeerror")]
    public Task limit_rangeerror() => ExecutionTestFromFile("limit-rangeerror");

    [Fact(DisplayName = "next-method-returns-non-object")]
    public Task next_method_returns_non_object() => ExecutionTestFromFile("next-method-returns-non-object");

    [Fact(DisplayName = "next-method-returns-throwing-done")]
    public Task next_method_returns_throwing_done() => ExecutionTestFromFile("next-method-returns-throwing-done");

    [Fact(DisplayName = "next-method-returns-throwing-value")]
    public Task next_method_returns_throwing_value() => ExecutionTestFromFile("next-method-returns-throwing-value");

    [Fact(DisplayName = "next-method-returns-throwing-value-done")]
    public Task next_method_returns_throwing_value_done() => ExecutionTestFromFile("next-method-returns-throwing-value-done");

    [Fact(DisplayName = "next-method-throws")]
    public Task next_method_throws() => ExecutionTestFromFile("next-method-throws");

    [Fact(DisplayName = "return-is-forwarded")]
    public Task return_is_forwarded() => ExecutionTestFromFile("return-is-forwarded");

    [Fact(DisplayName = "return-is-not-forwarded-after-exhaustion")]
    public Task return_is_not_forwarded_after_exhaustion() => ExecutionTestFromFile("return-is-not-forwarded-after-exhaustion");

    [Fact(DisplayName = "this-non-callable-next")]
    public Task this_non_callable_next() => ExecutionTestFromFile("this-non-callable-next");

    [Fact(DisplayName = "this-non-object")]
    public Task this_non_object() => ExecutionTestFromFile("this-non-object");

    [Fact(DisplayName = "throws-typeerror-when-generator-is-running")]
    public Task throws_typeerror_when_generator_is_running() => ExecutionTestFromFile("throws-typeerror-when-generator-is-running");

}
