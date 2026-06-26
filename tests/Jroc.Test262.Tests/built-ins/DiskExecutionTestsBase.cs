namespace Jroc.Test262.Tests.built_ins;

[Collection(InMemoryExecutionTestsBase.CollectionName)]
public abstract class DiskExecutionTestsBase : InMemoryExecutionTestsBase
{
    protected DiskExecutionTestsBase(string testCategory)
        : base(testCategory)
    {
    }
}
