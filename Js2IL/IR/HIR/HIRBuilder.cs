using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Acornima.Ast;
using Js2IL.Services;

using Js2IL.SymbolTables;

namespace Js2IL.HIR;
public static class HIRBuilder
{
    /// <summary>
    /// Attempts to parse a method from the AST node.
    /// Failure is not an error, just falls back to an older legacy IL emitter.
    /// TODO: Expand to make this the required path for IL emission.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool TryParseMethod([In, NotNull] Acornima.Ast.Node node, [In, NotNull] Scope scope, out HIRMethod? method)
    {
        switch (node)
        {
            case Acornima.Ast.Program programAst:
                var builder = new HIRMethodBuilder(scope);
                return builder.TryParseStatements(programAst.Body, out method);
            case Acornima.Ast.BlockStatement blockStmt:
                // Parse block statements by processing their body statements
                var blockBuilder = new HIRMethodBuilder(scope);
                return blockBuilder.TryParseStatements(blockStmt.Body, out method);
            case Acornima.Ast.MethodDefinition classMethodDef:
                var methodFuncExpr = classMethodDef.Value as FunctionExpression;                
                var methodBuilder = new HIRMethodBuilder(scope);
                return methodBuilder.TryParseStatements(methodFuncExpr.Body.Body, out method);
            case Acornima.Ast.ArrowFunctionExpression arrowFunc:
                // IR pipeline doesn't yet handle parameters or scope arrays
                // Fall back to legacy emitter for arrow functions with parameters
                if (arrowFunc.Params.Count > 0)
                {
                    method = null!;
                    return false;
                }
                // Concise (expression-body) arrow functions fall back to legacy emitter
                // (implicit return wrapping not yet implemented in HIR parser)
                if (arrowFunc.Body is not BlockStatement arrowBlock)
                {
                    method = null!;
                    return false;
                }
                var arrowBuilder = new HIRMethodBuilder(scope);
                return arrowBuilder.TryParseStatements(arrowBlock.Body, out method);
            case Acornima.Ast.FunctionExpression funcExpr:
                // FunctionExpression is used for class constructors and method values
                // IR pipeline doesn't yet handle parameters
                if (funcExpr.Params.Count > 0)
                {
                    method = null!;
                    return false;
                }
                if (funcExpr.Body is not BlockStatement funcBlock)
                {
                    method = null!;
                    return false;
                }
                var funcExprBuilder = new HIRMethodBuilder(scope);
                return funcExprBuilder.TryParseStatements(funcBlock.Body, out method);
            // Handle other node types as needed
            default:
                method = null!;
                return false;
        }
    }
}

class HIRMethodBuilder
{
    readonly Scope _scope;
    readonly List<HIRStatement> _statements = new();

    public HIRMethodBuilder(Scope scope)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));
        _scope = scope;
    }

    public bool TryParseStatements([In, NotNull] IEnumerable<Acornima.Ast.Statement> statements, out HIRMethod? method)
    {
        method = null;

        foreach (var statement in statements)
        {
            // Process each statement in the program body
            switch (statement)
            {
                case VariableDeclaration declStmt:
                    foreach (var decl in declStmt.Declarations)
                    {
                        if (!TryAddDeclarator(decl))
                        {
                            return false;
                        }
                    }
                    
                    break;
                case ExpressionStatement exprStmt:
                    if (!TryParseExpression(exprStmt.Expression, out var hirExpr))
                    {
                        return false;
                    }
                    var hirExprStmt = new HIRExpressionStatement(hirExpr!);
                    _statements.Add(hirExprStmt);
                    break;
                // Handle other statement types as needed
                default:
                    // Unsupported statement type
                    return false;
            }
        }
        method = new HIRMethod
        {
            Body = new HIRBlock(_statements)
        };

        return true;
    }

    private bool TryAddDeclarator(Acornima.Ast.VariableDeclarator decl)
    {
        if (decl.Id is Identifier id)
        {
            var symbol = _scope.FindSymbol(id.Name);
            if (decl.Init != null)
            {
                if (!TryParseExpression(decl.Init, out var hirInitExpr))
                {
                    return false;
                }

                var hirVarDeclWithInit = new HIRVariableDeclaration(symbol, hirInitExpr);
                _statements.Add(hirVarDeclWithInit);
                return true;
            }
            var hirVarDecl = new HIRVariableDeclaration(symbol);
            _statements.Add(hirVarDecl);
            return true;
        }

        return false;
    }

    private bool TryParseExpression(Acornima.Ast.Expression? expr, out HIRExpression? hirExpr)
    {
        hirExpr = null;

        switch (expr)
        {
            case BinaryExpression binaryExpr:
                // Handle binary expressions
                HIRExpression? leftExpr;
                HIRExpression? rightExpr;

                if (!TryParseExpression(binaryExpr.Left, out leftExpr) ||
                    !TryParseExpression(binaryExpr.Right, out rightExpr))
                {
                    return false;
                }

                hirExpr = new HIRBinaryExpression(binaryExpr.Operator, leftExpr!, rightExpr!);
                return true;

            case CallExpression callExpr:
                // Handle call expressions
                HIRExpression? calleeExpr;
                var argExprs = new List<HIRExpression>();

                if (!TryParseExpression(callExpr.Callee, out calleeExpr))
                {
                    return false;
                }

                foreach (var arg in callExpr.Arguments)
                {
                    if (!TryParseExpression(arg, out var argHirExpr))
                    {
                        return false;
                    }
                    argExprs.Add(argHirExpr!);
                }

                hirExpr = new HIRCallExpression(calleeExpr!, argExprs);
                return true;
            case Identifier identifierExpr:
                var symbol = _scope.FindSymbol(identifierExpr.Name);
                hirExpr = new HIRVariableExpression(symbol);
                return true;
            case MemberExpression memberExpr:
                // Handle member expressions
                HIRExpression? objectExpr;
                if (!TryParseExpression(memberExpr.Object, out objectExpr))
                {
                    return false;
                }

                if (memberExpr.Property is not Identifier propertyIdentifier)
                {
                    return false; // Only support identifier properties for now
                }

                hirExpr = new HIRPropertyAccessExpression(objectExpr!, propertyIdentifier.Name);
                return true;

            case NumericLiteral literalExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.Number, literalExpr.Value);
                return true;
            case StringLiteral stringLiteralExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.String, stringLiteralExpr.Value);
                return true;
            case BooleanLiteral booleanLiteralExpr:
                hirExpr = new HIRLiteralExpression(JavascriptType.Boolean, booleanLiteralExpr.Value);
                return true;
            // Handle other expression types as needed
            default:
                return false;
        }
    }
}