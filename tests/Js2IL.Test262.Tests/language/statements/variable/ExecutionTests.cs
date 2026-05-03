using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.variable;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\variable", "language.statements.variable") { }

    [Fact(DisplayName = "S12.2_A2")]
    public Task S12_2_A2()
        => ExecutionTest("S12.2_A2");

    [Fact(DisplayName = "S12.2_A3")]
    public Task S12_2_A3()
        => ExecutionTest("S12.2_A3");

    [Fact(DisplayName = "S12.2_A4")]
    public Task S12_2_A4()
        => ExecutionTest("S12.2_A4");

    [Fact(DisplayName = "arguments-non-strict")]
    public Task arguments_non_strict()
        => ExecutionTest("arguments-non-strict");

    [Fact(DisplayName = "binding-resolution")]
    public Task binding_resolution()
        => ExecutionTest("binding-resolution");

    [Fact(DisplayName = "eval-non-strict")]
    public Task eval_non_strict()
        => ExecutionTest("eval-non-strict");
}
