public class CompilerOptions
{
    public string? OutputDirectory { get; set; } = null;
    public bool Verbose { get; set; } = false;
    public bool AnalyzeUnused { get; set; } = false;
    
    /// <summary>
    /// Enable two-phase compilation: Phase 1 declares all callables upfront,
    /// Phase 2 compiles bodies (enabling forward references and mutual recursion).
    /// </summary>
    public bool TwoPhaseCompilation { get; set; } = false;
}