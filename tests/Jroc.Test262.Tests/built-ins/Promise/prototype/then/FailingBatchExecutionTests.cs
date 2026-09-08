using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.then;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.Promise.prototype.then") { }

    [Fact(DisplayName = "ctor-custom.js")]
    public Task ctor_custom()
        => ExecutionTestFromFile("ctor-custom");

    [Fact(DisplayName = "ctor-null.js")]
    public Task ctor_null()
        => ExecutionTestFromFile("ctor-null");

    [Fact(DisplayName = "ctor-poisoned.js")]
    public Task ctor_poisoned()
        => ExecutionTestFromFile("ctor-poisoned");

    [Fact(DisplayName = "ctor-throws.js")]
    public Task ctor_throws()
        => ExecutionTestFromFile("ctor-throws");

}
