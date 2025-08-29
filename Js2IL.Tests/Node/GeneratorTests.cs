using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Js2IL.Tests.Node
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(
            nameof(Require_Path_Join_Basic),
            configureSettings: s =>
            {
                // Trim trailing spaces/tabs before line breaks without changing line-ending style
                s.AddScrubber(sb =>
                {
                    var text = sb.ToString();
                    text = Regex.Replace(text, @"[ \t]+(\r?\n)", "$1");
                    sb.Clear();
                    sb.Append(text);
                });
                // Ensure a single newline at EOF to reduce flakiness
                s.AddScrubber(sb =>
                {
                    var text = sb.ToString();
                    text = text.TrimEnd('\r', '\n') + "\r\n";
                    sb.Clear();
                    sb.Append(text);
                });
            });

    [Fact]
        public Task Global___dirname_PrintsDirectory() => GenerateTest(
            nameof(Global___dirname_PrintsDirectory),
            configureSettings: s =>
            {
                s.AddScrubber(sb => sb.Replace('\\', '/'));
                // Trim trailing spaces/tabs before line breaks without changing line-ending style
                s.AddScrubber(sb =>
                {
                    var text = sb.ToString();
                    text = Regex.Replace(text, @"[ \t]+(\r?\n)", "$1");
                    sb.Clear();
                    sb.Append(text);
                });
                // Ensure a single newline at EOF to reduce flakiness
                s.AddScrubber(sb =>
                {
                    var text = sb.ToString();
                    text = text.TrimEnd('\r', '\n') + "\r\n";
                    sb.Clear();
                    sb.Append(text);
                });
            });
    }
}
