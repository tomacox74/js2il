namespace Jroc.Test262.Tests.built_ins.String.prototype.replaceAll;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "replaceAll.js")]
    public Task replaceAll() => ExecutionTestFromFile("replaceAll");
    [Fact(DisplayName = "replaceValue-call-skip-no-match.js")]
    public Task replaceValue_call_skip_no_match() => ExecutionTestFromFile("replaceValue-call-skip-no-match");
    [Fact(DisplayName = "replaceValue-tostring-abrupt.js")]
    public Task replaceValue_tostring_abrupt() => ExecutionTestFromFile("replaceValue-tostring-abrupt");
    [Fact(DisplayName = "replaceValue-value-replaces-string.js")]
    public Task replaceValue_value_replaces_string() => ExecutionTestFromFile("replaceValue-value-replaces-string");
    [Fact(DisplayName = "replaceValue-value-tostring.js")]
    public Task replaceValue_value_tostring() => ExecutionTestFromFile("replaceValue-value-tostring");
    [Fact(DisplayName = "searchValue-empty-string-this-empty-string.js")]
    public Task searchValue_empty_string_this_empty_string() => ExecutionTestFromFile("searchValue-empty-string-this-empty-string");
    [Fact(DisplayName = "searchValue-empty-string.js")]
    public Task searchValue_empty_string() => ExecutionTestFromFile("searchValue-empty-string");
    [Fact(DisplayName = "searchValue-tostring-abrupt.js")]
    public Task searchValue_tostring_abrupt() => ExecutionTestFromFile("searchValue-tostring-abrupt");
    [Fact(DisplayName = "searchValue-tostring-regexp.js")]
    public Task searchValue_tostring_regexp() => ExecutionTestFromFile("searchValue-tostring-regexp");
    [Fact(DisplayName = "this-is-null-throws.js")]
    public Task this_is_null_throws() => ExecutionTestFromFile("this-is-null-throws");
    [Fact(DisplayName = "this-is-undefined-throws.js")]
    public Task this_is_undefined_throws() => ExecutionTestFromFile("this-is-undefined-throws");
    [Fact(DisplayName = "this-tostring-abrupt.js")]
    public Task this_tostring_abrupt() => ExecutionTestFromFile("this-tostring-abrupt");
    [Fact(DisplayName = "this-tostring.js")]
    public Task this_tostring() => ExecutionTestFromFile("this-tostring");
}
