using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.then;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.prototype.then") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "reject-pending-fulfilled.js")]
    public Task reject_pending_fulfilled()
        => ExecutionTestFromFile("reject-pending-fulfilled");

    [Fact(DisplayName = "reject-pending-rejected.js")]
    public Task reject_pending_rejected()
        => ExecutionTestFromFile("reject-pending-rejected");

    [Fact(DisplayName = "reject-settled-fulfilled.js")]
    public Task reject_settled_fulfilled()
        => ExecutionTestFromFile("reject-settled-fulfilled");

    [Fact(DisplayName = "reject-settled-rejected.js")]
    public Task reject_settled_rejected()
        => ExecutionTestFromFile("reject-settled-rejected");

    [Fact(DisplayName = "rxn-handler-fulfilled-return-abrupt.js")]
    public Task rxn_handler_fulfilled_return_abrupt()
        => ExecutionTestFromFile("rxn-handler-fulfilled-return-abrupt");

    [Fact(DisplayName = "rxn-handler-rejected-return-abrupt.js")]
    public Task rxn_handler_rejected_return_abrupt()
        => ExecutionTestFromFile("rxn-handler-rejected-return-abrupt");
}
