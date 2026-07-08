using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

namespace Benchmarks;

public sealed class FullParamsConfig : ManualConfig
{
    public static bool DebugModeEnabled { get; set; }

    public FullParamsConfig()
    {
        SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);

        if (DebugModeEnabled)
        {
            Options |= ConfigOptions.DisableOptimizationsValidator;
        }
    }
}
