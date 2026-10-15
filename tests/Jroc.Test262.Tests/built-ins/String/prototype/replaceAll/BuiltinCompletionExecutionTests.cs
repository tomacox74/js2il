using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replaceAll;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.replaceAll") { }

    [Fact(DisplayName = "searchValue-replacer-RegExp-call-fn.js")]
    public Task test_searchValue_replacer_RegExp_call_fn() => ExecutionTest("searchValue-replacer-RegExp-call-fn");

    [Fact(DisplayName = "searchValue-replacer-RegExp-call.js")]
    public Task test_searchValue_replacer_RegExp_call() => ExecutionTest("searchValue-replacer-RegExp-call");
}
