namespace Jroc.Test262.Tests.built_ins;

[Collection(InMemoryExecutionTestsBase.CollectionName)]
public abstract class InMemoryExecutionTestsBase : global::Jroc.Test262.Tests.DiskExecutionTestsBase
{
    internal const string CollectionName = "Built-ins in-memory execution";

    protected InMemoryExecutionTestsBase(string testCategory)
        : base(testCategory)
    {
    }
}

[CollectionDefinition(InMemoryExecutionTestsBase.CollectionName)]
public sealed class InMemoryExecutionTestCollection
{
}
