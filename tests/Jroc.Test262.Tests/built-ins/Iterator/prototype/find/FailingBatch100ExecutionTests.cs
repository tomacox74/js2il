using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.find;

public class FailingBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Iterator.prototype.find") { }

    [Fact(DisplayName = "argument-effect-order")]
    public Task argument_effect_order() => ExecutionTestFromFile("argument-effect-order");

    [Fact(DisplayName = "argument-validation-failure-closes-underlying")]
    public Task argument_validation_failure_closes_underlying() => ExecutionTestFromFile("argument-validation-failure-closes-underlying");

    [Fact(DisplayName = "get-next-method-only-once")]
    public Task get_next_method_only_once() => ExecutionTestFromFile("get-next-method-only-once");

    [Fact(DisplayName = "get-next-method-throws")]
    public Task get_next_method_throws() => ExecutionTestFromFile("get-next-method-throws");

    [Fact(DisplayName = "get-return-method-throws")]
    public Task get_return_method_throws() => ExecutionTestFromFile("get-return-method-throws");

    [Fact(DisplayName = "iterator-has-no-return")]
    public Task iterator_has_no_return() => ExecutionTestFromFile("iterator-has-no-return");

    [Fact(DisplayName = "iterator-return-method-throws")]
    public Task iterator_return_method_throws() => ExecutionTestFromFile("iterator-return-method-throws");

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

    [Fact(DisplayName = "predicate-throws")]
    public Task predicate_throws() => ExecutionTestFromFile("predicate-throws");

    [Fact(DisplayName = "predicate-throws-then-closing-iterator-also-throws")]
    public Task predicate_throws_then_closing_iterator_also_throws() => ExecutionTestFromFile("predicate-throws-then-closing-iterator-also-throws");

    [Fact(DisplayName = "this-non-object")]
    public Task this_non_object() => ExecutionTestFromFile("this-non-object");

}
