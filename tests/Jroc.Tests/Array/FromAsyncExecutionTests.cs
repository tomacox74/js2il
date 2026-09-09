using System.Threading.Tasks;

namespace Jroc.Tests.Array;

public class FromAsyncExecutionTests : ExecutionTestsBase
{
    public FromAsyncExecutionTests() : base("Array") { }

    [Fact]
    public Task Array_FromAsync_NextCapture() => ExecutionTest(nameof(Array_FromAsync_NextCapture));

    [Fact]
    public Task Array_FromAsync_NativeNextOverride() => ExecutionTest(nameof(Array_FromAsync_NativeNextOverride));

    [Fact]
    public Task Array_FromAsync_AcquisitionErrors() => ExecutionTest(nameof(Array_FromAsync_AcquisitionErrors));

    [Fact]
    public Task Array_FromAsync_SyncFinalValue() => ExecutionTest(nameof(Array_FromAsync_SyncFinalValue));

    [Fact]
    public Task Array_FromAsync_SyncAwaitOrdering() => ExecutionTest(nameof(Array_FromAsync_SyncAwaitOrdering));

    [Fact]
    public Task Array_FromAsync_SyncClose() => ExecutionTest(nameof(Array_FromAsync_SyncClose));

    [Fact]
    public Task Array_FromAsync_RejectedValueSyncClose() => ExecutionTest(nameof(Array_FromAsync_RejectedValueSyncClose));

    [Fact]
    public Task Array_FromAsync_AsyncValuesAndClose() => ExecutionTest(nameof(Array_FromAsync_AsyncValuesAndClose));
}
