using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.startsWith;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.startsWith") { }

    [Fact(DisplayName = "coerced-values-of-position")] public Task coerced_values_of_position() => ExecutionTestFromFile("coerced-values-of-position");
    [Fact(DisplayName = "length")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "out-of-bounds-position")] public Task out_of_bounds_position() => ExecutionTestFromFile("out-of-bounds-position");
    [Fact(DisplayName = "return-abrupt-from-position-as-symbol")] public Task return_abrupt_from_position_as_symbol() => ExecutionTestFromFile("return-abrupt-from-position-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-position")] public Task return_abrupt_from_position() => ExecutionTestFromFile("return-abrupt-from-position");
    [Fact(DisplayName = "return-abrupt-from-searchstring-as-symbol")] public Task return_abrupt_from_searchstring_as_symbol() => ExecutionTestFromFile("return-abrupt-from-searchstring-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-searchstring-regexp-test")] public Task return_abrupt_from_searchstring_regexp_test() => ExecutionTestFromFile("return-abrupt-from-searchstring-regexp-test");
    [Fact(DisplayName = "return-abrupt-from-searchstring")] public Task return_abrupt_from_searchstring() => ExecutionTestFromFile("return-abrupt-from-searchstring");
    [Fact(DisplayName = "return-abrupt-from-this-as-symbol")] public Task return_abrupt_from_this_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this")] public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "return-true-if-searchstring-is-empty")] public Task return_true_if_searchstring_is_empty() => ExecutionTestFromFile("return-true-if-searchstring-is-empty");
    [Fact(DisplayName = "searchstring-found-with-position")] public Task searchstring_found_with_position() => ExecutionTestFromFile("searchstring-found-with-position");
    [Fact(DisplayName = "searchstring-found-without-position")] public Task searchstring_found_without_position() => ExecutionTestFromFile("searchstring-found-without-position");
    [Fact(DisplayName = "searchstring-is-regexp-throws")] public Task searchstring_is_regexp_throws() => ExecutionTestFromFile("searchstring-is-regexp-throws");
    [Fact(DisplayName = "searchstring-not-found-with-position")] public Task searchstring_not_found_with_position() => ExecutionTestFromFile("searchstring-not-found-with-position");
    [Fact(DisplayName = "searchstring-not-found-without-position")] public Task searchstring_not_found_without_position() => ExecutionTestFromFile("searchstring-not-found-without-position");
    [Fact(DisplayName = "this-is-null-throws")] public Task this_is_null_throws() => ExecutionTestFromFile("this-is-null-throws");
    [Fact(DisplayName = "this-is-undefined-throws")] public Task this_is_undefined_throws() => ExecutionTestFromFile("this-is-undefined-throws");
}
