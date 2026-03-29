using Acornima.Ast;
using Js2IL.SymbolTables;
using System;
using System.Collections.Generic;

namespace Js2IL.Utilities;

internal static class ArgumentsObjectSemantics
{
    public static bool UsesMappedArgumentsObject(Scope functionScope)
    {
        ArgumentNullException.ThrowIfNull(functionScope);

        if (functionScope.Kind != ScopeKind.Function || functionScope.AstNode is ArrowFunctionExpression)
        {
            return false;
        }

        return HasSimpleParameterList(functionScope.AstNode) && !IsStrictScope(functionScope);
    }

    public static string[] GetMappedParameterNames(Scope functionScope)
    {
        ArgumentNullException.ThrowIfNull(functionScope);
        return UsesMappedArgumentsObject(functionScope)
            ? GetOrderedSimpleParameterNames(functionScope.AstNode)
            : [];
    }

    public static bool IsStrictScope(Scope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        if (scope.Kind == ScopeKind.Class)
        {
            return true;
        }

        if (!scope.UsesGlobalScopeSemantics && scope.Parent != null && IsStrictScope(scope.Parent))
        {
            return true;
        }

        return HasUseStrictDirective(scope.AstNode);
    }

    private static bool HasSimpleParameterList(Node functionNode)
    {
        foreach (var parameter in GetParameters(functionNode))
        {
            if (parameter is not Identifier)
            {
                return false;
            }
        }

        return true;
    }

    private static string[] GetOrderedSimpleParameterNames(Node functionNode)
    {
        var names = new List<string>();
        foreach (var parameter in GetParameters(functionNode))
        {
            if (parameter is not Identifier identifier)
            {
                return [];
            }

            names.Add(identifier.Name);
        }

        return [.. names];
    }

    private static IReadOnlyList<Node> GetParameters(Node functionNode)
    {
        return functionNode switch
        {
            FunctionDeclaration declaration => declaration.Params,
            FunctionExpression expression => expression.Params,
            ArrowFunctionExpression arrow => arrow.Params,
            _ => []
        };
    }

    private static bool HasUseStrictDirective(Node node)
    {
        IEnumerable<Statement> statements = node switch
        {
            Program program => program.Body,
            FunctionDeclaration declaration when declaration.Body is BlockStatement block => block.Body,
            FunctionExpression expression when expression.Body is BlockStatement block => block.Body,
            ArrowFunctionExpression arrow when arrow.Body is BlockStatement block => block.Body,
            _ => []
        };

        foreach (var statement in statements)
        {
            if (statement is not ExpressionStatement expressionStatement
                || expressionStatement.Expression is not Literal literal
                || literal.Value is not string directive)
            {
                break;
            }

            if (string.Equals(directive, "use strict", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
