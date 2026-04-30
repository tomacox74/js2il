using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.literals.numeric;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.literals.numeric") { }

    [Fact(DisplayName = "binary")]
    public Task binary()
        => ExecutionTest("binary");

    [Fact(DisplayName = "legacy-octal-integer")]
    public Task legacy_octal_integer()
        => ExecutionTest("legacy-octal-integer");

    [Fact(DisplayName = "non-octal-decimal-integer")]
    public Task non_octal_decimal_integer()
        => ExecutionTest("non-octal-decimal-integer");

    [Fact(DisplayName = "octal")]
    public Task octal()
        => ExecutionTest("octal");
}
