using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainYearMonth.compare;

public sealed class NativePortBatch_c9ff873a_2c94_531e_a278_b041ee41b95a : DiskExecutionTestsBase
{
    public NativePortBatch_c9ff873a_2c94_531e_a278_b041ee41b95a() : base("Jroc.Test262.Tests.built_ins.Temporal.PlainYearMonth.compare") { }

    [Fact(DisplayName = "argument-number")]
    public Task argument_number() => ExecutionTestFromFile("argument-number");

}
