using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.definition;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class/definition", "language.statements.class_.definition") { }

    [Fact(DisplayName = "early-errors-class-async-method-duplicate-parameters")]
    public Task early_errors_class_async_method_duplicate_parameters()
        => CompilationFailureTest("early-errors-class-async-method-duplicate-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-NSPL-with-USD")]
    public Task early_errors_class_method_NSPL_with_USD()
        => CompilationFailureTest("early-errors-class-method-NSPL-with-USD", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-arguments-in-formal-parameters")]
    public Task early_errors_class_method_arguments_in_formal_parameters()
        => CompilationFailureTest("early-errors-class-method-arguments-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-await-in-formals-default")]
    public Task early_errors_class_method_await_in_formals_default()
        => CompilationFailureTest("early-errors-class-method-await-in-formals-default", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-await-in-formals")]
    public Task early_errors_class_method_await_in_formals()
        => CompilationFailureTest("early-errors-class-method-await-in-formals", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-eval-in-formal-parameters")]
    public Task early_errors_class_method_eval_in_formal_parameters()
        => CompilationFailureTest("early-errors-class-method-eval-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-as-function-expression-binding-identifier")]
    public Task methods_gen_yield_as_function_expression_binding_identifier()
        => CompilationFailureTest("methods-gen-yield-as-function-expression-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-as-identifier-in-nested-function")]
    public Task methods_gen_yield_as_identifier_in_nested_function()
        => CompilationFailureTest("methods-gen-yield-as-identifier-in-nested-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-as-logical-or-expression")]
    public Task methods_gen_yield_as_logical_or_expression()
        => CompilationFailureTest("methods-gen-yield-as-logical-or-expression", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-as-parameter")]
    public Task methods_gen_yield_as_parameter()
        => CompilationFailureTest("methods-gen-yield-as-parameter", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-star-after-newline")]
    public Task methods_gen_yield_star_after_newline()
        => CompilationFailureTest("methods-gen-yield-star-after-newline", "Failed to parse JavaScript");

    [Fact(DisplayName = "methods-gen-yield-weak-binding")]
    public Task methods_gen_yield_weak_binding()
        => CompilationFailureTest("methods-gen-yield-weak-binding", "Failed to parse JavaScript");
}
