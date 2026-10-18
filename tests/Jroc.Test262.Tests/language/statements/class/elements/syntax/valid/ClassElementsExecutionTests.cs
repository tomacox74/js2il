namespace Jroc.Test262.Tests.language.statements.test_class.elements.syntax.valid;

public sealed class ClassElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassElementsExecutionTests() : base("language.statements.test_class.elements.syntax.valid") { }

    [Fact(DisplayName = "grammar-field-accessor.js")]
    public Task grammar_field_accessor()
        => ExecutionTest("grammar-field-accessor");

    [Fact(DisplayName = "grammar-field-named-get-followed-by-generator-asi.js")]
    public Task grammar_field_named_get_followed_by_generator_asi()
        => ExecutionTest("grammar-field-named-get-followed-by-generator-asi");

    [Fact(DisplayName = "grammar-field-named-set-followed-by-generator-asi.js")]
    public Task grammar_field_named_set_followed_by_generator_asi()
        => ExecutionTest("grammar-field-named-set-followed-by-generator-asi");

    [Fact(DisplayName = "grammar-special-prototype-async-meth-valid.js")]
    public Task grammar_special_prototype_async_meth_valid()
        => ExecutionTest("grammar-special-prototype-async-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-accessor-meth-valid.js")]
    public Task grammar_static_ctor_accessor_meth_valid()
        => ExecutionTest("grammar-static-ctor-accessor-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-async-gen-meth-valid.js")]
    public Task grammar_static_ctor_async_gen_meth_valid()
        => ExecutionTest("grammar-static-ctor-async-gen-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-async-meth-valid.js")]
    public Task grammar_static_ctor_async_meth_valid()
        => ExecutionTest("grammar-static-ctor-async-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-gen-meth-valid.js")]
    public Task grammar_static_ctor_gen_meth_valid()
        => ExecutionTest("grammar-static-ctor-gen-meth-valid");

    [Fact(DisplayName = "grammar-static-ctor-meth-valid.js")]
    public Task grammar_static_ctor_meth_valid()
        => ExecutionTest("grammar-static-ctor-meth-valid");
}
