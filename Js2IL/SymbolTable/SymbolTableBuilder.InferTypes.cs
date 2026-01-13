using Acornima.Ast;
using Acornima;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Js2IL.SymbolTables;

public partial class SymbolTableBuilder
{
    /// <Summary>
    /// /// Infers variable CLR types from the JavaScript AST
    /// </Summary>
    private void InferVariableClrTypes(Scope scope)
    {
        InferVariableClrTypesRecursively(scope);
    }

    private void InferClassInstanceFieldClrTypes(Scope scope)
    {
        InferClassInstanceFieldClrTypesRecursively(scope);
    }

    void InferVariableClrTypesForScope(Scope scope)
    {
        // these are temporary gates as we slowing roll out type inference
        bool isClassMethod(Scope? scope) =>
            scope != null &&
            scope.Kind == ScopeKind.Function &&
            scope.Parent != null &&
            scope.Parent.Kind == ScopeKind.Class;

        bool isFunctionOrArrowFunction(Scope? scope) =>
            scope != null &&
            scope.Kind == ScopeKind.Function;

        // Check if this is a block scope that is a descendant of a class method,
        // without crossing another function boundary (e.g., nested function or arrow function).
        // Valid: classMethod -> block -> block -> block
        // Invalid: classMethod -> function -> block (crosses function boundary)
        bool isBlockScopeInClassMethod(Scope? scope)
        {
            if (scope == null || scope.Kind != ScopeKind.Block)
                return false;
            
            var current = scope.Parent;
            while (current != null)
            {
                if (isClassMethod(current))
                    return true; // Found class method ancestor - valid!
                
                if (current.Kind == ScopeKind.Function)
                    return false; // Hit another function before class method - invalid
                
                // Continue walking up through block scopes
                current = current.Parent;
            }
            return false;
        }

        // Check if this is a block scope that is a descendant of a function/arrow function,
        // without crossing another function boundary.
        // Valid: function -> block -> block -> block
        // Invalid: function -> function -> block (crosses function boundary)
        bool isBlockScopeInFunction(Scope? scope)
        {
            if (scope == null || scope.Kind != ScopeKind.Block)
                return false;

            var current = scope.Parent;
            while (current != null)
            {
                if (isFunctionOrArrowFunction(current))
                    return true;

                if (current.Kind == ScopeKind.Function)
                    return false;

                current = current.Parent;
            }
            return false;
        }

        if (scope.Kind != ScopeKind.Global &&
            isClassMethod(scope) == false &&
            isBlockScopeInClassMethod(scope) == false &&
            isFunctionOrArrowFunction(scope) == false &&
            isBlockScopeInFunction(scope) == false)
        {
            return;
        }

        var proposedClrTypes = new Dictionary<string, Type>();
        var unitializedClrTypes = new HashSet<string>();

        // Type inference is currently conservative:
        // - We infer stable types for uncaptured variables.
        // - Additionally, we infer stable types for captured const bindings (safe: cannot be reassigned).
        foreach (var binding in scope.Bindings.Values)
        {
            if (binding.IsCaptured && binding.Kind != BindingKind.Const)
                continue;

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
            binding.IsStableType = true;
        }      
    }

    void InferVariableClrTypesRecursively(Scope scope)
    {
        InferVariableClrTypesForScope(scope);

        foreach (var childScope in scope.Children)
        {
            InferVariableClrTypesRecursively(childScope);
        }
    }

    private void InferClassInstanceFieldClrTypesRecursively(Scope scope)
    {
        if (scope.Kind == ScopeKind.Class && scope.AstNode is ClassDeclaration classDecl)
        {
            InferClassInstanceFieldClrTypesForClassScope(scope, classDecl);
        }

        foreach (var childScope in scope.Children)
        {
            InferClassInstanceFieldClrTypesRecursively(childScope);
        }
    }

    private void InferClassInstanceFieldClrTypesForClassScope(Scope classScope, ClassDeclaration classDecl)
    {
        // Clear any previous results (builder should be single-use, but keep this deterministic).
        classScope.StableInstanceFieldClrTypes.Clear();
        classScope.StableInstanceFieldUserClassNames.Clear();

        var proposedClr = new Dictionary<string, Type>(StringComparer.Ordinal);
        var proposedUserClass = new Dictionary<string, string>(StringComparer.Ordinal);
        var conflicted = new HashSet<string>(StringComparer.Ordinal);

        void MarkConflict(string name)
        {
            conflicted.Add(name);
            proposedClr.Remove(name);
            proposedUserClass.Remove(name);
        }

        void ProposeClr(string name, Type? t)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (conflicted.Contains(name)) return;

            // Mixing user-class and primitive proposals is non-stable.
            if (proposedUserClass.ContainsKey(name))
            {
                MarkConflict(name);
                return;
            }

            // Any unknown assignment makes the field non-stable.
            if (t == null)
            {
                MarkConflict(name);
                return;
            }

            if (!proposedClr.TryGetValue(name, out var existing))
            {
                proposedClr[name] = t;
                return;
            }

            if (existing != t)
            {
                MarkConflict(name);
            }
        }

        void ProposeUserClass(string name, string? jsClassName)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (conflicted.Contains(name)) return;

            // Any unknown assignment makes the field non-stable.
            if (string.IsNullOrEmpty(jsClassName))
            {
                MarkConflict(name);
                return;
            }

            // Mixing primitive and user-class proposals is non-stable.
            if (proposedClr.ContainsKey(name))
            {
                MarkConflict(name);
                return;
            }

            if (!proposedUserClass.TryGetValue(name, out var existing))
            {
                proposedUserClass[name] = jsClassName;
                return;
            }

            if (!string.Equals(existing, jsClassName, StringComparison.Ordinal))
            {
                MarkConflict(name);
            }
        }

        string? TryInferNewExpressionUserClassName(Node expr)
        {
            if (expr is not NewExpression ne)
            {
                return null;
            }

            if (ne.Callee is not Identifier calleeId)
            {
                return null;
            }

            // Only treat it as a stable user class if we can resolve a class scope with that name.
            var root = classScope;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            static Scope? FindClassScopeRecursive(Scope scope, string className)
            {
                foreach (var child in scope.Children)
                {
                    if (child.Kind == ScopeKind.Class && string.Equals(child.Name, className, StringComparison.Ordinal))
                    {
                        return child;
                    }

                    var found = FindClassScopeRecursive(child, className);
                    if (found != null)
                    {
                        return found;
                    }
                }

                return null;
            }

            var classScopeMatch = FindClassScopeRecursive(root, calleeId.Name);
            return classScopeMatch != null ? calleeId.Name : null;
        }

        // Include class field initializers: `field = <expr>`.
        foreach (var pdef in classDecl.Body.Body.OfType<PropertyDefinition>())
        {
            if (pdef.Static)
            {
                continue;
            }

            string? name = pdef.Key switch
            {
                Identifier id => id.Name,
                PrivateIdentifier pid => pid.Name,
                _ => null
            };

            if (name == null)
            {
                continue;
            }

            if (pdef.Value is Node init)
            {
                var userClassName = TryInferNewExpressionUserClassName(init);
                if (userClassName != null)
                {
                    ProposeUserClass(name, userClassName);
                }
                else
                {
                    ProposeClr(name, InferExpressionClrType(init));
                }
            }
        }

        void Walk(Node? node)
        {
            if (node == null) return;

            // Assignment: this.x = <expr>
            if (node is AssignmentExpression assign
                && assign.Left is MemberExpression me
                && me.Object is ThisExpression
                && !me.Computed)
            {
                var propName = me.Property switch
                {
                    Identifier id => id.Name,
                    PrivateIdentifier pid => pid.Name,
                    _ => null
                };

                if (propName != null)
                {
                    var userClassName = TryInferNewExpressionUserClassName(assign.Right);
                    if (userClassName != null)
                    {
                        ProposeUserClass(propName, userClassName);
                    }
                    else
                    {
                        ProposeClr(propName, InferExpressionClrType(assign.Right));
                    }
                }
            }

            // Update: this.x++ / this.x--
            if (node is UpdateExpression update
                && update.Argument is MemberExpression ume
                && ume.Object is ThisExpression
                && !ume.Computed)
            {
                var propName = ume.Property switch
                {
                    Identifier id => id.Name,
                    PrivateIdentifier pid => pid.Name,
                    _ => null
                };
                if (propName != null)
                {
                    // Update expressions are numeric.
                    ProposeClr(propName, typeof(double));
                }
            }

            foreach (var child in node.ChildNodes)
            {
                Walk(child);
            }
        }

        // Walk all method bodies and collect assignments.
        foreach (var method in classDecl.Body.Body.OfType<MethodDefinition>())
        {
            if (method.Value is FunctionExpression fe)
            {
                Walk(fe.Body);
            }
        }

        // Persist stable results.
        foreach (var kvp in proposedClr)
        {
            classScope.StableInstanceFieldClrTypes[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in proposedUserClass)
        {
            classScope.StableInstanceFieldUserClassNames[kvp.Key] = kvp.Value;
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

                switch (binExpr.Operator)
                {
                    case Operator.Addition:
                        return InferAddOperatorType(binExpr);
                    case Operator.BitwiseAnd:
                    case Operator.BitwiseOr:
                    case Operator.BitwiseXor:
                    case Operator.LeftShift:
                    case Operator.RightShift:
                    case Operator.UnsignedRightShift:
                        return typeof(double);
                }

                return null;
            }
            case NonUpdateUnaryExpression unaryExpr:
            {
                if (unaryExpr.Operator == Operator.BitwiseNot)
                {
                    return typeof(double);
                }
                return  null;
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