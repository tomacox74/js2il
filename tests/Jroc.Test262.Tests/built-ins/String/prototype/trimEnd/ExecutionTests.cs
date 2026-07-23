using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.trimEnd;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.trimEnd") { }

    [Fact(DisplayName = "length")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc")] public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "this-value-boolean")] public Task this_value_boolean() => ExecutionTestFromFile("this-value-boolean");
    [Fact(DisplayName = "this-value-line-terminator")] public Task this_value_line_terminator() => ExecutionTestFromFile("this-value-line-terminator");
    [Fact(DisplayName = "this-value-not-obj-coercible")] public Task this_value_not_obj_coercible() => ExecutionTestFromFile("this-value-not-obj-coercible");
    [Fact(DisplayName = "this-value-number")] public Task this_value_number() => ExecutionTestFromFile("this-value-number");
    [Fact(DisplayName = "this-value-object-cannot-convert-to-primitive-err")] public Task this_value_object_cannot_convert_to_primitive_err() => ExecutionTestFromFile("this-value-object-cannot-convert-to-primitive-err");
    [Fact(DisplayName = "this-value-object-toprimitive-call-err")] public Task this_value_object_toprimitive_call_err() => ExecutionTestFromFile("this-value-object-toprimitive-call-err");
    [Fact(DisplayName = "this-value-object-toprimitive-meth-err")] public Task this_value_object_toprimitive_meth_err() => ExecutionTestFromFile("this-value-object-toprimitive-meth-err");
}
