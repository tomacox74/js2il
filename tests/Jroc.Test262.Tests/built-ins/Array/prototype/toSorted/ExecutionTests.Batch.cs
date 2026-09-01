namespace Jroc.Test262.Tests.built_ins.Array.prototype.toSorted;

public partial class ExecutionTests
{
    [Fact(DisplayName = "comparefn-controls-sort")]
    public Task comparefn_controls_sort() => ExecutionTestFromFile("comparefn-controls-sort");

    [Fact(DisplayName = "comparefn-default")]
    public Task comparefn_default() => ExecutionTestFromFile("comparefn-default");

    [Fact(DisplayName = "holes-not-preserved")]
    public Task holes_not_preserved() => ExecutionTestFromFile("holes-not-preserved");

    [Fact(DisplayName = "ignores-species")]
    public Task ignores_species() => ExecutionTestFromFile("ignores-species");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "this-value-boolean")]
    public Task this_value_boolean() => ExecutionTestFromFile("this-value-boolean");

    [Fact(DisplayName = "this-value-nullish")]
    public Task this_value_nullish() => ExecutionTestFromFile("this-value-nullish");

    [Fact(DisplayName = "zero-or-one-element")]
    public Task zero_or_one_element() => ExecutionTestFromFile("zero-or-one-element");
}
