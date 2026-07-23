using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.groupBy;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.groupBy") { }

    [Fact(DisplayName = "callback-arg")]
    public Task callback_arg() => ExecutionTestFromFile("callback-arg");
    [Fact(DisplayName = "callback-throws")]
    public Task callback_throws() => ExecutionTestFromFile("callback-throws");
    [Fact(DisplayName = "emptyList")]
    public Task emptyList() => ExecutionTestFromFile("emptyList");
    [Fact(DisplayName = "evenOdd")]
    public Task evenOdd() => ExecutionTestFromFile("evenOdd");
    [Fact(DisplayName = "groupLength")]
    public Task groupLength() => ExecutionTestFromFile("groupLength");
    [Fact(DisplayName = "invalid-callback")]
    public Task invalid_callback() => ExecutionTestFromFile("invalid-callback");
    [Fact(DisplayName = "invalid-iterable")]
    public Task invalid_iterable() => ExecutionTestFromFile("invalid-iterable");
    [Fact(DisplayName = "iterator-next-throws")]
    public Task iterator_next_throws() => ExecutionTestFromFile("iterator-next-throws");
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "map-instance")]
    public Task map_instance() => ExecutionTestFromFile("map-instance");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "negativeZero")]
    public Task negativeZero() => ExecutionTestFromFile("negativeZero");
    [Fact(DisplayName = "toPropertyKey")]
    public Task toPropertyKey() => ExecutionTestFromFile("toPropertyKey");
}
