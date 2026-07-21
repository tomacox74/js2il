using Jint;
using Microsoft.ClearScript.V8;
using Okojo;
using Okojo.Runtime;
using System.Diagnostics;
using YantraJS.Core;
using YantraJS.Core.Clr;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: MitataRuntimeHost <clearscript|jint|yantrajs|okojo> <script-path>");
    return 1;
}

var runtime = args[0].ToLowerInvariant();
var script = File.ReadAllText(args[1]);
var prelude = $$"""
globalThis.__BENCHMARK_RUNTIME = "{{runtime}}";
globalThis.__BENCHMARK_ITERATIONS = 1;
globalThis.performance = { now: () => Date.now() };
""";

switch (runtime)
{
    case "clearscript":
        using (var engine = new V8ScriptEngine())
        {
            engine.AddHostObject("__hostLog", (Action<object?>)(value => Console.WriteLine(value)));
            engine.Execute("globalThis.console = { log: value => __hostLog(value) };");
            engine.Execute(prelude);
            engine.Execute(script);
        }
        break;

    case "jint":
        {
            var engine = new Engine();
            engine.SetValue("__hostLog", (Action<object?>)(value => Console.WriteLine(value)));
            engine.Execute("globalThis.console = { log: value => __hostLog(value) };");
            engine.Execute(prelude);
            engine.Execute(script);
        }
        break;

    case "yantrajs":
        {
            var context = new JSContext();
            context[new JSString("__hostLog")] = new JSFunction((in Arguments values) =>
            {
                Console.WriteLine(values.Length > 0 ? values.Get1()?.ToString() : string.Empty);
                return JSUndefined.Value;
            });
            var console = new JSObject();
            console[KeyString.log] = new JSFunction((in Arguments values) =>
            {
                Console.WriteLine(values.Length > 0 ? values.Get1()?.ToString() : string.Empty);
                return JSUndefined.Value;
            });
            var performance = new JSObject();
            performance[KeyString.now] = new JSFunction((in Arguments _) =>
                new JSNumber((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency * 1000.0));
            context[KeyString.console] = console;
            context[new JSString("performance")] = performance;
            context[new JSString("__BENCHMARK_RUNTIME")] = new JSString(runtime);
            context[new JSString("__BENCHMARK_ITERATIONS")] = new JSNumber(1);
            context.Eval(script, args[1]);
        }
        break;

    case "okojo":
        using (var host = JsRuntime.CreateBuilder()
            .UseGlobals(globals => globals.Function("__hostLog", 1, info =>
            {
                Console.WriteLine(info.GetArgumentStringOrDefault(0, string.Empty));
                return JsValue.Undefined;
            }))
            .Build())
        {
            var realm = host.MainRealm;
            realm.Evaluate("globalThis.console = { log: value => __hostLog(String(value)) };");
            realm.Evaluate(prelude);
            realm.Evaluate(script);
        }
        break;

    default:
        Console.Error.WriteLine($"Unknown runtime '{args[0]}'.");
        return 1;
}

return 0;
