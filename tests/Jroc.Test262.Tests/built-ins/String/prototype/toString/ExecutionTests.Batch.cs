namespace Jroc.Test262.Tests.built_ins.String.prototype.toString;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "string-object.js")]
    public Task string_object() => ExecutionTestFromFile("string-object");
    [Fact(DisplayName = "string-primitive.js")]
    public Task string_primitive() => ExecutionTestFromFile("string-primitive");
}
