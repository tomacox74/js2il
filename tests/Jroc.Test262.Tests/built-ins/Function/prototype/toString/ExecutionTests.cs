using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.toString;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.toString") { }

    [Fact(DisplayName = "Function.js")]
    public Task Port200_Function()
        => ExecutionTestFromFile("Function");

    [Fact(DisplayName = "class-declaration-complex-heritage.js")]
    public Task Port200_class_declaration_complex_heritage()
        => ExecutionTestFromFile("class-declaration-complex-heritage");

    [Fact(DisplayName = "class-declaration-explicit-ctor.js")]
    public Task Port200_class_declaration_explicit_ctor()
        => ExecutionTestFromFile("class-declaration-explicit-ctor");

    [Fact(DisplayName = "class-expression-explicit-ctor.js")]
    public Task Port200_class_expression_explicit_ctor()
        => ExecutionTestFromFile("class-expression-explicit-ctor");

    [Fact(DisplayName = "symbol-named-builtins.js")]
    public Task Port200_symbol_named_builtins()
        => ExecutionTestFromFile("symbol-named-builtins");
}
