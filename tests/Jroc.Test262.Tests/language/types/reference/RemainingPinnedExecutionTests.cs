namespace Jroc.Test262.Tests.language.types.reference;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.reference") { }

    [Fact(DisplayName = "8.7.2-1-s.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task _8_7_2_1_s()
        => ExecutionTest("8.7.2-1-s");

    [Fact(DisplayName = "S8.7.2_A1_T1.js")]
    public Task S8_7_2_A1_T1()
        => CompilationFailureTest("S8.7.2_A1_T1");

    [Fact(DisplayName = "S8.7.2_A1_T2.js")]
    public Task S8_7_2_A1_T2()
        => CompilationFailureTest("S8.7.2_A1_T2");

    [Fact(DisplayName = "S8.7_A5_T2.js")]
    public Task S8_7_A5_T2()
        => ExecutionTest("S8.7_A5_T2");

    [Fact(DisplayName = "get-value-prop-base-primitive-realm.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task get_value_prop_base_primitive_realm()
        => ExecutionTest("get-value-prop-base-primitive-realm");

    [Fact(DisplayName = "put-value-prop-base-primitive-realm.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task put_value_prop_base_primitive_realm()
        => ExecutionTest("put-value-prop-base-primitive-realm");

    [Fact(DisplayName = "put-value-prop-base-primitive.js")]
    public Task put_value_prop_base_primitive()
        => ExecutionTest("put-value-prop-base-primitive");

}
