using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExpStringIteratorPrototype.next;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExpStringIteratorPrototype.next") { }

    [Fact(DisplayName = "next-iteration.js")]
    public Task next_iteration()
        => ExecutionTestFromFile("next-iteration");

    [Fact(DisplayName = "next-iteration-global.js")]
    public Task next_iteration_global()
        => ExecutionTestFromFile("next-iteration-global");

    [Fact(DisplayName = "custom-regexpexec.js")]
    public Task custom_regexpexec()
        => ExecutionTestFromFile("custom-regexpexec");

    [Fact(DisplayName = "custom-regexpexec-get-throws.js")]
    public Task custom_regexpexec_get_throws()
        => ExecutionTestFromFile("custom-regexpexec-get-throws");

    [Fact(DisplayName = "custom-regexpexec-not-callable.js")]
    public Task custom_regexpexec_not_callable()
        => ExecutionTestFromFile("custom-regexpexec-not-callable");
}
