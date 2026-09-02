using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.join;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype.join") { }

    [Fact(DisplayName = "S15.4.4.5_A3.1_T1.js")]
    public Task S15_4_4_5_A3_1_T1() => ExecutionTestFromFile("S15.4.4.5_A3.1_T1");

    [Fact(DisplayName = "S15.4.4.5_A3.1_T2.js")]
    public Task S15_4_4_5_A3_1_T2() => ExecutionTestFromFile("S15.4.4.5_A3.1_T2");

    [Fact(DisplayName = "S15.4.4.5_A3.2_T2.js")]
    public Task S15_4_4_5_A3_2_T2() => ExecutionTestFromFile("S15.4.4.5_A3.2_T2");

    [Fact(DisplayName = "S15.4.4.5_A6.6.js")]
    public Task S15_4_4_5_A6_6() => ExecutionTestFromFile("S15.4.4.5_A6.6");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
