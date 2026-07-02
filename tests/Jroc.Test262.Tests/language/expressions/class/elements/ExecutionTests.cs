using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.elements") { }

    [Fact(DisplayName = "class-name-static-initializer-anonymous")]
    public Task class_name_static_initializer_anonymous()
        => ExecutionTest("class-name-static-initializer-anonymous");

    [Fact(DisplayName = "class-name-static-initializer-decl")]
    public Task class_name_static_initializer_decl()
        => ExecutionTest("class-name-static-initializer-decl");

    [Fact(DisplayName = "class-name-static-initializer-expr")]
    public Task class_name_static_initializer_expr()
        => ExecutionTest("class-name-static-initializer-expr");

    [Fact(DisplayName = "fields-asi-1")]
    public Task fields_asi_1()
        => ExecutionTest("fields-asi-1");

    [Fact(DisplayName = "fields-asi-2")]
    public Task fields_asi_2()
        => ExecutionTest("fields-asi-2");

    [Fact(DisplayName = "fields-asi-5")]
    public Task fields_asi_5()
        => ExecutionTest("fields-asi-5", preferOutOfProc: true);

    [Fact(DisplayName = "fields-computed-name-static-propname-prototype")]
    public Task fields_computed_name_static_propname_prototype()
        => ExecutionTest("fields-computed-name-static-propname-prototype");

    [Fact(DisplayName = "fields-multiple-definitions-static-private-methods-proxy")]
    public Task fields_multiple_definitions_static_private_methods_proxy()
        => ExecutionTest("fields-multiple-definitions-static-private-methods-proxy");

    [Fact(DisplayName = "new-sc-line-method-computed-symbol-names")]
    public Task new_sc_line_method_computed_symbol_names()
        => ExecutionTest("new-sc-line-method-computed-symbol-names");
}
