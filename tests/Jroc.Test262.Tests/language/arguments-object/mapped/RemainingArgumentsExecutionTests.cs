using Xunit;

namespace Jroc.Test262.Tests.language.arguments_object.mapped;

public sealed class RemainingArgumentsExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public RemainingArgumentsExecutionTests() : base("language.arguments-object.mapped")
    {
    }

    [Fact(DisplayName = "language/arguments-object/mapped/Symbol.iterator.js")]
    public Task test_Symbol_iterator() => ExecutionTest("Symbol.iterator");
}
