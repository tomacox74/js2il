using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.resolve;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.resolve") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S25.4.4.5_A1.1_T1")]
    public Task S25_4_4_5_A1_1_T1()
        => ExecutionTestFromFile("S25.4.4.5_A1.1_T1");

    [Fact(DisplayName = "arg-non-thenable.js")]
    public Task arg_non_thenable()
        => ExecutionTestFromFile("arg-non-thenable");

    [Fact(DisplayName = "resolve-non-obj.js")]
    public Task resolve_non_obj()
        => ExecutionTestFromFile("resolve-non-obj");

    [Fact(DisplayName = "resolve-non-thenable.js")]
    public Task resolve_non_thenable()
        => ExecutionTestFromFile("resolve-non-thenable");
}
