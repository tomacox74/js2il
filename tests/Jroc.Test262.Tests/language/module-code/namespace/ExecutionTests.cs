using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.namespace_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base("language/module-code/namespace", "language.module_code.namespace_") { }

    [Fact(DisplayName = "Symbol.iterator")]
    public Task Symbol_iterator() => ExecutionTest("Symbol.iterator");
    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag() => ExecutionTest("Symbol.toStringTag");
}
