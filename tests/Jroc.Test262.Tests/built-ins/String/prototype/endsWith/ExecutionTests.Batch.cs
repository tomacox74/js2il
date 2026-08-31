namespace Jroc.Test262.Tests.built_ins.String.prototype.endsWith;

public partial class ExecutionTests
{
    [Fact(DisplayName = "String.prototype.endsWith_Fail.js")]
    public Task String_prototype_endsWith_Fail() => ExecutionTestFromFile("String.prototype.endsWith_Fail");
    [Fact(DisplayName = "String.prototype.endsWith_Fail_2.js")]
    public Task String_prototype_endsWith_Fail_2() => ExecutionTestFromFile("String.prototype.endsWith_Fail_2");
    [Fact(DisplayName = "String.prototype.endsWith_Success.js")]
    public Task String_prototype_endsWith_Success() => ExecutionTestFromFile("String.prototype.endsWith_Success");
    [Fact(DisplayName = "String.prototype.endsWith_Success_2.js")]
    public Task String_prototype_endsWith_Success_2() => ExecutionTestFromFile("String.prototype.endsWith_Success_2");
    [Fact(DisplayName = "String.prototype.endsWith_Success_3.js")]
    public Task String_prototype_endsWith_Success_3() => ExecutionTestFromFile("String.prototype.endsWith_Success_3");
    [Fact(DisplayName = "String.prototype.endsWith_Success_4.js")]
    public Task String_prototype_endsWith_Success_4() => ExecutionTestFromFile("String.prototype.endsWith_Success_4");
    [Fact(DisplayName = "endsWith.js")]
    public Task endsWith() => ExecutionTestFromFile("endsWith");
}
