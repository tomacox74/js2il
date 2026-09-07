namespace Jroc.Test262.Tests.built_ins.Number;

public partial class ExecutionTests
{
    [Fact(DisplayName = "string-binary-literal.js")]
    public Task string_binary_literal()
        => ExecutionTestFromFile("string-binary-literal");
}
