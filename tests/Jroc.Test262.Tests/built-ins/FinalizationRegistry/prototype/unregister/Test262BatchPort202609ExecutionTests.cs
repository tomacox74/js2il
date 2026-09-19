using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.FinalizationRegistry.prototype.unregister;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.FinalizationRegistry.prototype.unregister") { }

    [Fact(DisplayName = "custom-this")]
    public Task custom_this()
        => ExecutionTestFromFile("custom-this");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-does-not-have-internal-cells-throws")]
    public Task this_does_not_have_internal_cells_throws()
        => ExecutionTestFromFile("this-does-not-have-internal-cells-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws()
        => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-when-unregisterToken-cannot-be-held-weakly")]
    public Task throws_when_unregisterToken_cannot_be_held_weakly()
        => ExecutionTestFromFile("throws-when-unregisterToken-cannot-be-held-weakly");

    [Fact(DisplayName = "unregister-object-token")]
    public Task unregister_object_token()
        => ExecutionTestFromFile("unregister-object-token");

    [Fact(DisplayName = "unregister-symbol-token")]
    public Task unregister_symbol_token()
        => ExecutionTestFromFile("unregister-symbol-token");

}
