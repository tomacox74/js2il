using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.Symbol.toPrimitive;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Symbol.prototype.Symbol.toPrimitive") { }

    [Fact(DisplayName = "this-val-obj-non-symbol-wrapper")]
    public Task this_val_obj_non_symbol_wrapper() => ExecutionTestFromFile("this-val-obj-non-symbol-wrapper");

    [Fact(DisplayName = "this-val-symbol")]
    public Task this_val_symbol() => ExecutionTestFromFile("this-val-symbol");
}
