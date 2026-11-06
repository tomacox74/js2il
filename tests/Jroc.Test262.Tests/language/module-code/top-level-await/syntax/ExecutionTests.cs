using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.top_level_await.syntax;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base("language/module-code/top-level-await/syntax", "language.module_code.top_level_await.syntax") { }

    [Fact(DisplayName = "block-await-expr-array-literal")]
    public Task block_await_expr_array_literal() => ExecutionTest("block-await-expr-array-literal");
    [Fact(DisplayName = "block-await-expr-func-expression")]
    public Task block_await_expr_func_expression() => ExecutionTest("block-await-expr-func-expression");
    [Fact(DisplayName = "block-await-expr-identifier")]
    public Task block_await_expr_identifier() => ExecutionTest("block-await-expr-identifier");
    [Fact(DisplayName = "block-await-expr-literal-number")]
    public Task block_await_expr_literal_number() => ExecutionTest("block-await-expr-literal-number");
    [Fact(DisplayName = "block-await-expr-literal-string")]
    public Task block_await_expr_literal_string() => ExecutionTest("block-await-expr-literal-string");
    [Fact(DisplayName = "block-await-expr-nested")]
    public Task block_await_expr_nested() => ExecutionTest("block-await-expr-nested");
    [Fact(DisplayName = "block-await-expr-new-expr")]
    public Task block_await_expr_new_expr() => ExecutionTest("block-await-expr-new-expr");
    [Fact(DisplayName = "block-await-expr-null")]
    public Task block_await_expr_null() => ExecutionTest("block-await-expr-null");
    [Fact(DisplayName = "block-await-expr-obj-literal")]
    public Task block_await_expr_obj_literal() => ExecutionTest("block-await-expr-obj-literal");
    [Fact(DisplayName = "block-await-expr-regexp")]
    public Task block_await_expr_regexp() => ExecutionTest("block-await-expr-regexp");
    [Fact(DisplayName = "block-await-expr-template-literal")]
    public Task block_await_expr_template_literal() => ExecutionTest("block-await-expr-template-literal");
    [Fact(DisplayName = "block-await-expr-this")]
    public Task block_await_expr_this() => ExecutionTest("block-await-expr-this");
    [Fact(DisplayName = "catch-parameter")]
    public Task catch_parameter() => ExecutionTest("catch-parameter");
    [Fact(DisplayName = "export-class-decl-await-expr-array-literal")]
    public Task export_class_decl_await_expr_array_literal() => ExecutionTest("export-class-decl-await-expr-array-literal");
    [Fact(DisplayName = "export-class-decl-await-expr-func-expression")]
    public Task export_class_decl_await_expr_func_expression() => ExecutionTest("export-class-decl-await-expr-func-expression");
    [Fact(DisplayName = "export-class-decl-await-expr-identifier")]
    public Task export_class_decl_await_expr_identifier() => ExecutionTest("export-class-decl-await-expr-identifier");
    [Fact(DisplayName = "export-class-decl-await-expr-literal-number")]
    public Task export_class_decl_await_expr_literal_number() => ExecutionTest("export-class-decl-await-expr-literal-number");
}
