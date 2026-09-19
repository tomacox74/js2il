using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.asUintN;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.BigInt.asUintN") { }

    [Fact(DisplayName = "bigint-tobigint-toprimitive")]
    public Task bigint_tobigint_toprimitive()
        => ExecutionTestFromFile("bigint-tobigint-toprimitive");

    [Fact(DisplayName = "bits-toindex-errors")]
    public Task bits_toindex_errors()
        => ExecutionTestFromFile("bits-toindex-errors");

    [Fact(DisplayName = "bits-toindex-toprimitive")]
    public Task bits_toindex_toprimitive()
        => ExecutionTestFromFile("bits-toindex-toprimitive");

}
