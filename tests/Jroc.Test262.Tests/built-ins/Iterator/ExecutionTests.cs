using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task Port200_proto_from_ctor_realm()
        => ExecutionTestFromFile("proto-from-ctor-realm");
}
