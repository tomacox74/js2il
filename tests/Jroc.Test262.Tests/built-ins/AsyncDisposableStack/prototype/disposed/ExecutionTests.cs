using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.disposed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.disposed") { }

    [Fact(DisplayName = "does-not-have-asyncdisposablestate-internal-slot.js")]
    public Task does_not_have_asyncdisposablestate_internal_slot() => ExecutionTestFromFile("does-not-have-asyncdisposablestate-internal-slot");

    [Fact(DisplayName = "getter.js")]
    public Task getter() => ExecutionTestFromFile("getter");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "returns-false-when-not-disposed.js")]
    public Task returns_false_when_not_disposed() => ExecutionTestFromFile("returns-false-when-not-disposed");

    [Fact(DisplayName = "returns-true-when-disposed.js")]
    public Task returns_true_when_disposed() => ExecutionTestFromFile("returns-true-when-disposed");

    [Fact(DisplayName = "this-not-object-throw.js")]
    public Task this_not_object_throw() => ExecutionTestFromFile("this-not-object-throw");
}
