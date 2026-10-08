using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.DisposableStack;

public sealed class NativePortBatch_Test_5583f6ad_630a_5ee6_b8ac_0041fd606eab : DiskExecutionTestsBase
{
    public NativePortBatch_Test_5583f6ad_630a_5ee6_b8ac_0041fd606eab() : base("Jroc.Test262.Tests.built_ins.DisposableStack") { }

    [Fact(DisplayName = "proto-from-ctor-realm")]
    public Task proto_from_ctor_realm() => ExecutionTestFromFile("proto-from-ctor-realm");

}
