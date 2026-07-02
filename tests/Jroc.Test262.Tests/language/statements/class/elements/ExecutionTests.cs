using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

    [Fact(DisplayName = "abrupt-completition-on-field-initializer")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");

    [Fact(DisplayName = "class-field-is-observable-by-proxy")]
    public Task class_field_is_observable_by_proxy()
        => ExecutionTest("class-field-is-observable-by-proxy");

    [Fact(DisplayName = "new-sc-line-method-rs-static-method-privatename-identifier-alt")]
    public Task new_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("new-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-computed-names")]
    public Task after_same_line_static_gen_computed_names()
        => ExecutionTest("after-same-line-static-gen-computed-names");
}
