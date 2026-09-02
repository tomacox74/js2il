using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.fill.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.fill.BigInt") { }

    [Fact(DisplayName = "coerced-indexes.js")]
    public Task coerced_indexes() => ExecutionTestFromFile("coerced-indexes");

    [Fact(DisplayName = "fill-values-conversion-once.js")]
    public Task fill_values_conversion_once() => ExecutionTestFromFile("fill-values-conversion-once");

    [Fact(DisplayName = "fill-values-custom-start-and-end.js")]
    public Task fill_values_custom_start_and_end() => ExecutionTestFromFile("fill-values-custom-start-and-end");

    [Fact(DisplayName = "fill-values-non-numeric-throw.js")]
    public Task fill_values_non_numeric_throw() => ExecutionTestFromFile("fill-values-non-numeric-throw");

    [Fact(DisplayName = "fill-values-non-numeric.js")]
    public Task fill_values_non_numeric() => ExecutionTestFromFile("fill-values-non-numeric");

    [Fact(DisplayName = "fill-values-relative-end.js")]
    public Task fill_values_relative_end() => ExecutionTestFromFile("fill-values-relative-end");

    [Fact(DisplayName = "fill-values-relative-start.js")]
    public Task fill_values_relative_start() => ExecutionTestFromFile("fill-values-relative-start");

    [Fact(DisplayName = "fill-values-symbol-throws.js")]
    public Task fill_values_symbol_throws() => ExecutionTestFromFile("fill-values-symbol-throws");

    [Fact(DisplayName = "fill-values.js")]
    public Task fill_values() => ExecutionTestFromFile("fill-values");

    [Fact(DisplayName = "get-length-ignores-length-prop.js")]
    public Task get_length_ignores_length_prop() => ExecutionTestFromFile("get-length-ignores-length-prop");

    [Fact(DisplayName = "return-abrupt-from-end-as-symbol.js")]
    public Task return_abrupt_from_end_as_symbol() => ExecutionTestFromFile("return-abrupt-from-end-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-end.js")]
    public Task return_abrupt_from_end() => ExecutionTestFromFile("return-abrupt-from-end");

    [Fact(DisplayName = "return-abrupt-from-set-value.js")]
    public Task return_abrupt_from_set_value() => ExecutionTestFromFile("return-abrupt-from-set-value");

    [Fact(DisplayName = "return-abrupt-from-start-as-symbol.js")]
    public Task return_abrupt_from_start_as_symbol() => ExecutionTestFromFile("return-abrupt-from-start-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-start.js")]
    public Task return_abrupt_from_start() => ExecutionTestFromFile("return-abrupt-from-start");

    [Fact(DisplayName = "return-this.js")]
    public Task return_this() => ExecutionTestFromFile("return-this");

}
