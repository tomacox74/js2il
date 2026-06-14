using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.prototype") { }

    [Fact(DisplayName = "Promise_prototype_Symbol_toStringTag")]
    public Task Promise_prototype_Symbol_toStringTag()
        => ExecutionTest("Promise_prototype_Symbol_toStringTag");
}
