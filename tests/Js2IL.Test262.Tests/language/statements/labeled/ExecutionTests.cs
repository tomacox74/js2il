using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.labeled;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\labeled", "language.statements.labeled") { }

    [Fact(DisplayName = "S12.12_A1_T1")]
    public Task S12_12_A1_T1()
        => ExecutionTest("S12.12_A1_T1");

    [Fact(DisplayName = "value-await-non-module")]
    public Task value_await_non_module()
        => ExecutionTest("value-await-non-module");

    [Fact(DisplayName = "value-await-non-module-escaped")]
    public Task value_await_non_module_escaped()
        => ExecutionTest("value-await-non-module-escaped");

    [Fact(DisplayName = "value-yield-non-strict")]
    public Task value_yield_non_strict()
        => ExecutionTest("value-yield-non-strict");
    [Fact(DisplayName = "cptn-break")]
    public Task cptn_break()
        => ExecutionTest("cptn-break");

    [Fact(DisplayName = "cptn-nrml")]
    public Task cptn_nrml()
        => ExecutionTest("cptn-nrml");

}
