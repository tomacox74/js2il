using Acornima.Ast;
using Js2IL.DebugSymbols;
using Js2IL.SymbolTables;
using System;
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

    /// <summary>
    /// Compile-time resolved JavaScript module dependencies that should be loaded for this module.
    /// </summary>
    public List<ModuleDependency> Dependencies { get; } = new();

    /// <summary>
    /// Explicit module-record metadata extracted before static import/export lowering.
    /// This is used for graph-driven link/evaluate planning.
    /// This may remain null for legacy/common cases until the loader materializes a record.
    /// </summary>
    public ModuleRecord? ModuleRecord { get; set; }

    public SymbolTable? SymbolTable { get; set; }

    public Dictionary<Node, SourceSpan> DebugSequencePointOverrides { get; } = new(ReferenceEqualityComparer.Instance);
}

public sealed class ModuleDependency
{
    public required string Request { get; set; }

    public required string ResolvedPath { get; set; }

    public string? RequestedAliasModuleId { get; set; }
}

public enum ModuleLinkPhase
{
    Unlinked,
    Linking,
    Linked,
    LinkError
}

public enum ModuleEvaluationPhase
{
    Unevaluated,
    Planned,
    Evaluating,
    Evaluated,
    EvaluationError
}

public enum ModuleImportKind
{
    SideEffect,
    Default,
    Named,
    Namespace
}

public enum ModuleExportKind
{
    Local,
    Default,
    Indirect,
    Star,
    Namespace
}

public sealed class ModuleRequestRecord
{
    public required string Specifier { get; set; }

    public required string ResolvedPath { get; set; }
}

public sealed class ModuleImportEntry
{
    public required ModuleImportKind Kind { get; set; }

    public required string ModuleRequest { get; set; }

    public string? LocalName { get; set; }

    public string? ImportName { get; set; }
}

public sealed class ModuleExportEntry
{
    public required ModuleExportKind Kind { get; set; }

    public required string ExportName { get; set; }

    public string? LocalName { get; set; }

    public string? ModuleRequest { get; set; }
}

public sealed class ModuleResolvedExport
{
    public required string ExportName { get; set; }

    public required ModuleDefinition TargetModule { get; set; }

    public required string BindingName { get; set; }

    public required ModuleExportKind Kind { get; set; }
}

public sealed class ModuleRecord
{
    public List<ModuleRequestRecord> RequestedModules { get; } = new();

    public List<ModuleImportEntry> ImportEntries { get; } = new();

    public List<ModuleExportEntry> LocalExportEntries { get; } = new();

    public List<ModuleExportEntry> IndirectExportEntries { get; } = new();

    public List<ModuleExportEntry> StarExportEntries { get; } = new();

    public bool HasStaticModuleSyntax { get; set; }

    public Dictionary<string, ModuleResolvedExport> ResolvedExports { get; } = new(StringComparer.Ordinal);

    public List<string> LinkErrors { get; } = new();

    public ModuleLinkPhase LinkPhase { get; set; } = ModuleLinkPhase.Unlinked;

    public ModuleEvaluationPhase EvaluationPhase { get; set; } = ModuleEvaluationPhase.Unevaluated;

    public int EvaluationOrder { get; set; } = -1;

    public int EvaluationComponent { get; set; } = -1;
}

public sealed class Modules
{
    public required ModuleDefinition rootModule;

    public Dictionary<string, ModuleDefinition> _modules = new ();
}
