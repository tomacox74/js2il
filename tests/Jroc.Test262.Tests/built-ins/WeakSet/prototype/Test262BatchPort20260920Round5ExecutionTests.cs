using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakSet.prototype") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "prototype-attributes")]
    public Task prototype_attributes()
        => ExecutionTestFromFile("prototype-attributes");

}
