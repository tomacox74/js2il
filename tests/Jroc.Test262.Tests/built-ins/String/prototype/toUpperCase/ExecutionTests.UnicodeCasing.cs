namespace Jroc.Test262.Tests.built_ins.String.prototype.toUpperCase;

public partial class ExecutionTests
{
    [Fact(DisplayName = "special_casing.js")]
    public Task special_casing() => ExecutionTestFromFile("special_casing");
}
