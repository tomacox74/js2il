using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "try-reject-finally-reject")]
    public Task try_reject_finally_reject() => ExecutionTestFromFile("try-reject-finally-reject");

    [Fact(DisplayName = "try-reject-finally-return")]
    public Task try_reject_finally_return() => ExecutionTestFromFile("try-reject-finally-return");

    [Fact(DisplayName = "try-reject-finally-throw")]
    public Task try_reject_finally_throw() => ExecutionTestFromFile("try-reject-finally-throw");

    [Fact(DisplayName = "try-return-finally-reject")]
    public Task try_return_finally_reject() => ExecutionTestFromFile("try-return-finally-reject");

    [Fact(DisplayName = "try-return-finally-return")]
    public Task try_return_finally_return() => ExecutionTestFromFile("try-return-finally-return");

    [Fact(DisplayName = "try-throw-finally-reject")]
    public Task try_throw_finally_reject() => ExecutionTestFromFile("try-throw-finally-reject");

}
