using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.finally_;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    [Fact(DisplayName = "rejected-observable-then-calls-PromiseResolve.js")] public Task rejected_observable_then_calls_PromiseResolve() => ExecutionTestFromFile("rejected-observable-then-calls-PromiseResolve");
    [Fact(DisplayName = "rejected-observable-then-calls-argument.js")] public Task rejected_observable_then_calls_argument() => ExecutionTestFromFile("rejected-observable-then-calls-argument");
    [Fact(DisplayName = "resolved-observable-then-calls-PromiseResolve.js")] public Task resolved_observable_then_calls_PromiseResolve() => ExecutionTestFromFile("resolved-observable-then-calls-PromiseResolve");
    [Fact(DisplayName = "resolved-observable-then-calls-argument.js")] public Task resolved_observable_then_calls_argument() => ExecutionTestFromFile("resolved-observable-then-calls-argument");
    [Fact(DisplayName = "resolved-observable-then-calls.js")] public Task resolved_observable_then_calls() => ExecutionTestFromFile("resolved-observable-then-calls");
    [Fact(DisplayName = "species-constructor.js")] public Task species_constructor() => ExecutionTestFromFile("species-constructor");
    [Fact(DisplayName = "subclass-reject-count.js")] public Task subclass_reject_count() => ExecutionTestFromFile("subclass-reject-count");
    [Fact(DisplayName = "subclass-resolve-count.js")] public Task subclass_resolve_count() => ExecutionTestFromFile("subclass-resolve-count");

    public ExecutionTests() : base("built_ins.Promise.prototype.finally") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "is-a-method")]
    public Task is_a_method()
        => ExecutionTestFromFile("is-a-method");

    [Fact(DisplayName = "resolution-value-no-override")]
    public Task resolution_value_no_override()
        => ExecutionTestFromFile("resolution-value-no-override");

    [Fact(DisplayName = "rejection-reason-no-fulfill")]
    public Task rejection_reason_no_fulfill()
        => ExecutionTestFromFile("rejection-reason-no-fulfill");

    [Fact(DisplayName = "invokes-then-with-function")]
    public Task invokes_then_with_function()
        => ExecutionTestFromFile("invokes-then-with-function");

    [Fact(DisplayName = "invokes-then-with-non-function")]
    public Task invokes_then_with_non_function()
        => ExecutionTestFromFile("invokes-then-with-non-function");

    [Fact(DisplayName = "rejected-observable-then-calls.js")]
    public Task rejected_observable_then_calls()
        => ExecutionTestFromFile("rejected-observable-then-calls");

    [Fact(DisplayName = "rejection-reason-override-with-throw.js")]
    public Task rejection_reason_override_with_throw()
        => ExecutionTestFromFile("rejection-reason-override-with-throw");
}
