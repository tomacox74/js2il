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
    /// When true, JROC emits strongly-typed .NET contracts for CommonJS <c>module.exports</c>
    /// into the compiled assembly for hosting via <see cref="Jroc.Runtime.JsEngine"/>.
    /// </summary>
    public bool GenerateModuleExportContracts { get; set; } = true;

    /// <summary>
    /// Host-provided runtime intrinsics that should be visible during compilation.
    /// </summary>
    public JavaScriptRuntime.HostRuntimeIntrinsicDescriptors HostRuntimeIntrinsics { get; set; } =
        JavaScriptRuntime.HostRuntimeIntrinsicDescriptors.Empty;

    /// <summary>
    /// Computed per compilation. When true, the compiler emits a runtime prologue that enables
    /// prototype-chain behavior. This is automatically determined based on whether the code
    /// uses prototype-related features (e.g. __proto__, Object.getPrototypeOf, Object.setPrototypeOf).
    /// </summary>
    public bool PrototypeChainEnabled { get; internal set; } = false;
}
