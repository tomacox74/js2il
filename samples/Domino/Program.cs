namespace Domino;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var htmlPath = Environment.GetEnvironmentVariable("JROC_DOMINO_HTML_PATH")
                ?? Path.Combine(AppContext.BaseDirectory, "sample.html");
            var html = File.ReadAllText(htmlPath);

            using var exports = global::mixmark_io_domino.Import();
            using var window = exports.CreateWindow(html);
            var document = window.Document;
            var allElements = document.GetElementsByTagName("*");
            var links = document.GetElementsByTagName("a");

            Console.WriteLine($"title={document.Title}");
            Console.WriteLine($"elements={allElements.Length}");
            Console.WriteLine($"links={links.Length}");
        }
        catch (Exception ex) when (Environment.GetEnvironmentVariable("JROC_DOMINO_DIAG") == "1")
        {
            DumpDiagnostic(ex);
            throw;
        }
    }

    private static void DumpDiagnostic(Exception ex)
    {
        Console.WriteLine("[diag] Domino failure");
        Console.WriteLine(ex);
    }
}
