using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.JSON.rawJSON;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.rawJSON") { }

    [Fact(DisplayName = "basic.js")]
    public Task basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "bigint-raw-json-can-be-stringified.js")]
    public Task bigint_raw_json_can_be_stringified()
        => ExecutionTestFromFile("bigint-raw-json-can-be-stringified");

    [Fact(DisplayName = "builtin.js")]
    public Task builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "illegal-empty-and-start-end-chars.js")]
    public Task illegal_empty_and_start_end_chars()
        => ExecutionTestFromFile("illegal-empty-and-start-end-chars");

    [Fact(DisplayName = "invalid-JSON-text.js")]
    public Task invalid_JSON_text() => ExecutionTestFromFile("invalid-JSON-text");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "returns-expected-object.js")]
    public Task returns_expected_object() => ExecutionTestFromFile("returns-expected-object");
}
