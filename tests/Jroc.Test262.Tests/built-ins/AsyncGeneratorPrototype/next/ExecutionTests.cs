using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype.next;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorPrototype.next") { }

    [Fact(DisplayName = "iterator-result-prototype.js")]
    public Task Port200_iterator_result_prototype()
        => ExecutionTestFromFile("iterator-result-prototype");

    [Fact(DisplayName = "length.js")]
    public Task Port200_length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port200_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "request-queue-await-order.js")]
    public Task Port200_request_queue_await_order()
        => ExecutionTestFromFile("request-queue-await-order");

    [Fact(DisplayName = "request-queue-order-state-executing.js")]
    public Task Port200_request_queue_order_state_executing()
        => ExecutionTestFromFile("request-queue-order-state-executing");

    [Fact(DisplayName = "request-queue-order.js")]
    public Task Port200_request_queue_order()
        => ExecutionTestFromFile("request-queue-order");

    [Fact(DisplayName = "request-queue-promise-resolve-order.js")]
    public Task Port200_request_queue_promise_resolve_order()
        => ExecutionTestFromFile("request-queue-promise-resolve-order");

    [Fact(DisplayName = "this-val-not-async-generator.js")]
    public Task Port200_this_val_not_async_generator()
        => ExecutionTestFromFile("this-val-not-async-generator");

    [Fact(DisplayName = "this-val-not-object.js")]
    public Task Port200_this_val_not_object()
        => ExecutionTestFromFile("this-val-not-object");
}
