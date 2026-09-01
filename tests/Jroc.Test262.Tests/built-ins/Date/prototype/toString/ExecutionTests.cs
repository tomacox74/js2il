using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toString;

public partial class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.toString") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "format")]
    public Task format()
        => ExecutionTestFromFile("format");

    [Fact(DisplayName = "negative-year")]
    public Task negative_year()
        => ExecutionTestFromFile("negative-year");
}
