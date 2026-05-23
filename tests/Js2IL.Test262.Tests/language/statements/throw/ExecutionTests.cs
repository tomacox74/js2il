using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.throw_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\throw", "language.statements.throw_") { }

    [Fact(DisplayName = "S12.13_A1")]
    public Task S12_13_A1()
        => ExecutionTest("S12.13_A1");

    [Fact(DisplayName = "S12.13_A2_T1")]
    public Task S12_13_A2_T1()
        => ExecutionTest("S12.13_A2_T1");

    [Fact(DisplayName = "S12.13_A2_T2")]
    public Task S12_13_A2_T2()
        => ExecutionTest("S12.13_A2_T2");

    [Fact(DisplayName = "S12.13_A2_T4")]
    public Task S12_13_A2_T4()
        => ExecutionTest("S12.13_A2_T4");
    [Fact(DisplayName = "S12.13_A2_T3")]
    public Task S12_13_A2_T3()
        => ExecutionTest("S12.13_A2_T3");

    [Fact(DisplayName = "S12.13_A2_T5")]
    public Task S12_13_A2_T5()
        => ExecutionTest("S12.13_A2_T5");

    [Fact(DisplayName = "S12.13_A2_T7")]
    public Task S12_13_A2_T7()
        => ExecutionTest("S12.13_A2_T7");

    [Fact(DisplayName = "S12.13_A3_T1", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task S12_13_A3_T1()
        => ExecutionTest("S12.13_A3_T1");

    [Fact(DisplayName = "S12.13_A3_T2")]
    public Task S12_13_A3_T2()
        => ExecutionTest("S12.13_A3_T2");

    [Fact(DisplayName = "S12.13_A3_T3")]
    public Task S12_13_A3_T3()
        => ExecutionTest("S12.13_A3_T3");

}
