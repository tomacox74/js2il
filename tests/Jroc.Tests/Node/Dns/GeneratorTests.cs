using System.Threading.Tasks;

namespace Jroc.Tests.Node.Dns;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests()
        : base("Node/Dns")
    {
    }

    [Fact]
    public Task Require_Dns_Lookup()
        => GenerateTest(nameof(Require_Dns_Lookup));
}
