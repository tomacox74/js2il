public class CompilerOptions
{
    public string? OutputDirectory { get; set; } = null;
    public bool Verbose { get; set; } = false;
    public string? DiagnosticFilePath { get; set; } = null;
    public bool AnalyzeUnused { get; set; } = false;    
    public bool DiagnosticsEnabled => Verbose || !string.IsNullOrWhiteSpace(DiagnosticFilePath);

    /// <summary>
    /// When true, emits Portable PDB debug symbols alongside the generated assembly.
    /// </summary>
    public bool EmitPdb { get; set; } = false;

    /// <summary>
    /// When true, JS2IL emits strongly-typed .NET contracts for CommonJS <c>module.exports</c>
    /// into the compiled assembly for hosting via <see cref="Js2IL.Runtime.JsEngine"/>.
    /// </summary>
    public bool GenerateModuleExportContracts { get; set; } = true;

    /// <summary>
    /// Computed per compilation. When true, the compiler emits a runtime prologue that enables
    /// prototype-chain behavior. This is automatically determined based on whether the code
    /// uses prototype-related features (e.g. __proto__, Object.getPrototypeOf, Object.setPrototypeOf).
    /// </summary>
    public bool PrototypeChainEnabled { get; internal set; } = false;
}
