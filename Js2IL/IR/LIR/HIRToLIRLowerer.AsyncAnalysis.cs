using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    /// <summary>
    /// Counts the number of await expressions in an HIR block.
    /// Used to determine if an async function needs full state machine support.
    /// </summary>
    private static int CountAwaitExpressions(HIRBlock block)
    {
        int count = 0;
        foreach (var statement in block.Statements)
        {
            count += CountAwaitExpressionsInStatement(statement);
        }
        return count;
    }

    private static int CountAwaitExpressionsInStatement(HIRStatement statement)
    {
        int count = 0;
        switch (statement)
        {
            case HIRExpressionStatement exprStmt:
                count += CountAwaitExpressionsInExpression(exprStmt.Expression);
                break;
            case HIRVariableDeclaration varDecl:
                if (varDecl.Initializer != null)
                    count += CountAwaitExpressionsInExpression(varDecl.Initializer);
                break;
            case HIRReturnStatement returnStmt:
                if (returnStmt.Expression != null)
                    count += CountAwaitExpressionsInExpression(returnStmt.Expression);
                break;
            case HIRIfStatement ifStmt:
                count += CountAwaitExpressionsInExpression(ifStmt.Test);
                count += CountAwaitExpressionsInStatement(ifStmt.Consequent);
                if (ifStmt.Alternate != null)
                    count += CountAwaitExpressionsInStatement(ifStmt.Alternate);
                break;
            case HIRWhileStatement whileStmt:
                count += CountAwaitExpressionsInExpression(whileStmt.Test);
                count += CountAwaitExpressionsInStatement(whileStmt.Body);
                break;
            case HIRForStatement forStmt:
                if (forStmt.Init != null)
                    count += CountAwaitExpressionsInStatement(forStmt.Init);
                if (forStmt.Test != null)
                    count += CountAwaitExpressionsInExpression(forStmt.Test);
                if (forStmt.Update != null)
                    count += CountAwaitExpressionsInExpression(forStmt.Update);
                count += CountAwaitExpressionsInStatement(forStmt.Body);
                break;
            case HIRForOfStatement forOfStmt:
                // for-await-of has an implicit await on IteratorNext.
                if (forOfStmt.IsAwait)
                {
                    // Lowering also includes an awaited AsyncIteratorClose on abrupt completion.
                    count += 2;
                }
                count += CountAwaitExpressionsInExpression(forOfStmt.Iterable);
                count += CountAwaitExpressionsInStatement(forOfStmt.Body);
                break;
            case HIRForInStatement forInStmt:
                count += CountAwaitExpressionsInExpression(forInStmt.Enumerable);
                count += CountAwaitExpressionsInStatement(forInStmt.Body);
                break;
            case HIRTryStatement tryStmt:
                count += CountAwaitExpressionsInStatement(tryStmt.TryBlock);
                if (tryStmt.CatchBody != null)
                    count += CountAwaitExpressionsInStatement(tryStmt.CatchBody);
                if (tryStmt.FinallyBody != null)
                    count += CountAwaitExpressionsInStatement(tryStmt.FinallyBody);
                break;
            case HIRBlock blockStmt:
                count += CountAwaitExpressions(blockStmt);
                break;
            case HIRThrowStatement throwStmt:
                count += CountAwaitExpressionsInExpression(throwStmt.Argument);
                break;
        }
        return count;
    }

    private static int CountAwaitExpressionsInExpression(HIRExpression expression)
    {
        int count = 0;
        switch (expression)
        {
            case HIRAwaitExpression awaitExpr:
                count = 1; // Found one!
                count += CountAwaitExpressionsInExpression(awaitExpr.Argument);
                break;
            case HIRBinaryExpression binExpr:
                count += CountAwaitExpressionsInExpression(binExpr.Left);
                count += CountAwaitExpressionsInExpression(binExpr.Right);
                break;
            case HIRUnaryExpression unaryExpr:
                count += CountAwaitExpressionsInExpression(unaryExpr.Argument);
                break;
            case HIRCallExpression callExpr:
                count += CountAwaitExpressionsInExpression(callExpr.Callee);
                foreach (var arg in callExpr.Arguments)
                    count += CountAwaitExpressionsInExpression(arg);
                break;
            case HIRPropertyAccessExpression propAccessExpr:
                count += CountAwaitExpressionsInExpression(propAccessExpr.Object);
                break;
            case HIRConditionalExpression condExpr:
                count += CountAwaitExpressionsInExpression(condExpr.Test);
                count += CountAwaitExpressionsInExpression(condExpr.Consequent);
                count += CountAwaitExpressionsInExpression(condExpr.Alternate);
                break;
            case HIRArrayExpression arrayExpr:
                foreach (var elem in arrayExpr.Elements)
                    if (elem != null)
                        count += CountAwaitExpressionsInExpression(elem);
                break;
            case HIRObjectExpression objExpr:
                foreach (var member in objExpr.Members)
                {
                    switch (member)
                    {
                        case HIRObjectProperty prop:
                            count += CountAwaitExpressionsInExpression(prop.Value);
                            break;
                        case HIRObjectComputedProperty computed:
                            count += CountAwaitExpressionsInExpression(computed.KeyExpression);
                            count += CountAwaitExpressionsInExpression(computed.Value);
                            break;
                        case HIRObjectSpreadProperty spread:
                            count += CountAwaitExpressionsInExpression(spread.Argument);
                            break;
                        default:
                            throw new NotSupportedException($"Unhandled object literal member type in await counter: {member.GetType().FullName}");
                    }
                }
                break;
            case HIRAssignmentExpression assignExpr:
                count += CountAwaitExpressionsInExpression(assignExpr.Value);
                break;
            case HIRNewExpression newExpr:
                count += CountAwaitExpressionsInExpression(newExpr.Callee);
                foreach (var arg in newExpr.Arguments)
                    count += CountAwaitExpressionsInExpression(arg);
                break;
        }
        return count;
    }
}
