using System.Text.RegularExpressions;
using Acornima.Ast;
using Jroc.Services;

namespace Benchmarks;

internal static partial class CallableArityAnalysis
{
    public static int Run()
    {
        var repositoryRoot = FindRepositoryRoot();
        var scenarioRoot = Path.Combine(
            repositoryRoot,
            "tests",
            "performance",
            "Benchmarks",
            "Scenarios");
        var parser = new JavaScriptParser();
        var callArities = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var constructArities = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var scenarioCount = 0;

        foreach (var path in Directory.EnumerateFiles(
                     scenarioRoot,
                     "*.js",
                     SearchOption.AllDirectories).Order())
        {
            scenarioCount++;
            var ast = parser.ParseJavaScript(File.ReadAllText(path), path);
            parser.VisitAst(ast, node =>
            {
                switch (node)
                {
                    case CallExpression call:
                        Increment(
                            callArities,
                            DescribeArguments(call.Arguments));
                        break;
                    case NewExpression construct:
                        Increment(
                            constructArities,
                            DescribeArguments(construct.Arguments));
                        break;
                }
            });
        }

        var runtimeArities = AnalyzeRuntimeCallbackDispatch(repositoryRoot);

        Console.WriteLine($"Scenarios analyzed: {scenarioCount}");
        WriteDistribution("Benchmark call expressions", callArities);
        WriteDistribution("Benchmark construct expressions", constructArities);
        WriteDistribution("Runtime fixed dispatcher references", runtimeArities);
        return 0;
    }

    private static SortedDictionary<string, int> AnalyzeRuntimeCallbackDispatch(
        string repositoryRoot)
    {
        var runtimeRoot = Path.Combine(repositoryRoot, "src", "JavaScriptRuntime");
        var distribution = new SortedDictionary<string, int>(StringComparer.Ordinal);

        foreach (var path in Directory.EnumerateFiles(
                     runtimeRoot,
                     "*.cs",
                     SearchOption.AllDirectories))
        {
            if (string.Equals(
                    Path.GetFileName(path),
                    "Closure.cs",
                    StringComparison.Ordinal))
            {
                continue;
            }

            var source = File.ReadAllText(path);
            foreach (Match match in FixedDispatcherRegex().Matches(source))
            {
                Increment(distribution, match.Groups["arity"].Value);
            }

            var arbitraryCount = ArbitraryDispatcherRegex().Matches(source).Count;
            if (arbitraryCount > 0)
            {
                distribution["arbitrary"] =
                    distribution.GetValueOrDefault("arbitrary") + arbitraryCount;
            }
        }

        return distribution;
    }

    private static string DescribeArguments(NodeList<Expression> arguments)
    {
        if (arguments.Any(static argument => argument is SpreadElement))
        {
            return "spread";
        }

        return arguments.Count <= 5
            ? arguments.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : "6+";
    }

    private static void Increment(IDictionary<string, int> distribution, string key)
    {
        distribution.TryGetValue(key, out var count);
        distribution[key] = count + 1;
    }

    private static void WriteDistribution(
        string title,
        IReadOnlyDictionary<string, int> distribution)
    {
        var total = distribution.Values.Sum();
        Console.WriteLine();
        Console.WriteLine($"{title} (total {total}):");
        foreach (var (arity, count) in distribution)
        {
            var percentage = total == 0 ? 0d : count * 100d / total;
            Console.WriteLine($"  {arity,9}: {count,5} ({percentage,6:F2}%)");
        }
    }

    private static string FindRepositoryRoot()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var current = new DirectoryInfo(start);
            while (current is not null)
            {
                if (File.Exists(Path.Combine(current.FullName, "package.json"))
                    && Directory.Exists(Path.Combine(current.FullName, "src", "JavaScriptRuntime")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the JROC repository root.");
    }

    [GeneratedRegex(@"Closure\.InvokeWithArgs(?<arity>[0-5])\s*\(")]
    private static partial Regex FixedDispatcherRegex();

    [GeneratedRegex(@"Closure\.InvokeWithArgs\s*\(")]
    private static partial Regex ArbitraryDispatcherRegex();
}
