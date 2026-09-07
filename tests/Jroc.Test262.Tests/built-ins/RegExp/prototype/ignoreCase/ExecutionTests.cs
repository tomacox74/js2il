using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.ignoreCase;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.ignoreCase") { }

    [Fact(DisplayName = "15.10.7.3-2.js")]
    public Task _15_10_7_3_2()
        => ExecutionTestFromFile("15.10.7.3-2");

    [Fact(DisplayName = "S15.10.7.3_A10.js")]
    public Task S15_10_7_3_A10()
        => ExecutionTestFromFile("S15.10.7.3_A10");

    [Fact(DisplayName = "S15.10.7.3_A8.js")]
    public Task S15_10_7_3_A8()
        => ExecutionTestFromFile("S15.10.7.3_A8");

    [Fact(DisplayName = "S15.10.7.3_A9.js")]
    public Task S15_10_7_3_A9()
        => ExecutionTestFromFile("S15.10.7.3_A9");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "this-val-invalid-obj.js")]
    public Task this_val_invalid_obj()
        => ExecutionTestFromFile("this-val-invalid-obj");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

}
