using Acornima.Ast;
using Jroc.Services;
using Jroc.SymbolTables;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private HIRBitsetCountPattern? TryRecognizeBitsetCount(ForStatement loop)
    {
        var callable = _currentScope;
        while (callable.Kind != ScopeKind.Function && callable.Parent != null)
        {
            callable = callable.Parent;
        }
        if (callable.Parent?.Kind != ScopeKind.Class
            || !TryGetEnclosingClassDefinition(out var classScope, out var classBody)
            || classScope.RequiresDynamicInstanceProperties
            || loop.Init is not VariableDeclaration
            {
                Kind: VariableDeclarationKind.Let,
                Declarations.Count: 1
            } initializer
            || initializer.Declarations[0] is not
            {
                Id: Identifier index,
                Init: NumericLiteral { Value: 1 }
            }
            || loop.Test is not BinaryExpression
            {
                Operator: Acornima.Operator.LessThan,
                Left: Identifier tested,
                Right: MemberExpression
                {
                    Computed: false,
                    Object: ThisExpression,
                    Property: Identifier endField
                }
            }
            || tested.Name != index.Name
            || loop.Update is not UpdateExpression
            {
                Operator: Acornima.Operator.Increment,
                Argument: Identifier incremented
            }
            || incremented.Name != index.Name
            || loop.Body is not BlockStatement { Body.Count: 1 } loopBody
            || loopBody.Body[0] is not IfStatement
            {
                Alternate: null,
                Test: UnaryExpression
                {
                    Operator: Acornima.Operator.LogicalNot,
                    Argument: CallExpression
                    {
                        Callee: MemberExpression
                        {
                            Computed: false,
                            Object: MemberExpression
                            {
                                Computed: false,
                                Object: ThisExpression,
                                Property: Identifier receiverField
                            },
                            Property: Identifier bitTestMethod
                        }
                    } call
                },
                Consequent: BlockStatement { Body.Count: 1 } consequent
            }
            || call.Arguments.Count != 1
            || call.Arguments[0] is not Identifier argument
            || argument.Name != index.Name
            || consequent.Body[0] is not ExpressionStatement
            {
                Expression: UpdateExpression
                {
                    Operator: Acornima.Operator.Increment,
                    Argument: Identifier total
                }
            }
            || !classScope.StableInstanceFieldClrTypes.TryGetValue(endField.Name, out var endType)
            || endType != typeof(double))
        {
            return null;
        }

        var countingMethods = classBody.Body.OfType<MethodDefinition>()
            .Where(method => !method.Static
                && method.Value.Body is BlockStatement body
                && body.Body.Any(statement => ReferenceEquals(statement, loop)))
            .ToArray();
        if (countingMethods.Length != 1
            || countingMethods[0].Value.Body is not BlockStatement { Body.Count: 3 } countBody
            || countBody.Body[0] is not VariableDeclaration
            {
                Kind: VariableDeclarationKind.Let,
                Declarations.Count: 1
            } totalDeclaration
            || totalDeclaration.Declarations[0] is not
            {
                Id: Identifier declaredTotal,
                Init: NumericLiteral { Value: 1 }
            }
            || declaredTotal.Name != total.Name
            || !ReferenceEquals(countBody.Body[1], loop)
            || countBody.Body[2] is not ReturnStatement
            {
                Argument: Identifier returnedTotal
            }
            || returnedTotal.Name != total.Name)
        {
            return null;
        }

        var constructors = classBody.Body.OfType<MethodDefinition>()
            .Where(ClassElementNames.IsConstructor).ToArray();
        if (constructors.Length != 1
            || constructors[0].Value.Body is not BlockStatement constructorBody)
        {
            return null;
        }

        var creations = constructorBody.Body.OfType<ExpressionStatement>()
            .Select(statement => statement.Expression)
            .OfType<AssignmentExpression>()
            .Where(assignment => assignment.Operator == Acornima.Operator.Assignment
                && assignment.Left is MemberExpression
                {
                    Computed: false,
                    Object: ThisExpression,
                    Property: Identifier name
                } && name.Name == receiverField.Name)
            .ToArray();
        if (creations.Length != 1
            || creations[0].Right is not NewExpression { Callee: Identifier className })
        {
            return null;
        }

        var bitsetClasses = classScope.Parent?.Children
            .Where(scope => scope.Kind == ScopeKind.Class
                && scope.AstNode is ClassDeclaration { Id: Identifier name }
                && name.Name == className.Name)
            .ToArray();
        if (bitsetClasses is not { Length: 1 }
            || bitsetClasses[0].AstNode is not ClassDeclaration bitsetClass
            || bitsetClasses[0].RequiresDynamicInstanceProperties)
        {
            return null;
        }

        var wordsField = TryGetBitsetWordFieldName(
            bitsetClass.Body, bitsetClasses[0], bitTestMethod.Name);
        return wordsField is null
            ? null
            : new HIRBitsetCountPattern(
                receiverField.Name, wordsField, endField.Name, total.Name);
    }
}
