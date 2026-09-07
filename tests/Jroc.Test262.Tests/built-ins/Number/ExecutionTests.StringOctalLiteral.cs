namespace Jroc.Test262.Tests.built_ins.Number;

public partial class ExecutionTests
{
    [Fact(DisplayName = "string-octal-literal.js")]
    public Task string_octal_literal()
        => ExecutionTestFromFile("string-octal-literal");
}
