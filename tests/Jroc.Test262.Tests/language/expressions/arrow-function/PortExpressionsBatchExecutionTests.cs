using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "binding-tests-1")]
    public Task arrow_binding_tests_1()
        => ExecutionTest("arrow/binding-tests-1");

    [Fact(DisplayName = "binding-tests-2")]
    public Task arrow_binding_tests_2()
        => ExecutionTest("arrow/binding-tests-2");

    [Fact(DisplayName = "binding-tests-3")]
    public Task arrow_binding_tests_3()
        => ExecutionTest("arrow/binding-tests-3");

    [Fact(DisplayName = "capturing-closure-variables-1")]
    public Task arrow_capturing_closure_variables_1()
        => ExecutionTest("arrow/capturing-closure-variables-1");

    [Fact(DisplayName = "capturing-closure-variables-2")]
    public Task arrow_capturing_closure_variables_2()
        => ExecutionTest("arrow/capturing-closure-variables-2");

    [Fact(DisplayName = "empty-function-body-returns-undefined")]
    public Task empty_function_body_returns_undefined()
        => ExecutionTest("empty-function-body-returns-undefined");

    [Fact(DisplayName = "expression-body-implicit-return")]
    public Task expression_body_implicit_return()
        => ExecutionTest("expression-body-implicit-return");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTest("extensibility");

    [Fact(DisplayName = "lexical-arguments")]
    public Task lexical_arguments()
        => ExecutionTest("lexical-arguments");

    [Fact(DisplayName = "lexical-this")]
    public Task lexical_this()
        => ExecutionTest("lexical-this");

    [Fact(DisplayName = "object-literal-return-requires-body-parens")]
    public Task object_literal_return_requires_body_parens()
        => ExecutionTest("object-literal-return-requires-body-parens");

    [Fact(DisplayName = "prototype-rules")]
    public Task prototype_rules()
        => ExecutionTest("prototype-rules");
}
