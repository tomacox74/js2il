using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.delete;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.delete") { }

    [Fact(DisplayName = "11.4.1-4.a-4.js")]
    public Task test_11_4_1_4_a_4() => ExecutionTest("11.4.1-4.a-4");

    [Fact(DisplayName = "11.4.1-4.a-6.js")]
    public Task test_11_4_1_4_a_6() => ExecutionTest("11.4.1-4.a-6");

    [Fact(DisplayName = "11.4.1-4.a-7.js", Skip = "eval is not supported.")]
    public Task test_11_4_1_4_a_7() => ExecutionTest("11.4.1-4.a-7");

    [Fact(DisplayName = "S11.4.1_A3.3_T1.js")]
    public Task test_S11_4_1_A3_3_T1() => ExecutionTest("S11.4.1_A3.3_T1");

    [Fact(DisplayName = "super-property-method.js")]
    public Task test_super_property_method() => ExecutionTest("super-property-method");

    [Fact(DisplayName = "super-property-null-base.js")]
    public Task test_super_property_null_base() => ExecutionTest("super-property-null-base");

    [Fact(DisplayName = "super-property-topropertykey.js")]
    public Task test_super_property_topropertykey() => ExecutionTest("super-property-topropertykey");

    [Fact(DisplayName = "super-property-uninitialized-this.js")]
    public Task test_super_property_uninitialized_this() => ExecutionTest("super-property-uninitialized-this");

    [Fact(DisplayName = "super-property.js")]
    public Task test_super_property() => ExecutionTest("super-property");
}
