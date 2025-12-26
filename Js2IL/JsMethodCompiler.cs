using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.SymbolTables;

namespace Js2IL;

/// <summary>
/// Per method compiling from JS to IL
/// </summary>
/// <remarks>
/// AST -> HIR -> LIR -> IL
/// </remarks>
public class JsMethodCompiler
{
    static public bool TryCompileMethod(Node node, Scope scope, out int bodyOffset)
    {
        bodyOffset = -1;

        if (!HIRBuilder.TryParseMethod(node, scope, out var hirMethod))
        {
            return false;
        }
 
        if (!HIRToLIRLowerer.TryLower(hirMethod!))
        {
            return false;
        }

        return true;
    }
}