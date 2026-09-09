using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.values;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.values") { }

        [Fact(DisplayName = "iter-prototype")]
    public Task iter_prototype()
        => ExecutionTestFromFile("iter-prototype");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");
    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");
    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");
}
