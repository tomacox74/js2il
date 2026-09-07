using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function;

public class FunctionExpressionAsyncArrowFunctionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionAsyncArrowFunctionConformanceBatchExecutionTests() : base("language/expressions/async-arrow-function", "language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "async-lineterminator-identifier-throws.js")]
    public Task async_lineterminator_identifier_throws()
        => ExecutionTest("async-lineterminator-identifier-throws");

    [Fact(DisplayName = "escaped-async-line-terminator.js")]
    public Task escaped_async_line_terminator()
        => ExecutionTest("escaped-async-line-terminator");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTest("name");

}
