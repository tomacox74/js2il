using Acornima.Ast;
using Js2IL.SymbolTables;
using System.Collections.Generic;

namespace Js2IL;

public sealed class ModuleDefinition
{
    // Module definition details would go here
    public required Acornima.Ast.Program Ast { get; set; }
    public required string Path { get; set; }

    /// <summary>
    /// Canonical logical module id used for runtime/hosting lookup (e.g. "calculator/index", "turndown/lib/index").
    /// This should be path-like (forward slashes), without a ".js" extension.
    /// </summary>
    public required string ModuleId { get; set; }

    /// <summary>
    /// Additional module ids that should resolve to this module (aliases).
    /// Example: requiring "pkg" may resolve to canonical "pkg/lib/index".
    /// </summary>
    public List<string> AliasModuleIds { get; } = new();

    /// <summary>
    /// Stable CLR type name for the module root type (used in generated type names).
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// CLR namespace for the generated module root type.
    /// When null/empty, the compiler falls back to "Modules" for local modules
    /// and "Packages" for npm package modules.
    /// </summary>
    public string? ClrNamespace { get; set; }

    /// <summary>
    /// CLR type name for the generated module root type.
    /// When null/empty, the compiler falls back to <see cref="Name"/>.
    /// </summary>
    public string? ClrTypeName { get; set; }

    /// <summary>
    /// True when this module originates from an npm package under node_modules.
    /// Package modules are emitted under a separate CLR namespace to avoid collisions.
    /// </summary>
    public bool IsPackageModule { get; set; }

    public SymbolTable? SymbolTable { get; set; }
}

public sealed class Modules
{
    public required ModuleDefinition rootModule;

    public Dictionary<string, ModuleDefinition> _modules = new ();
}