using System.Threading.Tasks;

namespace Jroc.Tests.JSON
{
	public class ExecutionTests : ExecutionTestsBase
	{
		public ExecutionTests() : base("JSON") { }

	[Fact]
		public Task JSON_Parse_SimpleObject()
			=> ExecutionTest(nameof(JSON_Parse_SimpleObject));

		[Fact]
		public Task JSON_Parse_Reviver_Holder()
			=> ExecutionTest(nameof(JSON_Parse_Reviver_Holder));

		[Fact]
		public Task JSON_Parse_ReviverProxyArrayLengthUsesToLength()
			=> ExecutionTest(nameof(JSON_Parse_ReviverProxyArrayLengthUsesToLength));

		[Fact]
		public Task JSON_Parse_SymbolPrimitiveRejectsOverriddenCoercion()
			=> ExecutionTest(nameof(JSON_Parse_SymbolPrimitiveRejectsOverriddenCoercion));
	}
}
