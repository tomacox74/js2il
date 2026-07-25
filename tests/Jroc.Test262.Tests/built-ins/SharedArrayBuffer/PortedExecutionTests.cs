using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.SharedArrayBuffer") { }

    [Fact(DisplayName = "allocation-limit")]
    public Task allocation_limit()
        => ExecutionTest("allocation-limit");

    [Fact(DisplayName = "init-zero")]
    public Task init_zero()
        => ExecutionTest("init-zero");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTest("is-a-constructor");

    [Fact(DisplayName = "length-is-absent")]
    public Task length_is_absent()
        => ExecutionTest("length-is-absent");

    [Fact(DisplayName = "length-is-too-large-throws")]
    public Task length_is_too_large_throws()
        => ExecutionTest("length-is-too-large-throws");

    [Fact(DisplayName = "negative-length-throws")]
    public Task negative_length_throws()
        => ExecutionTest("negative-length-throws");

    [Fact(DisplayName = "return-abrupt-from-length")]
    public Task return_abrupt_from_length()
        => ExecutionTest("return-abrupt-from-length");

    [Fact(DisplayName = "zero-length")]
    public Task zero_length()
        => ExecutionTest("zero-length");

}
