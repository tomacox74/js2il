using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.zip;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator.zip") { }

    [Fact(DisplayName = "basic-longest.js")]
    public Task Ported_basic_longest() => ExecutionTestFromFile("basic-longest");

    [Fact(DisplayName = "basic-shortest.js")]
    public Task Ported_basic_shortest() => ExecutionTestFromFile("basic-shortest");

    [Fact(DisplayName = "basic-strict.js")]
    public Task Ported_basic_strict() => ExecutionTestFromFile("basic-strict");

    [Fact(DisplayName = "is-function.js")]
    public Task Ported_is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iterables-containing-string-objects.js")]
    public Task Ported_iterables_containing_string_objects() => ExecutionTestFromFile("iterables-containing-string-objects");

    [Fact(DisplayName = "iterables-iteration-after-reading-options.js")]
    public Task Ported_iterables_iteration_after_reading_options() => ExecutionTestFromFile("iterables-iteration-after-reading-options");

    [Fact(DisplayName = "iterables-iteration-get-iterator-flattenable-abrupt-completion.js")]
    public Task Ported_iterables_iteration_get_iterator_flattenable_abrupt_completion() => ExecutionTestFromFile("iterables-iteration-get-iterator-flattenable-abrupt-completion");

    [Fact(DisplayName = "iterables-iteration-iterator-step-value-abrupt-completion.js")]
    public Task Ported_iterables_iteration_iterator_step_value_abrupt_completion() => ExecutionTestFromFile("iterables-iteration-iterator-step-value-abrupt-completion");

    [Fact(DisplayName = "iterables-iteration.js")]
    public Task Ported_iterables_iteration() => ExecutionTestFromFile("iterables-iteration");

    [Fact(DisplayName = "iterator-zip-iteration-iterator-close-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_iterator_close_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-iterator-close-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-iterator-step-value-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_iterator_step_value_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-iterator-step-value-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-longest-iterator-close-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_longest_iterator_close_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-longest-iterator-close-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-shortest-iterator-close-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_shortest_iterator_close_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-shortest-iterator-close-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-strict-iterator-close-i-is-not-zero-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_strict_iterator_close_i_is_not_zero_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-strict-iterator-close-i-is-not-zero-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-strict-iterator-close-i-is-zero-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_strict_iterator_close_i_is_zero_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-strict-iterator-close-i-is-zero-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration-strict-iterator-step-abrupt-completion.js")]
    public Task Ported_iterator_zip_iteration_strict_iterator_step_abrupt_completion() => ExecutionTestFromFile("iterator-zip-iteration-strict-iterator-step-abrupt-completion");

    [Fact(DisplayName = "iterator-zip-iteration.js")]
    public Task Ported_iterator_zip_iteration() => ExecutionTestFromFile("iterator-zip-iteration");

    [Fact(DisplayName = "length.js")]
    public Task Ported_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "options-mode.js")]
    public Task Ported_options_mode() => ExecutionTestFromFile("options-mode");

    [Fact(DisplayName = "options-padding.js")]
    public Task Ported_options_padding() => ExecutionTestFromFile("options-padding");

    [Fact(DisplayName = "options.js")]
    public Task Ported_options() => ExecutionTestFromFile("options");

    [Fact(DisplayName = "padding-iteration-get-iterator-abrupt-completion.js")]
    public Task Ported_padding_iteration_get_iterator_abrupt_completion() => ExecutionTestFromFile("padding-iteration-get-iterator-abrupt-completion");

    [Fact(DisplayName = "padding-iteration-iterator-close-abrupt-completion.js")]
    public Task Ported_padding_iteration_iterator_close_abrupt_completion() => ExecutionTestFromFile("padding-iteration-iterator-close-abrupt-completion");

    [Fact(DisplayName = "padding-iteration-iterator-step-value-abrupt-completion.js")]
    public Task Ported_padding_iteration_iterator_step_value_abrupt_completion() => ExecutionTestFromFile("padding-iteration-iterator-step-value-abrupt-completion");

    [Fact(DisplayName = "padding-iteration.js")]
    public Task Ported_padding_iteration() => ExecutionTestFromFile("padding-iteration");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Ported_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task Ported_proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "result-is-iterator.js")]
    public Task Ported_result_is_iterator() => ExecutionTestFromFile("result-is-iterator");

    [Fact(DisplayName = "suspended-start-iterator-close-calls-next.js")]
    public Task Ported_suspended_start_iterator_close_calls_next() => ExecutionTestFromFile("suspended-start-iterator-close-calls-next");

    [Fact(DisplayName = "suspended-start-iterator-close-calls-return.js")]
    public Task Ported_suspended_start_iterator_close_calls_return() => ExecutionTestFromFile("suspended-start-iterator-close-calls-return");

    [Fact(DisplayName = "suspended-yield-iterator-close-calls-next.js")]
    public Task Ported_suspended_yield_iterator_close_calls_next() => ExecutionTestFromFile("suspended-yield-iterator-close-calls-next");

    [Fact(DisplayName = "suspended-yield-iterator-close-calls-return.js")]
    public Task Ported_suspended_yield_iterator_close_calls_return() => ExecutionTestFromFile("suspended-yield-iterator-close-calls-return");

}
