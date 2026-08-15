using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SuppressedError;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SuppressedError") { }

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "message-method-prop-cast.js")]
    public Task message_method_prop_cast() => ExecutionTestFromFile("message-method-prop-cast");

    [Fact(DisplayName = "message-method-prop.js")]
    public Task message_method_prop() => ExecutionTestFromFile("message-method-prop");

    [Fact(DisplayName = "message-tostring-abrupt.js")]
    public Task message_tostring_abrupt() => ExecutionTestFromFile("message-tostring-abrupt");

    [Fact(DisplayName = "message-tostring-abrupt-symbol.js")]
    public Task message_tostring_abrupt_symbol() => ExecutionTestFromFile("message-tostring-abrupt-symbol");

    [Fact(DisplayName = "message-undefined-no-prop.js")]
    public Task message_undefined_no_prop() => ExecutionTestFromFile("message-undefined-no-prop");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "newtarget-is-undefined.js")]
    public Task newtarget_is_undefined() => ExecutionTestFromFile("newtarget-is-undefined");

    [Fact(DisplayName = "newtarget-proto-fallback.js")]
    public Task newtarget_proto_fallback() => ExecutionTestFromFile("newtarget-proto-fallback");

    [Fact(DisplayName = "newtarget-proto.js")]
    public Task newtarget_proto() => ExecutionTestFromFile("newtarget-proto");

    [Fact(DisplayName = "order-of-args-evaluation.js")]
    public Task order_of_args_evaluation() => ExecutionTestFromFile("order-of-args-evaluation");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");
}
