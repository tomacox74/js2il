using Acornima;
using Acornima.Ast;
using Js2IL.Utilities;

namespace Js2IL.Services;

public class JavaScriptParser : IParser
{
    private readonly Parser _scriptParser;
    private readonly Parser _topLevelAwaitParser;

    public JavaScriptParser()
    {
        _scriptParser = new Parser(CreateParserOptions(allowAwaitOutsideFunction: false));
        _topLevelAwaitParser = new Parser(CreateParserOptions(allowAwaitOutsideFunction: true));
    }

    private static ParserOptions CreateParserOptions(bool allowAwaitOutsideFunction)
    {
        return new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            AllowReturnOutsideFunction = true,
            AllowImportExportEverywhere = true,
            AllowAwaitOutsideFunction = allowAwaitOutsideFunction
        };
    }

    public Acornima.Ast.Program ParseJavaScript(string source, string sourceFile)
    {
        try
        {
            return _scriptParser.ParseScript(source, sourceFile);
        }
        catch (ParseErrorException ex)
        {
            try
            {
                return _topLevelAwaitParser.ParseScript(source, sourceFile);
            }
            catch (ParseErrorException)
            {
                throw new Exception($"Failed to parse JavaScript: {ex.Message}", ex);
            }
        }
    }

    public void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor)
    {
        var walker = new AstWalker();
        walker.Visit(ast, visitor);
    }
} 