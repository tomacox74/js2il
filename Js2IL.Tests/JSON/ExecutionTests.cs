using System.Threading.Tasks;

namespace Js2IL.Tests.JSON
{
	public class ExecutionTests : ExecutionTestsBase
	{
		public ExecutionTests() : base("JSON") { }

	[Fact]
		public Task JSON_Parse_SimpleObject()
			=> ExecutionTest(nameof(JSON_Parse_SimpleObject));
	}
}
