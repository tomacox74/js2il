using Jroc.Tests;
namespace Jroc.Test262.Tests.language.expressions.object_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_") { }
    [Fact(DisplayName = "__proto__-fn-name")]
    public Task _proto_fn_name()
        => ExecutionTest("__proto__-fn-name");
    [Fact(DisplayName = "__proto__-permitted-dup-shorthand")]
    public Task _proto_permitted_dup_shorthand()
        => ExecutionTest("__proto__-permitted-dup-shorthand");
    [Fact(DisplayName = "__proto__-permitted-dup")]
    public Task _proto_permitted_dup()
        => ExecutionTest("__proto__-permitted-dup");
    [Fact(DisplayName = "11.1.5_3-3-1")]
    public Task _11_1_5_3_3_1()
        => ExecutionTest("11.1.5_3-3-1");
    [Fact(DisplayName = "11.1.5-0-1")]
    public Task _11_1_5_0_1()
        => ExecutionTest("11.1.5-0-1");
    [Fact(DisplayName = "11.1.5-0-2")]
    public Task _11_1_5_0_2()
        => ExecutionTest("11.1.5-0-2");
    [Fact(DisplayName = "11.1.5-2gs")]
    public Task _11_1_5_2gs()
        => ExecutionTest("11.1.5-2gs");
    [Fact(DisplayName = "11.1.5_4-4-a-3", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_4_4_a_3()
        => ExecutionTest("11.1.5_4-4-a-3");
    [Fact(DisplayName = "11.1.5_4-4-b-1", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_4_4_b_1()
        => ExecutionTest("11.1.5_4-4-b-1");
    [Fact(DisplayName = "11.1.5_4-5-1")]
    public Task _11_1_5_4_5_1()
        => ExecutionTest("11.1.5_4-5-1");
    [Fact(DisplayName = "11.1.5_5-4-1")]
    public Task _11_1_5_5_4_1()
        => ExecutionTest("11.1.5_5-4-1");
    [Fact(DisplayName = "11.1.5_6-3-1", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_6_3_1()
        => ExecutionTest("11.1.5_6-3-1");
    [Fact(DisplayName = "11.1.5_6-3-2", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_6_3_2()
        => ExecutionTest("11.1.5_6-3-2");
    [Fact(DisplayName = "11.1.5_7-3-1", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_7_3_1()
        => ExecutionTest("11.1.5_7-3-1");
    [Fact(DisplayName = "11.1.5_7-3-2", Skip = "Blocked: eval is not supported yet.")]
    public Task _11_1_5_7_3_2()
        => ExecutionTest("11.1.5_7-3-2");
    [Fact(DisplayName = "S11.1.5_A1.1")]
    public Task S11_1_5_A1_1()
        => ExecutionTest("S11.1.5_A1.1");
    [Fact(DisplayName = "S11.1.5_A1.2")]
    public Task S11_1_5_A1_2()
        => ExecutionTest("S11.1.5_A1.2");
    [Fact(DisplayName = "S11.1.5_A1.3")]
    public Task S11_1_5_A1_3()
        => ExecutionTest("S11.1.5_A1.3");
    [Fact(DisplayName = "S11.1.5_A1.4")]
    public Task S11_1_5_A1_4()
        => ExecutionTest("S11.1.5_A1.4");
    [Fact(DisplayName = "S11.1.5_A2")]
    public Task S11_1_5_A2()
        => ExecutionTest("S11.1.5_A2");
    [Fact(DisplayName = "S11.1.5_A3")]
    public Task S11_1_5_A3()
        => ExecutionTest("S11.1.5_A3");
    [Fact(DisplayName = "S11.1.5_A4.1")]
    public Task S11_1_5_A4_1()
        => ExecutionTest("S11.1.5_A4.1");
    [Fact(DisplayName = "S11.1.5_A4.2")]
    public Task S11_1_5_A4_2()
        => ExecutionTest("S11.1.5_A4.2");
    [Fact(DisplayName = "S11.1.5_A4.3")]
    public Task S11_1_5_A4_3()
        => ExecutionTest("S11.1.5_A4.3");
    [Fact(DisplayName = "__proto__-duplicate-computed")]
    public Task proto_duplicate_computed()
        => ExecutionTest("__proto__-duplicate-computed");
    [Fact(DisplayName = "__proto__-poisoned-object-prototype")]
    public Task proto_poisoned_object_prototype()
        => ExecutionTest("__proto__-poisoned-object-prototype");
    [Fact(DisplayName = "__proto__-value-non-object")]
    public Task proto_value_non_object()
        => ExecutionTest("__proto__-value-non-object");
    [Fact(DisplayName = "__proto__-value-null")]
    public Task proto_value_null()
        => ExecutionTest("__proto__-value-null");
    [Fact(DisplayName = "__proto__-value-obj")]
    public Task proto_value_obj()
        => ExecutionTest("__proto__-value-obj");
    [Fact(DisplayName = "computed-__proto__")]
    public Task computed_proto()
        => ExecutionTest("computed-__proto__");
    [Fact(DisplayName = "literal-property-name-bigint")]
    public Task literal_property_name_bigint()
        => ExecutionTest("literal-property-name-bigint");
    [Fact(DisplayName = "name-invoke-ctor")]
    public Task method_definition_name_invoke_ctor()
        => ExecutionTest("method-definition/name-invoke-ctor");
    [Fact(DisplayName = "name-invoke-fn-no-strict")]
    public Task method_definition_name_invoke_fn_no_strict()
        => ExecutionTest("method-definition/name-invoke-fn-no-strict");
    [Fact(DisplayName = "name-invoke-fn-strict")]
    public Task method_definition_name_invoke_fn_strict()
        => ExecutionTest("method-definition/name-invoke-fn-strict");
    [Fact(DisplayName = "name-params")]
    public Task method_definition_name_params()
        => ExecutionTest("method-definition/name-params");
    [Fact(DisplayName = "name-prototype")]
    public Task method_definition_name_prototype()
        => ExecutionTest("method-definition/name-prototype");
    [Fact(DisplayName = "name-prototype-prop")]
    public Task method_definition_name_prototype_prop()
        => ExecutionTest("method-definition/name-prototype-prop");
}
