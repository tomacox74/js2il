using Jint;
using Jint.Native;
using System.Diagnostics;

namespace JintComparison;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Jint Performance Test ===");
        Console.WriteLine($"Running PrimeJavaScript.js with Jint v{typeof(Engine).Assembly.GetName().Version}");
        Console.WriteLine();

        // Read the JavaScript file
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "PrimeJavaScript.js");
        if (!File.Exists(scriptPath))
        {
            Console.Error.WriteLine($"Error: Could not find {scriptPath}");
            return;
        }

        var script = File.ReadAllText(scriptPath);

        // Measure Jint execution time
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var engine = new Engine(options =>
            {
                options.EnableModules(AppContext.BaseDirectory);
            });

            // Provide console mock
            engine.SetValue("console", new
            {
                log = new Func<object, object, object, object, object, object>(
                    (a1, a2, a3, a4, a5) =>
                    {
                        var args = new[] { a1, a2, a3, a4, a5 }
                            .Where(a => a != null && !ReferenceEquals(a, JsValue.Undefined))
                            .Select(a => a?.ToString() ?? "null");
                        Console.WriteLine(string.Join(" ", args));
                        return JsValue.Undefined;
                    })
            });

            // Provide process.argv mock
            engine.SetValue("process", new
            {
                argv = new[] { "jint", "verbose" }
            });

            // Provide performance.now() mock
            var perfStart = Stopwatch.GetTimestamp();
            engine.SetValue("performance", new
            {
                now = new Func<double>(() =>
                {
                    var elapsed = Stopwatch.GetTimestamp() - perfStart;
                    return (double)elapsed / Stopwatch.Frequency * 1000.0;
                })
            });

            // Provide require() mock for perf_hooks
            engine.SetValue("require", new Func<string, object>(moduleName =>
            {
                if (moduleName == "perf_hooks")
                {
                    return new
                    {
                        performance = new
                        {
                            now = new Func<double>(() =>
                            {
                                var elapsed = Stopwatch.GetTimestamp() - perfStart;
                                return (double)elapsed / Stopwatch.Frequency * 1000.0;
                            })
                        }
                    };
                }
                throw new Exception($"Module '{moduleName}' not found");
            }));

            // Execute the script
            engine.Execute(script);
            
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"=== Jint Total Execution Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalSeconds:F2}s) ===");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.Error.WriteLine($"Error executing script: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
        }
    }
}
