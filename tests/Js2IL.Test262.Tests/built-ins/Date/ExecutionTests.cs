using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Date;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date") { }

    [Fact(DisplayName = "15.9.1.15-1")]
    public Task _15_9_1_15_1()
        => ExecutionTest("15.9.1.15-1");

    [Fact(DisplayName = "coercion-order")]
    public Task coercion_order()
        => ExecutionTest("coercion-order");

    [Fact(DisplayName = "coercion-errors")]
    public Task coercion_errors()
        => ExecutionTest("coercion-errors");

    [Fact(DisplayName = "construct_with_date", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task construct_with_date()
        => ExecutionTest("construct_with_date");

    [Fact(DisplayName = "length", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "S15.9.2.1_A1")]
    public Task S15_9_2_1_A1()
        => ExecutionTest("S15.9.2.1_A1");

}
