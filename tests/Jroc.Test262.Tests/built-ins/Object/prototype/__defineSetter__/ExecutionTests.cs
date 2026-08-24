using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.__defineSetter__;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.__defineSetter__") { }

    [Fact(DisplayName = "define-existing")]
    public Task define_existing()
        => ExecutionTestFromFile("define-existing");

    [Fact(DisplayName = "define-non-configurable")]
    public Task define_non_configurable()
        => ExecutionTestFromFile("define-non-configurable");

    [Fact(DisplayName = "define-non-extensible")]
    public Task define_non_extensible()
        => ExecutionTestFromFile("define-non-extensible");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
