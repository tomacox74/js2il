using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.identifier_resolution;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/identifier-resolution", "language.identifier_resolution") { }

    [Fact(DisplayName = "S10.2.2_A1_T1.js")]
    public Task S10_2_2_A1_T1()
        => ExecutionTest("S10.2.2_A1_T1");

    [Fact(DisplayName = "S10.2.2_A1_T2.js")]
    public Task S10_2_2_A1_T2()
        => ExecutionTest("S10.2.2_A1_T2");

    [Fact(DisplayName = "S10.2.2_A1_T3.js")]
    public Task S10_2_2_A1_T3()
        => ExecutionTest("S10.2.2_A1_T3");

    [Fact(DisplayName = "S10.2.2_A1_T4.js")]
    public Task S10_2_2_A1_T4()
        => ExecutionTest("S10.2.2_A1_T4");

    [Fact(DisplayName = "S11.1.2_A1_T1.js")]
    public Task S11_1_2_A1_T1()
        => ExecutionTest("S11.1.2_A1_T1");

    [Fact(DisplayName = "S11.1.2_A1_T2.js")]
    public Task S11_1_2_A1_T2()
        => ExecutionTest("S11.1.2_A1_T2");

    [Fact(DisplayName = "static-init-invalid-await.js")]
    public Task static_init_invalid_await()
        => CompilationFailureTest("static-init-invalid-await");

    [Fact(DisplayName = "unscopables.js")]
    public Task unscopables()
        => ExecutionTest("unscopables");
}
