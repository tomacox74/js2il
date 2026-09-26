using Acornima.Ast;
using Jroc.SymbolTables;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private string? TryRecognizeBitsetSearch(WhileStatement loop)
    {
        if (loop.Test is not CallExpression
            {
                Callee: MemberExpression
                {
                    Computed: false,
                    Object: ThisExpression,
                    Property: Identifier methodName
                }
            } call
            || call.Arguments.Count != 1
            || call.Arguments[0] is not Identifier index
            || loop.Body is not BlockStatement { Body.Count: 1 } block
            || block.Body[0] is not ExpressionStatement
            {
                Expression: UpdateExpression
                {
                    Operator: Acornima.Operator.Increment,
                    Argument: Identifier incremented
                }
            }
            || incremented.Name != index.Name
            || !TryGetEnclosingClassDefinition(out var classScope, out var classBody)
            || classScope.RequiresDynamicInstanceProperties)
        {
            return null;
        }

        var matches = classBody.Body.OfType<MethodDefinition>()
            .Where(method => !method.Static && !method.Computed
                && method.Key is Identifier name && name.Name == methodName.Name)
            .ToArray();
        if (matches.Length != 1
            || matches[0].Value.Params.Count != 1
            || matches[0].Value.Params[0] is not Identifier parameter
            || matches[0].Value.Body is not BlockStatement { Body.Count: 3 } methodBody
            || !IsOffsetDeclaration(methodBody.Body[0], "wordOffset",
                parameter.Name, Acornima.Operator.UnsignedRightShift, 5)
            || !IsOffsetDeclaration(methodBody.Body[1], "bitOffset",
                parameter.Name, Acornima.Operator.BitwiseAnd, 31)
            || methodBody.Body[2] is not ReturnStatement
            {
                Argument: BinaryExpression
                {
                    Operator: Acornima.Operator.BitwiseAnd,
                    Left: MemberExpression
                    {
                        Computed: true,
                        Object: MemberExpression
                        {
                            Computed: false,
                            Object: ThisExpression,
                            Property: Identifier field
                        },
                        Property: Identifier { Name: "wordOffset" }
                    },
                    Right: BinaryExpression
                    {
                        Operator: Acornima.Operator.LeftShift,
                        Left: NumericLiteral { Value: 1 },
                        Right: Identifier { Name: "bitOffset" }
                    }
                }
            }
            || !classScope.StableInstanceFieldClrTypes.TryGetValue(field.Name, out var fieldType)
            || fieldType != typeof(JavaScriptRuntime.Int32Array))
        {
            return null;
        }

        return field.Name;
    }

    private static bool IsOffsetDeclaration(
        Statement statement, string name, string parameter, Acornima.Operator op, double amount)
        => statement is VariableDeclaration { Declarations.Count: 1 } declaration
            && declaration.Declarations[0] is
            {
                Id: Identifier identifier,
                Init: BinaryExpression
                {
                    Left: Identifier operand,
                    Right: NumericLiteral literal
                } expression
            }
            && identifier.Name == name && operand.Name == parameter
            && expression.Operator == op && literal.Value == amount;
}
