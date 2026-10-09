using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.elements.syntax.valid;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.elements.syntax.valid") { }

    [Fact(DisplayName = "grammar-field-accessor.js")]
    public Task grammar_field_accessor()
        => ExecutionTestFromFile("grammar-field-accessor");

    [Fact(DisplayName = "grammar-special-prototype-async-meth-valid.js")]
    public Task grammar_special_prototype_async_meth_valid()
        => ExecutionTestFromFile("grammar-special-prototype-async-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-accessor-meth-valid.js")]
    public Task grammar_static_ctor_accessor_meth_valid()
        => ExecutionTestFromFile("grammar-static-ctor-accessor-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-async-gen-meth-valid.js")]
    public Task grammar_static_ctor_async_gen_meth_valid()
        => ExecutionTestFromFile("grammar-static-ctor-async-gen-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-async-meth-valid.js")]
    public Task grammar_static_ctor_async_meth_valid()
        => ExecutionTestFromFile("grammar-static-ctor-async-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-gen-meth-valid.js")]
    public Task grammar_static_ctor_gen_meth_valid()
        => ExecutionTestFromFile("grammar-static-ctor-gen-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-meth-valid.js")]
    public Task grammar_static_ctor_meth_valid()
        => ExecutionTestFromFile("grammar-static-ctor-meth-valid");
}
