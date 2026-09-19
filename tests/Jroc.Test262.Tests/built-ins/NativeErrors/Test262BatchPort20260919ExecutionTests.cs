using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.NativeErrors") { }

    [Fact(DisplayName = "cause_property_native_error")]
    public Task cause_property_native_error()
        => ExecutionTestFromFile("cause_property_native_error");

    [Fact(DisplayName = "message_property_native_error")]
    public Task message_property_native_error()
        => ExecutionTestFromFile("message_property_native_error");

    [Fact(DisplayName = "nativeerror-tostring-message-throws-symbol")]
    public Task nativeerror_tostring_message_throws_symbol()
        => ExecutionTestFromFile("nativeerror-tostring-message-throws-symbol");

    [Fact(DisplayName = "nativeerror-tostring-message-throws-toprimitive")]
    public Task nativeerror_tostring_message_throws_toprimitive()
        => ExecutionTestFromFile("nativeerror-tostring-message-throws-toprimitive");

}
