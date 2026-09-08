using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.all;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.Promise.all") { }

    [Fact(DisplayName = "invoke-resolve-get-once-multiple-calls.js")]
    public Task invoke_resolve_get_once_multiple_calls()
        => ExecutionTestFromFile("invoke-resolve-get-once-multiple-calls");

    [Fact(DisplayName = "invoke-then-error-close.js")]
    public Task invoke_then_error_close()
        => ExecutionTestFromFile("invoke-then-error-close");

    [Fact(DisplayName = "invoke-then-get-error-close.js")]
    public Task invoke_then_get_error_close()
        => ExecutionTestFromFile("invoke-then-get-error-close");

    [Fact(DisplayName = "iter-next-val-err-no-close.js")]
    public Task iter_next_val_err_no_close()
        => ExecutionTestFromFile("iter-next-val-err-no-close");

    [Fact(DisplayName = "iter-step-err-no-close.js")]
    public Task iter_step_err_no_close()
        => ExecutionTestFromFile("iter-step-err-no-close");

    [Fact(DisplayName = "resolve-throws-iterator-return-is-not-callable.js")]
    public Task resolve_throws_iterator_return_is_not_callable()
        => ExecutionTestFromFile("resolve-throws-iterator-return-is-not-callable");

    [Fact(DisplayName = "resolve-throws-iterator-return-null-or-undefined.js")]
    public Task resolve_throws_iterator_return_null_or_undefined()
        => ExecutionTestFromFile("resolve-throws-iterator-return-null-or-undefined");

}
