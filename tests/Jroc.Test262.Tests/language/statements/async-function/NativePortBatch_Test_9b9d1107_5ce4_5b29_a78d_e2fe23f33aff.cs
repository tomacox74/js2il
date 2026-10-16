using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.statements.async_function;

public sealed class NativePortBatch_Test_9b9d1107_5ce4_5b29_a78d_e2fe23f33aff : DiskExecutionTestsBase
{
    public NativePortBatch_Test_9b9d1107_5ce4_5b29_a78d_e2fe23f33aff() : base("Jroc.Test262.Tests.language.statements.async_function") { }

    [Fact(DisplayName = "try-throw-finally-throw")]
    public Task try_throw_finally_throw() => ExecutionTestFromFile("try-throw-finally-throw");

    [Fact(DisplayName = "unscopables-with")]
    public Task unscopables_with() => ExecutionTestFromFile("unscopables-with");

    [Fact(DisplayName = "unscopables-with-in-nested-fn")]
    public Task unscopables_with_in_nested_fn() => ExecutionTestFromFile("unscopables-with-in-nested-fn");

}
