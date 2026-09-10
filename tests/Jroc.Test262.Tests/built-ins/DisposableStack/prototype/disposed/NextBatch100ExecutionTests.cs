using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.disposed;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.disposed") { }

    [Fact(DisplayName = "does-not-have-disposablestate-internal-slot")]
    public Task does_not_have_disposablestate_internal_slot() => ExecutionTestFromFile("does-not-have-disposablestate-internal-slot");

    [Fact(DisplayName = "getter")]
    public Task getter() => ExecutionTestFromFile("getter");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "returns-false-when-not-disposed")]
    public Task returns_false_when_not_disposed() => ExecutionTestFromFile("returns-false-when-not-disposed");

    [Fact(DisplayName = "returns-true-when-disposed")]
    public Task returns_true_when_disposed() => ExecutionTestFromFile("returns-true-when-disposed");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw() => ExecutionTestFromFile("this-not-object-throw");
}
