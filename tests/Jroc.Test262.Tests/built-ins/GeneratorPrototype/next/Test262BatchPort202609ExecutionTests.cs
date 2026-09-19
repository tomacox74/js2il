using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.next;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.GeneratorPrototype.next") { }

    [Fact(DisplayName = "consecutive-yields")]
    public Task consecutive_yields()
        => ExecutionTestFromFile("consecutive-yields");

    [Fact(DisplayName = "context-method-invocation")]
    public Task context_method_invocation()
        => ExecutionTestFromFile("context-method-invocation");

    [Fact(DisplayName = "lone-return")]
    public Task lone_return()
        => ExecutionTestFromFile("lone-return");

    [Fact(DisplayName = "lone-yield")]
    public Task lone_yield()
        => ExecutionTestFromFile("lone-yield");

    [Fact(DisplayName = "no-control-flow")]
    public Task no_control_flow()
        => ExecutionTestFromFile("no-control-flow");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "result-prototype")]
    public Task result_prototype()
        => ExecutionTestFromFile("result-prototype");

    [Fact(DisplayName = "return-yield-expr")]
    public Task return_yield_expr()
        => ExecutionTestFromFile("return-yield-expr");

    [Fact(DisplayName = "this-val-not-generator")]
    public Task this_val_not_generator()
        => ExecutionTestFromFile("this-val-not-generator");

    [Fact(DisplayName = "this-val-not-object")]
    public Task this_val_not_object()
        => ExecutionTestFromFile("this-val-not-object");

}
