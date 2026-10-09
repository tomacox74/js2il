using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.ArrayIteratorPrototype.next;

public sealed class NativePortBatch_Test_8c13b2ba_4ccf_5ce1_a465_b4aad7a0e645 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_8c13b2ba_4ccf_5ce1_a465_b4aad7a0e645() : base("Jroc.Test262.Tests.built_ins.ArrayIteratorPrototype.next") { }

    [Fact(DisplayName = "args-mapped-expansion-after-exhaustion")]
    public Task args_mapped_expansion_after_exhaustion() => ExecutionTestFromFile("args-mapped-expansion-after-exhaustion");

    [Fact(DisplayName = "args-mapped-iteration")]
    public Task args_mapped_iteration() => ExecutionTestFromFile("args-mapped-iteration");

    [Fact(DisplayName = "args-unmapped-iteration")]
    public Task args_unmapped_iteration() => ExecutionTestFromFile("args-unmapped-iteration");

}
