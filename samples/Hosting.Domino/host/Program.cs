using Js2IL.Runtime;
using System.Reflection;
using System.Linq;

namespace Hosting.Domino;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var htmlPath = Path.Combine(AppContext.BaseDirectory, "sample.html");
            var html = File.ReadAllText(htmlPath);

            var compiledModulePath = Path.Combine(AppContext.BaseDirectory, "index.dll");
            var asm = Assembly.LoadFrom(compiledModulePath);

            if (Environment.GetEnvironmentVariable("JS2IL_DOMINO_LISTMODULES") == "1")
            {
                var ids = asm
                    .GetCustomAttributes<JsCompiledModuleAttribute>()
                    .Select(a => a.ModuleId)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.Ordinal)
                    .Order(StringComparer.Ordinal)
                    .ToArray();

                Console.WriteLine($"manifest.count={ids.Length}");
                Console.WriteLine($"manifest.has.@mixmark-io/domino={ids.Contains("@mixmark-io/domino", StringComparer.Ordinal)}");
                Console.WriteLine($"manifest.has.@mixmark-io/domino/lib/index={ids.Contains("@mixmark-io/domino/lib/index", StringComparer.Ordinal)}");
                Console.WriteLine($"manifest.has.index={ids.Contains("index", StringComparer.Ordinal)}");
                foreach (var id in ids.Take(20))
                {
                    Console.WriteLine($"manifest.id={id}");
                }
                return;
            }

            using var exportsDisposable = JsEngine.LoadModule(asm, moduleId: "@mixmark-io/domino");
            dynamic exports = exportsDisposable;

            dynamic window = exports.createWindow(html);
            dynamic document = window.document;

            string title = Convert.ToString(document.title) ?? string.Empty;

            dynamic allElements = document.getElementsByTagName("*");
            dynamic links = document.getElementsByTagName("a");

            var elementCount = Convert.ToInt32(allElements.length);
            var linkCount = Convert.ToInt32(links.length);

            Console.WriteLine($"title={title}");
            Console.WriteLine($"elements={elementCount}");
            Console.WriteLine($"links={linkCount}");
        }
        catch (Exception ex) when (Environment.GetEnvironmentVariable("JS2IL_DOMINO_DIAG") == "1")
        {
            DumpDiagnostic(ex);
            throw;
        }
    }

    private static void DumpDiagnostic(Exception ex)
    {
        Console.WriteLine("[diag] Hosting.Domino failure");
        Console.WriteLine(ex);

        var current = ex;
        while (current != null)
        {
            if (current is JsErrorException jsErr)
            {
                Console.WriteLine($"[diag] JsErrorException.JsName: {jsErr.JsName}");
                Console.WriteLine($"[diag] JsErrorException.JsMessage: {jsErr.JsMessage}");
                if (!string.IsNullOrWhiteSpace(jsErr.JsStack))
                {
                    Console.WriteLine("[diag] JsErrorException.JsStack:");
                    Console.WriteLine(jsErr.JsStack);
                }
            }

            current = current.InnerException;
        }
    }
}
