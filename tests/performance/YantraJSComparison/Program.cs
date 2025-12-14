using YantraJS.Core;
using YantraJS.Core.Clr;
using System.Diagnostics;

namespace YantraJSComparison;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== YantraJS Performance Test ===");
        Console.WriteLine($"Running PrimeJavaScript.js with YantraJS");
        Console.WriteLine();

        // Read the JavaScript file
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "PrimeJavaScript.js");
        if (!File.Exists(scriptPath))
        {
            Console.Error.WriteLine($"Error: Could not find {scriptPath}");
            return;
        }

        var script = File.ReadAllText(scriptPath);

        // Measure YantraJS execution time
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var context = new JSContext();

            // Provide console mock
            var consoleObj = new JSObject();
            consoleObj[KeyStrings.log] = new JSFunction((in Arguments a) =>
            {
                var parts = new List<string>();
                for (int i = 0; i < a.Length; i++)
                {
                    var val = a.GetAt(i);
                    parts.Add(val?.ToString() ?? "undefined");
                }
                Console.WriteLine(string.Join(" ", parts));
                return JSUndefined.Value;
            });
            context[KeyStrings.console] = consoleObj;

            // Provide process.argv mock
            var processObj = new JSObject();
            var argvArray = new JSArray();
            argvArray[0] = new JSString("yantrajs");
            argvArray[1] = new JSString("verbose");
            processObj["argv"] = argvArray;
            context["process"] = processObj;

            // Provide performance.now() mock
            var perfStart = Stopwatch.GetTimestamp();
            var performanceObj = new JSObject();
            performanceObj["now"] = new JSFunction((in Arguments a) =>
            {
                var elapsed = Stopwatch.GetTimestamp() - perfStart;
                return new JSNumber((double)elapsed / Stopwatch.Frequency * 1000.0);
            });
            context["performance"] = performanceObj;

            // Provide require() mock for perf_hooks
            context[KeyStrings.require] = new JSFunction((in Arguments a) =>
            {
                var moduleName = a.Length > 0 ? a.Get1()?.ToString() : null;
                if (moduleName == "perf_hooks")
                {
                    var perfHooksObj = new JSObject();
                    var perfObj = new JSObject();
                    perfObj["now"] = new JSFunction((in Arguments innerArgs) =>
                    {
                        var elapsed = Stopwatch.GetTimestamp() - perfStart;
                        return new JSNumber((double)elapsed / Stopwatch.Frequency * 1000.0);
                    });
                    perfHooksObj["performance"] = perfObj;
                    return perfHooksObj;
                }
                throw new Exception($"Module '{moduleName}' not found");
            });

            // Execute the script
            context.Eval(script, "PrimeJavaScript.js");
            
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"=== YantraJS Total Execution Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalSeconds:F2}s) ===");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.Error.WriteLine($"Error executing script: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
        }
    }
}

