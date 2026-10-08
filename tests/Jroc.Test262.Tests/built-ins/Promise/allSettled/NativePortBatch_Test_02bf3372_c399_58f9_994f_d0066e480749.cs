using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettled;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.built_ins.Promise.allSettled") { }

    [Fact(DisplayName = "iter-step-err-no-close")]
    public Task iter_step_err_no_close() => ExecutionTestFromFile("iter-step-err-no-close");

    [Fact(DisplayName = "resolve-not-callable-reject-with-typeerror")]
    public Task resolve_not_callable_reject_with_typeerror() => ExecutionTestFromFile("resolve-not-callable-reject-with-typeerror");

    [Fact(DisplayName = "resolve-poisoned-then")]
    public Task resolve_poisoned_then() => ExecutionTestFromFile("resolve-poisoned-then");

    [Fact(DisplayName = "resolve-thenable")]
    public Task resolve_thenable() => ExecutionTestFromFile("resolve-thenable");

}
