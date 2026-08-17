using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.setPrototypeOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.setPrototypeOf") { }

    [Fact(DisplayName = "o-not-obj-coercible")]
    public Task o_not_obj_coercible()
        => ExecutionTestFromFile("o-not-obj-coercible");
    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");
    [Fact(DisplayName = "proto-not-obj")]
    public Task proto_not_obj()
        => ExecutionTestFromFile("proto-not-obj");
    [Fact(DisplayName = "success")]
    public Task success()
        => ExecutionTestFromFile("success");
    [Fact(DisplayName = "bigint.js")]
    public Task bigint_js()
        => ExecutionTestFromFile("bigint");
    [Fact(DisplayName = "length.js")]
    public Task length_js()
        => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name_js()
        => ExecutionTestFromFile("name");
    [Fact(DisplayName = "o-not-obj.js")]
    public Task o_not_obj_js()
        => ExecutionTestFromFile("o-not-obj");
    [Fact(DisplayName = "set-failure-cycle.js")]
    public Task set_failure_cycle_js()
        => ExecutionTestFromFile("set-failure-cycle");
}
