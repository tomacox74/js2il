using Acornima.Ast;

namespace Js2IL.Utilities;

public class AstWalker
{
    public void Visit(Node? node, Action<Node> visitor)
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
                // Only visit the Id if it's a pattern (for destructuring validation)
                // Don't visit simple Identifier to avoid false positives in unused code analysis
                if (varDeclarator.Id is not Identifier)
                {
                    Visit(varDeclarator.Id, visitor);
                }
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

            case ObjectPattern objPattern:
                VisitNodes(objPattern.Properties, visitor);
                break;

            case ArrayPattern arrPattern:
                VisitNodes(arrPattern.Elements, visitor);
                break;

            case RestElement restElem:
                Visit(restElem.Argument, visitor);
                break;

            case ClassDeclaration classDecl:
                Visit(classDecl.SuperClass, visitor);
                Visit(classDecl.Body, visitor);
                break;

            case ClassExpression classExpr:
                Visit(classExpr.SuperClass, visitor);
                Visit(classExpr.Body, visitor);
                break;

            case ClassBody classBody:
                VisitNodes(classBody.Body, visitor);
                break;

            case MethodDefinition methodDef:
                Visit(methodDef.Key, visitor);
                Visit(methodDef.Value, visitor);
                break;

            case PropertyDefinition propDef:
                Visit(propDef.Key, visitor);
                Visit(propDef.Value, visitor);
                break;
        }
    }

    public void VisitWithContext(Node? node, Action<Node> enterNode, Action<Node> exitNode)
    {
        if (node == null) return;

        // Visit the current node (enter)
        enterNode(node);

        // Visit child nodes based on node type
        switch (node)
        {
            case Acornima.Ast.Program program:
                VisitNodesWithContext(program.Body, enterNode, exitNode);
                break;

            case BlockStatement block:
                VisitNodesWithContext(block.Body, enterNode, exitNode);
                break;

            case FunctionDeclaration func:
                VisitNodesWithContext(func.Params, enterNode, exitNode);
                VisitWithContext(func.Body, enterNode, exitNode);
                break;

            case FunctionExpression funcExpr:
                VisitNodesWithContext(funcExpr.Params, enterNode, exitNode);
                VisitWithContext(funcExpr.Body, enterNode, exitNode);
                break;

            case ArrowFunctionExpression arrowFunc:
                VisitNodesWithContext(arrowFunc.Params, enterNode, exitNode);
                VisitWithContext(arrowFunc.Body, enterNode, exitNode);
                break;

            case VariableDeclaration varDecl:
                VisitNodesWithContext(varDecl.Declarations, enterNode, exitNode);
                break;

            case VariableDeclarator varDeclarator:
                if (varDeclarator.Id is not Identifier)
                {
                    VisitWithContext(varDeclarator.Id, enterNode, exitNode);
                }
                VisitWithContext(varDeclarator.Init, enterNode, exitNode);
                break;

            case ExpressionStatement exprStmt:
                VisitWithContext(exprStmt.Expression, enterNode, exitNode);
                break;

            case BinaryExpression binaryExpr:
                VisitWithContext(binaryExpr.Left, enterNode, exitNode);
                VisitWithContext(binaryExpr.Right, enterNode, exitNode);
                break;

            case CallExpression callExpr:
                VisitWithContext(callExpr.Callee, enterNode, exitNode);
                VisitNodesWithContext(callExpr.Arguments, enterNode, exitNode);
                break;

            case MemberExpression memberExpr:
                VisitWithContext(memberExpr.Object, enterNode, exitNode);
                VisitWithContext(memberExpr.Property, enterNode, exitNode);
                break;

            case ReturnStatement returnStmt:
                VisitWithContext(returnStmt.Argument, enterNode, exitNode);
                break;

            case IfStatement ifStmt:
                VisitWithContext(ifStmt.Test, enterNode, exitNode);
                VisitWithContext(ifStmt.Consequent, enterNode, exitNode);
                VisitWithContext(ifStmt.Alternate, enterNode, exitNode);
                break;

            case WhileStatement whileStmt:
                VisitWithContext(whileStmt.Test, enterNode, exitNode);
                VisitWithContext(whileStmt.Body, enterNode, exitNode);
                break;

            case ForStatement forStmt:
                VisitWithContext(forStmt.Init, enterNode, exitNode);
                VisitWithContext(forStmt.Test, enterNode, exitNode);
                VisitWithContext(forStmt.Update, enterNode, exitNode);
                VisitWithContext(forStmt.Body, enterNode, exitNode);
                break;

            case ForInStatement forInStmt:
                VisitWithContext(forInStmt.Left, enterNode, exitNode);
                VisitWithContext(forInStmt.Right, enterNode, exitNode);
                VisitWithContext(forInStmt.Body, enterNode, exitNode);
                break;

            case ForOfStatement forOfStmt:
                VisitWithContext(forOfStmt.Left, enterNode, exitNode);
                VisitWithContext(forOfStmt.Right, enterNode, exitNode);
                VisitWithContext(forOfStmt.Body, enterNode, exitNode);
                break;

            case SwitchStatement switchStmt:
                VisitWithContext(switchStmt.Discriminant, enterNode, exitNode);
                VisitNodesWithContext(switchStmt.Cases, enterNode, exitNode);
                break;

            case SwitchCase switchCase:
                VisitWithContext(switchCase.Test, enterNode, exitNode);
                VisitNodesWithContext(switchCase.Consequent, enterNode, exitNode);
                break;

            case TryStatement tryStmt:
                VisitWithContext(tryStmt.Block, enterNode, exitNode);
                VisitWithContext(tryStmt.Handler, enterNode, exitNode);
                VisitWithContext(tryStmt.Finalizer, enterNode, exitNode);
                break;

            case CatchClause catchClause:
                VisitWithContext(catchClause.Param, enterNode, exitNode);
                VisitWithContext(catchClause.Body, enterNode, exitNode);
                break;

            case ThrowStatement throwStmt:
                VisitWithContext(throwStmt.Argument, enterNode, exitNode);
                break;

            case ArrayExpression arrayExpr:
                VisitNodesWithContext(arrayExpr.Elements, enterNode, exitNode);
                break;

            case ObjectExpression objExpr:
                VisitNodesWithContext(objExpr.Properties, enterNode, exitNode);
                break;

            case Property prop:
                VisitWithContext(prop.Key, enterNode, exitNode);
                VisitWithContext(prop.Value, enterNode, exitNode);
                break;

            case ConditionalExpression condExpr:
                VisitWithContext(condExpr.Test, enterNode, exitNode);
                VisitWithContext(condExpr.Consequent, enterNode, exitNode);
                VisitWithContext(condExpr.Alternate, enterNode, exitNode);
                break;

            case UnaryExpression unaryExpr:
                VisitWithContext(unaryExpr.Argument, enterNode, exitNode);
                break;

            case AssignmentExpression assignExpr:
                VisitWithContext(assignExpr.Left, enterNode, exitNode);
                VisitWithContext(assignExpr.Right, enterNode, exitNode);
                break;

            case SequenceExpression seqExpr:
                VisitNodesWithContext(seqExpr.Expressions, enterNode, exitNode);
                break;

            case ObjectPattern objPattern:
                VisitNodesWithContext(objPattern.Properties, enterNode, exitNode);
                break;

            case ArrayPattern arrPattern:
                VisitNodesWithContext(arrPattern.Elements, enterNode, exitNode);
                break;

            case RestElement restElem:
                VisitWithContext(restElem.Argument, enterNode, exitNode);
                break;

            case ClassDeclaration classDecl:
                VisitWithContext(classDecl.SuperClass, enterNode, exitNode);
                VisitWithContext(classDecl.Body, enterNode, exitNode);
                break;

            case ClassExpression classExpr:
                VisitWithContext(classExpr.SuperClass, enterNode, exitNode);
                VisitWithContext(classExpr.Body, enterNode, exitNode);
                break;

            case ClassBody classBody:
                VisitNodesWithContext(classBody.Body, enterNode, exitNode);
                break;

            case MethodDefinition methodDef:
                VisitWithContext(methodDef.Key, enterNode, exitNode);
                VisitWithContext(methodDef.Value, enterNode, exitNode);
                break;

            case PropertyDefinition propDef:
                VisitWithContext(propDef.Key, enterNode, exitNode);
                VisitWithContext(propDef.Value, enterNode, exitNode);
                break;
        }

        // Exit the current node
        exitNode(node);
    }

    private void VisitNodes<T>(IEnumerable<T> nodes, Action<Node> visitor) where T : Node?
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            Visit(node, visitor);
        }
    }

    private void VisitNodesWithContext<T>(IEnumerable<T> nodes, Action<Node> enterNode, Action<Node> exitNode) where T : Node?
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            VisitWithContext(node, enterNode, exitNode);
        }
    }
} 