using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.regexp_modifiers.syntax.valid;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.regexp_modifiers.syntax.valid") { }

    [Fact(DisplayName = "add-and-remove-modifiers-can-have-empty-remove-modifiers.js")]
    public Task add_and_remove_modifiers_can_have_empty_remove_modifiers()
        => ExecutionTestFromFile("add-and-remove-modifiers-can-have-empty-remove-modifiers");

    [Fact(DisplayName = "add-and-remove-modifiers.js")]
    public Task add_and_remove_modifiers()
        => ExecutionTestFromFile("add-and-remove-modifiers");

    [Fact(DisplayName = "add-modifiers-when-nested.js")]
    public Task add_modifiers_when_nested()
        => ExecutionTestFromFile("add-modifiers-when-nested");

    [Fact(DisplayName = "add-modifiers-when-not-set-as-flags.js")]
    public Task add_modifiers_when_not_set_as_flags()
        => ExecutionTestFromFile("add-modifiers-when-not-set-as-flags");

    [Fact(DisplayName = "add-modifiers-when-set-as-flags.js")]
    public Task add_modifiers_when_set_as_flags()
        => ExecutionTestFromFile("add-modifiers-when-set-as-flags");

    [Fact(DisplayName = "remove-modifiers-when-nested.js")]
    public Task remove_modifiers_when_nested()
        => ExecutionTestFromFile("remove-modifiers-when-nested");

    [Fact(DisplayName = "remove-modifiers-when-not-set-as-flags.js")]
    public Task remove_modifiers_when_not_set_as_flags()
        => ExecutionTestFromFile("remove-modifiers-when-not-set-as-flags");

    [Fact(DisplayName = "remove-modifiers-when-set-as-flags.js")]
    public Task remove_modifiers_when_set_as_flags()
        => ExecutionTestFromFile("remove-modifiers-when-set-as-flags");

}
