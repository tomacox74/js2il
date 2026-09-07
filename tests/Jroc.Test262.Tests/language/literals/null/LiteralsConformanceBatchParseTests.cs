using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.@null;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/null", "language.literals.null") { }

    [Fact(DisplayName = "null-with-unicode.js")]
    public Task null_with_unicode()
        => CompilationFailureTest("null-with-unicode");

}
