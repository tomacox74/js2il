using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.module_code.top_level_await;

public sealed class NativePortBatch_Port700 : DiskExecutionTestsBase
{
    public NativePortBatch_Port700() : base("Jroc.Test262.Tests.language.module_code.top_level_await") { }

    [Fact(DisplayName = "new-await")]
    public Task Port700_new_await() => CompilationFailureTest("new-await");

    [Fact(DisplayName = "no-operand")]
    public Task Port700_no_operand() => CompilationFailureTest("no-operand");

}
