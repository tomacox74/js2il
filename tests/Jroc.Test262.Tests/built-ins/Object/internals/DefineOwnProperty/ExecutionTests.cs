using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.internals.DefineOwnProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.internals.DefineOwnProperty") { }

    [Fact(DisplayName = "nan-equivalence-define-own-property-reassign")]
    public Task nan_equivalence_define_own_property_reassign()
        => ExecutionTestFromFile("nan-equivalence-define-own-property-reassign");

    [Fact(DisplayName = "nan-equivalence-define-own-property-reconfigure")]
    public Task nan_equivalence_define_own_property_reconfigure()
        => ExecutionTestFromFile("nan-equivalence-define-own-property-reconfigure");
}
