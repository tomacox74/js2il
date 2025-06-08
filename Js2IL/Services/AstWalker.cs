using Acornima.Ast;

namespace Js2IL.Services;

public class AstWalker
{
    public void Visit(Node node, Action<Node> visitor)
    {
        if (node == null) return;

        // Visit the current node
        visitor(node);

        // Visit child nodes based on node type
        switch (node)
        {
            case Acornima.Ast.Program program:
                VisitNodes(program.Body, visitor);
                break;

            case BlockStatement block:
                VisitNodes(block.Body, visitor);
                break;

            case FunctionDeclaration func:
                VisitNodes(func.Params, visitor);
                Visit(func.Body, visitor);
                break;

            case FunctionExpression funcExpr:
                VisitNodes(funcExpr.Params, visitor);
                Visit(funcExpr.Body, visitor);
                break;

            case ArrowFunctionExpression arrowFunc:
                VisitNodes(arrowFunc.Params, visitor);
                Visit(arrowFunc.Body, visitor);
                break;

            case VariableDeclaration varDecl:
                VisitNodes(varDecl.Declarations, visitor);
                break;

            case VariableDeclarator varDeclarator:
                Visit(varDeclarator.Init, visitor);
                break;

            case ExpressionStatement exprStmt:
                Visit(exprStmt.Expression, visitor);
                break;

            case BinaryExpression binaryExpr:
                Visit(binaryExpr.Left, visitor);
                Visit(binaryExpr.Right, visitor);
                break;

            case CallExpression callExpr:
                Visit(callExpr.Callee, visitor);
                VisitNodes(callExpr.Arguments, visitor);
                break;

            case MemberExpression memberExpr:
                Visit(memberExpr.Object, visitor);
                Visit(memberExpr.Property, visitor);
                break;

            case ReturnStatement returnStmt:
                Visit(returnStmt.Argument, visitor);
                break;

            case IfStatement ifStmt:
                Visit(ifStmt.Test, visitor);
                Visit(ifStmt.Consequent, visitor);
                Visit(ifStmt.Alternate, visitor);
                break;

            case WhileStatement whileStmt:
                Visit(whileStmt.Test, visitor);
                Visit(whileStmt.Body, visitor);
                break;

            case ForStatement forStmt:
                Visit(forStmt.Init, visitor);
                Visit(forStmt.Test, visitor);
                Visit(forStmt.Update, visitor);
                Visit(forStmt.Body, visitor);
                break;

            case ForInStatement forInStmt:
                Visit(forInStmt.Left, visitor);
                Visit(forInStmt.Right, visitor);
                Visit(forInStmt.Body, visitor);
                break;

            case ForOfStatement forOfStmt:
                Visit(forOfStmt.Left, visitor);
                Visit(forOfStmt.Right, visitor);
                Visit(forOfStmt.Body, visitor);
                break;

            case SwitchStatement switchStmt:
                Visit(switchStmt.Discriminant, visitor);
                VisitNodes(switchStmt.Cases, visitor);
                break;

            case SwitchCase switchCase:
                Visit(switchCase.Test, visitor);
                VisitNodes(switchCase.Consequent, visitor);
                break;

            case TryStatement tryStmt:
                Visit(tryStmt.Block, visitor);
                Visit(tryStmt.Handler, visitor);
                Visit(tryStmt.Finalizer, visitor);
                break;

            case CatchClause catchClause:
                Visit(catchClause.Param, visitor);
                Visit(catchClause.Body, visitor);
                break;

            case ThrowStatement throwStmt:
                Visit(throwStmt.Argument, visitor);
                break;

            case ArrayExpression arrayExpr:
                VisitNodes(arrayExpr.Elements, visitor);
                break;

            case ObjectExpression objExpr:
                VisitNodes(objExpr.Properties, visitor);
                break;

            case Property prop:
                Visit(prop.Key, visitor);
                Visit(prop.Value, visitor);
                break;

            case ConditionalExpression condExpr:
                Visit(condExpr.Test, visitor);
                Visit(condExpr.Consequent, visitor);
                Visit(condExpr.Alternate, visitor);
                break;

            case UnaryExpression unaryExpr:
                Visit(unaryExpr.Argument, visitor);
                break;

            case AssignmentExpression assignExpr:
                Visit(assignExpr.Left, visitor);
                Visit(assignExpr.Right, visitor);
                break;

            case SequenceExpression seqExpr:
                VisitNodes(seqExpr.Expressions, visitor);
                break;
        }
    }

    private void VisitNodes<T>(IEnumerable<T> nodes, Action<Node> visitor) where T : Node
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            Visit(node, visitor);
        }
    }
} 