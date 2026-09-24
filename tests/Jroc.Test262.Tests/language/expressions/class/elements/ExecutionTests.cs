using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.elements") { }

    [Fact(DisplayName = "after-same-line-static-gen-private-method-usage")]
    public Task after_same_line_static_gen_private_method_usage()
        => ExecutionTest("after-same-line-static-gen-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-method-usage")]
    public Task after_same_line_static_method_private_method_usage()
        => ExecutionTest("after-same-line-static-method-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-names")]
    public Task after_same_line_static_method_private_names()
        => ExecutionTest("after-same-line-static-method-private-names");

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

    [Fact(DisplayName = "multiple-definitions-private-method-usage")]
    public Task multiple_definitions_private_method_usage()
        => ExecutionTest("multiple-definitions-private-method-usage");

    [Fact(DisplayName = "multiple-definitions-private-names")]
    public Task multiple_definitions_private_names()
        => ExecutionTest("multiple-definitions-private-names");

    [Fact(DisplayName = "new-sc-line-method-computed-symbol-names")]
    public Task new_sc_line_method_computed_symbol_names()
        => ExecutionTest("new-sc-line-method-computed-symbol-names");

    [Fact(DisplayName = "after-same-line-gen-string-literal-names.js")]
    public Task ported_after_same_line_gen_string_literal_names() => ExecutionTest("after-same-line-gen-string-literal-names");

    [Fact(DisplayName = "after-same-line-method-string-literal-names.js")]
    public Task ported_after_same_line_method_string_literal_names() => ExecutionTest("after-same-line-method-string-literal-names");

    [Fact(DisplayName = "after-same-line-static-gen-string-literal-names.js")]
    public Task ported_after_same_line_static_gen_string_literal_names() => ExecutionTest("after-same-line-static-gen-string-literal-names");

    [Fact(DisplayName = "after-same-line-static-method-string-literal-names.js")]
    public Task ported_after_same_line_static_method_string_literal_names() => ExecutionTest("after-same-line-static-method-string-literal-names");

    [Fact(DisplayName = "multiple-definitions-string-literal-names.js")]
    public Task ported_multiple_definitions_string_literal_names() => ExecutionTest("multiple-definitions-string-literal-names");

    [Fact(DisplayName = "multiple-stacked-definitions-string-literal-names.js")]
    public Task ported_multiple_stacked_definitions_string_literal_names() => ExecutionTest("multiple-stacked-definitions-string-literal-names");

    [Fact(DisplayName = "new-no-sc-line-method-string-literal-names.js")]
    public Task ported_new_no_sc_line_method_string_literal_names() => ExecutionTest("new-no-sc-line-method-string-literal-names");

    [Fact(DisplayName = "new-sc-line-gen-string-literal-names.js")]
    public Task ported_new_sc_line_gen_string_literal_names() => ExecutionTest("new-sc-line-gen-string-literal-names");

    [Fact(DisplayName = "new-sc-line-method-string-literal-names.js")]
    public Task ported_new_sc_line_method_string_literal_names() => ExecutionTest("new-sc-line-method-string-literal-names");

    [Fact(DisplayName = "regular-definitions-string-literal-names.js")]
    public Task ported_regular_definitions_string_literal_names() => ExecutionTest("regular-definitions-string-literal-names");

    [Fact(DisplayName = "same-line-gen-string-literal-names.js")]
    public Task ported_same_line_gen_string_literal_names() => ExecutionTest("same-line-gen-string-literal-names");

    [Fact(DisplayName = "same-line-method-string-literal-names.js")]
    public Task ported_same_line_method_string_literal_names() => ExecutionTest("same-line-method-string-literal-names");

    [Fact(DisplayName = "wrapped-in-sc-string-literal-names.js")]
    public Task ported_wrapped_in_sc_string_literal_names() => ExecutionTest("wrapped-in-sc-string-literal-names");
}
