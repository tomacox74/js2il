using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Js2IL.Services;

namespace Js2IL.Utilities;

internal static class DynamicFunctionSupport
{
    internal const string ScopeNamePrefix = "DynamicFunction_";

    internal static string GetScopeName(Node siteNode)
    {
        var loc = siteNode.Location.Start;
        return $"{ScopeNamePrefix}L{loc.Line}C{loc.Column + 1}";
    }

    internal static bool TryGetStringLiteralArguments(IEnumerable<Node> arguments, out List<string> literalArgs)
    {
        literalArgs = new List<string>();
        foreach (var argument in arguments)
        {
            if (argument is not Literal { Value: string literalValue })
            {
                literalArgs.Clear();
                return false;
            }

            literalArgs.Add(literalValue);
        }

        return true;
    }

    internal static bool TryParseFunctionExpression(
        JavaScriptParser parser,
        string sourceFile,
        IReadOnlyList<string> literalArgs,
        int startLine,
        int startColumn,
        out FunctionExpression? functionExpression,
        out string? errorMessage)
    {
        functionExpression = null;
        errorMessage = null;

        var syntheticSource = BuildSyntheticFunctionExpressionSource(literalArgs, startLine, startColumn);
        try
        {
            var program = parser.ParseJavaScript(syntheticSource, sourceFile);
            if (program.Body.FirstOrDefault() is ExpressionStatement { Expression: FunctionExpression parsedFunction })
            {
                functionExpression = parsedFunction;
                return true;
            }

            errorMessage = "Dynamic Function constructor source did not parse to a function expression.";
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
    }

    internal static bool TryParseFunctionExpression(
        JavaScriptParser parser,
        string sourceFile,
        IReadOnlyList<string> literalArgs,
        out FunctionExpression? functionExpression,
        out string? errorMessage)
    {
        return TryParseFunctionExpression(
            parser,
            sourceFile,
            literalArgs,
            startLine: 1,
            startColumn: 0,
            out functionExpression,
            out errorMessage);
    }

    private static string BuildSyntheticFunctionExpressionSource(
        IReadOnlyList<string> literalArgs,
        int startLine,
        int startColumn)
    {
        var parameterSource = literalArgs.Count > 1
            ? string.Join(",", literalArgs.Take(literalArgs.Count - 1))
            : string.Empty;
        var bodySource = literalArgs.Count == 0 ? string.Empty : literalArgs[^1];

        var linePrefix = startLine > 1 ? new string('\n', startLine - 1) : string.Empty;
        var columnPrefix = startColumn > 0 ? new string(' ', startColumn) : string.Empty;

        return $"{linePrefix}{columnPrefix}(function({parameterSource}) {{\n{bodySource}\n}})";
    }
}
