namespace Jroc.Test262.Tests.built_ins.Object.hasOwn;

public partial class ExecutionTests
{
    [Fact(DisplayName = "descriptor.js")]
    public Task descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "hasown_inherited_getter_and_setter_configurable_enumerable.js")]
    public Task hasown_inherited_getter_and_setter_configurable_enumerable() => ExecutionTestFromFile("hasown_inherited_getter_and_setter_configurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_getter_and_setter_configurable_nonenumerable.js")]
    public Task hasown_inherited_getter_and_setter_configurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_getter_and_setter_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_getter_and_setter_nonconfigurable_enumerable.js")]
    public Task hasown_inherited_getter_and_setter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_inherited_getter_and_setter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_getter_and_setter_nonconfigurable_nonenumerable.js")]
    public Task hasown_inherited_getter_and_setter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_getter_and_setter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_getter_configurable_nonenumerable.js")]
    public Task hasown_inherited_getter_configurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_getter_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_getter_nonconfigurable_enumerable.js")]
    public Task hasown_inherited_getter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_inherited_getter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_getter_nonconfigurable_nonenumerable.js")]
    public Task hasown_inherited_getter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_getter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_nonwritable_nonconfigurable_enumerable.js")]
    public Task hasown_inherited_nonwritable_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_inherited_nonwritable_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_nonwritable_nonconfigurable_nonenumerable.js")]
    public Task hasown_inherited_nonwritable_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_nonwritable_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_setter_configurable_enumerable.js")]
    public Task hasown_inherited_setter_configurable_enumerable() => ExecutionTestFromFile("hasown_inherited_setter_configurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_setter_configurable_nonenumerable.js")]
    public Task hasown_inherited_setter_configurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_setter_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_setter_nonconfigurable_enumerable.js")]
    public Task hasown_inherited_setter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_inherited_setter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_setter_nonconfigurable_nonenumerable.js")]
    public Task hasown_inherited_setter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_setter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_writable_nonconfigurable_enumerable.js")]
    public Task hasown_inherited_writable_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_inherited_writable_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_writable_nonconfigurable_nonenumerable.js")]
    public Task hasown_inherited_writable_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_inherited_writable_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_getter_and_setter_configurable_enumerable.js")]
    public Task hasown_own_getter_and_setter_configurable_enumerable() => ExecutionTestFromFile("hasown_own_getter_and_setter_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_getter_and_setter_configurable_nonenumerable.js")]
    public Task hasown_own_getter_and_setter_configurable_nonenumerable() => ExecutionTestFromFile("hasown_own_getter_and_setter_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_getter_and_setter_nonconfigurable_enumerable.js")]
    public Task hasown_own_getter_and_setter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_own_getter_and_setter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_own_getter_and_setter_nonconfigurable_nonenumerable.js")]
    public Task hasown_own_getter_and_setter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_own_getter_and_setter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_getter_nonconfigurable_enumerable.js")]
    public Task hasown_own_getter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_own_getter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_own_getter_nonconfigurable_nonenumerable.js")]
    public Task hasown_own_getter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_own_getter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_nonwritable_nonconfigurable_enumerable.js")]
    public Task hasown_own_nonwritable_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_own_nonwritable_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_own_nonwriteable_nonconfigurable_nonenumerable.js")]
    public Task hasown_own_nonwriteable_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_own_nonwriteable_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_setter_nonconfigurable_enumerable.js")]
    public Task hasown_own_setter_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_own_setter_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_own_setter_nonconfigurable_nonenumerable.js")]
    public Task hasown_own_setter_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_own_setter_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_writable_nonconfigurable_enumerable.js")]
    public Task hasown_own_writable_nonconfigurable_enumerable() => ExecutionTestFromFile("hasown_own_writable_nonconfigurable_enumerable");

    [Fact(DisplayName = "hasown_own_writable_nonconfigurable_nonenumerable.js")]
    public Task hasown_own_writable_nonconfigurable_nonenumerable() => ExecutionTestFromFile("hasown_own_writable_nonconfigurable_nonenumerable");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prototype.js")]
    public Task prototype() => ExecutionTestFromFile("prototype");

    [Fact(DisplayName = "symbol_own_property.js")]
    public Task symbol_own_property() => ExecutionTestFromFile("symbol_own_property");

    [Fact(DisplayName = "toobject_before_topropertykey.js")]
    public Task toobject_before_topropertykey() => ExecutionTestFromFile("toobject_before_topropertykey");

    [Fact(DisplayName = "toobject_null.js")]
    public Task toobject_null() => ExecutionTestFromFile("toobject_null");

    [Fact(DisplayName = "toobject_undefined.js")]
    public Task toobject_undefined() => ExecutionTestFromFile("toobject_undefined");
}
