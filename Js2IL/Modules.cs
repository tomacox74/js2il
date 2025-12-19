using Acornima.Ast;
using Js2IL.SymbolTables;
using System.Collections.Generic;

namespace Js2IL;

public class ModuleDefinition
{
    // Module definition details would go here
    public required Acornima.Ast.Program Ast { get; set; }
    public required string Path { get; set; }

    public SymbolTable? SymbolTable { get; set; }
}

public class Modules
{
    public required ModuleDefinition rootModule;

    public Dictionary<string, ModuleDefinition> _modules = new ();
}