using Acornima.Ast;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private static HIRInt32ArrayLocalVectorOrPattern? TryRecognizeInt32ArrayLocalVectorOr(
        ForStatement loop)
    {
        if (loop.Init is not VariableDeclaration
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
                Right: NumericLiteral or Identifier
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
                        Object: Identifier array,
                        Property: Identifier elementIndex
                    },
                    Right: NumericLiteral mask
                }
            }
            || elementIndex.Name != index.Name
            || mask.Value != System.Math.Truncate(mask.Value)
            || mask.Value < int.MinValue || mask.Value > int.MaxValue
            || array.Name == index.Name)
        {
            return null;
        }

        return new HIRInt32ArrayLocalVectorOrPattern(mask.Value);
    }
}
