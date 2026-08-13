namespace Jroc.SymbolTables;

/// <summary>
/// Shared synthetic binding names used by native static ES module linking.
/// </summary>
public static class EsModuleNames
{
    /// <summary>
    /// Synthetic module-scope binding that holds the value of an <c>export default &lt;expression&gt;</c>.
    /// The name uses characters invalid in a JavaScript identifier so it can never collide with a
    /// user-declared binding.
    /// </summary>
    public const string SyntheticDefault = "*default*";

    /// <summary>
    /// Builds the hidden module-scope binding name that stores the required source module object for a
    /// given (request-normalized) specifier. Multiple imports from the same specifier share one binding.
    /// </summary>
    public static string SourceBindingName(string specifier) => "*esm-src:" + specifier;
}

/// <summary>
/// Ordered plan describing how a native static ES module links its imports and exports at runtime.
/// The plan is attached to the module's global scope and consumed by the lowerer to emit the module
/// main prologue (local export registration, <c>__esModule</c> marking, source module requires, and
/// default/namespace import initialization). Named import reads and export write-mirroring are driven
/// by <see cref="BindingInfo.EsModuleImport"/>/<see cref="BindingInfo.EsModuleExports"/> and do not
/// appear in this plan.
/// </summary>
public sealed class EsModuleLinkPlan
{
    public required string ModuleId { get; init; }

    /// <summary>
    /// Exported names in source order. Each becomes a <c>RegisterLocalExport</c> call so a live,
    /// enumerable accessor over the backing binding cell is installed on the module's exports object.
    /// </summary>
    public List<string> LocalExportNames { get; } = new();

    /// <summary>
    /// Distinct source module requests in source order. Each is required once and stored into a hidden
    /// module-scope binding shared by every import from that request.
    /// </summary>
    public List<EsModuleSourceRequire> Requires { get; } = new();

    /// <summary>
    /// Default/namespace import bindings that receive a single value at module initialization.
    /// Named imports are excluded because their reads are intercepted live.
    /// </summary>
    public List<EsModuleImportInit> ImportInits { get; } = new();

    /// <summary>
    /// Re-exports of named imports (<c>export { imported }</c>) and indirect exports
    /// (<c>export { imported } from "mod"</c>). These install a live forwarding accessor on the exports
    /// object that reads the source module property on every access, rather than a cell-backed local
    /// export (a named import never writes its own binding cell).
    /// </summary>
    public List<EsModuleReexport> Reexports { get; } = new();

    /// <summary>
    /// Namespace re-exports (<c>export * as ns from "mod"</c>). Each installs a live accessor exposing the
    /// (stable) namespace object of the required source module.
    /// </summary>
    public List<EsModuleNamespaceReexport> NamespaceReexports { get; } = new();

    /// <summary>
    /// Star re-exports (<c>export * from "mod"</c>). At module initialization each enumerates the source
    /// module's own enumerable keys and installs forwarding accessors (excluding <c>default</c> and any
    /// name already exported locally, which takes precedence).
    /// </summary>
    public List<EsModuleStarReexport> StarReexports { get; } = new();
}

/// <summary>
/// A single source module require: the (already request-normalized) specifier and the hidden
/// module-scope binding that stores the required module object.
/// </summary>
public sealed class EsModuleSourceRequire
{
    public required string Specifier { get; init; }

    public required BindingInfo SourceBinding { get; init; }
}

/// <summary>
/// A default or namespace import binding initialized once from a required source module object.
/// </summary>
public sealed class EsModuleImportInit
{
    public required BindingInfo LocalBinding { get; init; }

    public required BindingInfo SourceBinding { get; init; }

    public required EsModuleImportKind Kind { get; init; }
}

/// <summary>
/// A re-export of a named import (<c>export { imported as exported }</c>) or an indirect export
/// (<c>export { imported as exported } from "mod"</c>). The exports object exposes a live accessor that
/// forwards to the source module's property so importers observe live values.
/// </summary>
public sealed class EsModuleReexport
{
    public required string ExportName { get; init; }

    public required BindingInfo SourceBinding { get; init; }

    public required string ImportName { get; init; }
}

/// <summary>
/// A namespace re-export (<c>export * as ns from "mod"</c>). The exports object exposes a live accessor
/// resolving the (stable) namespace object of the required source module.
/// </summary>
public sealed class EsModuleNamespaceReexport
{
    public required string ExportName { get; init; }

    public required BindingInfo SourceBinding { get; init; }
}

/// <summary>
/// A star re-export (<c>export * from "mod"</c>). At module initialization the required source module's
/// own enumerable keys are enumerated and forwarding accessors installed for each (excluding
/// <c>default</c> and names already exported locally).
/// </summary>
public sealed class EsModuleStarReexport
{
    public required BindingInfo SourceBinding { get; init; }
}
