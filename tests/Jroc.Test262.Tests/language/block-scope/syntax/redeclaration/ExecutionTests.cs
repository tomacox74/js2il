using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.redeclaration;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.syntax.redeclaration") { }

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-async-function.js")]
    public Task async_function_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-async-generator.js")]
    public Task async_function_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-class.js")]
    public Task async_function_name_redeclaration_attempt_with_class() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-const.js")]
    public Task async_function_name_redeclaration_attempt_with_const() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-function.js")]
    public Task async_function_name_redeclaration_attempt_with_function() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-generator.js")]
    public Task async_function_name_redeclaration_attempt_with_generator() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-let.js")]
    public Task async_function_name_redeclaration_attempt_with_let() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-function-name-redeclaration-attempt-with-var.js")]
    public Task async_function_name_redeclaration_attempt_with_var() => CompilationFailureTest("async-function-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-async-function.js")]
    public Task async_generator_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-async-generator.js")]
    public Task async_generator_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-class.js")]
    public Task async_generator_name_redeclaration_attempt_with_class() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-const.js")]
    public Task async_generator_name_redeclaration_attempt_with_const() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-function.js")]
    public Task async_generator_name_redeclaration_attempt_with_function() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-generator.js")]
    public Task async_generator_name_redeclaration_attempt_with_generator() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-let.js")]
    public Task async_generator_name_redeclaration_attempt_with_let() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-generator-name-redeclaration-attempt-with-var.js")]
    public Task async_generator_name_redeclaration_attempt_with_var() => CompilationFailureTest("async-generator-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-async-function.js")]
    public Task class_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("class-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-async-generator.js")]
    public Task class_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("class-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-class.js")]
    public Task class_name_redeclaration_attempt_with_class() => CompilationFailureTest("class-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-const.js")]
    public Task class_name_redeclaration_attempt_with_const() => CompilationFailureTest("class-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-function.js")]
    public Task class_name_redeclaration_attempt_with_function() => CompilationFailureTest("class-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-generator.js")]
    public Task class_name_redeclaration_attempt_with_generator() => CompilationFailureTest("class-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-let.js")]
    public Task class_name_redeclaration_attempt_with_let() => CompilationFailureTest("class-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-redeclaration-attempt-with-var.js")]
    public Task class_name_redeclaration_attempt_with_var() => CompilationFailureTest("class-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-async-function.js")]
    public Task const_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("const-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-async-generator.js")]
    public Task const_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("const-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-class.js")]
    public Task const_name_redeclaration_attempt_with_class() => CompilationFailureTest("const-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-const.js")]
    public Task const_name_redeclaration_attempt_with_const() => CompilationFailureTest("const-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-function.js")]
    public Task const_name_redeclaration_attempt_with_function() => CompilationFailureTest("const-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-generator.js")]
    public Task const_name_redeclaration_attempt_with_generator() => CompilationFailureTest("const-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-let.js")]
    public Task const_name_redeclaration_attempt_with_let() => CompilationFailureTest("const-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "const-name-redeclaration-attempt-with-var.js")]
    public Task const_name_redeclaration_attempt_with_var() => CompilationFailureTest("const-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-async-function.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-async-generator.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-class.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_class() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-const.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_const() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-function.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_function() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-generator.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_generator() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "fn-scope-var-name-redeclaration-attempt-with-let.js")]
    public Task fn_scope_var_name_redeclaration_attempt_with_let() => CompilationFailureTest("fn-scope-var-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-declaration-attempt-to-redeclare-with-var-declaration-nested-in-function.js")]
    public Task function_declaration_attempt_to_redeclare_with_var_declaration_nested_in_function() => CompilationFailureTest("function-declaration-attempt-to-redeclare-with-var-declaration-nested-in-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-async-function.js")]
    public Task function_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("function-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-async-generator.js")]
    public Task function_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("function-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-class.js")]
    public Task function_name_redeclaration_attempt_with_class() => CompilationFailureTest("function-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-const.js")]
    public Task function_name_redeclaration_attempt_with_const() => CompilationFailureTest("function-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-function.js")]
    public Task function_name_redeclaration_attempt_with_function() => CompilationFailureTest("function-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-generator.js")]
    public Task function_name_redeclaration_attempt_with_generator() => CompilationFailureTest("function-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-let.js")]
    public Task function_name_redeclaration_attempt_with_let() => CompilationFailureTest("function-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "function-name-redeclaration-attempt-with-var.js")]
    public Task function_name_redeclaration_attempt_with_var() => CompilationFailureTest("function-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-async-function.js")]
    public Task generator_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("generator-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-async-generator.js")]
    public Task generator_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("generator-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-class.js")]
    public Task generator_name_redeclaration_attempt_with_class() => CompilationFailureTest("generator-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-const.js")]
    public Task generator_name_redeclaration_attempt_with_const() => CompilationFailureTest("generator-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-function.js")]
    public Task generator_name_redeclaration_attempt_with_function() => CompilationFailureTest("generator-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-generator.js")]
    public Task generator_name_redeclaration_attempt_with_generator() => CompilationFailureTest("generator-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-let.js")]
    public Task generator_name_redeclaration_attempt_with_let() => CompilationFailureTest("generator-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "generator-name-redeclaration-attempt-with-var.js")]
    public Task generator_name_redeclaration_attempt_with_var() => CompilationFailureTest("generator-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-async-function.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-async-generator.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-class.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_class() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-const.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_const() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-function.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_function() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-generator.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_generator() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-name-redeclaration-attempt-with-let.js")]
    public Task inner_block_var_name_redeclaration_attempt_with_let() => CompilationFailureTest("inner-block-var-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-async-function.js")]
    public Task inner_block_var_redeclaration_attempt_after_async_function() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-async-generator.js")]
    public Task inner_block_var_redeclaration_attempt_after_async_generator() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-class.js")]
    public Task inner_block_var_redeclaration_attempt_after_class() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-const.js")]
    public Task inner_block_var_redeclaration_attempt_after_const() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-function.js")]
    public Task inner_block_var_redeclaration_attempt_after_function() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-generator.js")]
    public Task inner_block_var_redeclaration_attempt_after_generator() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "inner-block-var-redeclaration-attempt-after-let.js")]
    public Task inner_block_var_redeclaration_attempt_after_let() => CompilationFailureTest("inner-block-var-redeclaration-attempt-after-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-async-function.js")]
    public Task let_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("let-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-async-generator.js")]
    public Task let_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("let-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-class.js")]
    public Task let_name_redeclaration_attempt_with_class() => CompilationFailureTest("let-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-const.js")]
    public Task let_name_redeclaration_attempt_with_const() => CompilationFailureTest("let-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-function.js")]
    public Task let_name_redeclaration_attempt_with_function() => CompilationFailureTest("let-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-generator.js")]
    public Task let_name_redeclaration_attempt_with_generator() => CompilationFailureTest("let-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-let.js")]
    public Task let_name_redeclaration_attempt_with_let() => CompilationFailureTest("let-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "let-name-redeclaration-attempt-with-var.js")]
    public Task let_name_redeclaration_attempt_with_var() => CompilationFailureTest("let-name-redeclaration-attempt-with-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-async-function.js")]
    public Task var_name_redeclaration_attempt_with_async_function() => CompilationFailureTest("var-name-redeclaration-attempt-with-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-async-generator.js")]
    public Task var_name_redeclaration_attempt_with_async_generator() => CompilationFailureTest("var-name-redeclaration-attempt-with-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-class.js")]
    public Task var_name_redeclaration_attempt_with_class() => CompilationFailureTest("var-name-redeclaration-attempt-with-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-const.js")]
    public Task var_name_redeclaration_attempt_with_const() => CompilationFailureTest("var-name-redeclaration-attempt-with-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-function.js")]
    public Task var_name_redeclaration_attempt_with_function() => CompilationFailureTest("var-name-redeclaration-attempt-with-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-generator.js")]
    public Task var_name_redeclaration_attempt_with_generator() => CompilationFailureTest("var-name-redeclaration-attempt-with-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-name-redeclaration-attempt-with-let.js")]
    public Task var_name_redeclaration_attempt_with_let() => CompilationFailureTest("var-name-redeclaration-attempt-with-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-async-function.js")]
    public Task var_redeclaration_attempt_after_async_function() => CompilationFailureTest("var-redeclaration-attempt-after-async-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-async-generator.js")]
    public Task var_redeclaration_attempt_after_async_generator() => CompilationFailureTest("var-redeclaration-attempt-after-async-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-class.js")]
    public Task var_redeclaration_attempt_after_class() => CompilationFailureTest("var-redeclaration-attempt-after-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-const.js")]
    public Task var_redeclaration_attempt_after_const() => CompilationFailureTest("var-redeclaration-attempt-after-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-function.js")]
    public Task var_redeclaration_attempt_after_function() => CompilationFailureTest("var-redeclaration-attempt-after-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-generator.js")]
    public Task var_redeclaration_attempt_after_generator() => CompilationFailureTest("var-redeclaration-attempt-after-generator", "Failed to parse JavaScript");

    [Fact(DisplayName = "var-redeclaration-attempt-after-let.js")]
    public Task var_redeclaration_attempt_after_let() => CompilationFailureTest("var-redeclaration-attempt-after-let", "Failed to parse JavaScript");
}
