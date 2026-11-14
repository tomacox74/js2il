using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype.@throw;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorPrototype.throw") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port200_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "request-queue-order-state-executing.js")]
    public Task Port200_request_queue_order_state_executing()
        => ExecutionTestFromFile("request-queue-order-state-executing");

    [Fact(DisplayName = "return-rejected-promise.js")]
    public Task Port200_return_rejected_promise()
        => ExecutionTestFromFile("return-rejected-promise");

    [Fact(DisplayName = "this-val-not-async-generator.js")]
    public Task Port200_this_val_not_async_generator()
        => ExecutionTestFromFile("this-val-not-async-generator");

    [Fact(DisplayName = "this-val-not-object.js")]
    public Task Port200_this_val_not_object()
        => ExecutionTestFromFile("this-val-not-object");

    [Fact(DisplayName = "throw-state-completed.js")]
    public Task Port200_throw_state_completed()
        => ExecutionTestFromFile("throw-state-completed");

    [Fact(DisplayName = "throw-suspendedStart-promise.js")]
    public Task Port200_throw_suspendedStart_promise()
        => ExecutionTestFromFile("throw-suspendedStart-promise");

    [Fact(DisplayName = "throw-suspendedStart.js")]
    public Task Port200_throw_suspendedStart()
        => ExecutionTestFromFile("throw-suspendedStart");

    [Fact(DisplayName = "throw-suspendedYield-promise.js")]
    public Task Port200_throw_suspendedYield_promise()
        => ExecutionTestFromFile("throw-suspendedYield-promise");

    [Fact(DisplayName = "throw-suspendedYield-try-catch.js")]
    public Task Port200_throw_suspendedYield_try_catch()
        => ExecutionTestFromFile("throw-suspendedYield-try-catch");

    [Fact(DisplayName = "throw-suspendedYield-try-finally-return.js")]
    public Task Port200_throw_suspendedYield_try_finally_return()
        => ExecutionTestFromFile("throw-suspendedYield-try-finally-return");

    [Fact(DisplayName = "throw-suspendedYield-try-finally-throw.js")]
    public Task Port200_throw_suspendedYield_try_finally_throw()
        => ExecutionTestFromFile("throw-suspendedYield-try-finally-throw");

    [Fact(DisplayName = "throw-suspendedYield-try-finally.js")]
    public Task Port200_throw_suspendedYield_try_finally()
        => ExecutionTestFromFile("throw-suspendedYield-try-finally");

    [Fact(DisplayName = "throw-suspendedYield.js")]
    public Task Port200_throw_suspendedYield()
        => ExecutionTestFromFile("throw-suspendedYield");
}
