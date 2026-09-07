using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.boolean;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/boolean", "language.literals.boolean") { }

    [Fact(DisplayName = "false-with-unicode.js")]
    public Task false_with_unicode()
        => CompilationFailureTest("false-with-unicode");

    [Fact(DisplayName = "true-with-unicode.js")]
    public Task true_with_unicode()
        => CompilationFailureTest("true-with-unicode");

}
