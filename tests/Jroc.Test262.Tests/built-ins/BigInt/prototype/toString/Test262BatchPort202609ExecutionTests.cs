using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.prototype.toString;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.BigInt.prototype.toString") { }

    [Fact(DisplayName = "a-z")]
    public Task a_z()
        => ExecutionTestFromFile("a-z");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "radix-2-to-36")]
    public Task radix_2_to_36()
        => ExecutionTestFromFile("radix-2-to-36");

    [Fact(DisplayName = "radix-err")]
    public Task radix_err()
        => ExecutionTestFromFile("radix-err");

    [Fact(DisplayName = "radix-tointegerorinfinity-throws-symbol")]
    public Task radix_tointegerorinfinity_throws_symbol()
        => ExecutionTestFromFile("radix-tointegerorinfinity-throws-symbol");

    [Fact(DisplayName = "radix-tointegerorinfinity-throws-toprimitive-or-bigint")]
    public Task radix_tointegerorinfinity_throws_toprimitive_or_bigint()
        => ExecutionTestFromFile("radix-tointegerorinfinity-throws-toprimitive-or-bigint");

    [Fact(DisplayName = "string-is-code-units-of-decimal-digits-only")]
    public Task string_is_code_units_of_decimal_digits_only()
        => ExecutionTestFromFile("string-is-code-units-of-decimal-digits-only");

}
