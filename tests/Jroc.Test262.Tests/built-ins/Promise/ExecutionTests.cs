using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "exec-args")]
    public Task exec_args()
        => ExecutionTestFromFile("exec-args");

    [Fact(DisplayName = "executor-call-context-sloppy")]
    public Task executor_call_context_sloppy()
        => ExecutionTestFromFile("executor-call-context-sloppy");

    [Fact(DisplayName = "executor-call-context-strict")]
    public Task executor_call_context_strict()
        => ExecutionTestFromFile("executor-call-context-strict");

    [Fact(DisplayName = "executor-function-extensible")]
    public Task executor_function_extensible()
        => ExecutionTestFromFile("executor-function-extensible");

    [Fact(DisplayName = "executor-function-property-order")]
    public Task executor_function_property_order()
        => ExecutionTestFromFile("executor-function-property-order");

    [Fact(DisplayName = "executor-function-prototype")]
    public Task executor_function_prototype()
        => ExecutionTestFromFile("executor-function-prototype");

    [Fact(DisplayName = "property-order")]
    public Task property_order()
        => ExecutionTestFromFile("property-order");

    [Fact(DisplayName = "reject-function-extensible")]
    public Task reject_function_extensible()
        => ExecutionTestFromFile("reject-function-extensible");

    [Fact(DisplayName = "resolve-function-extensible")]
    public Task resolve_function_extensible()
        => ExecutionTestFromFile("resolve-function-extensible");

    [Fact(DisplayName = "exception-after-resolve-in-executor")]
    public Task exception_after_resolve_in_executor()
        => ExecutionTestFromFile("exception-after-resolve-in-executor");

    [Fact(DisplayName = "executor-function-length")]
    public Task executor_function_length()
        => ExecutionTestFromFile("executor-function-length");

    [Fact(DisplayName = "executor-function-name")]
    public Task executor_function_name()
        => ExecutionTestFromFile("executor-function-name");

    [Fact(DisplayName = "executor-function-not-a-constructor")]
    public Task executor_function_not_a_constructor()
        => ExecutionTestFromFile("executor-function-not-a-constructor");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "promise")]
    public Task promise()
        => ExecutionTestFromFile("promise");

    [Fact(DisplayName = "reject-function-length")]
    public Task reject_function_length()
        => ExecutionTestFromFile("reject-function-length");

    [Fact(DisplayName = "reject-function-prototype")]
    public Task reject_function_prototype()
        => ExecutionTestFromFile("reject-function-prototype");

    [Fact(DisplayName = "reject-ignored-via-fn-immed")]
    public Task reject_ignored_via_fn_immed()
        => ExecutionTestFromFile("reject-ignored-via-fn-immed");

    [Fact(DisplayName = "reject-via-fn-immed")]
    public Task reject_via_fn_immed()
        => ExecutionTestFromFile("reject-via-fn-immed");

    [Fact(DisplayName = "resolve-function-length")]
    public Task resolve_function_length()
        => ExecutionTestFromFile("resolve-function-length");

    [Fact(DisplayName = "resolve-function-prototype")]
    public Task resolve_function_prototype()
        => ExecutionTestFromFile("resolve-function-prototype");

    [Fact(DisplayName = "resolve-ignored-via-fn-immed")]
    public Task resolve_ignored_via_fn_immed()
        => ExecutionTestFromFile("resolve-ignored-via-fn-immed");

    [Fact(DisplayName = "resolve-non-obj-immed")]
    public Task resolve_non_obj_immed()
        => ExecutionTestFromFile("resolve-non-obj-immed");

    [Fact(DisplayName = "resolve-non-thenable-immed")]
    public Task resolve_non_thenable_immed()
        => ExecutionTestFromFile("resolve-non-thenable-immed");
}
