using System.Threading.Tasks;

namespace Jroc.Tests.Node.DiagnosticsChannel;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests()
        : base("Node/DiagnosticsChannel")
    {
    }

    [Fact]
    public Task Require_DiagnosticsChannel()
        => GenerateTest(nameof(Require_DiagnosticsChannel));
}
