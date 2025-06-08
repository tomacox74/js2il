using Acornima.Ast;

namespace Js2IL.Services;

public interface IParser
{
    Acornima.Ast.Program ParseJavaScript(string source);
    void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor);
} 