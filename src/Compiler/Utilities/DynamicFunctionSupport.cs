using System;
using System.Collections.Generic;
using System.Linq;
using Acornima.Ast;
using Jroc.Services;

namespace Jroc.Utilities;

internal static class DynamicFunctionSupport
{
    internal const string ScopeNamePrefix = "<>DynamicFunction_";

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

    internal static bool IsFunctionConstructorCandidate(Node callee)
        => callee is Identifier { Name: "Function" }
            || callee is MemberExpression { Computed: false, Property: Identifier { Name: "Function" } };

    internal static bool TryGetStaticStringArguments(
        IEnumerable<Node> arguments,
        Func<string, Node?> resolveInitializer,
        out List<string> sources)
    {
        sources = new List<string>();
        foreach (var argument in arguments)
        {
            if (!TryGetString(argument, new HashSet<string>(StringComparer.Ordinal), out var source))
            {
                sources.Clear();
                return false;
            }
            sources.Add(source);
        }
        return true;

        bool TryGetString(Node node, HashSet<string> visited, out string source)
        {
            switch (node)
            {
                case Literal { Value: string value }:
                    source = value;
                    return true;
                case TemplateLiteral { Expressions.Count: 0 } template
                    when template.Quasis.Count == 1 && template.Quasis[0].Value.Cooked is string cooked:
                    source = cooked;
                    return true;
                case Identifier identifier when visited.Add(identifier.Name):
                    if (resolveInitializer(identifier.Name) is { } initializer)
                    {
                        return TryGetString(initializer, visited, out source);
                    }
                    break;
            }
            source = string.Empty;
            return false;
        }
    }

    internal static bool TryParseFunctionExpression(
        JavaScriptParser parser,
        string sourceFile,
        IReadOnlyList<string> literalArgs,
        int startLine,
        int startColumn,
        out FunctionExpression? functionExpression,
        out string? errorMessage,
        bool createFactory = false)
    {
        functionExpression = null;
        errorMessage = null;

        var syntheticSource = BuildSyntheticFunctionExpressionSource(literalArgs, startLine, startColumn, createFactory);
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
        catch (Exception ex) when (ex is Acornima.ParseErrorException
            || ex.InnerException is Acornima.ParseErrorException)
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
        int startColumn,
        bool createFactory)
    {
        var parameterSource = literalArgs.Count > 1
            ? string.Join(",", literalArgs.Take(literalArgs.Count - 1))
            : string.Empty;
        var bodySource = literalArgs.Count == 0 ? string.Empty : literalArgs[^1];

        var linePrefix = startLine > 1 ? new string('\n', startLine - 1) : string.Empty;
        var columnPrefix = startColumn > 0 ? new string(' ', startColumn) : string.Empty;

        var functionSource = $"function({parameterSource}) {{\n{bodySource}\n}}";
        // The outer thunk allows the runtime to allocate the function in its constructor's realm.
        return createFactory
            ? $"{linePrefix}{columnPrefix}(function() {{ return {functionSource}; }})"
            : $"{linePrefix}{columnPrefix}({functionSource})";
    }
}
