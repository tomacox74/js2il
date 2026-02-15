using Acornima;
using Acornima.Ast;
using Js2IL.Utilities;

namespace Js2IL.Services;

public class JavaScriptParser : IParser
{
    private readonly Parser _parser;

    public JavaScriptParser()
    {
        var parserOptions = new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            AllowReturnOutsideFunction = true,
            AllowImportExportEverywhere = true,
            // JS2IL executes code via a CommonJS-style module wrapper (not ESM).
            // Top-level await is an ESM feature, so keep it disabled.
            AllowAwaitOutsideFunction = false
        };
        _parser = new Parser(parserOptions);
    }

    public Acornima.Ast.Program ParseJavaScript(string source, string sourceFile)
    {
        try
        {
            return _parser.ParseScript(source, sourceFile);
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