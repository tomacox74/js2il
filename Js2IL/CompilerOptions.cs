public enum PrototypeChainMode
{
    /// <summary>
    /// Never enable prototype-chain behavior.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Enable prototype-chain behavior only when the compiler detects prototype-related usage
    /// (e.g. __proto__, Object.getPrototypeOf, Object.setPrototypeOf).
    /// </summary>
    Auto = 1,

    /// <summary>
    /// Always enable prototype-chain behavior.
    /// </summary>
    On = 2
}

public enum StrictModeDirectivePrologueMode
{
    /// <summary>
    /// Missing "use strict" is a validation error.
    /// </summary>
    Error = 0,

    /// <summary>
    /// Missing "use strict" is downgraded to a warning.
    /// </summary>
    Warn = 1,

    /// <summary>
    /// Missing "use strict" is ignored (no error/warning).
    /// </summary>
    Ignore = 2
}

public class CompilerOptions
{
    public string? OutputDirectory { get; set; } = null;
    public bool Verbose { get; set; } = false;
    public bool AnalyzeUnused { get; set; } = false;    

    /// <summary>
    /// Controls how missing strict-mode directive prologues ("use strict"; at the start of a module/script)
    /// are reported.
    /// </summary>
    public StrictModeDirectivePrologueMode StrictMode { get; set; } = StrictModeDirectivePrologueMode.Error;

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
    /// Controls whether prototype-chain semantics are enabled.
    /// Default is <see cref="PrototypeChainMode.Auto"/> to preserve existing behavior unless
    /// a script clearly uses prototype-related features.
    /// </summary>
    public PrototypeChainMode PrototypeChain { get; set; } = PrototypeChainMode.Auto;

    /// <summary>
    /// Computed per compilation. When true, the compiler emits a runtime prologue that enables
    /// prototype-chain behavior.
    /// </summary>
    public bool PrototypeChainEnabled { get; internal set; } = false;
}
