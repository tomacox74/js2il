using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.copyWithin;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.copyWithin") { }


    [Fact(DisplayName = "negative-end")]
    public Task negative_end()
        => ExecutionTestFromFile("negative-end");

    [Fact(DisplayName = "negative-out-of-bounds-end")]
    public Task negative_out_of_bounds_end()
        => ExecutionTestFromFile("negative-out-of-bounds-end");

    [Fact(DisplayName = "negative-out-of-bounds-start")]
    public Task negative_out_of_bounds_start()
        => ExecutionTestFromFile("negative-out-of-bounds-start");

    [Fact(DisplayName = "negative-out-of-bounds-target")]
    public Task negative_out_of_bounds_target()
        => ExecutionTestFromFile("negative-out-of-bounds-target");

    [Fact(DisplayName = "negative-start")]
    public Task negative_start()
        => ExecutionTestFromFile("negative-start");

    [Fact(DisplayName = "negative-target")]
    public Task negative_target()
        => ExecutionTestFromFile("negative-target");

    [Fact(DisplayName = "non-negative-out-of-bounds-end")]
    public Task non_negative_out_of_bounds_end()
        => ExecutionTestFromFile("non-negative-out-of-bounds-end");

    [Fact(DisplayName = "non-negative-out-of-bounds-target-and-start")]
    public Task non_negative_out_of_bounds_target_and_start()
        => ExecutionTestFromFile("non-negative-out-of-bounds-target-and-start");

    [Fact(DisplayName = "non-negative-target-and-start")]
    public Task non_negative_target_and_start()
        => ExecutionTestFromFile("non-negative-target-and-start");

    [Fact(DisplayName = "non-negative-target-start-and-end")]
    public Task non_negative_target_start_and_end()
        => ExecutionTestFromFile("non-negative-target-start-and-end");


    [Fact(DisplayName = "undefined-end")]
    public Task undefined_end()
        => ExecutionTestFromFile("undefined-end");

}
