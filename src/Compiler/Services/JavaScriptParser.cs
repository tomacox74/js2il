using Acornima;
using Acornima.Ast;
using Jroc.Utilities;

namespace Jroc.Services;

public class JavaScriptParser : IParser
{
    private readonly Parser _scriptParser;
    private readonly Parser _topLevelAwaitParser;
    private readonly Parser _moduleParser;

    public JavaScriptParser()
    {
        _scriptParser = new Parser(CreateParserOptions(allowAwaitOutsideFunction: false));
        _topLevelAwaitParser = new Parser(CreateParserOptions(allowAwaitOutsideFunction: true));
        _moduleParser = new Parser(new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            ExperimentalESFeatures = ExperimentalESFeatures.Decorators
        });
    }

    private static ParserOptions CreateParserOptions(bool allowAwaitOutsideFunction)
    {
        return new ParserOptions
        {
            EcmaVersion = EcmaVersion.Latest,
            ExperimentalESFeatures = ExperimentalESFeatures.Decorators,
            AllowReturnOutsideFunction = true,
            AllowImportExportEverywhere = true,
            AllowAwaitOutsideFunction = allowAwaitOutsideFunction
        };
    }

    public Acornima.Ast.Program ParseJavaScript(string source, string sourceFile)
    {
        try
        {
            return ClassAutoAccessorNormalizer.Normalize(_scriptParser.ParseScript(source, sourceFile));
        }
        catch (ParseErrorException ex)
        {
            try
            {
                return ClassAutoAccessorNormalizer.Normalize(_topLevelAwaitParser.ParseScript(source, sourceFile));
            }
            catch (ParseErrorException)
            {
                throw new Exception($"Failed to parse JavaScript: {ex.Message}", ex);
            }
        }
    }

    public Acornima.Ast.Program ParseJavaScriptModule(string source, string sourceFile)
    {
        try
        {
            return ClassAutoAccessorNormalizer.Normalize(_moduleParser.ParseModule(source, sourceFile));
        }
        catch (ParseErrorException ex)
        {
            throw new Exception($"Failed to parse JavaScript module: {ex.Message}", ex);
        }
    }

    public void VisitAst(Acornima.Ast.Program ast, Action<Node> visitor)
    {
        var walker = new AstWalker();
        walker.Visit(ast, visitor);
    }
} 