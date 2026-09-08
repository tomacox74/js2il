using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettled;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.Promise.allSettled") { }

    [Fact(DisplayName = "call-resolve-element-after-return.js")]
    public Task call_resolve_element_after_return()
        => ExecutionTestFromFile("call-resolve-element-after-return");

    [Fact(DisplayName = "call-resolve-element-items.js")]
    public Task call_resolve_element_items()
        => ExecutionTestFromFile("call-resolve-element-items");

    [Fact(DisplayName = "call-resolve-element.js")]
    public Task call_resolve_element()
        => ExecutionTestFromFile("call-resolve-element");

    [Fact(DisplayName = "capability-executor-called-twice.js")]
    public Task capability_executor_called_twice()
        => ExecutionTestFromFile("capability-executor-called-twice");

    [Fact(DisplayName = "capability-executor-not-callable.js")]
    public Task capability_executor_not_callable()
        => ExecutionTestFromFile("capability-executor-not-callable");

    [Fact(DisplayName = "capability-resolve-throws-no-close.js")]
    public Task capability_resolve_throws_no_close()
        => ExecutionTestFromFile("capability-resolve-throws-no-close");

    [Fact(DisplayName = "ctx-ctor-throws.js")]
    public Task ctx_ctor_throws()
        => ExecutionTestFromFile("ctx-ctor-throws");

    [Fact(DisplayName = "ctx-ctor.js")]
    public Task ctx_ctor()
        => ExecutionTestFromFile("ctx-ctor");

    [Fact(DisplayName = "invoke-resolve-error-close.js")]
    public Task invoke_resolve_error_close()
        => ExecutionTestFromFile("invoke-resolve-error-close");

    [Fact(DisplayName = "invoke-resolve-get-once-multiple-calls.js")]
    public Task invoke_resolve_get_once_multiple_calls()
        => ExecutionTestFromFile("invoke-resolve-get-once-multiple-calls");

    [Fact(DisplayName = "invoke-resolve-get-once-no-calls.js")]
    public Task invoke_resolve_get_once_no_calls()
        => ExecutionTestFromFile("invoke-resolve-get-once-no-calls");

    [Fact(DisplayName = "invoke-resolve-return.js")]
    public Task invoke_resolve_return()
        => ExecutionTestFromFile("invoke-resolve-return");

    [Fact(DisplayName = "invoke-resolve.js")]
    public Task invoke_resolve()
        => ExecutionTestFromFile("invoke-resolve");

    [Fact(DisplayName = "invoke-then-error-close.js")]
    public Task invoke_then_error_close()
        => ExecutionTestFromFile("invoke-then-error-close");

    [Fact(DisplayName = "invoke-then-get-error-close.js")]
    public Task invoke_then_get_error_close()
        => ExecutionTestFromFile("invoke-then-get-error-close");

    [Fact(DisplayName = "invoke-then.js")]
    public Task invoke_then()
        => ExecutionTestFromFile("invoke-then");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function()
        => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "iter-next-val-err-no-close.js")]
    public Task iter_next_val_err_no_close()
        => ExecutionTestFromFile("iter-next-val-err-no-close");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "new-reject-function.js")]
    public Task new_reject_function()
        => ExecutionTestFromFile("new-reject-function");

    [Fact(DisplayName = "new-resolve-function.js")]
    public Task new_resolve_function()
        => ExecutionTestFromFile("new-resolve-function");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "reject-element-function-extensible.js")]
    public Task reject_element_function_extensible()
        => ExecutionTestFromFile("reject-element-function-extensible");

    [Fact(DisplayName = "reject-element-function-length.js")]
    public Task reject_element_function_length()
        => ExecutionTestFromFile("reject-element-function-length");

    [Fact(DisplayName = "reject-element-function-multiple-calls.js")]
    public Task reject_element_function_multiple_calls()
        => ExecutionTestFromFile("reject-element-function-multiple-calls");

    [Fact(DisplayName = "reject-element-function-name.js")]
    public Task reject_element_function_name()
        => ExecutionTestFromFile("reject-element-function-name");

    [Fact(DisplayName = "reject-element-function-nonconstructor.js")]
    public Task reject_element_function_nonconstructor()
        => ExecutionTestFromFile("reject-element-function-nonconstructor");

    [Fact(DisplayName = "reject-element-function-property-order.js")]
    public Task reject_element_function_property_order()
        => ExecutionTestFromFile("reject-element-function-property-order");

    [Fact(DisplayName = "reject-element-function-prototype.js")]
    public Task reject_element_function_prototype()
        => ExecutionTestFromFile("reject-element-function-prototype");

    [Fact(DisplayName = "resolve-before-loop-exit-from-same.js")]
    public Task resolve_before_loop_exit_from_same()
        => ExecutionTestFromFile("resolve-before-loop-exit-from-same");

    [Fact(DisplayName = "resolve-before-loop-exit.js")]
    public Task resolve_before_loop_exit()
        => ExecutionTestFromFile("resolve-before-loop-exit");

    [Fact(DisplayName = "resolve-element-function-extensible.js")]
    public Task resolve_element_function_extensible()
        => ExecutionTestFromFile("resolve-element-function-extensible");

    [Fact(DisplayName = "resolve-element-function-length.js")]
    public Task resolve_element_function_length()
        => ExecutionTestFromFile("resolve-element-function-length");

    [Fact(DisplayName = "resolve-element-function-name.js")]
    public Task resolve_element_function_name()
        => ExecutionTestFromFile("resolve-element-function-name");

    [Fact(DisplayName = "resolve-element-function-nonconstructor.js")]
    public Task resolve_element_function_nonconstructor()
        => ExecutionTestFromFile("resolve-element-function-nonconstructor");

    [Fact(DisplayName = "resolve-element-function-property-order.js")]
    public Task resolve_element_function_property_order()
        => ExecutionTestFromFile("resolve-element-function-property-order");

    [Fact(DisplayName = "resolve-element-function-prototype.js")]
    public Task resolve_element_function_prototype()
        => ExecutionTestFromFile("resolve-element-function-prototype");

    [Fact(DisplayName = "resolve-from-same-thenable.js")]
    public Task resolve_from_same_thenable()
        => ExecutionTestFromFile("resolve-from-same-thenable");

    [Fact(DisplayName = "resolve-throws-iterator-return-is-not-callable.js")]
    public Task resolve_throws_iterator_return_is_not_callable()
        => ExecutionTestFromFile("resolve-throws-iterator-return-is-not-callable");

    [Fact(DisplayName = "resolve-throws-iterator-return-null-or-undefined.js")]
    public Task resolve_throws_iterator_return_null_or_undefined()
        => ExecutionTestFromFile("resolve-throws-iterator-return-null-or-undefined");

    [Fact(DisplayName = "species-get-error.js")]
    public Task species_get_error()
        => ExecutionTestFromFile("species-get-error");

}
