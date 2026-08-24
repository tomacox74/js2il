using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.__lookupSetter__;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.__lookupSetter__") { }

    [Fact(DisplayName = "lookup-own-data")]
    public Task lookup_own_data()
        => ExecutionTestFromFile("lookup-own-data");

    [Fact(DisplayName = "lookup-proto-data")]
    public Task lookup_proto_data()
        => ExecutionTestFromFile("lookup-proto-data");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
