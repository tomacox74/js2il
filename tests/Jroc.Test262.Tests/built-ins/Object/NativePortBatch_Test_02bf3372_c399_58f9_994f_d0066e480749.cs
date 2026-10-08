using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.Object;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.built_ins.Object") { }

    [Fact(DisplayName = "symbol_object-returns-fresh-symbol")]
    public Task symbol_object_returns_fresh_symbol() => ExecutionTestFromFile("symbol_object-returns-fresh-symbol");

}
