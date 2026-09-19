using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "methods-called-as-functions")]
    public Task methods_called_as_functions()
        => ExecutionTestFromFile("methods-called-as-functions");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

}
