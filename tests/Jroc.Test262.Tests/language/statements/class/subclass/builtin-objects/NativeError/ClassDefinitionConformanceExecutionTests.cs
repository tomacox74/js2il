namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.NativeError;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.NativeError") { }

    [Fact(DisplayName = "EvalError-message.js")]
    public Task EvalError_message()
        => ExecutionTest("EvalError-message");

    [Fact(DisplayName = "EvalError-name.js")]
    public Task EvalError_name()
        => ExecutionTest("EvalError-name");

    [Fact(DisplayName = "RangeError-message.js")]
    public Task RangeError_message()
        => ExecutionTest("RangeError-message");

    [Fact(DisplayName = "RangeError-name.js")]
    public Task RangeError_name()
        => ExecutionTest("RangeError-name");

    [Fact(DisplayName = "ReferenceError-message.js")]
    public Task ReferenceError_message()
        => ExecutionTest("ReferenceError-message");

    [Fact(DisplayName = "ReferenceError-name.js")]
    public Task ReferenceError_name()
        => ExecutionTest("ReferenceError-name");

    [Fact(DisplayName = "SyntaxError-message.js")]
    public Task SyntaxError_message()
        => ExecutionTest("SyntaxError-message");

    [Fact(DisplayName = "SyntaxError-name.js")]
    public Task SyntaxError_name()
        => ExecutionTest("SyntaxError-name");

    [Fact(DisplayName = "TypeError-message.js")]
    public Task TypeError_message()
        => ExecutionTest("TypeError-message");

    [Fact(DisplayName = "TypeError-name.js")]
    public Task TypeError_name()
        => ExecutionTest("TypeError-name");

    [Fact(DisplayName = "URIError-message.js")]
    public Task URIError_message()
        => ExecutionTest("URIError-message");

    [Fact(DisplayName = "URIError-name.js")]
    public Task URIError_name()
        => ExecutionTest("URIError-name");
}
