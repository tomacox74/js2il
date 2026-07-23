using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.repeat;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.repeat") { }

    [Fact(DisplayName = "count-coerced-to-zero-returns-empty-string")] public Task count_coerced_to_zero_returns_empty_string() => ExecutionTestFromFile("count-coerced-to-zero-returns-empty-string");
    [Fact(DisplayName = "count-is-infinity-throws")] public Task count_is_infinity_throws() => ExecutionTestFromFile("count-is-infinity-throws");
    [Fact(DisplayName = "count-is-zero-returns-empty-string")] public Task count_is_zero_returns_empty_string() => ExecutionTestFromFile("count-is-zero-returns-empty-string");
    [Fact(DisplayName = "count-less-than-zero-throws")] public Task count_less_than_zero_throws() => ExecutionTestFromFile("count-less-than-zero-throws");
    [Fact(DisplayName = "empty-string-returns-empty")] public Task empty_string_returns_empty() => ExecutionTestFromFile("empty-string-returns-empty");
    [Fact(DisplayName = "length")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "repeat-string-n-times")] public Task repeat_string_n_times() => ExecutionTestFromFile("repeat-string-n-times");
    [Fact(DisplayName = "repeat")] public Task repeat() => ExecutionTestFromFile("repeat");
    [Fact(DisplayName = "return-abrupt-from-count-as-symbol")] public Task return_abrupt_from_count_as_symbol() => ExecutionTestFromFile("return-abrupt-from-count-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-count")] public Task return_abrupt_from_count() => ExecutionTestFromFile("return-abrupt-from-count");
    [Fact(DisplayName = "return-abrupt-from-this-as-symbol")] public Task return_abrupt_from_this_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this")] public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "this-is-null-throws")] public Task this_is_null_throws() => ExecutionTestFromFile("this-is-null-throws");
    [Fact(DisplayName = "this-is-undefined-throws")] public Task this_is_undefined_throws() => ExecutionTestFromFile("this-is-undefined-throws");
}
