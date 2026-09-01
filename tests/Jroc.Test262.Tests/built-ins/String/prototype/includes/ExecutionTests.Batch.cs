namespace Jroc.Test262.Tests.built_ins.String.prototype.includes;

public partial class ExecutionTests
{
    [Fact(DisplayName = "String.prototype.includes_FailBadLocation.js")]
    public Task String_prototype_includes_FailBadLocation() => ExecutionTestFromFile("String.prototype.includes_FailBadLocation");
    [Fact(DisplayName = "String.prototype.includes_FailLocation.js")]
    public Task String_prototype_includes_FailLocation() => ExecutionTestFromFile("String.prototype.includes_FailLocation");
    [Fact(DisplayName = "String.prototype.includes_FailMissingLetter.js")]
    public Task String_prototype_includes_FailMissingLetter() => ExecutionTestFromFile("String.prototype.includes_FailMissingLetter");
    [Fact(DisplayName = "String.prototype.includes_Success.js")]
    public Task String_prototype_includes_Success() => ExecutionTestFromFile("String.prototype.includes_Success");
    [Fact(DisplayName = "String.prototype.includes_SuccessNoLocation.js")]
    public Task String_prototype_includes_SuccessNoLocation() => ExecutionTestFromFile("String.prototype.includes_SuccessNoLocation");
    [Fact(DisplayName = "String.prototype.includes_lengthProp.js")]
    public Task String_prototype_includes_lengthProp() => ExecutionTestFromFile("String.prototype.includes_lengthProp");
    [Fact(DisplayName = "includes.js")]
    public Task includes() => ExecutionTestFromFile("includes");
}
