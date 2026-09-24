using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.class_.accessor_name_inst;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.accessor_name_inst") { }

    [Fact(DisplayName = "computed")]
    public Task computed()
        => ExecutionTest("computed");

    [Fact(DisplayName = "computed-err-evaluation")]
    public Task computed_err_evaluation()
        => ExecutionTest("computed-err-evaluation");

    [Fact(DisplayName = "computed-err-unresolvable")]
    public Task computed_err_unresolvable()
        => ExecutionTest("computed-err-unresolvable");

    [Fact(DisplayName = "literal-numeric-binary.js")]
    public Task ported_literal_numeric_binary() => ExecutionTest("literal-numeric-binary");

    [Fact(DisplayName = "literal-numeric-exponent.js")]
    public Task ported_literal_numeric_exponent() => ExecutionTest("literal-numeric-exponent");

    [Fact(DisplayName = "literal-numeric-hex.js")]
    public Task ported_literal_numeric_hex() => ExecutionTest("literal-numeric-hex");

    [Fact(DisplayName = "literal-numeric-leading-decimal.js")]
    public Task ported_literal_numeric_leading_decimal() => ExecutionTest("literal-numeric-leading-decimal");

    [Fact(DisplayName = "literal-numeric-non-canonical.js")]
    public Task ported_literal_numeric_non_canonical() => ExecutionTest("literal-numeric-non-canonical");

    [Fact(DisplayName = "literal-numeric-octal.js")]
    public Task ported_literal_numeric_octal() => ExecutionTest("literal-numeric-octal");

    [Fact(DisplayName = "literal-numeric-zero.js")]
    public Task ported_literal_numeric_zero() => ExecutionTest("literal-numeric-zero");

    [Fact(DisplayName = "literal-string-char-escape.js")]
    public Task ported_literal_string_char_escape() => ExecutionTest("literal-string-char-escape");

    [Fact(DisplayName = "literal-string-default-escaped-ext.js")]
    public Task ported_literal_string_default_escaped_ext() => ExecutionTest("literal-string-default-escaped-ext");

    [Fact(DisplayName = "literal-string-default-escaped.js")]
    public Task ported_literal_string_default_escaped() => ExecutionTest("literal-string-default-escaped");

    [Fact(DisplayName = "literal-string-default.js")]
    public Task ported_literal_string_default() => ExecutionTest("literal-string-default");

    [Fact(DisplayName = "literal-string-double-quote.js")]
    public Task ported_literal_string_double_quote() => ExecutionTest("literal-string-double-quote");

    [Fact(DisplayName = "literal-string-empty.js")]
    public Task ported_literal_string_empty() => ExecutionTest("literal-string-empty");

    [Fact(DisplayName = "literal-string-hex-escape.js")]
    public Task ported_literal_string_hex_escape() => ExecutionTest("literal-string-hex-escape");

    [Fact(DisplayName = "literal-string-line-continuation.js")]
    public Task ported_literal_string_line_continuation() => ExecutionTest("literal-string-line-continuation");

    [Fact(DisplayName = "literal-string-single-quote.js")]
    public Task ported_literal_string_single_quote() => ExecutionTest("literal-string-single-quote");

    [Fact(DisplayName = "literal-string-unicode-escape.js")]
    public Task ported_literal_string_unicode_escape() => ExecutionTest("literal-string-unicode-escape");
}
