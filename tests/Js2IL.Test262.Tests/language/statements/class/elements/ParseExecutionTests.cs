using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.class_.elements;

public class ParseExecutionTests : FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\statements\class\elements", "language.statements.class_.elements") { }

    [Fact(DisplayName = "fields-asi-3")]
    public Task fields_asi_3()
        => CompilationFailureTest("fields-asi-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-asi-4")]
    public Task fields_asi_4()
        => CompilationFailureTest("fields-asi-4", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-asi-same-line-1")]
    public Task fields_asi_same_line_1()
        => CompilationFailureTest("fields-asi-same-line-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "fields-asi-same-line-2")]
    public Task fields_asi_same_line_2()
        => CompilationFailureTest("fields-asi-same-line-2", "Failed to parse JavaScript");
}
