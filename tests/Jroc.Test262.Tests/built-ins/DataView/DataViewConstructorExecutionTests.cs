using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView;

public class DataViewConstructorExecutionTests : DiskExecutionTestsBase
{
    public DataViewConstructorExecutionTests() : base("built_ins.DataView") { }

    [Fact(DisplayName = "toindex-bytelength-sab.js")]
    public Task toindex_bytelength_sab() => ExecutionTestFromFile("toindex-bytelength-sab");

    [Fact(DisplayName = "toindex-bytelength.js")]
    public Task toindex_bytelength() => ExecutionTestFromFile("toindex-bytelength");

}
