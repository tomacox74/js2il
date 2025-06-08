using Acornima;
using Acornima.Ast;

namespace Js2IL.Services;

public class JavaScriptParser : IParser
{
    private readonly Parser _parser;

    public JavaScriptParser()
    {
        var parserOptions = new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            AllowReturnOutsideFunction = true
        };
        _parser = new Parser(parserOptions);
    }

    public Acornima.Ast.Program ParseJavaScript(string source)
    {
        try
        {
            return _parser.ParseScript(source);
        }
        catch (ParseErrorException ex)
        {
            throw new Exception($"Failed to parse JavaScript: {ex.Message}", ex);
        }
    }

    public void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor)
    {
        var walker = new AstWalker();
        walker.Visit(ast, visitor);
    }
} 