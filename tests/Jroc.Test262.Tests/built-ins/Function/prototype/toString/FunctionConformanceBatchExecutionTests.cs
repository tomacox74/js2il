using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.toString;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.toString") { }

    [Fact(DisplayName = "AsyncFunction.js")]
    public Task AsyncFunction() => ExecutionTestFromFile("AsyncFunction");

    [Fact(DisplayName = "AsyncGenerator.js")]
    public Task AsyncGenerator() => ExecutionTestFromFile("AsyncGenerator");

    [Fact(DisplayName = "GeneratorFunction.js")]
    public Task GeneratorFunction() => ExecutionTestFromFile("GeneratorFunction");

    [Fact(DisplayName = "S15.3.4.2_A10.js")]
    public Task S15_3_4_2_A10() => ExecutionTestFromFile("S15.3.4.2_A10");

    [Fact(DisplayName = "S15.3.4.2_A11.js")]
    public Task S15_3_4_2_A11() => ExecutionTestFromFile("S15.3.4.2_A11");

    [Fact(DisplayName = "S15.3.4.2_A12.js")]
    public Task S15_3_4_2_A12() => ExecutionTestFromFile("S15.3.4.2_A12");

    [Fact(DisplayName = "S15.3.4.2_A13.js")]
    public Task S15_3_4_2_A13() => ExecutionTestFromFile("S15.3.4.2_A13");

    [Fact(DisplayName = "S15.3.4.2_A14.js")]
    public Task S15_3_4_2_A14() => ExecutionTestFromFile("S15.3.4.2_A14");

    [Fact(DisplayName = "S15.3.4.2_A16.js")]
    public Task S15_3_4_2_A16() => ExecutionTestFromFile("S15.3.4.2_A16");

    [Fact(DisplayName = "S15.3.4.2_A6.js")]
    public Task S15_3_4_2_A6() => ExecutionTestFromFile("S15.3.4.2_A6");

    [Fact(DisplayName = "S15.3.4.2_A8.js")]
    public Task S15_3_4_2_A8() => ExecutionTestFromFile("S15.3.4.2_A8");

    [Fact(DisplayName = "S15.3.4.2_A9.js")]
    public Task S15_3_4_2_A9() => ExecutionTestFromFile("S15.3.4.2_A9");

    [Fact(DisplayName = "arrow-function.js")]
    public Task arrow_function() => ExecutionTestFromFile("arrow-function");

    [Fact(DisplayName = "async-arrow-function.js")]
    public Task async_arrow_function() => ExecutionTestFromFile("async-arrow-function");

    [Fact(DisplayName = "async-function-declaration.js")]
    public Task async_function_declaration() => ExecutionTestFromFile("async-function-declaration");

    [Fact(DisplayName = "async-function-expression.js")]
    public Task async_function_expression() => ExecutionTestFromFile("async-function-expression");

    [Fact(DisplayName = "async-generator-declaration.js")]
    public Task async_generator_declaration() => ExecutionTestFromFile("async-generator-declaration");

    [Fact(DisplayName = "async-generator-expression.js")]
    public Task async_generator_expression() => ExecutionTestFromFile("async-generator-expression");

    [Fact(DisplayName = "async-generator-method-class-expression-static.js")]
    public Task async_generator_method_class_expression_static() => ExecutionTestFromFile("async-generator-method-class-expression-static");

    [Fact(DisplayName = "async-generator-method-class-expression.js")]
    public Task async_generator_method_class_expression() => ExecutionTestFromFile("async-generator-method-class-expression");

    [Fact(DisplayName = "async-generator-method-class-statement-static.js")]
    public Task async_generator_method_class_statement_static() => ExecutionTestFromFile("async-generator-method-class-statement-static");

    [Fact(DisplayName = "async-generator-method-class-statement.js")]
    public Task async_generator_method_class_statement() => ExecutionTestFromFile("async-generator-method-class-statement");

    [Fact(DisplayName = "async-generator-method-object.js")]
    public Task async_generator_method_object() => ExecutionTestFromFile("async-generator-method-object");

    [Fact(DisplayName = "async-method-class-expression-static.js")]
    public Task async_method_class_expression_static() => ExecutionTestFromFile("async-method-class-expression-static");

    [Fact(DisplayName = "async-method-class-expression.js")]
    public Task async_method_class_expression() => ExecutionTestFromFile("async-method-class-expression");

    [Fact(DisplayName = "async-method-class-statement-static.js")]
    public Task async_method_class_statement_static() => ExecutionTestFromFile("async-method-class-statement-static");

    [Fact(DisplayName = "async-method-class-statement.js")]
    public Task async_method_class_statement() => ExecutionTestFromFile("async-method-class-statement");

    [Fact(DisplayName = "async-method-object.js")]
    public Task async_method_object() => ExecutionTestFromFile("async-method-object");

    [Fact(DisplayName = "bound-function.js")]
    public Task bound_function() => ExecutionTestFromFile("bound-function");

    [Fact(DisplayName = "built-in-function-object.js")]
    public Task built_in_function_object() => ExecutionTestFromFile("built-in-function-object");

    [Fact(DisplayName = "class-declaration-implicit-ctor.js")]
    public Task class_declaration_implicit_ctor() => ExecutionTestFromFile("class-declaration-implicit-ctor");

    [Fact(DisplayName = "class-expression-implicit-ctor.js")]
    public Task class_expression_implicit_ctor() => ExecutionTestFromFile("class-expression-implicit-ctor");

    [Fact(DisplayName = "function-declaration-non-simple-parameter-list.js")]
    public Task function_declaration_non_simple_parameter_list() => ExecutionTestFromFile("function-declaration-non-simple-parameter-list");

    [Fact(DisplayName = "function-declaration.js")]
    public Task function_declaration() => ExecutionTestFromFile("function-declaration");

    [Fact(DisplayName = "function-expression.js")]
    public Task function_expression() => ExecutionTestFromFile("function-expression");

    [Fact(DisplayName = "generator-function-declaration.js")]
    public Task generator_function_declaration() => ExecutionTestFromFile("generator-function-declaration");

    [Fact(DisplayName = "generator-function-expression.js")]
    public Task generator_function_expression() => ExecutionTestFromFile("generator-function-expression");

    [Fact(DisplayName = "generator-method.js")]
    public Task generator_method() => ExecutionTestFromFile("generator-method");

    [Fact(DisplayName = "getter-class-expression-static.js")]
    public Task getter_class_expression_static() => ExecutionTestFromFile("getter-class-expression-static");

    [Fact(DisplayName = "getter-class-expression.js")]
    public Task getter_class_expression() => ExecutionTestFromFile("getter-class-expression");

    [Fact(DisplayName = "getter-class-statement-static.js")]
    public Task getter_class_statement_static() => ExecutionTestFromFile("getter-class-statement-static");

    [Fact(DisplayName = "getter-class-statement.js")]
    public Task getter_class_statement() => ExecutionTestFromFile("getter-class-statement");

    [Fact(DisplayName = "getter-object.js")]
    public Task getter_object() => ExecutionTestFromFile("getter-object");

    [Fact(DisplayName = "line-terminator-normalisation-CR-LF.js")]
    public Task line_terminator_normalisation_CR_LF() => ExecutionTestFromFile("line-terminator-normalisation-CR-LF");

    [Fact(DisplayName = "line-terminator-normalisation-CR.js")]
    public Task line_terminator_normalisation_CR() => ExecutionTestFromFile("line-terminator-normalisation-CR");

    [Fact(DisplayName = "line-terminator-normalisation-LF.js")]
    public Task line_terminator_normalisation_LF() => ExecutionTestFromFile("line-terminator-normalisation-LF");

    [Fact(DisplayName = "method-class-expression-static.js")]
    public Task method_class_expression_static() => ExecutionTestFromFile("method-class-expression-static");

    [Fact(DisplayName = "method-class-expression.js")]
    public Task method_class_expression() => ExecutionTestFromFile("method-class-expression");

    [Fact(DisplayName = "method-class-statement-static.js")]
    public Task method_class_statement_static() => ExecutionTestFromFile("method-class-statement-static");

    [Fact(DisplayName = "method-class-statement.js")]
    public Task method_class_statement() => ExecutionTestFromFile("method-class-statement");

    [Fact(DisplayName = "method-object.js")]
    public Task method_object() => ExecutionTestFromFile("method-object");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "proxy-arrow-function.js")]
    public Task proxy_arrow_function() => ExecutionTestFromFile("proxy-arrow-function");

    [Fact(DisplayName = "proxy-async-function.js")]
    public Task proxy_async_function() => ExecutionTestFromFile("proxy-async-function");

    [Fact(DisplayName = "proxy-async-generator-function.js")]
    public Task proxy_async_generator_function() => ExecutionTestFromFile("proxy-async-generator-function");

    [Fact(DisplayName = "proxy-async-generator-method-definition.js")]
    public Task proxy_async_generator_method_definition() => ExecutionTestFromFile("proxy-async-generator-method-definition");

    [Fact(DisplayName = "proxy-async-method-definition.js")]
    public Task proxy_async_method_definition() => ExecutionTestFromFile("proxy-async-method-definition");

    [Fact(DisplayName = "proxy-bound-function.js")]
    public Task proxy_bound_function() => ExecutionTestFromFile("proxy-bound-function");

    [Fact(DisplayName = "proxy-class.js")]
    public Task proxy_class() => ExecutionTestFromFile("proxy-class");

    [Fact(DisplayName = "proxy-function-expression.js")]
    public Task proxy_function_expression() => ExecutionTestFromFile("proxy-function-expression");

    [Fact(DisplayName = "proxy-generator-function.js")]
    public Task proxy_generator_function() => ExecutionTestFromFile("proxy-generator-function");

    [Fact(DisplayName = "proxy-method-definition.js")]
    public Task proxy_method_definition() => ExecutionTestFromFile("proxy-method-definition");

    [Fact(DisplayName = "proxy-non-callable-throws.js")]
    public Task proxy_non_callable_throws() => ExecutionTestFromFile("proxy-non-callable-throws");

    [Fact(DisplayName = "setter-class-expression-static.js")]
    public Task setter_class_expression_static() => ExecutionTestFromFile("setter-class-expression-static");

    [Fact(DisplayName = "setter-class-expression.js")]
    public Task setter_class_expression() => ExecutionTestFromFile("setter-class-expression");

    [Fact(DisplayName = "setter-class-statement-static.js")]
    public Task setter_class_statement_static() => ExecutionTestFromFile("setter-class-statement-static");

    [Fact(DisplayName = "setter-class-statement.js")]
    public Task setter_class_statement() => ExecutionTestFromFile("setter-class-statement");

    [Fact(DisplayName = "setter-object.js")]
    public Task setter_object() => ExecutionTestFromFile("setter-object");

    [Fact(DisplayName = "unicode.js")]
    public Task unicode() => ExecutionTestFromFile("unicode");

}
