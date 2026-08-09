using System.Threading.Tasks;

namespace Jroc.Tests.Node.Dns;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests()
        : base("Node/Dns")
    {
    }

    [Fact]
    public Task Require_Dns_Lookup()
        => ExecutionTest(nameof(Require_Dns_Lookup));
}
