using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise.prototype.finally_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.prototype.finally") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "is-a-method")]
    public Task is_a_method()
        => ExecutionTestFromFile("is-a-method");

    [Fact(DisplayName = "resolution-value-no-override")]
    public Task resolution_value_no_override()
        => ExecutionTestFromFile("resolution-value-no-override");

    [Fact(DisplayName = "rejection-reason-no-fulfill")]
    public Task rejection_reason_no_fulfill()
        => ExecutionTestFromFile("rejection-reason-no-fulfill");
}
