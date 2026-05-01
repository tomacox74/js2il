using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.try_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\try", "language.statements.try_") { }

    [Fact(DisplayName = "12.14-10")]
    public Task _12_14_10()
        => ExecutionTest("12.14-10");

    [Fact(DisplayName = "12.14-11")]
    public Task _12_14_11()
        => ExecutionTest("12.14-11");

    [Fact(DisplayName = "12.14-12")]
    public Task _12_14_12()
        => ExecutionTest("12.14-12");

    [Fact(DisplayName = "12.14-3")]
    public Task _12_14_3()
        => ExecutionTest("12.14-3");

    [Fact(DisplayName = "12.14-4")]
    public Task _12_14_4()
        => ExecutionTest("12.14-4");

    [Fact(DisplayName = "12.14-6")]
    public Task _12_14_6()
        => ExecutionTest("12.14-6");

    [Fact(DisplayName = "12.14-8")]
    public Task _12_14_8()
        => ExecutionTest("12.14-8");

    [Fact(DisplayName = "12.14-9")]
    public Task _12_14_9()
        => ExecutionTest("12.14-9");
}
