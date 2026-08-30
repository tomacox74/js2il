namespace Benchmarks;

internal static class JrocBenchmarkLabels
{
#if JROC_PREVIOUS
    public const string RuntimeName = "JrocPrevious";
    public const string CompileAndExecute = "JrocPrevious (compile+execute)";
    public const string Execute = "jroc-previous-execute";
#else
    public const string RuntimeName = "jroc";
    public const string CompileAndExecute = "jroc (compile+execute)";
    public const string Execute = "jroc-execute";
#endif
}
