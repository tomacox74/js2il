using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.raw;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.raw") { }

    [Fact(DisplayName = "nextkey-is-symbol-throws.js")]
    public Task nextkey_is_symbol_throws()
        => ExecutionTestFromFile("nextkey-is-symbol-throws");

    [Fact(DisplayName = "return-the-string-value.js")]
    public Task return_the_string_value()
        => ExecutionTestFromFile("return-the-string-value");

    [Fact(DisplayName = "returns-abrupt-from-next-key-toString.js")]
    public Task returns_abrupt_from_next_key_toString()
        => ExecutionTestFromFile("returns-abrupt-from-next-key-toString");

    [Fact(DisplayName = "returns-abrupt-from-next-key.js")]
    public Task returns_abrupt_from_next_key()
        => ExecutionTestFromFile("returns-abrupt-from-next-key");

    [Fact(DisplayName = "returns-abrupt-from-substitution-symbol.js")]
    public Task returns_abrupt_from_substitution_symbol()
        => ExecutionTestFromFile("returns-abrupt-from-substitution-symbol");

    [Fact(DisplayName = "substitutions-are-appended-on-same-index.js")]
    public Task substitutions_are_appended_on_same_index()
        => ExecutionTestFromFile("substitutions-are-appended-on-same-index");

    [Fact(DisplayName = "template-length-is-symbol-throws.js")]
    public Task template_length_is_symbol_throws()
        => ExecutionTestFromFile("template-length-is-symbol-throws");

    [Fact(DisplayName = "template-length-throws.js")]
    public Task template_length_throws()
        => ExecutionTestFromFile("template-length-throws");
}
