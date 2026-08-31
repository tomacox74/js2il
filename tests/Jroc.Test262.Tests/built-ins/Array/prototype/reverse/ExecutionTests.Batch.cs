namespace Jroc.Test262.Tests.built_ins.Array.prototype.reverse;

public partial class ExecutionTests
{
    [Fact(DisplayName = "array-has-one-entry")]
    public Task array_has_one_entry() => ExecutionTestFromFile("array-has-one-entry");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "S15.4.4.8_A1_T2")]
    public Task S15_4_4_8_A1_T2() => ExecutionTestFromFile("S15.4.4.8_A1_T2");
}
