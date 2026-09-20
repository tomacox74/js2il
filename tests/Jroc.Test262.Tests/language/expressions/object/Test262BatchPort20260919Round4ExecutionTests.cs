using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.@object;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.expressions.object") { }

    [Fact(DisplayName = "method")]
    public Task method()
        => ExecutionTest("method");

    [Fact(DisplayName = "not-defined")]
    public Task not_defined()
        => ExecutionTest("not-defined");

    [Fact(DisplayName = "prop-def-id-valid")]
    public Task prop_def_id_valid()
        => ExecutionTest("prop-def-id-valid");

    [Fact(DisplayName = "prop-dup-data-data")]
    public Task prop_dup_data_data()
        => ExecutionTest("prop-dup-data-data");

    [Fact(DisplayName = "prop-dup-data-set")]
    public Task prop_dup_data_set()
        => ExecutionTest("prop-dup-data-set");

    [Fact(DisplayName = "prop-dup-get-data")]
    public Task prop_dup_get_data()
        => ExecutionTest("prop-dup-get-data");

    [Fact(DisplayName = "prop-dup-get-get")]
    public Task prop_dup_get_get()
        => ExecutionTest("prop-dup-get-get");

    [Fact(DisplayName = "prop-dup-get-set-get")]
    public Task prop_dup_get_set_get()
        => ExecutionTest("prop-dup-get-set-get");

    [Fact(DisplayName = "prop-dup-set-data")]
    public Task prop_dup_set_data()
        => ExecutionTest("prop-dup-set-data");

    [Fact(DisplayName = "prop-dup-set-get-set")]
    public Task prop_dup_set_get_set()
        => ExecutionTest("prop-dup-set-get-set");

    [Fact(DisplayName = "prop-dup-set-set")]
    public Task prop_dup_set_set()
        => ExecutionTest("prop-dup-set-set");

    [Fact(DisplayName = "properties-names-eval-arguments")]
    public Task properties_names_eval_arguments()
        => ExecutionTest("properties-names-eval-arguments");

    [Fact(DisplayName = "property-name-yield")]
    public Task property_name_yield()
        => ExecutionTest("property-name-yield");

    [Fact(DisplayName = "scope-gen-meth-paramsbody-var-close")]
    public Task scope_gen_meth_paramsbody_var_close()
        => ExecutionTest("scope-gen-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-gen-meth-paramsbody-var-open")]
    public Task scope_gen_meth_paramsbody_var_open()
        => ExecutionTest("scope-gen-meth-paramsbody-var-open");

}
