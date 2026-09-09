using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.statementList;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/statementList", "language.statementList") { }

    [Fact(DisplayName = "block-array-literal-with-item.js")]
    public Task block_array_literal_with_item()
        => ExecutionTest("block-array-literal-with-item");

    [Fact(DisplayName = "block-array-literal.js")]
    public Task block_array_literal()
        => ExecutionTest("block-array-literal");

    [Fact(DisplayName = "block-arrow-function-assignment-expr.js")]
    public Task block_arrow_function_assignment_expr()
        => ExecutionTest("block-arrow-function-assignment-expr");

    [Fact(DisplayName = "block-arrow-function-functionbody.js")]
    public Task block_arrow_function_functionbody()
        => ExecutionTest("block-arrow-function-functionbody");

    [Fact(DisplayName = "block-block-with-labels.js")]
    public Task block_block_with_labels()
        => ExecutionTest("block-block-with-labels");

    [Fact(DisplayName = "block-block.js")]
    public Task block_block()
        => ExecutionTest("block-block");

    [Fact(DisplayName = "block-expr-arrow-function-boolean-literal.js")]
    public Task block_expr_arrow_function_boolean_literal()
        => ExecutionTest("block-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "block-let-declaration.js")]
    public Task block_let_declaration()
        => ExecutionTest("block-let-declaration");

    [Fact(DisplayName = "block-regexp-literal-flags.js")]
    public Task block_regexp_literal_flags()
        => ExecutionTest("block-regexp-literal-flags");

    [Fact(DisplayName = "block-regexp-literal.js")]
    public Task block_regexp_literal()
        => ExecutionTest("block-regexp-literal");

    [Fact(DisplayName = "block-with-statment-array-literal-with-item.js")]
    public Task block_with_statment_array_literal_with_item()
        => ExecutionTest("block-with-statment-array-literal-with-item");

    [Fact(DisplayName = "block-with-statment-array-literal.js")]
    public Task block_with_statment_array_literal()
        => ExecutionTest("block-with-statment-array-literal");

    [Fact(DisplayName = "block-with-statment-arrow-function-assignment-expr.js")]
    public Task block_with_statment_arrow_function_assignment_expr()
        => ExecutionTest("block-with-statment-arrow-function-assignment-expr");

    [Fact(DisplayName = "block-with-statment-arrow-function-functionbody.js")]
    public Task block_with_statment_arrow_function_functionbody()
        => ExecutionTest("block-with-statment-arrow-function-functionbody");

    [Fact(DisplayName = "block-with-statment-block-with-labels.js")]
    public Task block_with_statment_block_with_labels()
        => ExecutionTest("block-with-statment-block-with-labels");

    [Fact(DisplayName = "block-with-statment-block.js")]
    public Task block_with_statment_block()
        => ExecutionTest("block-with-statment-block");

    [Fact(DisplayName = "block-with-statment-expr-arrow-function-boolean-literal.js")]
    public Task block_with_statment_expr_arrow_function_boolean_literal()
        => ExecutionTest("block-with-statment-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "block-with-statment-let-declaration.js")]
    public Task block_with_statment_let_declaration()
        => ExecutionTest("block-with-statment-let-declaration");

    [Fact(DisplayName = "block-with-statment-regexp-literal-flags.js")]
    public Task block_with_statment_regexp_literal_flags()
        => ExecutionTest("block-with-statment-regexp-literal-flags");

    [Fact(DisplayName = "block-with-statment-regexp-literal.js")]
    public Task block_with_statment_regexp_literal()
        => ExecutionTest("block-with-statment-regexp-literal");

    [Fact(DisplayName = "class-array-literal-with-item.js")]
    public Task class_array_literal_with_item()
        => ExecutionTest("class-array-literal-with-item");

    [Fact(DisplayName = "class-array-literal.js")]
    public Task class_array_literal()
        => ExecutionTest("class-array-literal");

    [Fact(DisplayName = "class-arrow-function-assignment-expr.js")]
    public Task class_arrow_function_assignment_expr()
        => ExecutionTest("class-arrow-function-assignment-expr");

    [Fact(DisplayName = "class-arrow-function-functionbody.js")]
    public Task class_arrow_function_functionbody()
        => ExecutionTest("class-arrow-function-functionbody");

    [Fact(DisplayName = "class-block-with-labels.js")]
    public Task class_block_with_labels()
        => ExecutionTest("class-block-with-labels");

    [Fact(DisplayName = "class-block.js")]
    public Task class_block()
        => ExecutionTest("class-block");

    [Fact(DisplayName = "class-expr-arrow-function-boolean-literal.js")]
    public Task class_expr_arrow_function_boolean_literal()
        => ExecutionTest("class-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "class-let-declaration.js")]
    public Task class_let_declaration()
        => ExecutionTest("class-let-declaration");

    [Fact(DisplayName = "class-regexp-literal-flags.js")]
    public Task class_regexp_literal_flags()
        => ExecutionTest("class-regexp-literal-flags");

    [Fact(DisplayName = "class-regexp-literal.js")]
    public Task class_regexp_literal()
        => ExecutionTest("class-regexp-literal");

    [Fact(DisplayName = "fn-array-literal-with-item.js")]
    public Task fn_array_literal_with_item()
        => ExecutionTest("fn-array-literal-with-item");

    [Fact(DisplayName = "fn-array-literal.js")]
    public Task fn_array_literal()
        => ExecutionTest("fn-array-literal");

    [Fact(DisplayName = "fn-arrow-function-assignment-expr.js")]
    public Task fn_arrow_function_assignment_expr()
        => ExecutionTest("fn-arrow-function-assignment-expr");

    [Fact(DisplayName = "fn-arrow-function-functionbody.js")]
    public Task fn_arrow_function_functionbody()
        => ExecutionTest("fn-arrow-function-functionbody");

    [Fact(DisplayName = "fn-block-with-labels.js")]
    public Task fn_block_with_labels()
        => ExecutionTest("fn-block-with-labels");

    [Fact(DisplayName = "fn-block.js")]
    public Task fn_block()
        => ExecutionTest("fn-block");

    [Fact(DisplayName = "fn-expr-arrow-function-boolean-literal.js")]
    public Task fn_expr_arrow_function_boolean_literal()
        => ExecutionTest("fn-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "fn-let-declaration.js")]
    public Task fn_let_declaration()
        => ExecutionTest("fn-let-declaration");

    [Fact(DisplayName = "fn-regexp-literal-flags.js")]
    public Task fn_regexp_literal_flags()
        => ExecutionTest("fn-regexp-literal-flags");

    [Fact(DisplayName = "fn-regexp-literal.js")]
    public Task fn_regexp_literal()
        => ExecutionTest("fn-regexp-literal");
}
