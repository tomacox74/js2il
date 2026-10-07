namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.NativeError;

public sealed class ConstructorCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructorCompletionExecutionTests() : base("Class.ConstructorCompletion") { }

    [Fact(DisplayName = "EvalError-super.js")]
    public Task EvalError_super()
        => ExecutionTestFromFile("EvalError-super");

    [Fact(DisplayName = "RangeError-super.js")]
    public Task RangeError_super()
        => ExecutionTestFromFile("RangeError-super");

    [Fact(DisplayName = "ReferenceError-super.js")]
    public Task ReferenceError_super()
        => ExecutionTestFromFile("ReferenceError-super");

    [Fact(DisplayName = "SyntaxError-super.js")]
    public Task SyntaxError_super()
        => ExecutionTestFromFile("SyntaxError-super");

    [Fact(DisplayName = "TypeError-super.js")]
    public Task TypeError_super()
        => ExecutionTestFromFile("TypeError-super");

    [Fact(DisplayName = "URIError-super.js")]
    public Task URIError_super()
        => ExecutionTestFromFile("URIError-super");

}
