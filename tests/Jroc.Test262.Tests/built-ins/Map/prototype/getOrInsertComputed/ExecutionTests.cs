using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.getOrInsertComputed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.getOrInsertComputed") { }

    [Fact(DisplayName = "canonical-key-passed-to-callback")]
    public Task canonical_key_passed_to_callback()
        => ExecutionTestFromFile("canonical-key-passed-to-callback");

    [Fact(DisplayName = "different-types-function-callbackfn-does-not-throw")]
    public Task different_types_function_callbackfn_does_not_throw()
        => ExecutionTestFromFile("different-types-function-callbackfn-does-not-throw");

    [Fact(DisplayName = "does-not-evaluate-callbackfn-if-key-present")]
    public Task does_not_evaluate_callbackfn_if_key_present()
        => ExecutionTestFromFile("does-not-evaluate-callbackfn-if-key-present");

    [Fact(DisplayName = "overwrites-mutation-from-callbackfn")]
    public Task overwrites_mutation_from_callbackfn()
        => ExecutionTestFromFile("overwrites-mutation-from-callbackfn");
}
