using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.groupBy;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.groupBy") { }

    [Fact(DisplayName = "callback-arg")]
    public Task callback_arg() => ExecutionTestFromFile("callback-arg");
    [Fact(DisplayName = "emptyList")]
    public Task emptyList() => ExecutionTestFromFile("emptyList");
    [Fact(DisplayName = "invalid-callback")]
    public Task invalid_callback() => ExecutionTestFromFile("invalid-callback");
    [Fact(DisplayName = "invalid-property-key")]
    public Task invalid_property_key() => ExecutionTestFromFile("invalid-property-key");
    [Fact(DisplayName = "iterator-next-throws")]
    public Task iterator_next_throws() => ExecutionTestFromFile("iterator-next-throws");
    [Fact(DisplayName = "null-prototype")]
    public Task null_prototype() => ExecutionTestFromFile("null-prototype");
    [Fact(DisplayName = "toPropertyKey")]
    public Task toPropertyKey() => ExecutionTestFromFile("toPropertyKey");
}
