using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replace;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.replace") { }

    [Fact(DisplayName = "15.5.4.11-1")]
    public Task _15_5_4_11_1()
        => ExecutionTestFromFile("15.5.4.11-1");

    [Fact(DisplayName = "S15.5.4.11_A1_T1")]
    public Task S15_5_4_11_A1_T1()
        => ExecutionTestFromFile("S15.5.4.11_A1_T1");

    [Fact(DisplayName = "S15.5.4.11_A1_T4")]
    public Task S15_5_4_11_A1_T4()
        => ExecutionTestFromFile("S15.5.4.11_A1_T4");

    [Fact(DisplayName = "S15.5.4.11_A2_T1")]
    public Task S15_5_4_11_A2_T1()
        => ExecutionTestFromFile("S15.5.4.11_A2_T1");

    [Fact(DisplayName = "S15.5.4.11_A2_T3")]
    public Task S15_5_4_11_A2_T3()
        => ExecutionTestFromFile("S15.5.4.11_A2_T3");

    [Fact(DisplayName = "S15.5.4.11_A2_T6")]
    public Task S15_5_4_11_A2_T6()
        => ExecutionTestFromFile("S15.5.4.11_A2_T6");
    [Fact(DisplayName = "S15.5.4.11_A12")]
    public Task S15_5_4_11_A12()
        => ExecutionTestFromFile("S15.5.4.11_A12");
    [Fact(DisplayName = "S15.5.4.11_A2_T5")]
    public Task S15_5_4_11_A2_T5()
        => ExecutionTestFromFile("S15.5.4.11_A2_T5");
    [Fact(DisplayName = "S15.5.4.11_A2_T2")]
    public Task S15_5_4_11_A2_T2()
        => ExecutionTestFromFile("S15.5.4.11_A2_T2");
    [Fact(DisplayName = "S15.5.4.11_A2_T4")]
    public Task S15_5_4_11_A2_T4()
        => ExecutionTestFromFile("S15.5.4.11_A2_T4");

    [Fact(DisplayName = "S15.5.4.11_A1_T8")]
    public Task S15_5_4_11_A1_T8()
        => ExecutionTestFromFile("S15.5.4.11_A1_T8");

    [Fact(DisplayName = "S15.5.4.11_A1_T14")]
    public Task S15_5_4_11_A1_T14()
        => ExecutionTestFromFile("S15.5.4.11_A1_T14");

    [Fact(DisplayName = "cstm-replace-get-err.js")]
    public Task cstm_replace_get_err()
        => ExecutionTestFromFile("cstm-replace-get-err");

    [Fact(DisplayName = "cstm-replace-invocation.js")]
    public Task cstm_replace_invocation()
        => ExecutionTestFromFile("cstm-replace-invocation");

    [Fact(DisplayName = "cstm-replace-is-null.js")]
    public Task cstm_replace_is_null()
        => ExecutionTestFromFile("cstm-replace-is-null");

    [Fact(DisplayName = "cstm-replace-on-bigint-primitive.js")]
    public Task cstm_replace_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-replace-on-bigint-primitive");

    [Fact(DisplayName = "cstm-replace-on-boolean-primitive.js")]
    public Task cstm_replace_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-replace-on-boolean-primitive");

    [Fact(DisplayName = "cstm-replace-on-number-primitive.js")]
    public Task cstm_replace_on_number_primitive()
        => ExecutionTestFromFile("cstm-replace-on-number-primitive");

    [Fact(DisplayName = "cstm-replace-on-string-primitive.js")]
    public Task cstm_replace_on_string_primitive()
        => ExecutionTestFromFile("cstm-replace-on-string-primitive");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "regexp-capture-by-index.js")]
    public Task regexp_capture_by_index()
        => ExecutionTestFromFile("regexp-capture-by-index");

    [Fact(DisplayName = "replaceValue-evaluation-order-regexp-object.js")]
    public Task replaceValue_evaluation_order_regexp_object()
        => ExecutionTestFromFile("replaceValue-evaluation-order-regexp-object");

    [Fact(DisplayName = "replaceValue-evaluation-order.js")]
    public Task replaceValue_evaluation_order()
        => ExecutionTestFromFile("replaceValue-evaluation-order");

    [Fact(DisplayName = "S15.5.4.11_A1_T10.js")]
    public Task S15_5_4_11_A1_T10()
        => ExecutionTestFromFile("S15.5.4.11_A1_T10");

    [Fact(DisplayName = "S15.5.4.11_A1_T11.js")]
    public Task S15_5_4_11_A1_T11()
        => ExecutionTestFromFile("S15.5.4.11_A1_T11");

    [Fact(DisplayName = "S15.5.4.11_A1_T12.js")]
    public Task S15_5_4_11_A1_T12()
        => ExecutionTestFromFile("S15.5.4.11_A1_T12");

    [Fact(DisplayName = "S15.5.4.11_A1_T13.js")]
    public Task S15_5_4_11_A1_T13()
        => ExecutionTestFromFile("S15.5.4.11_A1_T13");

    [Fact(DisplayName = "S15.5.4.11_A1_T15.js")]
    public Task S15_5_4_11_A1_T15()
        => ExecutionTestFromFile("S15.5.4.11_A1_T15");

    [Fact(DisplayName = "S15.5.4.11_A1_T16.js")]
    public Task S15_5_4_11_A1_T16()
        => ExecutionTestFromFile("S15.5.4.11_A1_T16");

    [Fact(DisplayName = "S15.5.4.11_A1_T17.js")]
    public Task S15_5_4_11_A1_T17()
        => ExecutionTestFromFile("S15.5.4.11_A1_T17");

    [Fact(DisplayName = "S15.5.4.11_A1_T2.js")]
    public Task S15_5_4_11_A1_T2()
        => ExecutionTestFromFile("S15.5.4.11_A1_T2");

    [Fact(DisplayName = "S15.5.4.11_A1_T5.js")]
    public Task S15_5_4_11_A1_T5()
        => ExecutionTestFromFile("S15.5.4.11_A1_T5");

    [Fact(DisplayName = "S15.5.4.11_A1_T6.js")]
    public Task S15_5_4_11_A1_T6()
        => ExecutionTestFromFile("S15.5.4.11_A1_T6");

    [Fact(DisplayName = "S15.5.4.11_A1_T7.js")]
    public Task S15_5_4_11_A1_T7()
        => ExecutionTestFromFile("S15.5.4.11_A1_T7");

    [Fact(DisplayName = "S15.5.4.11_A1_T9.js")]
    public Task S15_5_4_11_A1_T9()
        => ExecutionTestFromFile("S15.5.4.11_A1_T9");

    [Fact(DisplayName = "S15.5.4.11_A2_T10.js")]
    public Task S15_5_4_11_A2_T10()
        => ExecutionTestFromFile("S15.5.4.11_A2_T10");

    [Fact(DisplayName = "S15.5.4.11_A2_T7.js")]
    public Task S15_5_4_11_A2_T7()
        => ExecutionTestFromFile("S15.5.4.11_A2_T7");

    [Fact(DisplayName = "S15.5.4.11_A2_T8.js")]
    public Task S15_5_4_11_A2_T8()
        => ExecutionTestFromFile("S15.5.4.11_A2_T8");

    [Fact(DisplayName = "S15.5.4.11_A2_T9.js")]
    public Task S15_5_4_11_A2_T9()
        => ExecutionTestFromFile("S15.5.4.11_A2_T9");

    [Fact(DisplayName = "S15.5.4.11_A3_T1.js")]
    public Task S15_5_4_11_A3_T1()
        => ExecutionTestFromFile("S15.5.4.11_A3_T1");

    [Fact(DisplayName = "S15.5.4.11_A3_T2.js")]
    public Task S15_5_4_11_A3_T2()
        => ExecutionTestFromFile("S15.5.4.11_A3_T2");

    [Fact(DisplayName = "S15.5.4.11_A3_T3.js")]
    public Task S15_5_4_11_A3_T3()
        => ExecutionTestFromFile("S15.5.4.11_A3_T3");

    [Fact(DisplayName = "S15.5.4.11_A4_T1.js")]
    public Task S15_5_4_11_A4_T1()
        => ExecutionTestFromFile("S15.5.4.11_A4_T1");

    [Fact(DisplayName = "S15.5.4.11_A4_T2.js")]
    public Task S15_5_4_11_A4_T2()
        => ExecutionTestFromFile("S15.5.4.11_A4_T2");

    [Fact(DisplayName = "S15.5.4.11_A4_T3.js")]
    public Task S15_5_4_11_A4_T3()
        => ExecutionTestFromFile("S15.5.4.11_A4_T3");

    [Fact(DisplayName = "S15.5.4.11_A4_T4.js")]
    public Task S15_5_4_11_A4_T4()
        => ExecutionTestFromFile("S15.5.4.11_A4_T4");

    [Fact(DisplayName = "S15.5.4.11_A5_T1.js")]
    public Task S15_5_4_11_A5_T1()
        => ExecutionTestFromFile("S15.5.4.11_A5_T1");

    [Fact(DisplayName = "S15.5.4.11_A6.js")]
    public Task S15_5_4_11_A6()
        => ExecutionTestFromFile("S15.5.4.11_A6");

    [Fact(DisplayName = "S15.5.4.11_A7.js")]
    public Task S15_5_4_11_A7()
        => ExecutionTestFromFile("S15.5.4.11_A7");

    [Fact(DisplayName = "this-value-not-obj-coercible.js")]
    public Task this_value_not_obj_coercible()
        => ExecutionTestFromFile("this-value-not-obj-coercible");

    [Fact(DisplayName = "tostring-this-throws-symbol.js")]
    public Task tostring_this_throws_symbol()
        => ExecutionTestFromFile("tostring-this-throws-symbol");

    [Fact(DisplayName = "tostring-this-throws-toprimitive.js")]
    public Task tostring_this_throws_toprimitive()
        => ExecutionTestFromFile("tostring-this-throws-toprimitive");

}
