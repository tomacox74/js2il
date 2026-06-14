using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.hasOwn;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.hasOwn") { }

    [Fact(DisplayName = "hasown")]
    public Task hasown()
        => ExecutionTestFromFile("hasown");

    [Fact(DisplayName = "hasown_nonexistent")]
    public Task hasown_nonexistent()
        => ExecutionTestFromFile("hasown_nonexistent");

    [Fact(DisplayName = "hasown_inherited_exists")]
    public Task hasown_inherited_exists()
        => ExecutionTestFromFile("hasown_inherited_exists");

    [Fact(DisplayName = "hasown_inherited_getter")]
    public Task hasown_inherited_getter()
        => ExecutionTestFromFile("hasown_inherited_getter");

    [Fact(DisplayName = "hasown_inherited_getter_and_setter")]
    public Task hasown_inherited_getter_and_setter()
        => ExecutionTestFromFile("hasown_inherited_getter_and_setter");

    [Fact(DisplayName = "hasown_inherited_nonwritable_configurable_enumerable")]
    public Task hasown_inherited_nonwritable_configurable_enumerable()
        => ExecutionTestFromFile("hasown_inherited_nonwritable_configurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_nonwritable_configurable_nonenumerable")]
    public Task hasown_inherited_nonwritable_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_inherited_nonwritable_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_writable_configurable_enumerable")]
    public Task hasown_inherited_writable_configurable_enumerable()
        => ExecutionTestFromFile("hasown_inherited_writable_configurable_enumerable");

    [Fact(DisplayName = "hasown_inherited_writable_configurable_nonenumerable")]
    public Task hasown_inherited_writable_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_inherited_writable_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_inherited_setter")]
    public Task hasown_inherited_setter()
        => ExecutionTestFromFile("hasown_inherited_setter");

    [Fact(DisplayName = "hasown_inherited_getter_configurable_enumerable")]
    public Task hasown_inherited_getter_configurable_enumerable()
        => ExecutionTestFromFile("hasown_inherited_getter_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_property_exists")]
    public Task hasown_own_property_exists()
        => ExecutionTestFromFile("hasown_own_property_exists");

    [Fact(DisplayName = "hasown_own_getter")]
    public Task hasown_own_getter()
        => ExecutionTestFromFile("hasown_own_getter");

    [Fact(DisplayName = "hasown_own_getter_and_setter")]
    public Task hasown_own_getter_and_setter()
        => ExecutionTestFromFile("hasown_own_getter_and_setter");

    [Fact(DisplayName = "hasown_own_nonwritable_configurable_enumerable")]
    public Task hasown_own_nonwritable_configurable_enumerable()
        => ExecutionTestFromFile("hasown_own_nonwritable_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_nonwriteable_configurable_nonenumerable")]
    public Task hasown_own_nonwriteable_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_own_nonwriteable_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_writable_configurable_enumerable")]
    public Task hasown_own_writable_configurable_enumerable()
        => ExecutionTestFromFile("hasown_own_writable_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_writable_configurable_nonenumerable")]
    public Task hasown_own_writable_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_own_writable_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_setter")]
    public Task hasown_own_setter()
        => ExecutionTestFromFile("hasown_own_setter");

    [Fact(DisplayName = "hasown_own_getter_configurable_enumerable")]
    public Task hasown_own_getter_configurable_enumerable()
        => ExecutionTestFromFile("hasown_own_getter_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_getter_configurable_nonenumerable")]
    public Task hasown_own_getter_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_own_getter_configurable_nonenumerable");

    [Fact(DisplayName = "hasown_own_setter_configurable_enumerable")]
    public Task hasown_own_setter_configurable_enumerable()
        => ExecutionTestFromFile("hasown_own_setter_configurable_enumerable");

    [Fact(DisplayName = "hasown_own_setter_configurable_nonenumerable")]
    public Task hasown_own_setter_configurable_nonenumerable()
        => ExecutionTestFromFile("hasown_own_setter_configurable_nonenumerable");

}
