using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.@throw;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.GeneratorPrototype.throw") { }

    [Fact(DisplayName = "from-state-completed")]
    public Task from_state_completed()
        => ExecutionTestFromFile("from-state-completed");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "this-val-not-generator")]
    public Task this_val_not_generator()
        => ExecutionTestFromFile("this-val-not-generator");

    [Fact(DisplayName = "this-val-not-object")]
    public Task this_val_not_object()
        => ExecutionTestFromFile("this-val-not-object");

    [Fact(DisplayName = "try-catch-before-try")]
    public Task try_catch_before_try()
        => ExecutionTestFromFile("try-catch-before-try");

    [Fact(DisplayName = "try-catch-following-catch")]
    public Task try_catch_following_catch()
        => ExecutionTestFromFile("try-catch-following-catch");

    [Fact(DisplayName = "try-catch-within-catch")]
    public Task try_catch_within_catch()
        => ExecutionTestFromFile("try-catch-within-catch");

    [Fact(DisplayName = "try-catch-within-try")]
    public Task try_catch_within_try()
        => ExecutionTestFromFile("try-catch-within-try");

    [Fact(DisplayName = "try-finally-before-try")]
    public Task try_finally_before_try()
        => ExecutionTestFromFile("try-finally-before-try");

    [Fact(DisplayName = "try-finally-following-finally")]
    public Task try_finally_following_finally()
        => ExecutionTestFromFile("try-finally-following-finally");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-catch")]
    public Task try_finally_nested_try_catch_within_catch()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-catch");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-finally")]
    public Task try_finally_nested_try_catch_within_finally()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-finally");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-inner-try")]
    public Task try_finally_nested_try_catch_within_inner_try()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-inner-try");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-outer-try-after-nested")]
    public Task try_finally_nested_try_catch_within_outer_try_after_nested()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-outer-try-after-nested");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-outer-try-before-nested")]
    public Task try_finally_nested_try_catch_within_outer_try_before_nested()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-outer-try-before-nested");

    [Fact(DisplayName = "try-finally-within-finally")]
    public Task try_finally_within_finally()
        => ExecutionTestFromFile("try-finally-within-finally");

    [Fact(DisplayName = "try-finally-within-try")]
    public Task try_finally_within_try()
        => ExecutionTestFromFile("try-finally-within-try");

}
