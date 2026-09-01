namespace Jroc.Test262.Tests.built_ins.String.prototype.toLocaleLowerCase;

public partial class ExecutionTests
{
    [Fact(DisplayName = "S15.5.4.17_A7.js")]
    public Task S15_5_4_17_A7() => ExecutionTestFromFile("S15.5.4.17_A7");
    [Fact(DisplayName = "S15.5.4.17_A8.js")]
    public Task S15_5_4_17_A8() => ExecutionTestFromFile("S15.5.4.17_A8");
    [Fact(DisplayName = "S15.5.4.17_A9.js")]
    public Task S15_5_4_17_A9() => ExecutionTestFromFile("S15.5.4.17_A9");
    [Fact(DisplayName = "special_casing.js")]
    public Task special_casing() => ExecutionTestFromFile("special_casing");
    [Fact(DisplayName = "supplementary_plane.js")]
    public Task supplementary_plane() => ExecutionTestFromFile("supplementary_plane");
}
