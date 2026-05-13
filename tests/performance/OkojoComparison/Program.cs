using Okojo;
using Okojo.Runtime;
using System.Diagnostics;

namespace OkojoComparison;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Okojo Performance Test ===");
        Console.WriteLine($"Running PrimeJavaScript.js with Okojo v{typeof(JsRuntime).Assembly.GetName().Version}");
        Console.WriteLine();

        var scriptPath = Path.Combine(AppContext.BaseDirectory, "PrimeJavaScript.js");
        if (!File.Exists(scriptPath))
        {
            Console.Error.WriteLine($"Error: Could not find {scriptPath}");
            return;
        }

        var script = File.ReadAllText(scriptPath);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var runtime = JsRuntime.CreateBuilder()
                .UseGlobals(globals => globals.Function("__hostLog", 1, info =>
                {
                    Console.WriteLine(info.GetArgumentStringOrDefault(0, string.Empty));
                    return JsValue.Undefined;
                }))
                .Build();

            var realm = runtime.MainRealm;
            realm.Evaluate(
                """
                globalThis.console = {
                    log: function() {
                        var parts = [];
                        for (var i = 0; i < arguments.length; i++) {
                            parts.push(String(arguments[i]));
                        }
                        __hostLog(parts.join(' '));
                    }
                };
                globalThis.process = { argv: ['okojo', 'verbose'] };
                (function() {
                    var _t0 = Date.now();
                    globalThis.performance = {
                        now: function() { return Date.now() - _t0; }
                    };
                })();
                globalThis.require = function(mod) {
                    if (mod === 'perf_hooks') {
                        return { performance: globalThis.performance };
                    }
                    throw new Error("Module '" + mod + "' not found");
                };
                """
            );

            realm.Evaluate(script);

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"=== Okojo Total Execution Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalSeconds:F2}s) ===");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.Error.WriteLine($"Error executing script: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
        }
    }
}
