using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.trimStart;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.trimStart") { }

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
    [Fact(DisplayName = "this-value-object-toprimitive-meth-priority")] public Task this_value_object_toprimitive_meth_priority() => ExecutionTestFromFile("this-value-object-toprimitive-meth-priority");
    [Fact(DisplayName = "this-value-object-toprimitive-returns-object-err")] public Task this_value_object_toprimitive_returns_object_err() => ExecutionTestFromFile("this-value-object-toprimitive-returns-object-err");
    [Fact(DisplayName = "this-value-object-tostring-call-err")] public Task this_value_object_tostring_call_err() => ExecutionTestFromFile("this-value-object-tostring-call-err");
    [Fact(DisplayName = "this-value-object-tostring-meth-err")] public Task this_value_object_tostring_meth_err() => ExecutionTestFromFile("this-value-object-tostring-meth-err");
    [Fact(DisplayName = "this-value-object-tostring-meth-priority")] public Task this_value_object_tostring_meth_priority() => ExecutionTestFromFile("this-value-object-tostring-meth-priority");
    [Fact(DisplayName = "this-value-object-tostring-returns-object-err")] public Task this_value_object_tostring_returns_object_err() => ExecutionTestFromFile("this-value-object-tostring-returns-object-err");
    [Fact(DisplayName = "this-value-object-valueof-call-err")] public Task this_value_object_valueof_call_err() => ExecutionTestFromFile("this-value-object-valueof-call-err");
    [Fact(DisplayName = "this-value-object-valueof-meth-err")] public Task this_value_object_valueof_meth_err() => ExecutionTestFromFile("this-value-object-valueof-meth-err");
    [Fact(DisplayName = "this-value-object-valueof-meth-priority")] public Task this_value_object_valueof_meth_priority() => ExecutionTestFromFile("this-value-object-valueof-meth-priority");
    [Fact(DisplayName = "this-value-object-valueof-returns-object-err")] public Task this_value_object_valueof_returns_object_err() => ExecutionTestFromFile("this-value-object-valueof-returns-object-err");
    [Fact(DisplayName = "this-value-symbol-typeerror")] public Task this_value_symbol_typeerror() => ExecutionTestFromFile("this-value-symbol-typeerror");
    [Fact(DisplayName = "this-value-whitespace")] public Task this_value_whitespace() => ExecutionTestFromFile("this-value-whitespace");
}
