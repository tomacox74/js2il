using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.includes;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.includes") { }

    [Fact(DisplayName = "search-undefined-after-shrinking-buffer")]
    public Task search_undefined_after_shrinking_buffer()
        => ExecutionTestFromFile("search-undefined-after-shrinking-buffer");

}
