namespace Jroc.Test262.Tests.built_ins.AggregateError;

public partial class ExecutionTests
{
    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task proto_from_ctor_realm()
        => ExecutionTestFromFile("proto-from-ctor-realm");
}
