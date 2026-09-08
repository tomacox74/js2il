using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.finally_;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.Promise.prototype.finally") { }

    [Fact(DisplayName = "subclass-species-constructor-reject-count.js")]
    public Task subclass_species_constructor_reject_count()
        => ExecutionTestFromFile("subclass-species-constructor-reject-count");

    [Fact(DisplayName = "subclass-species-constructor-resolve-count.js")]
    public Task subclass_species_constructor_resolve_count()
        => ExecutionTestFromFile("subclass-species-constructor-resolve-count");

    [Fact(DisplayName = "this-value-then-not-callable.js")]
    public Task this_value_then_not_callable()
        => ExecutionTestFromFile("this-value-then-not-callable");

}
