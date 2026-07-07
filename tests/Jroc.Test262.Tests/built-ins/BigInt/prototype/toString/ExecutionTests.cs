using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.BigInt.prototype.toString") { }

    [Fact(DisplayName = "default-radix")]
    public Task default_radix()
        => ExecutionTestFromFile("default-radix");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "thisbigintvalue-not-valid-throws")]
    public Task thisbigintvalue_not_valid_throws()
        => ExecutionTestFromFile("thisbigintvalue-not-valid-throws");
}
