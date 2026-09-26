using Acornima.Ast;
using Jroc.SymbolTables;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private HIRInt32ArrayVectorOrPattern? TryRecognizeInt32ArrayVectorOr(ForStatement loop)
    {
        var callable = _currentScope;
        while (callable.Kind != ScopeKind.Function && callable.Parent != null)
        {
            callable = callable.Parent;
        }
        if (callable.Parent?.Kind != ScopeKind.Class
            || !TryGetEnclosingClassDefinition(out var classScope, out _)
            || classScope.RequiresDynamicInstanceProperties
            || loop.Init is not VariableDeclaration
            {
                Kind: VariableDeclarationKind.Let,
                Declarations.Count: 1
            } initializer
            || initializer.Declarations[0] is not
            {
                Id: Identifier index,
                Init: NumericLiteral { Value: >= 0 and <= int.MaxValue }
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
                Prefix: false,
                Argument: Identifier incremented
            }
            || incremented.Name != index.Name
            || loop.Body is not BlockStatement { Body.Count: 1 } body
            || body.Body[0] is not ExpressionStatement
            {
                Expression: AssignmentExpression
                {
                    Operator: Acornima.Operator.BitwiseOrAssignment,
                    Left: MemberExpression
                    {
                        Computed: true,
                        Object: MemberExpression
                        {
                            Computed: false,
                            Object: ThisExpression,
                            Property: Identifier arrayField
                        },
                        Property: Identifier elementIndex
                    },
                    Right: NumericLiteral mask
                }
            }
            || elementIndex.Name != index.Name
            || mask.Value != System.Math.Truncate(mask.Value)
            || mask.Value < int.MinValue || mask.Value > int.MaxValue
            || !classScope.StableInstanceFieldClrTypes.TryGetValue(arrayField.Name, out var arrayType)
            || arrayType != typeof(JavaScriptRuntime.Int32Array)
            || !classScope.StableInstanceFieldClrTypes.TryGetValue(endField.Name, out var endType)
            || endType != typeof(double)
            || arrayField.Name == endField.Name)
        {
            return null;
        }

        return new HIRInt32ArrayVectorOrPattern(arrayField.Name, endField.Name, mask.Value);
    }
}
