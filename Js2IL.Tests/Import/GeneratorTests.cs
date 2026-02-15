using Xunit;

namespace Js2IL.Tests.Import
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Import")
        {
        }

        [Fact(Skip = "Temporarily skipped while import() generator snapshots are stabilized")]
        public Task Import_BasicImport()
        {
            var testName = nameof(Import_BasicImport);
            return GenerateTest(testName, configureSettings: null, additionalScripts: new[] { "Import_BasicImport_Dep" });
        }
    }
}
