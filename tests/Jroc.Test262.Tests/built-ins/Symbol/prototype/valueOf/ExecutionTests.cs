using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.valueOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.valueOf") { }

    [Fact(DisplayName = "this-val-symbol")]
    public Task this_val_symbol()
        => ExecutionTestFromFile("this-val-symbol");
}
