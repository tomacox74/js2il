using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.getOwnPropertyDescriptor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

    [Fact(DisplayName = "resultdesc-is-invalid-descriptor")]
    public Task resultdesc_is_invalid_descriptor()
        => ExecutionTestFromFile("resultdesc-is-invalid-descriptor");

    [Fact(DisplayName = "result-is-undefined-target-is-not-extensible")]
    public Task result_is_undefined_target_is_not_extensible()
        => ExecutionTestFromFile("result-is-undefined-target-is-not-extensible");
}
