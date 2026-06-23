using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.fromEntries;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.fromEntries") { }

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-key-accessor")]
    public Task iterator_closed_for_throwing_entry_key_accessor()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-key-accessor");

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-key-tostring")]
    public Task iterator_closed_for_throwing_entry_key_tostring()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-key-tostring");

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-value-accessor")]
    public Task iterator_closed_for_throwing_entry_value_accessor()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-value-accessor");

    [Fact(DisplayName = "iterator-not-closed-for-next-returning-non-object")]
    public Task iterator_not_closed_for_next_returning_non_object()
        => ExecutionTestFromFile("iterator-not-closed-for-next-returning-non-object");

    [Fact(DisplayName = "iterator-not-closed-for-throwing-done-accessor")]
    public Task iterator_not_closed_for_throwing_done_accessor()
        => ExecutionTestFromFile("iterator-not-closed-for-throwing-done-accessor");
}
