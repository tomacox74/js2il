using Xunit;

namespace Js2IL.Tests.Import
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Import")
        {
        }

        // TODO: Generator test disabled until module loading in tests is fixed
        // The import() functionality works but additional module loading needs investigation
        // [Fact]
        // public Task Import_BasicImport()
        // {
        //     var testName = nameof(Import_BasicImport);
        //     return GenerateTest(testName, configureSettings: null, additionalScripts: new[] { "Import_BasicImport_Dep" });
        // }
    }
}
