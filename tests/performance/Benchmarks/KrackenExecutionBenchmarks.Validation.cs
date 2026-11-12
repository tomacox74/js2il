using System.Security.Cryptography;
using System.Text;
using YantraJS.Core;

namespace Benchmarks;

public partial class KrackenExecutionBenchmarks
{
    public static void ValidatePreparation()
    {
        var scenario = $"__kracken-parity-{Guid.NewGuid():N}";
        var dataPath = Path.Combine(KrackenScriptsDirectory, scenario + "-data.js");
        var testPath = Path.Combine(Path.GetTempPath(), scenario + ".js");
        var benchmark = new KrackenExecutionBenchmarks { ScriptName = testPath };

        try
        {
            File.WriteAllText(testPath, """
                if (DSP.value !== 17) {
                    throw new Error('Data must be initialized before workload registration');
                }
                runTest(function() {
                    DSP.value += samples[nextSample];
                    nextSample = (nextSample + 1) % samples.length;
                    calls++;
                }, 3);
                """);

            (string Name, Action Setup, Action Run, Func<string> Snapshot, Action Cleanup)[] runtimes =
            [
                ("jroc", benchmark.SetupJroc, benchmark.RunJrocTest,
                    () => benchmark._jrocExports!.Invoke("snapshot")!.ToString()!, benchmark.CleanupJroc),
                ("jint", benchmark.SetupJint, benchmark.RunJintTest,
                    () => benchmark._jintEngine!.Invoke("snapshot").ToString(), benchmark.CleanupJint),
                ("okojo", benchmark.SetupOkojo, benchmark.RunOkojoTest,
                    () => benchmark._okojoRealm!.Call(
                        benchmark._okojoRealm.Global["snapshot"], benchmark._okojoRealm.GlobalObject).ToString(),
                    benchmark.CleanupOkojo),
                ("yantrajs", benchmark.SetupYantraJs, benchmark.RunYantraJsTest,
                    () => benchmark._yantraJsContext![new JSString("snapshot")]
                        .InvokeFunction(Arguments.Empty).ToString(),
                    benchmark.CleanupYantraJs),
                ("sharpts", benchmark.SetupSharpTs, benchmark.RunSharpTsTest,
                    () => benchmark._sharpTsScript!.GetFunction("snapshot")().ToString()!,
                    benchmark.CleanupSharpTs)
            ];

            // SharpTS 1.0.12 cannot emit the implicit-global audio fixtures.
            var cases = new[]
            {
                (Name: "Preparation_PreservesSharedWorkloadState", Declaration: "var ", OriginalRunner: "", Runtimes: runtimes),
                (Name: "Preparation_ReplacesOriginalDataRunner", Declaration: "var ",
                    OriginalRunner: "function runTest(workload, iterations) { workload(); }\n", Runtimes: runtimes),
                (Name: "Preparation_PreservesImplicitGlobals", Declaration: "", OriginalRunner: "",
                    Runtimes: runtimes.Where(runtime => runtime.Name != "sharpts").ToArray())
            };

            foreach (var test in cases)
            {
                File.WriteAllText(dataPath, test.OriginalRunner + test.Declaration + """
                    DSP = { value: 17 };
                    var samples = [2, 4, 8];
                    var nextSample = 0;
                    var calls = 0;
                    function snapshot() {
                        return calls + ':' + nextSample + ':' + DSP.value;
                    }
                    """);
                var source = benchmark.LoadScriptSource();
                var sourceHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source)));
                foreach (var runtime in test.Runtimes)
                {
                    try
                    {
                        runtime.Setup();
                        RequireSnapshot("0:0:17", "setup");
                        runtime.Run();
                        RequireSnapshot("3:0:31", "first invocation");
                        runtime.Run();
                        RequireSnapshot("6:0:45", "second invocation");
                        Console.WriteLine(
                            $"{test.Name}[{runtime.Name}] passed; source SHA256={sourceHash}");
                    }
                    finally
                    {
                        runtime.Cleanup();
                    }

                    void RequireSnapshot(string expected, string phase)
                    {
                        var actual = runtime.Snapshot();
                        if (actual != expected)
                        {
                            throw new InvalidOperationException(
                                $"{runtime.Name} {phase}: expected state '{expected}', got '{actual}'.");
                        }
                    }
                }
            }
        }
        finally
        {
            File.Delete(dataPath);
            File.Delete(testPath);
        }
    }
}
