using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncFromSyncIteratorPrototype.@throw;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncFromSyncIteratorPrototype.throw") { }

    [Fact(DisplayName = "iterator-result-poisoned-done.js")]
    public Task Port200_iterator_result_poisoned_done()
        => ExecutionTestFromFile("iterator-result-poisoned-done");

    [Fact(DisplayName = "iterator-result-poisoned-value.js")]
    public Task Port200_iterator_result_poisoned_value()
        => ExecutionTestFromFile("iterator-result-poisoned-value");

    [Fact(DisplayName = "iterator-result-rejected-promise-close.js")]
    public Task Port200_iterator_result_rejected_promise_close()
        => ExecutionTestFromFile("iterator-result-rejected-promise-close");

    [Fact(DisplayName = "iterator-result-unwrap-promise.js")]
    public Task Port200_iterator_result_unwrap_promise()
        => ExecutionTestFromFile("iterator-result-unwrap-promise");

    [Fact(DisplayName = "iterator-result.js")]
    public Task Port200_iterator_result()
        => ExecutionTestFromFile("iterator-result");

    [Fact(DisplayName = "poisoned-get-throw.js")]
    public Task Port200_poisoned_get_throw()
        => ExecutionTestFromFile("poisoned-get-throw");

    [Fact(DisplayName = "poisoned-throw.js")]
    public Task Port200_poisoned_throw()
        => ExecutionTestFromFile("poisoned-throw");

    [Fact(DisplayName = "result-object-error.js")]
    public Task Port200_result_object_error()
        => ExecutionTestFromFile("result-object-error");

    [Fact(DisplayName = "throw-null.js")]
    public Task Port200_throw_null()
        => ExecutionTestFromFile("throw-null");

    [Fact(DisplayName = "throw-result-poisoned-wrapper.js")]
    public Task Port200_throw_result_poisoned_wrapper()
        => ExecutionTestFromFile("throw-result-poisoned-wrapper");

    [Fact(DisplayName = "throw-undefined-get-return-undefined.js")]
    public Task Port200_throw_undefined_get_return_undefined()
        => ExecutionTestFromFile("throw-undefined-get-return-undefined");

    [Fact(DisplayName = "throw-undefined-poisoned-return.js")]
    public Task Port200_throw_undefined_poisoned_return()
        => ExecutionTestFromFile("throw-undefined-poisoned-return");

    [Fact(DisplayName = "throw-undefined-return-not-object.js")]
    public Task Port200_throw_undefined_return_not_object()
        => ExecutionTestFromFile("throw-undefined-return-not-object");

    [Fact(DisplayName = "throw-undefined-return-object.js")]
    public Task Port200_throw_undefined_return_object()
        => ExecutionTestFromFile("throw-undefined-return-object");

    [Fact(DisplayName = "throw-undefined.js")]
    public Task Port200_throw_undefined()
        => ExecutionTestFromFile("throw-undefined");
}
