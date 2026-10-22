using System.Collections;
using Acornima.Ast;

namespace Jroc.Utilities;

internal static class AstChildVisitor
{
    // Preserve the validation walk's property order, including aliases and metadata.
    // Unlike ChildNodes, visiting a null required child must not end enumeration.
    internal static void Visit(Node node, Action<Node?> visit)
    {
        switch (node)
        {
            case Acornima.Ast.Program program:
                VisitList(program.Body, visit);
                break;
            case BlockStatement block:
                VisitList(block.Body, visit);
                break;
            case IFunction function:
                visit(function.Id);
                VisitList(function.Params, visit);
                visit(function.Body);
                break;
            case IClass @class:
                visit(@class.Id);
                visit(@class.SuperClass);
                visit(@class.Body);
                VisitList(@class.Decorators, visit);
                break;
            case ClassBody body:
                VisitList(body.Body, visit);
                break;
            case MethodDefinition method:
                visit(method.Value);
                VisitList(method.Decorators, visit);
                visit(method.Key);
                break;
            case PropertyDefinition field:
                visit(field.Value);
                VisitList(field.Decorators, visit);
                visit(field.Key);
                break;
            case AccessorProperty accessor:
                visit(accessor.Value);
                VisitList(accessor.Decorators, visit);
                visit(accessor.Key);
                break;
            case Decorator decorator:
                visit(decorator.Expression);
                break;
            case VariableDeclaration declaration:
                VisitList(declaration.Declarations, visit);
                break;
            case VariableDeclarator declarator:
                visit(declarator.Id);
                visit(declarator.Init);
                break;
            case ExpressionStatement statement:
                visit(statement.Expression);
                break;
            case BinaryExpression binary:
                visit(binary.Left);
                visit(binary.Right);
                break;
            case AssignmentExpression assignment:
                visit(assignment.Left);
                visit(assignment.Right);
                break;
            case AssignmentPattern assignment:
                visit(assignment.Left);
                visit(assignment.Right);
                break;
            case UnaryExpression unary:
                visit(unary.Argument);
                break;
            case AwaitExpression awaitExpression:
                visit(awaitExpression.Argument);
                break;
            case YieldExpression yieldExpression:
                visit(yieldExpression.Argument);
                break;
            case CallExpression call:
                visit(call.Callee);
                VisitList(call.Arguments, visit);
                break;
            case NewExpression construct:
                visit(construct.Callee);
                VisitList(construct.Arguments, visit);
                break;
            case MemberExpression member:
                visit(member.Object);
                visit(member.Property);
                break;
            case ChainExpression chain:
                visit(chain.Expression);
                break;
            case ParenthesizedExpression parentheses:
                visit(parentheses.Expression);
                break;
            case ConditionalExpression conditional:
                visit(conditional.Test);
                visit(conditional.Consequent);
                visit(conditional.Alternate);
                break;
            case ReturnStatement statement:
                visit(statement.Argument);
                break;
            case ThrowStatement statement:
                visit(statement.Argument);
                break;
            case IfStatement statement:
                visit(statement.Test);
                visit(statement.Consequent);
                visit(statement.Alternate);
                break;
            case WhileStatement loop:
                visit(loop.Test);
                visit(loop.Body);
                break;
            case DoWhileStatement loop:
                visit(loop.Body);
                visit(loop.Test);
                break;
            case ForStatement loop:
                visit(loop.Init);
                visit(loop.Test);
                visit(loop.Update);
                visit(loop.Body);
                break;
            case ForInStatement loop:
                visit(loop.Left);
                visit(loop.Right);
                visit(loop.Body);
                break;
            case ForOfStatement loop:
                visit(loop.Left);
                visit(loop.Right);
                visit(loop.Body);
                break;
            case SwitchStatement statement:
                visit(statement.Discriminant);
                VisitList(statement.Cases, visit);
                break;
            case SwitchCase @case:
                visit(@case.Test);
                VisitList(@case.Consequent, visit);
                break;
            case LabeledStatement label:
                visit(label.Label);
                visit(label.Body);
                break;
            case BreakStatement statement:
                visit(statement.Label);
                break;
            case ContinueStatement statement:
                visit(statement.Label);
                break;
            case TryStatement statement:
                visit(statement.Block);
                visit(statement.Handler);
                visit(statement.Finalizer);
                break;
            case CatchClause clause:
                visit(clause.Param);
                visit(clause.Body);
                break;
            case WithStatement statement:
                visit(statement.Object);
                visit(statement.Body);
                break;
            case ArrayExpression array:
                VisitList(array.Elements, visit);
                break;
            case ArrayPattern array:
                VisitList(array.Elements, visit);
                break;
            case ObjectExpression obj:
                VisitList(obj.Properties, visit);
                break;
            case ObjectPattern obj:
                VisitList(obj.Properties, visit);
                break;
            case Property property:
                visit(property.Key);
                visit(property.Value);
                break;
            case RestElement rest:
                visit(rest.Argument);
                break;
            case SpreadElement spread:
                visit(spread.Argument);
                break;
            case SequenceExpression sequence:
                VisitList(sequence.Expressions, visit);
                break;
            case TaggedTemplateExpression template:
                visit(template.Tag);
                visit(template.Quasi);
                break;
            case TemplateLiteral template:
                VisitList(template.Quasis, visit);
                VisitList(template.Expressions, visit);
                break;
            case MetaProperty meta:
                visit(meta.Meta);
                visit(meta.Property);
                break;
            case ImportDeclaration import:
                VisitList(import.Specifiers, visit);
                visit(import.Source);
                VisitList(import.Attributes, visit);
                break;
            case ImportSpecifier import:
                visit(import.Imported);
                visit(import.Local);
                break;
            case ImportDeclarationSpecifier import:
                visit(import.Local);
                break;
            case ImportAttribute attribute:
                visit(attribute.Key);
                visit(attribute.Value);
                break;
            case ImportExpression import:
                visit(import.Source);
                visit(import.Options);
                break;
            case ExportAllDeclaration export:
                visit(export.Exported);
                visit(export.Source);
                VisitList(export.Attributes, visit);
                break;
            case ExportDefaultDeclaration export:
                visit(export.Declaration);
                break;
            case ExportNamedDeclaration export:
                visit(export.Declaration);
                VisitList(export.Specifiers, visit);
                visit(export.Source);
                VisitList(export.Attributes, visit);
                break;
            case ExportSpecifier export:
                visit(export.Local);
                visit(export.Exported);
                break;
            case Identifier or PrivateIdentifier or Literal or TemplateElement
                or ThisExpression or Super or EmptyStatement or DebuggerStatement:
                break;
            default:
                throw new InvalidOperationException($"No typed child traversal is defined for AST node '{node.TypeText}'.");
        }

        if (node is Statement statementNode)
        {
            visit(statementNode.LabelSet);
        }

        switch (node.UserData)
        {
            case Node child:
                visit(child);
                break;
            case IEnumerable collection when node.UserData is not string:
                foreach (var item in collection)
                {
                    if (item is Node element)
                    {
                        visit(element);
                    }
                }
                break;
        }
    }

    private static void VisitList<T>(in NodeList<T> nodes, Action<Node?> visit) where T : Node?
    {
        foreach (var node in nodes)
        {
            visit(node);
        }
    }
}
