using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function") { }

    [Fact(DisplayName = "S15.3.2.1_A1_T10.js")]
    public Task Port200_S15_3_2_1_A1_T10()
        => ExecutionTestFromFile("S15.3.2.1_A1_T10");

    [Fact(DisplayName = "S15.3.2.1_A1_T2.js")]
    public Task Port200_S15_3_2_1_A1_T2()
        => ExecutionTestFromFile("S15.3.2.1_A1_T2");

    [Fact(DisplayName = "S15.3.2.1_A1_T3.js")]
    public Task Port200_S15_3_2_1_A1_T3()
        => ExecutionTestFromFile("S15.3.2.1_A1_T3");

    [Fact(DisplayName = "S15.3.2.1_A1_T5.js")]
    public Task Port200_S15_3_2_1_A1_T5()
        => ExecutionTestFromFile("S15.3.2.1_A1_T5");

    [Fact(DisplayName = "S15.3.2.1_A1_T7.js")]
    public Task Port200_S15_3_2_1_A1_T7()
        => ExecutionTestFromFile("S15.3.2.1_A1_T7");

    [Fact(DisplayName = "S15.3.2.1_A3_T1.js")]
    public Task Port200_S15_3_2_1_A3_T1()
        => ExecutionTestFromFile("S15.3.2.1_A3_T1");

    [Fact(DisplayName = "S15.3.2.1_A3_T10.js")]
    public Task Port200_S15_3_2_1_A3_T10()
        => ExecutionTestFromFile("S15.3.2.1_A3_T10");

    [Fact(DisplayName = "S15.3.2.1_A3_T11.js")]
    public Task Port200_S15_3_2_1_A3_T11()
        => ExecutionTestFromFile("S15.3.2.1_A3_T11");

    [Fact(DisplayName = "S15.3.2.1_A3_T12.js")]
    public Task Port200_S15_3_2_1_A3_T12()
        => ExecutionTestFromFile("S15.3.2.1_A3_T12");

    [Fact(DisplayName = "S15.3.2.1_A3_T13.js")]
    public Task Port200_S15_3_2_1_A3_T13()
        => ExecutionTestFromFile("S15.3.2.1_A3_T13");

    [Fact(DisplayName = "S15.3.2.1_A3_T14.js")]
    public Task Port200_S15_3_2_1_A3_T14()
        => ExecutionTestFromFile("S15.3.2.1_A3_T14");

    [Fact(DisplayName = "S15.3.2.1_A3_T2.js")]
    public Task Port200_S15_3_2_1_A3_T2()
        => ExecutionTestFromFile("S15.3.2.1_A3_T2");

    [Fact(DisplayName = "S15.3.2.1_A3_T3.js")]
    public Task Port200_S15_3_2_1_A3_T3()
        => ExecutionTestFromFile("S15.3.2.1_A3_T3");

    [Fact(DisplayName = "S15.3.2.1_A3_T4.js")]
    public Task Port200_S15_3_2_1_A3_T4()
        => ExecutionTestFromFile("S15.3.2.1_A3_T4");

    [Fact(DisplayName = "S15.3.2.1_A3_T5.js")]
    public Task Port200_S15_3_2_1_A3_T5()
        => ExecutionTestFromFile("S15.3.2.1_A3_T5");

    [Fact(DisplayName = "S15.3.2.1_A3_T6.js")]
    public Task Port200_S15_3_2_1_A3_T6()
        => ExecutionTestFromFile("S15.3.2.1_A3_T6");

    [Fact(DisplayName = "S15.3.2.1_A3_T7.js")]
    public Task Port200_S15_3_2_1_A3_T7()
        => ExecutionTestFromFile("S15.3.2.1_A3_T7");

    [Fact(DisplayName = "S15.3.2.1_A3_T8.js")]
    public Task Port200_S15_3_2_1_A3_T8()
        => ExecutionTestFromFile("S15.3.2.1_A3_T8");

    [Fact(DisplayName = "S15.3.2.1_A3_T9.js")]
    public Task Port200_S15_3_2_1_A3_T9()
        => ExecutionTestFromFile("S15.3.2.1_A3_T9");

    [Fact(DisplayName = "S15.3_A2_T1.js")]
    public Task Port200_S15_3_A2_T1()
        => ExecutionTestFromFile("S15.3_A2_T1");

    [Fact(DisplayName = "S15.3_A2_T2.js")]
    public Task Port200_S15_3_A2_T2()
        => ExecutionTestFromFile("S15.3_A2_T2");

    [Fact(DisplayName = "S15.3_A3_T1.js")]
    public Task Port200_S15_3_A3_T1()
        => ExecutionTestFromFile("S15.3_A3_T1");

    [Fact(DisplayName = "S15.3_A3_T2.js")]
    public Task Port200_S15_3_A3_T2()
        => ExecutionTestFromFile("S15.3_A3_T2");

    [Fact(DisplayName = "S15.3_A3_T3.js")]
    public Task Port200_S15_3_A3_T3()
        => ExecutionTestFromFile("S15.3_A3_T3");

    [Fact(DisplayName = "S15.3_A3_T4.js")]
    public Task Port200_S15_3_A3_T4()
        => ExecutionTestFromFile("S15.3_A3_T4");

    [Fact(DisplayName = "S15.3_A3_T5.js")]
    public Task Port200_S15_3_A3_T5()
        => ExecutionTestFromFile("S15.3_A3_T5");

    [Fact(DisplayName = "S15.3_A3_T6.js")]
    public Task Port200_S15_3_A3_T6()
        => ExecutionTestFromFile("S15.3_A3_T6");

    [Fact(DisplayName = "call-bind-this-realm-undef.js")]
    public Task Port200_call_bind_this_realm_undef()
        => ExecutionTestFromFile("call-bind-this-realm-undef");

    [Fact(DisplayName = "call-bind-this-realm-value.js")]
    public Task Port200_call_bind_this_realm_value()
        => ExecutionTestFromFile("call-bind-this-realm-value");

    [Fact(DisplayName = "instance-name.js")]
    public Task Port200_instance_name()
        => ExecutionTestFromFile("instance-name");

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task Port200_proto_from_ctor_realm()
        => ExecutionTestFromFile("proto-from-ctor-realm");
}
