using Acornima.Ast;
using Js2IL.SymbolTables;
using System.Collections.Generic;

namespace Js2IL;

public sealed class ModuleDefinition
{
    // Module definition details would go here
    public required Acornima.Ast.Program Ast { get; set; }
    public required string Path { get; set; }

    // Stable module id used for generated type names (e.g. Modules.<Name>, Scopes.<Name>).
    // Must match JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdFromSpecifier at runtime.
    public required string Name { get; set; }

    public SymbolTable? SymbolTable { get; set; }
}

public sealed class Modules
{
    public required ModuleDefinition rootModule;

    public Dictionary<string, ModuleDefinition> _modules = new ();
}