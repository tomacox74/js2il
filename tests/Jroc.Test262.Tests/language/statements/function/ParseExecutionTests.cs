using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.statements.function_;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\statements\function", "language.statements.function_") { }

    [Fact(DisplayName = "13.0_4-5gs")]
    public Task _13_0_4_5gs()
        => CompilationFailureTest("13.0_4-5gs");

}
