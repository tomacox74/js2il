namespace Jroc.Test262.Tests.language.destructuring.binding;

public class DestructuringCompletionExecutionTests : DiskExecutionTestsBase
{
    public DestructuringCompletionExecutionTests() : base("language.destructuring.binding") { }

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer.js")]
    public Task typedarray_backed_by_resizable_buffer() => ExecutionTest("typedarray-backed-by-resizable-buffer");
}
