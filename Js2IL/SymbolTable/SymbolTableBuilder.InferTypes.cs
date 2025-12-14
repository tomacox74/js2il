using Acornima.Ast;
using Acornima;
namespace Js2IL.SymbolTables;

public partial class SymbolTableBuilder
{
    /// <Summary>
    /// /// Infers variable CLR types from the JavaScript AST
    /// </Summary>
    private void InferVariableClrTypes(Scope scope)
    {
        var proposedClrTypes = new Dictionary<string, Type>();
        var unitializedClrTypes = new HashSet<string>();

        // currently only infer types for uncaptured variables
        foreach (var binding in scope.Bindings.Values)
        {
            if (binding.IsCaptured)
            {
                continue;
            }

            if (binding.DeclarationNode is VariableDeclarator variableDeclarator)
            {
                // for some types we can still infer the type later via assignments
                // this doesn't work for value types.. i.e. a number can't also be null in the CLR type system
                // to consider.. use Nullable<T> for value types in future optimizations?
                if (variableDeclarator.Init == null)
                {
                    unitializedClrTypes.Add(binding.Name);
                    continue;
                }

                var inferredType = InferExpressionClrType(variableDeclarator.Init);
                if (inferredType != null)
                {
                    proposedClrTypes[binding.Name] = inferredType;
                }
            }
        }

        // now we walk the statements in the scope to check assignments
        foreach (var statement in scope.AstNode.ChildNodes)
        {
            if (statement is ExpressionStatement exprStmt)
            {
                if (exprStmt.Expression is AssignmentExpression assignExpr && assignExpr.Left is Identifier identifier)
                {
                    if (proposedClrTypes.TryGetValue(identifier.Name, out var inferredType))
                    {
                        // if any assigment conflicts with the inferred type, we remove the proposed type
                           var rightType = InferExpressionClrType(assignExpr.Right);
                        if (rightType != inferredType)
                        {
                            // conflict, remove the proposed type
                            proposedClrTypes.Remove(identifier.Name);
                        }
                    }
                    else
                    {
                        if (unitializedClrTypes.Contains(identifier.Name))
                        {
                            // a uninitialized variable can still be a nullable type (not a value type like number)
                            // to consider.. use Nullable<T> for value types in future optimizations?
                            var rightType = InferExpressionClrType(assignExpr.Right);
                            if (rightType?.IsValueType == false)
                            {
                                proposedClrTypes[identifier.Name] = rightType;
                            }
                            unitializedClrTypes.Remove(identifier.Name);
                        }
                    }
                }
                else if (exprStmt.Expression is UpdateExpression updateExpr && updateExpr.Argument is Identifier updateVariableIdentity)
                {
                    // update expressions only valid for number types
                    if (proposedClrTypes.TryGetValue(updateVariableIdentity.Name, out var inferredType))
                    {
                        if (inferredType != typeof(double))
                        {
                            // conflict, remove the proposed type
                            proposedClrTypes.Remove(updateVariableIdentity.Name);
                        }
                    }

                    unitializedClrTypes.Remove(updateVariableIdentity.Name);
                }
            }
        }

        foreach (var kvp in proposedClrTypes)
        {
            var binding = scope.Bindings[kvp.Key];
            binding.ClrType = kvp.Value;
        }
    }

    Type? InferExpressionClrType(Node expr)
    {
        switch (expr)
        {
            case NumericLiteral:
                return typeof(double);
            case StringLiteral:
                return typeof(string);
            case BooleanLiteral:
                return typeof(bool);
            case NonLogicalBinaryExpression binExpr:
            {
                if (binExpr.Operator == Operator.Addition)
                
                {
                    return InferAddOperatorType(binExpr);
                }

                return null;
            }
        }

        // Add more inference rules as needed
        return null;
    }

    Type? InferAddOperatorType(NonLogicalBinaryExpression binaryExpression)
    {
        var leftType = InferExpressionClrType(binaryExpression.Left);
        var rightType = InferExpressionClrType(binaryExpression.Right);

        // If either side is a string, + performs string concatenation
        if (leftType == typeof(string) || rightType == typeof(string))
        {
            return typeof(string);
        }

        // Treat C# null as JavaScript null/undefined for inference purposes.
        // For the supported primitive set (number, boolean, null/undefined):
        // - If either operand is number, boolean or null/undefined, the result is a number.
        // - Boolean coerces to number (true -> 1, false -> 0).
        // - null/undefined coerces to number (null -> 0, undefined -> NaN) but the result type is still Number.
        bool LeftIsSupportedNumberLike = leftType == typeof(double) || leftType == typeof(bool) || leftType == null;
        bool RightIsSupportedNumberLike = rightType == typeof(double) || rightType == typeof(bool) || rightType == null;

        if (LeftIsSupportedNumberLike && RightIsSupportedNumberLike)
        {
            return typeof(double);
        }

        // If we get here, at least one side is an unsupported/unknown non-string type
        // (e.g. Object, Symbol, BigInt or some other Type) - we don't infer the result.
        return null;
    }
}