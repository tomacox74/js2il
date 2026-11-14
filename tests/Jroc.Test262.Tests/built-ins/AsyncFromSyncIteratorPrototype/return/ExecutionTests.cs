using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncFromSyncIteratorPrototype.@return;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncFromSyncIteratorPrototype.return") { }

    [Fact(DisplayName = "absent-value-not-passed.js")]
    public Task Port200_absent_value_not_passed()
        => ExecutionTestFromFile("absent-value-not-passed");

    [Fact(DisplayName = "iterator-result-poisoned-done.js")]
    public Task Port200_iterator_result_poisoned_done()
        => ExecutionTestFromFile("iterator-result-poisoned-done");

    [Fact(DisplayName = "iterator-result-poisoned-value.js")]
    public Task Port200_iterator_result_poisoned_value()
        => ExecutionTestFromFile("iterator-result-poisoned-value");

    [Fact(DisplayName = "iterator-result-unwrap-promise.js")]
    public Task Port200_iterator_result_unwrap_promise()
        => ExecutionTestFromFile("iterator-result-unwrap-promise");

    [Fact(DisplayName = "iterator-result.js")]
    public Task Port200_iterator_result()
        => ExecutionTestFromFile("iterator-result");

    [Fact(DisplayName = "poisoned-get-return.js")]
    public Task Port200_poisoned_get_return()
        => ExecutionTestFromFile("poisoned-get-return");

    [Fact(DisplayName = "poisoned-return.js")]
    public Task Port200_poisoned_return()
        => ExecutionTestFromFile("poisoned-return");

    [Fact(DisplayName = "result-object-error.js")]
    public Task Port200_result_object_error()
        => ExecutionTestFromFile("result-object-error");

    [Fact(DisplayName = "return-null.js")]
    public Task Port200_return_null()
        => ExecutionTestFromFile("return-null");

    [Fact(DisplayName = "return-undefined.js")]
    public Task Port200_return_undefined()
        => ExecutionTestFromFile("return-undefined");
}
