using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.concat;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator.concat") { }

    [Fact(DisplayName = "arguments-checked-in-order.js")]
    public Task Ported_arguments_checked_in_order() => ExecutionTestFromFile("arguments-checked-in-order");

    [Fact(DisplayName = "fresh-iterator-result.js")]
    public Task Ported_fresh_iterator_result() => ExecutionTestFromFile("fresh-iterator-result");

    [Fact(DisplayName = "get-iterator-method-only-once.js")]
    public Task Ported_get_iterator_method_only_once() => ExecutionTestFromFile("get-iterator-method-only-once");

    [Fact(DisplayName = "get-iterator-method-throws.js")]
    public Task Ported_get_iterator_method_throws() => ExecutionTestFromFile("get-iterator-method-throws");

    [Fact(DisplayName = "get-value-after-done.js")]
    public Task Ported_get_value_after_done() => ExecutionTestFromFile("get-value-after-done");

    [Fact(DisplayName = "inner-iterator-created-in-order.js")]
    public Task Ported_inner_iterator_created_in_order() => ExecutionTestFromFile("inner-iterator-created-in-order");

    [Fact(DisplayName = "is-function.js")]
    public Task Ported_is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iterable-primitive-wrapper-objects.js")]
    public Task Ported_iterable_primitive_wrapper_objects() => ExecutionTestFromFile("iterable-primitive-wrapper-objects");

    [Fact(DisplayName = "length.js")]
    public Task Ported_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "many-arguments.js")]
    public Task Ported_many_arguments() => ExecutionTestFromFile("many-arguments");

    [Fact(DisplayName = "name.js")]
    public Task Ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "next-method-called-with-zero-arguments.js")]
    public Task Ported_next_method_called_with_zero_arguments() => ExecutionTestFromFile("next-method-called-with-zero-arguments");

    [Fact(DisplayName = "next-method-returns-non-object.js")]
    public Task Ported_next_method_returns_non_object() => ExecutionTestFromFile("next-method-returns-non-object");

    [Fact(DisplayName = "next-method-returns-throwing-done.js")]
    public Task Ported_next_method_returns_throwing_done() => ExecutionTestFromFile("next-method-returns-throwing-done");

    [Fact(DisplayName = "next-method-returns-throwing-value-done.js")]
    public Task Ported_next_method_returns_throwing_value_done() => ExecutionTestFromFile("next-method-returns-throwing-value-done");

    [Fact(DisplayName = "next-method-returns-throwing-value.js")]
    public Task Ported_next_method_returns_throwing_value() => ExecutionTestFromFile("next-method-returns-throwing-value");

    [Fact(DisplayName = "next-method-throws.js")]
    public Task Ported_next_method_throws() => ExecutionTestFromFile("next-method-throws");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Ported_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task Ported_proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "result-is-iterator.js")]
    public Task Ported_result_is_iterator() => ExecutionTestFromFile("result-is-iterator");

    [Fact(DisplayName = "return-is-forwarded.js")]
    public Task Ported_return_is_forwarded() => ExecutionTestFromFile("return-is-forwarded");

    [Fact(DisplayName = "return-is-not-forwarded-after-exhaustion.js")]
    public Task Ported_return_is_not_forwarded_after_exhaustion() => ExecutionTestFromFile("return-is-not-forwarded-after-exhaustion");

    [Fact(DisplayName = "return-is-not-forwarded-before-initial-start.js")]
    public Task Ported_return_is_not_forwarded_before_initial_start() => ExecutionTestFromFile("return-is-not-forwarded-before-initial-start");

    [Fact(DisplayName = "return-method-called-with-zero-arguments.js")]
    public Task Ported_return_method_called_with_zero_arguments() => ExecutionTestFromFile("return-method-called-with-zero-arguments");

    [Fact(DisplayName = "single-argument.js")]
    public Task Ported_single_argument() => ExecutionTestFromFile("single-argument");

    [Fact(DisplayName = "throws-typeerror-when-generator-is-running-next.js")]
    public Task Ported_throws_typeerror_when_generator_is_running_next() => ExecutionTestFromFile("throws-typeerror-when-generator-is-running-next");

    [Fact(DisplayName = "throws-typeerror-when-generator-is-running-return.js")]
    public Task Ported_throws_typeerror_when_generator_is_running_return() => ExecutionTestFromFile("throws-typeerror-when-generator-is-running-return");

    [Fact(DisplayName = "throws-typeerror-when-iterator-not-an-object.js")]
    public Task Ported_throws_typeerror_when_iterator_not_an_object() => ExecutionTestFromFile("throws-typeerror-when-iterator-not-an-object");

    [Fact(DisplayName = "zero-arguments.js")]
    public Task Ported_zero_arguments() => ExecutionTestFromFile("zero-arguments");

}
