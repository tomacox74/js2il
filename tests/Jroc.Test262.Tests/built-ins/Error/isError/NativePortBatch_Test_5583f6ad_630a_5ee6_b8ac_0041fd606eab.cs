using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Error.isError;

public sealed class NativePortBatch_Test_5583f6ad_630a_5ee6_b8ac_0041fd606eab : DiskExecutionTestsBase
{
    public NativePortBatch_Test_5583f6ad_630a_5ee6_b8ac_0041fd606eab() : base("Jroc.Test262.Tests.built_ins.Error.isError") { }

    [Fact(DisplayName = "errors-other-realm")]
    public Task errors_other_realm() => ExecutionTestFromFile("errors-other-realm");

}
