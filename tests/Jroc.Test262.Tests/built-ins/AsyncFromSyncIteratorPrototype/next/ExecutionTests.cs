using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncFromSyncIteratorPrototype.next;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncFromSyncIteratorPrototype.next") { }

    [Fact(DisplayName = "absent-value-not-passed.js")]
    public Task Port200_absent_value_not_passed()
        => ExecutionTestFromFile("absent-value-not-passed");

    [Fact(DisplayName = "for-await-iterator-next-rejected-promise-close.js")]
    public Task Port200_for_await_iterator_next_rejected_promise_close()
        => ExecutionTestFromFile("for-await-iterator-next-rejected-promise-close");

    [Fact(DisplayName = "for-await-next-rejected-promise-close.js")]
    public Task Port200_for_await_next_rejected_promise_close()
        => ExecutionTestFromFile("for-await-next-rejected-promise-close");

    [Fact(DisplayName = "iterator-result-poisoned-done.js")]
    public Task Port200_iterator_result_poisoned_done()
        => ExecutionTestFromFile("iterator-result-poisoned-done");

    [Fact(DisplayName = "iterator-result-poisoned-value.js")]
    public Task Port200_iterator_result_poisoned_value()
        => ExecutionTestFromFile("iterator-result-poisoned-value");

    [Fact(DisplayName = "iterator-result-poisoned-wrapper.js")]
    public Task Port200_iterator_result_poisoned_wrapper()
        => ExecutionTestFromFile("iterator-result-poisoned-wrapper");

    [Fact(DisplayName = "iterator-result-prototype.js")]
    public Task Port200_iterator_result_prototype()
        => ExecutionTestFromFile("iterator-result-prototype");

    [Fact(DisplayName = "iterator-result-rejected.js")]
    public Task Port200_iterator_result_rejected()
        => ExecutionTestFromFile("iterator-result-rejected");

    [Fact(DisplayName = "iterator-result-unwrap-promise.js")]
    public Task Port200_iterator_result_unwrap_promise()
        => ExecutionTestFromFile("iterator-result-unwrap-promise");

    [Fact(DisplayName = "next-result-poisoned-wrapper.js")]
    public Task Port200_next_result_poisoned_wrapper()
        => ExecutionTestFromFile("next-result-poisoned-wrapper");

    [Fact(DisplayName = "yield-iterator-next-rejected-promise-close.js")]
    public Task Port200_yield_iterator_next_rejected_promise_close()
        => ExecutionTestFromFile("yield-iterator-next-rejected-promise-close");

    [Fact(DisplayName = "yield-next-rejected-promise-close.js")]
    public Task Port200_yield_next_rejected_promise_close()
        => ExecutionTestFromFile("yield-next-rejected-promise-close");
}
