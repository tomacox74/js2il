using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.string_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.literals.string_") { }

    [Fact(DisplayName = "legacy-octal-escape-sequence")]
    public Task legacy_octal_escape_sequence()
        => ExecutionTest("legacy-octal-escape-sequence");
}
