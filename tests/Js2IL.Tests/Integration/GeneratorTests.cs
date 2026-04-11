using System.Threading.Tasks;

namespace Js2IL.Tests.Integration
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Integration") { }

        [Fact]
        public Task Compile_Scripts_GenerateNodeSupportMd() => GenerateTest(nameof(Compile_Scripts_GenerateNodeSupportMd));

        [Fact]
        public Task Compile_Scripts_BumpVersion() => GenerateTest(nameof(Compile_Scripts_BumpVersion));

        [Fact]
        public Task Compile_Scripts_DecompileGeneratorTest() => GenerateTest(nameof(Compile_Scripts_DecompileGeneratorTest));

        [Fact]
        public Task Compile_Scripts_Test262Bootstrap() => GenerateTest(nameof(Compile_Scripts_Test262Bootstrap));

        [Fact]
        public Task Compile_Scripts_ConvertEcmaExtractHtmlToMarkdown()
            => GenerateTest(nameof(Compile_Scripts_ConvertEcmaExtractHtmlToMarkdown), ["node_modules/turndown/index"]);

        [Fact]
        public Task Compile_Scripts_ExtractEcma262SectionHtml_UrlMode()
            => GenerateTest(
                nameof(Compile_Scripts_ExtractEcma262SectionHtml_UrlMode),
                ["Compile_Scripts_ExtractEcma262SectionHtml_TestHarness"]);

        [Fact]
        public Task Compile_Scripts_ExtractEcma262SectionHtml_AutoMode()
            => GenerateTest(
                nameof(Compile_Scripts_ExtractEcma262SectionHtml_AutoMode),
                ["Compile_Scripts_ExtractEcma262SectionHtml_TestHarness"]);

        [Fact]
        public Task Compile_Performance_Dromaeo_Object_Array_Modern() => GenerateTest(nameof(Compile_Performance_Dromaeo_Object_Array_Modern));

        [Fact]
        public Task Compile_Performance_Dromaeo_Object_Regexp() => GenerateTest(nameof(Compile_Performance_Dromaeo_Object_Regexp));

        [Fact]
        public Task Compile_Performance_PrimeJavaScript() => GenerateTest(nameof(Compile_Performance_PrimeJavaScript));
    }
}
