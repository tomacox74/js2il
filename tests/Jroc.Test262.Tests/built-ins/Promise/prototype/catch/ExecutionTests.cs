using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.catch_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.prototype.catch") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S25.4.5.1_A3.1_T1")]
    public Task S25_4_5_1_A3_1_T1()
        => ExecutionTestFromFile("S25.4.5.1_A3.1_T1");

    [Fact(DisplayName = "S25.4.5.1_A3.1_T2")]
    public Task S25_4_5_1_A3_1_T2()
        => ExecutionTestFromFile("S25.4.5.1_A3.1_T2");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "this-value-non-object")]
    public Task this_value_non_object()
        => ExecutionTestFromFile("this-value-non-object");

    [Fact(DisplayName = "invokes-then")]
    public Task invokes_then()
        => ExecutionTestFromFile("invokes-then");

    [Fact(DisplayName = "this-value-obj-coercible")]
    public Task this_value_obj_coercible()
        => ExecutionTestFromFile("this-value-obj-coercible");

    [Fact(DisplayName = "this-value-then-poisoned")]
    public Task this_value_then_poisoned()
        => ExecutionTestFromFile("this-value-then-poisoned");

    [Fact(DisplayName = "this-value-then-throws")]
    public Task this_value_then_throws()
        => ExecutionTestFromFile("this-value-then-throws");
}
