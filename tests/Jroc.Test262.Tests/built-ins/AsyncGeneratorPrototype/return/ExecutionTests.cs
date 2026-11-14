using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype.@return;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorPrototype.return") { }

    [Fact(DisplayName = "iterator-result-prototype.js")]
    public Task Port200_iterator_result_prototype()
        => ExecutionTestFromFile("iterator-result-prototype");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port200_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "request-queue-order-state-executing.js")]
    public Task Port200_request_queue_order_state_executing()
        => ExecutionTestFromFile("request-queue-order-state-executing");

    [Fact(DisplayName = "return-state-completed-broken-promise.js")]
    public Task Port200_return_state_completed_broken_promise()
        => ExecutionTestFromFile("return-state-completed-broken-promise");

    [Fact(DisplayName = "return-state-completed.js")]
    public Task Port200_return_state_completed()
        => ExecutionTestFromFile("return-state-completed");

    [Fact(DisplayName = "return-suspendedStart-broken-promise.js")]
    public Task Port200_return_suspendedStart_broken_promise()
        => ExecutionTestFromFile("return-suspendedStart-broken-promise");

    [Fact(DisplayName = "return-suspendedStart-promise.js")]
    public Task Port200_return_suspendedStart_promise()
        => ExecutionTestFromFile("return-suspendedStart-promise");

    [Fact(DisplayName = "return-suspendedStart.js")]
    public Task Port200_return_suspendedStart()
        => ExecutionTestFromFile("return-suspendedStart");

    [Fact(DisplayName = "return-suspendedYield-broken-promise-try-catch.js")]
    public Task Port200_return_suspendedYield_broken_promise_try_catch()
        => ExecutionTestFromFile("return-suspendedYield-broken-promise-try-catch");

    [Fact(DisplayName = "return-suspendedYield-promise.js")]
    public Task Port200_return_suspendedYield_promise()
        => ExecutionTestFromFile("return-suspendedYield-promise");

    [Fact(DisplayName = "return-suspendedYield-try-finally-return.js")]
    public Task Port200_return_suspendedYield_try_finally_return()
        => ExecutionTestFromFile("return-suspendedYield-try-finally-return");

    [Fact(DisplayName = "return-suspendedYield-try-finally-throw.js")]
    public Task Port200_return_suspendedYield_try_finally_throw()
        => ExecutionTestFromFile("return-suspendedYield-try-finally-throw");

    [Fact(DisplayName = "return-suspendedYield-try-finally.js")]
    public Task Port200_return_suspendedYield_try_finally()
        => ExecutionTestFromFile("return-suspendedYield-try-finally");

    [Fact(DisplayName = "return-suspendedYield.js")]
    public Task Port200_return_suspendedYield()
        => ExecutionTestFromFile("return-suspendedYield");

    [Fact(DisplayName = "this-val-not-async-generator.js")]
    public Task Port200_this_val_not_async_generator()
        => ExecutionTestFromFile("this-val-not-async-generator");

    [Fact(DisplayName = "this-val-not-object.js")]
    public Task Port200_this_val_not_object()
        => ExecutionTestFromFile("this-val-not-object");
}
