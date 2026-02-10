using System.Linq;
using Js2IL.HIR;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private static int CountYieldExpressionsInStatement(HIRStatement statement)
    {
        int count = 0;
        switch (statement)
        {
            case HIRExpressionStatement exprStmt:
                count += CountYieldExpressionsInExpression(exprStmt.Expression);
                break;
            case HIRVariableDeclaration varDecl:
                if (varDecl.Initializer != null)
                    count += CountYieldExpressionsInExpression(varDecl.Initializer);
                break;
            case HIRReturnStatement returnStmt:
                if (returnStmt.Expression != null)
                    count += CountYieldExpressionsInExpression(returnStmt.Expression);
                break;
            case HIRIfStatement ifStmt:
                count += CountYieldExpressionsInExpression(ifStmt.Test);
                count += CountYieldExpressionsInStatement(ifStmt.Consequent);
                if (ifStmt.Alternate != null)
                    count += CountYieldExpressionsInStatement(ifStmt.Alternate);
                break;
            case HIRWhileStatement whileStmt:
                count += CountYieldExpressionsInExpression(whileStmt.Test);
                count += CountYieldExpressionsInStatement(whileStmt.Body);
                break;
            case HIRForStatement forStmt:
                if (forStmt.Init != null)
                    count += CountYieldExpressionsInStatement(forStmt.Init);
                if (forStmt.Test != null)
                    count += CountYieldExpressionsInExpression(forStmt.Test);
                if (forStmt.Update != null)
                    count += CountYieldExpressionsInExpression(forStmt.Update);
                count += CountYieldExpressionsInStatement(forStmt.Body);
                break;
            case HIRForOfStatement forOfStmt:
                count += CountYieldExpressionsInExpression(forOfStmt.Iterable);
                count += CountYieldExpressionsInStatement(forOfStmt.Body);
                break;
            case HIRForInStatement forInStmt:
                count += CountYieldExpressionsInExpression(forInStmt.Enumerable);
                count += CountYieldExpressionsInStatement(forInStmt.Body);
                break;
            case HIRTryStatement tryStmt:
                count += CountYieldExpressionsInStatement(tryStmt.TryBlock);
                if (tryStmt.CatchBody != null)
                    count += CountYieldExpressionsInStatement(tryStmt.CatchBody);
                if (tryStmt.FinallyBody != null)
                    count += CountYieldExpressionsInStatement(tryStmt.FinallyBody);
                break;
            case HIRBlock blockStmt:
                foreach (var s in blockStmt.Statements)
                    count += CountYieldExpressionsInStatement(s);
                break;
            case HIRThrowStatement throwStmt:
                count += CountYieldExpressionsInExpression(throwStmt.Argument);
                break;
        }
        return count;
    }

    private static int CountYieldExpressionsInExpression(HIRExpression expression)
    {
        int count = 0;
        switch (expression)
        {
            case HIRYieldExpression yieldExpr:
                count = 1;
                if (yieldExpr.Argument != null)
                    count += CountYieldExpressionsInExpression(yieldExpr.Argument);
                break;
            case HIRAwaitExpression awaitExpr:
                count += CountYieldExpressionsInExpression(awaitExpr.Argument);
                break;
            case HIRBinaryExpression binExpr:
                count += CountYieldExpressionsInExpression(binExpr.Left);
                count += CountYieldExpressionsInExpression(binExpr.Right);
                break;
            case HIRUnaryExpression unaryExpr:
                count += CountYieldExpressionsInExpression(unaryExpr.Argument);
                break;
            case HIRCallExpression callExpr:
                count += CountYieldExpressionsInExpression(callExpr.Callee);
                foreach (var arg in callExpr.Arguments)
                    count += CountYieldExpressionsInExpression(arg);
                break;
            case HIRPropertyAccessExpression propAccessExpr:
                count += CountYieldExpressionsInExpression(propAccessExpr.Object);
                break;
            case HIRConditionalExpression condExpr:
                count += CountYieldExpressionsInExpression(condExpr.Test);
                count += CountYieldExpressionsInExpression(condExpr.Consequent);
                count += CountYieldExpressionsInExpression(condExpr.Alternate);
                break;
            case HIRArrayExpression arrayExpr:
                foreach (var elem in arrayExpr.Elements.Where(static e => e != null))
                    count += CountYieldExpressionsInExpression(elem!);
                break;
            case HIRObjectExpression objExpr:
                foreach (var member in objExpr.Members)
                {
                    switch (member)
                    {
                        case HIRObjectProperty prop:
                            count += CountYieldExpressionsInExpression(prop.Value);
                            break;
                        case HIRObjectComputedProperty computed:
                            count += CountYieldExpressionsInExpression(computed.KeyExpression);
                            count += CountYieldExpressionsInExpression(computed.Value);
                            break;
                    }
                }
                break;
        }
        return count;
    }
}
