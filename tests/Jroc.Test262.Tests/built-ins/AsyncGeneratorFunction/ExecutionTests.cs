using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorFunction;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction") { }

    [Fact(DisplayName = "AsyncGeneratorFunction_length")]
    public Task AsyncGeneratorFunction_length()
        => ExecutionTestFromFile("AsyncGeneratorFunction_length");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "has-instance")]
    public Task has_instance()
        => ExecutionTestFromFile("has-instance");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "instance-length")]
    public Task instance_length()
        => ExecutionTestFromFile("instance-length");

    [Fact(DisplayName = "instance-prototype")]
    public Task instance_prototype()
        => ExecutionTestFromFile("instance-prototype");

    [Fact(DisplayName = "instance-await-expr-in-param.js")]
    public Task Port200_instance_await_expr_in_param()
        => ExecutionTestFromFile("instance-await-expr-in-param");

    [Fact(DisplayName = "instance-yield-expr-in-param.js")]
    public Task Port200_instance_yield_expr_in_param()
        => ExecutionTestFromFile("instance-yield-expr-in-param");

    [Fact(DisplayName = "invoked-as-constructor-no-arguments.js")]
    public Task Port200_invoked_as_constructor_no_arguments()
        => ExecutionTestFromFile("invoked-as-constructor-no-arguments");

    [Fact(DisplayName = "invoked-as-function-multiple-arguments.js")]
    public Task Port200_invoked_as_function_multiple_arguments()
        => ExecutionTestFromFile("invoked-as-function-multiple-arguments");

    [Fact(DisplayName = "invoked-as-function-no-arguments.js")]
    public Task Port200_invoked_as_function_no_arguments()
        => ExecutionTestFromFile("invoked-as-function-no-arguments");

    [Fact(DisplayName = "invoked-as-function-single-argument.js")]
    public Task Port200_invoked_as_function_single_argument()
        => ExecutionTestFromFile("invoked-as-function-single-argument");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task Port200_is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");
}
