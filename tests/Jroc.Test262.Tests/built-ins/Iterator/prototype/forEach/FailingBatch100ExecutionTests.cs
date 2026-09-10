using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.forEach;

public class FailingBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Iterator.prototype.forEach") { }

    [Fact(DisplayName = "argument-effect-order")]
    public Task argument_effect_order() => ExecutionTestFromFile("argument-effect-order");

    [Fact(DisplayName = "argument-validation-failure-closes-underlying")]
    public Task argument_validation_failure_closes_underlying() => ExecutionTestFromFile("argument-validation-failure-closes-underlying");

}
