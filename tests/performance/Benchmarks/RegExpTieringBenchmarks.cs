#if SOURCE_JROC_PROJECTS
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using JavaScriptRuntime;
using System.Text.RegularExpressions;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class RegExpTieringBenchmarks
{
    private Regex _interpreted = null!;
    private string _input = "";

    [Params(1, 2, 3, 10, 1_000)]
    public int Constructions { get; set; }

    [Params("Ascii", "Ansi", "Emoji")]
    public string Pattern { get; set; } = "";

    [GlobalSetup]
    public void Setup()
    {
        using var scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        var source = Pattern switch
        {
            "Ascii" => "a.*a",
            "Ansi" => "\x1b\\[[0-?]*[ -/]*[@-~]",
            "Emoji" => "(?:" + string.Join("|",
                Enumerable.Range(0x1F600, 128).Select(char.ConvertFromUtf32)) + ")",
            _ => throw new InvalidOperationException(Pattern)
        };
        _input = Pattern switch
        {
            "Ascii" => "a" + new string('b', 4_096) + "a",
            "Ansi" => "\x1b[" + new string('1', 4_096) + "m",
            "Emoji" => new string('b', 4_096) + char.ConvertFromUtf32(0x1F640),
            _ => throw new InvalidOperationException(Pattern)
        };
        _interpreted = new RegExp(source, Pattern == "Emoji" ? "u" : "").Regex;
    }

    [Benchmark(Baseline = true)]
    public bool Interpreted() => Run(enabled: false);

    [Benchmark]
    public bool Adaptive() => Run(enabled: true);

    [Benchmark]
    public bool ConservativeAdaptive() => Run(enabled: true, threshold: RegExp.CompiledRegexPromotionThreshold);

    private bool Run(bool enabled, int threshold = 3)
    {
        // Isolate per-entry promotion/JIT and matching. Pattern translation/parsing
        // is outside timing; every invocation starts with a fresh adaptive entry.
        var tier = new RegExpAdaptiveCompilation(new RegExpCompilationBudget(1, threshold, enabled));
        var result = false;
        for (var index = 0; index < Constructions; index++)
        {
            result = tier.GetRegexForConstruction(_interpreted).IsMatch(_input);
        }
        return result;
    }
}
#endif
