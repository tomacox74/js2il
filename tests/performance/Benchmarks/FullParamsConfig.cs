using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

namespace Benchmarks;

public sealed class FullParamsConfig : ManualConfig
{
    public FullParamsConfig()
    {
        SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(200);
    }
}
