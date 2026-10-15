using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String") { }

    [Fact(DisplayName = "S15.5.1.1_A1_T6.js", Skip = "eval is not supported.")]
    public Task test_S15_5_1_1_A1_T6() => ExecutionTest("S15.5.1.1_A1_T6");

    [Fact(DisplayName = "S15.5.1.1_A1_T9.js")]
    public Task test_S15_5_1_1_A1_T9() => ExecutionTest("S15.5.1.1_A1_T9");

    [Fact(DisplayName = "S9.8_A1_T1.js", Skip = "eval is not supported.")]
    public Task test_S9_8_A1_T1() => ExecutionTest("S9.8_A1_T1");

    [Fact(DisplayName = "numeric-properties.js")]
    public Task test_numeric_properties() => ExecutionTest("numeric-properties");

    [Fact(DisplayName = "symbol-wrapping.js")]
    public Task test_symbol_wrapping() => ExecutionTest("symbol-wrapping");
}
