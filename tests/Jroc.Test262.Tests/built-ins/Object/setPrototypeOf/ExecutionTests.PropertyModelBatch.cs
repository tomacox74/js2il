namespace Jroc.Test262.Tests.built_ins.Object.setPrototypeOf;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "set-error.js")]
    public Task set_error() => ExecutionTestFromFile("set-error");

    [Fact(DisplayName = "set-failure-non-extensible.js")]
    public Task set_failure_non_extensible() => ExecutionTestFromFile("set-failure-non-extensible");
}
