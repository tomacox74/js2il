using Acornima.Ast;

namespace Jroc.Services;

public interface IParser
{
    Acornima.Ast.Program ParseJavaScript(string source, string sourceFile);
    void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor);
} 