using System.Threading.Tasks;

namespace Js2IL.Tests.JSON
{
	public class ExecutionTests : ExecutionTestsBase
	{
		public ExecutionTests() : base("JSON") { }

		[Fact(Skip = "Pending JSON implementation")]
		public Task JSON_Parse_SimpleObject()
			=> ExecutionTest(nameof(JSON_Parse_SimpleObject));
	}
}
