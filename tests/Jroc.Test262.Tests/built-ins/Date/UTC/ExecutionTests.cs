using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.UTC;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.UTC") { }

    [Fact(DisplayName = "fp-evaluation-order")]
    public Task fp_evaluation_order()
        => ExecutionTestFromFile("fp-evaluation-order");

    [Fact(DisplayName = "infinity-make-time")]
    public Task infinity_make_time()
        => ExecutionTestFromFile("infinity-make-time");

    [Fact(DisplayName = "non-integer-values")]
    public Task non_integer_values()
        => ExecutionTestFromFile("non-integer-values");

    [Fact(DisplayName = "return-value")]
    public Task return_value()
        => ExecutionTestFromFile("return-value");

    [Fact(DisplayName = "time-clip")]
    public Task time_clip()
        => ExecutionTestFromFile("time-clip");

    [Fact(DisplayName = "year-offset")]
    public Task year_offset()
        => ExecutionTestFromFile("year-offset");
}
