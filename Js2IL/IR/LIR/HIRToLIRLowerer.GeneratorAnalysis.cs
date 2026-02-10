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
            case HIRDestructuringVariableDeclaration destructuringDecl:
                count += CountYieldExpressionsInExpression(destructuringDecl.Initializer);
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
            case HIRDoWhileStatement doWhileStmt:
                count += CountYieldExpressionsInStatement(doWhileStmt.Body);
                count += CountYieldExpressionsInExpression(doWhileStmt.Test);
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
            case HIRSwitchStatement switchStmt:
                count += CountYieldExpressionsInExpression(switchStmt.Discriminant);
                foreach (var @case in switchStmt.Cases)
                {
                    if (@case.Test != null)
                        count += CountYieldExpressionsInExpression(@case.Test);
                    foreach (var s in @case.Consequent)
                        count += CountYieldExpressionsInStatement(s);
                }
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
            case HIRLabeledStatement labeledStmt:
                count += CountYieldExpressionsInStatement(labeledStmt.Body);
                break;
            case HIRThrowStatement throwStmt:
                count += CountYieldExpressionsInExpression(throwStmt.Argument);
                break;
            case HIRStoreUserClassInstanceFieldStatement storeInstanceField:
                count += CountYieldExpressionsInExpression(storeInstanceField.Value);
                break;
            case HIRStoreUserClassStaticFieldStatement storeStaticField:
                count += CountYieldExpressionsInExpression(storeStaticField.Value);
                break;
            case HIRSequencePointStatement:
            case HIRBreakStatement:
            case HIRContinueStatement:
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
            case HIRUpdateExpression updateExpr:
                count += CountYieldExpressionsInExpression(updateExpr.Argument);
                break;
            case HIRCallExpression callExpr:
                count += CountYieldExpressionsInExpression(callExpr.Callee);
                foreach (var arg in callExpr.Arguments)
                    count += CountYieldExpressionsInExpression(arg);
                break;
            case HIROptionalCallExpression optionalCallExpr:
                count += CountYieldExpressionsInExpression(optionalCallExpr.Callee);
                foreach (var arg in optionalCallExpr.Arguments)
                    count += CountYieldExpressionsInExpression(arg);
                break;
            case HIRNewExpression newExpr:
                count += CountYieldExpressionsInExpression(newExpr.Callee);
                foreach (var arg in newExpr.Arguments)
                    count += CountYieldExpressionsInExpression(arg);
                break;
            case HIRPropertyAccessExpression propAccessExpr:
                count += CountYieldExpressionsInExpression(propAccessExpr.Object);
                break;
            case HIROptionalPropertyAccessExpression optionalPropAccessExpr:
                count += CountYieldExpressionsInExpression(optionalPropAccessExpr.Object);
                break;
            case HIRIndexAccessExpression indexAccessExpr:
                count += CountYieldExpressionsInExpression(indexAccessExpr.Object);
                count += CountYieldExpressionsInExpression(indexAccessExpr.Index);
                break;
            case HIROptionalIndexAccessExpression optionalIndexAccessExpr:
                count += CountYieldExpressionsInExpression(optionalIndexAccessExpr.Object);
                count += CountYieldExpressionsInExpression(optionalIndexAccessExpr.Index);
                break;
            case HIRAssignmentExpression assignExpr:
                count += CountYieldExpressionsInExpression(assignExpr.Value);
                break;
            case HIRIndexAssignmentExpression indexAssignExpr:
                count += CountYieldExpressionsInExpression(indexAssignExpr.Object);
                count += CountYieldExpressionsInExpression(indexAssignExpr.Index);
                count += CountYieldExpressionsInExpression(indexAssignExpr.Value);
                break;
            case HIRPropertyAssignmentExpression propAssignExpr:
                count += CountYieldExpressionsInExpression(propAssignExpr.Object);
                count += CountYieldExpressionsInExpression(propAssignExpr.Value);
                break;
            case HIRDestructuringAssignmentExpression destructuringAssignExpr:
                count += CountYieldExpressionsInExpression(destructuringAssignExpr.Value);
                break;
            case HIRConditionalExpression condExpr:
                count += CountYieldExpressionsInExpression(condExpr.Test);
                count += CountYieldExpressionsInExpression(condExpr.Consequent);
                count += CountYieldExpressionsInExpression(condExpr.Alternate);
                break;
            case HIRArrayExpression arrayExpr:
                foreach (var elem in arrayExpr.Elements)
                    count += CountYieldExpressionsInExpression(elem);
                break;
            case HIRSpreadElement spreadExpr:
                count += CountYieldExpressionsInExpression(spreadExpr.Argument);
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
                        case HIRObjectSpreadProperty spread:
                            count += CountYieldExpressionsInExpression(spread.Argument);
                            break;
                    }
                }
                break;
            case HIRSequenceExpression sequenceExpr:
                foreach (var expr in sequenceExpr.Expressions)
                    count += CountYieldExpressionsInExpression(expr);
                break;
            case HIRTemplateLiteralExpression templateExpr:
                foreach (var expr in templateExpr.Expressions)
                    count += CountYieldExpressionsInExpression(expr);
                break;
            case HIRFunctionExpression:
            case HIRArrowFunctionExpression:
            case HIRLiteralExpression:
            case HIRVariableExpression:
            case HIRThisExpression:
            case HIRSuperExpression:
            case HIRScopesArrayExpression:
            case HIRUserClassTypeExpression:
            case HIRLoadUserClassInstanceFieldExpression:
                break;
        }
        return count;
    }
}
