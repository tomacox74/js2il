using Acornima.Ast;

namespace Jroc.Services;

public interface IParser
{
    Acornima.Ast.Program ParseJavaScript(string source, string sourceFile);
    Acornima.Ast.Program ParseJavaScriptModule(string source, string sourceFile)
        => throw new NotSupportedException("This parser does not support the JavaScript Module parse goal.");
    void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor);
} 