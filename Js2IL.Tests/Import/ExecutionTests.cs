using Xunit;

namespace Js2IL.Tests.Import
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Import")
        {
        }

        [Fact]
        public Task Import_BasicImport()
        {
            var testName = nameof(Import_BasicImport);
            return ExecutionTest(testName, additionalScripts: new[] { "Import_BasicImport_Dep" });
        }
    }
}
