using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.__proto__;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.__proto__") { }

    [Fact(DisplayName = "get-fn-name")]
    public Task get_fn_name()
        => ExecutionTestFromFile("get-fn-name");

    [Fact(DisplayName = "get-ordinary-obj")]
    public Task get_ordinary_obj()
        => ExecutionTestFromFile("get-ordinary-obj");

    [Fact(DisplayName = "get-to-obj-abrupt")]
    public Task get_to_obj_abrupt()
        => ExecutionTestFromFile("get-to-obj-abrupt");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "set-cycle")]
    public Task set_cycle()
        => ExecutionTestFromFile("set-cycle");

    [Fact(DisplayName = "set-fn-name")]
    public Task set_fn_name()
        => ExecutionTestFromFile("set-fn-name");

    [Fact(DisplayName = "set-immutable")]
    public Task set_immutable()
        => ExecutionTestFromFile("set-immutable");

    [Fact(DisplayName = "set-invalid-value")]
    public Task set_invalid_value()
        => ExecutionTestFromFile("set-invalid-value");

    [Fact(DisplayName = "set-non-extensible")]
    public Task set_non_extensible()
        => ExecutionTestFromFile("set-non-extensible");

    [Fact(DisplayName = "set-non-obj-coercible")]
    public Task set_non_obj_coercible()
        => ExecutionTestFromFile("set-non-obj-coercible");

    [Fact(DisplayName = "set-non-object")]
    public Task set_non_object()
        => ExecutionTestFromFile("set-non-object");

    [Fact(DisplayName = "set-ordinary-obj")]
    public Task set_ordinary_obj()
        => ExecutionTestFromFile("set-ordinary-obj");
}
