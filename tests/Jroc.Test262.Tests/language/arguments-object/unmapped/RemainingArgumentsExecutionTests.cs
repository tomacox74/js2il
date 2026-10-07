using Xunit;

namespace Jroc.Test262.Tests.language.arguments_object.unmapped;

public sealed class RemainingArgumentsExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public RemainingArgumentsExecutionTests() : base("language.arguments-object.unmapped")
    {
    }

    [Fact(DisplayName = "language/arguments-object/unmapped/Symbol.iterator.js")]
    public Task test_Symbol_iterator() => ExecutionTest("Symbol.iterator");
}
