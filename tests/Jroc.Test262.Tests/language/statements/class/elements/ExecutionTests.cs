using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

    [Fact(DisplayName = "abrupt-completition-on-field-initializer")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");

    [Fact(DisplayName = "after-same-line-static-gen-private-method-usage")]
    public Task after_same_line_static_gen_private_method_usage()
        => ExecutionTest("after-same-line-static-gen-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-method-usage")]
    public Task after_same_line_static_method_private_method_usage()
        => ExecutionTest("after-same-line-static-method-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-names")]
    public Task after_same_line_static_method_private_names()
        => ExecutionTest("after-same-line-static-method-private-names");

    [Fact(DisplayName = "class-field-is-observable-by-proxy")]
    public Task class_field_is_observable_by_proxy()
        => ExecutionTest("class-field-is-observable-by-proxy");

    [Fact(DisplayName = "multiple-definitions-private-method-usage")]
    public Task multiple_definitions_private_method_usage()
        => ExecutionTest("multiple-definitions-private-method-usage");

    [Fact(DisplayName = "multiple-definitions-private-names")]
    public Task multiple_definitions_private_names()
        => ExecutionTest("multiple-definitions-private-names");

    [Fact(DisplayName = "new-sc-line-method-rs-static-method-privatename-identifier-alt")]
    public Task new_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("new-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-computed-names")]
    public Task after_same_line_static_gen_computed_names()
        => ExecutionTest("after-same-line-static-gen-computed-names");
}
