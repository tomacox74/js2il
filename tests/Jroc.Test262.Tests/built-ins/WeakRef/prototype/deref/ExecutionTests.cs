using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakRef.prototype.deref;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakRef.prototype.deref") { }

    [Fact(DisplayName = "custom-this.js")]
    public Task custom_this() => ExecutionTestFromFile("custom-this");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-does-not-have-internal-target-throws.js")]
    public Task this_does_not_have_internal_target_throws()
        => ExecutionTestFromFile("this-does-not-have-internal-target-throws");

    [Fact(DisplayName = "this-not-object-throws.js")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");
}
