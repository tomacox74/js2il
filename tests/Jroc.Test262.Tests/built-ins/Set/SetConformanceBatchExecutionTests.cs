using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set") { }

    [Fact(DisplayName = "set-iterable-empty-does-not-call-add.js")]
    public Task set_iterable_empty_does_not_call_add() => ExecutionTestFromFile("set-iterable-empty-does-not-call-add");

    [Fact(DisplayName = "set-iterable-throws-when-add-is-not-callable.js")]
    public Task set_iterable_throws_when_add_is_not_callable() => ExecutionTestFromFile("set-iterable-throws-when-add-is-not-callable");

    [Fact(DisplayName = "set-iterator-next-failure.js")]
    public Task set_iterator_next_failure() => ExecutionTestFromFile("set-iterator-next-failure");

    [Fact(DisplayName = "set-iterator-value-failure.js")]
    public Task set_iterator_value_failure() => ExecutionTestFromFile("set-iterator-value-failure");

    [Fact(DisplayName = "set-newtarget.js")]
    public Task set_newtarget() => ExecutionTestFromFile("set-newtarget");

    [Fact(DisplayName = "valid-values.js")]
    public Task valid_values() => ExecutionTestFromFile("valid-values");

}
