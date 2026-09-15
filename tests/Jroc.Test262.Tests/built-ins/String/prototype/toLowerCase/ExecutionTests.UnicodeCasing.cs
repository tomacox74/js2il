namespace Jroc.Test262.Tests.built_ins.String.prototype.toLowerCase;

public partial class ExecutionTests
{
    [Fact(DisplayName = "Final_Sigma_U180E.js")]
    public Task Final_Sigma_U180E() => ExecutionTestFromFile("Final_Sigma_U180E");

    [Fact(DisplayName = "special_casing.js")]
    public Task special_casing() => ExecutionTestFromFile("special_casing");

    [Fact(DisplayName = "special_casing_conditional.js")]
    public Task special_casing_conditional() => ExecutionTestFromFile("special_casing_conditional");
}
