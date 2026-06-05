using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.BigInt") { }

    [Fact(DisplayName = "constructor-from-binary-string")]
    public Task constructor_from_binary_string()
        => ExecutionTestFromFile("constructor-from-binary-string");

    [Fact(DisplayName = "constructor-from-decimal-string")]
    public Task constructor_from_decimal_string()
        => ExecutionTestFromFile("constructor-from-decimal-string");

    [Fact(DisplayName = "constructor-empty-string")]
    public Task constructor_empty_string()
        => ExecutionTestFromFile("constructor-empty-string");

    [Fact(DisplayName = "constructor-from-hex-string")]
    public Task constructor_from_hex_string()
        => ExecutionTestFromFile("constructor-from-hex-string");

    [Fact(DisplayName = "constructor-from-octal-string")]
    public Task constructor_from_octal_string()
        => ExecutionTestFromFile("constructor-from-octal-string");

    [Fact(DisplayName = "constructor-trailing-leading-spaces")]
    public Task constructor_trailing_leading_spaces()
        => ExecutionTestFromFile("constructor-trailing-leading-spaces");

    [Fact(DisplayName = "constructor-from-string-syntax-errors")]
    public Task constructor_from_string_syntax_errors()
        => ExecutionTestFromFile("constructor-from-string-syntax-errors");

    [Fact(DisplayName = "constructor-coercion")]
    public Task constructor_coercion()
        => ExecutionTestFromFile("constructor-coercion");

    [Fact(DisplayName = "non-integer-rangeerror")]
    public Task non_integer_rangeerror()
        => ExecutionTestFromFile("non-integer-rangeerror");

    [Fact(DisplayName = "tostring-throws")]
    public Task tostring_throws()
        => ExecutionTestFromFile("tostring-throws");
}
