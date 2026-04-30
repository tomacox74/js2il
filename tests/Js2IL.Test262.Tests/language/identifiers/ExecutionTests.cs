using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.identifiers;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.identifiers") { }

    [Fact(DisplayName = "part-digits")]
    public Task part_digits()
        => ExecutionTest("part-digits");

    [Fact(DisplayName = "part-digits-via-escape-hex")]
    public Task part_digits_via_escape_hex()
        => ExecutionTest("part-digits-via-escape-hex");

    [Fact(DisplayName = "part-digits-via-escape-hex4")]
    public Task part_digits_via_escape_hex4()
        => ExecutionTest("part-digits-via-escape-hex4");

    [Fact(DisplayName = "vals-eng-alpha-lower")]
    public Task vals_eng_alpha_lower()
        => ExecutionTest("vals-eng-alpha-lower");

    [Fact(DisplayName = "vals-eng-alpha-upper")]
    public Task vals_eng_alpha_upper()
        => ExecutionTest("vals-eng-alpha-upper");
}
