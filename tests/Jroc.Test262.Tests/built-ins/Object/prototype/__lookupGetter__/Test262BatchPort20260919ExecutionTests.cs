using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.__lookupGetter__;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Object.prototype.__lookupGetter__") { }

    [Fact(DisplayName = "lookup-proto-get-err")]
    public Task lookup_proto_get_err()
        => ExecutionTestFromFile("lookup-proto-get-err");

}
