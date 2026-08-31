namespace Jroc.Test262.Tests.built_ins.String.prototype.toLocaleUpperCase;

public partial class ExecutionTests
{
    [Fact(DisplayName = "S15.5.4.19_A7.js")]
    public Task S15_5_4_19_A7() => ExecutionTestFromFile("S15.5.4.19_A7");
    [Fact(DisplayName = "S15.5.4.19_A8.js")]
    public Task S15_5_4_19_A8() => ExecutionTestFromFile("S15.5.4.19_A8");
    [Fact(DisplayName = "S15.5.4.19_A9.js")]
    public Task S15_5_4_19_A9() => ExecutionTestFromFile("S15.5.4.19_A9");
    [Fact(DisplayName = "supplementary_plane.js")]
    public Task supplementary_plane() => ExecutionTestFromFile("supplementary_plane");
}
