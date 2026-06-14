using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.literals.string_;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\literals\string", "language.literals.string_") { }

    [Fact(DisplayName = "S7.8.4_A1.1_T1")]
    public Task S7_8_4_A1_1_T1()
        => CompilationFailureTest("S7.8.4_A1.1_T1");

    [Fact(DisplayName = "S7.8.4_A1.2_T1")]
    public Task S7_8_4_A1_2_T1()
        => CompilationFailureTest("S7.8.4_A1.2_T1");

}
