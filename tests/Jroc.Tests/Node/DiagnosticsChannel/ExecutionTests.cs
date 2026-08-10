using System.Threading.Tasks;

namespace Jroc.Tests.Node.DiagnosticsChannel;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests()
        : base("Node/DiagnosticsChannel")
    {
    }

    [Fact]
    public Task Require_DiagnosticsChannel()
        => ExecutionTest(nameof(Require_DiagnosticsChannel));
}
