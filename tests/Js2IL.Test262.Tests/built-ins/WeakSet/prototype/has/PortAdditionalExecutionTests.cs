using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet.prototype.has;

public class PortAdditionalExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.WeakSet.prototype.has") { }

    [Fact(DisplayName = "this-not-object-throw-null")]
    public Task this_not_object_throw_null()
        => ExecutionTestFromFile("this-not-object-throw-null");

}
