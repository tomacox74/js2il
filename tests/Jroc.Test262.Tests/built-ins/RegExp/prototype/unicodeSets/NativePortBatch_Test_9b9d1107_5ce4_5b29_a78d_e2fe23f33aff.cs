using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.unicodeSets;

public sealed class NativePortBatch_Test_9b9d1107_5ce4_5b29_a78d_e2fe23f33aff : DiskExecutionTestsBase
{
    public NativePortBatch_Test_9b9d1107_5ce4_5b29_a78d_e2fe23f33aff() : base("Jroc.Test262.Tests.built_ins.RegExp.prototype.unicodeSets") { }

    [Fact(DisplayName = "cross-realm")]
    public Task cross_realm() => ExecutionTestFromFile("cross-realm");

    [Fact(DisplayName = "this-val-regexp")]
    public Task this_val_regexp() => ExecutionTestFromFile("this-val-regexp");

}
