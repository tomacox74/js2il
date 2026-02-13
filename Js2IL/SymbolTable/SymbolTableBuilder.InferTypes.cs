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

    private void InferCallableReturnClrTypes(Scope scope)
    {
        InferCallableReturnClrTypesRecursively(scope);
    }

    private void InferCallableReturnClrTypesRecursively(Scope scope)
    {
        // Very conservative: only infer stable return types when we can trivially prove
        // a single stable primitive return (bool/double) with no control-flow ambiguity.
        if (scope.Kind == ScopeKind.Function)
        {
            // Reset per-run inferred markers.
            scope.StableReturnIsThis = false;

            // Async/generator callables never return the direct expression value in JS;
            // they return a Promise/Iterator wrapper object.
            // Inferring a primitive return CLR type here would corrupt the emitted method signature.
            scope.StableReturnClrType = (scope.IsAsync || scope.IsGenerator)
                ? null
                : InferStableReturnClrTypeForCallableScope(scope);
        }

        foreach (var child in scope.Children)
        {
            InferCallableReturnClrTypesRecursively(child);
        }
    }

    private Type? InferStableReturnClrTypeForCallableScope(Scope callableScope)
    {
        // Ignore class constructors.
        if (callableScope.Parent?.Kind == ScopeKind.Class &&
            string.Equals(callableScope.Name, "constructor", StringComparison.Ordinal))
        {
            return null;
        }

        // Class methods are commonly represented as MethodDefinition scopes (not FunctionExpression),
        // with the actual function body living on MethodDefinition.Value.
        if (callableScope.AstNode is MethodDefinition md && md.Value is FunctionExpression mfunc)
        {
            if (mfunc.Body is not BlockStatement mbody)
            {
                return null;
            }

            return InferStableReturnClrTypeFromBlockBody(callableScope, mfunc, mbody);
        }

        if (callableScope.AstNode is FunctionExpression funcExpr)
        {
            if (funcExpr.Body is not BlockStatement body)
            {
                return null;
            }

            return InferStableReturnClrTypeFromBlockBody(callableScope, funcExpr, body);
        }

        if (callableScope.AstNode is FunctionDeclaration funcDecl)
        {
            if (funcDecl.Body is not BlockStatement body)
            {
                return null;
            }

            return InferStableReturnClrTypeFromBlockBody(callableScope, funcDecl, body);
        }

        if (callableScope.AstNode is ArrowFunctionExpression arrowExpr)
        {
            // Expression-bodied arrows are an implicit return.
            if (arrowExpr.Body is not BlockStatement body)
            {
                var inferredExpr = InferExpressionClrType(arrowExpr.Body, callableScope);
                return inferredExpr == typeof(double) || inferredExpr == typeof(bool)
                    ? inferredExpr
                    : null;
            }

            return InferStableReturnClrTypeFromBlockBody(callableScope, arrowExpr, body);
        }

        return null;
    }

    private Type? InferStableReturnClrTypeFromBlockBody(Scope callableScope, Node functionBoundaryNode, BlockStatement body)
    {
        // Bail out on try/finally/catch: return epilogues in lowering are currently object-typed.
        bool hasTry = false;
        var returns = new List<ReturnStatement>();

        void Walk(Node? node)
        {
            if (node == null || hasTry)
            {
                return;
            }

            // Do not traverse into nested function boundaries.
            if (node is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                if (!ReferenceEquals(node, functionBoundaryNode))
                {
                    return;
                }
            }

            if (node is TryStatement)
            {
                hasTry = true;
                return;
            }

            if (node is ReturnStatement rs)
            {
                returns.Add(rs);
                return;
            }

            foreach (var child in node.ChildNodes)
            {
                Walk(child);
                if (hasTry) return;
            }
        }

        Walk(body);

        if (hasTry)
        {
            return null;
        }

        // Stable `return this` inference for class instance methods.
        // If every return statement returns `this` (and there are no bare returns), we can safely
        // treat the callable as returning the receiver type for chaining.
        if (callableScope.Parent?.Kind == ScopeKind.Class
            && returns.Count > 0
            && returns.All(r => r.Argument is ThisExpression))
        {
            callableScope.StableReturnIsThis = true;
            return null;
        }

        // Require exactly one return statement.
        if (returns.Count != 1)
        {
            return null;
        }

        var onlyReturn = returns[0];
        if (onlyReturn.Argument == null)
        {
            return null;
        }

        // Special-case: class instance methods that return `this`.
        // We record this separately from StableReturnClrType because the target CLR type is a user-defined
        // generated TypeDef (not representable as a System.Type at inference time).
        if (callableScope.Parent?.Kind == ScopeKind.Class
            && onlyReturn.Argument is ThisExpression)
        {
            callableScope.StableReturnIsThis = true;
            return null;
        }

        // Require the return to be the final *top-level* statement.
        // If the return is nested (if/loop), control-flow analysis is required; we don't do that here.
        if (body.Body.Count == 0 || !ReferenceEquals(body.Body.Last(), onlyReturn))
        {
            return null;
        }

        var inferred = InferExpressionClrType(onlyReturn.Argument, callableScope);

        // Only allow a small, well-understood value-like primitive set.
        // (String return typing needs additional lowering guarantees; keep it disabled for now.)
        if (inferred == typeof(double) || inferred == typeof(bool))
        {
            return inferred;
        }

        return null;
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

                var inferredType = InferExpressionClrType(variableDeclarator.Init, scope, proposedClrTypes);
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
                           var rightType = InferExpressionClrType(assignExpr.Right, scope, proposedClrTypes);
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
                            var rightType = InferExpressionClrType(assignExpr.Right, scope, proposedClrTypes);
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

        Type? TryInferNewExpressionIntrinsicClrType(Node expr)
        {
            if (expr is not NewExpression ne)
            {
                return null;
            }

            if (ne.Callee is not Identifier calleeId)
            {
                return null;
            }

            // If the callee name maps to an intrinsic runtime type, infer that CLR type.
            // This enables strongly-typed class fields for patterns like: this.buf = new Int32Array(n)
            // (and similar intrinsic constructors).
            return JavaScriptRuntime.IntrinsicObjectRegistry.Get(calleeId.Name);
        }

        Type? InferClassFieldExpressionClrType(Node expr)
        {
            switch (expr)
            {
                case NumericLiteral:
                    return typeof(double);
                case StringLiteral:
                    return typeof(string);
                case BooleanLiteral:
                    return typeof(bool);
                case NullLiteral:
                    return typeof(JavaScriptRuntime.JsNull);
                case MemberExpression me when me.Object is ThisExpression && !me.Computed:
                {
                    var name = me.Property switch
                    {
                        Identifier id => id.Name,
                        PrivateIdentifier pid => pid.Name,
                        _ => null
                    };

                    if (name != null && proposedClr.TryGetValue(name, out var existingType))
                    {
                        return existingType;
                    }

                    return null;
                }
                case NonLogicalBinaryExpression binExpr:
                {
                    var leftType = InferClassFieldExpressionClrType(binExpr.Left);
                    var rightType = InferClassFieldExpressionClrType(binExpr.Right);

                    switch (binExpr.Operator)
                    {
                        case Operator.Addition:
                        {
                            if (leftType == typeof(string) || rightType == typeof(string))
                            {
                                return typeof(string);
                            }

                            bool leftIsSupportedNumberLike = leftType == typeof(double) || leftType == typeof(bool) || leftType == typeof(JavaScriptRuntime.JsNull);
                            bool rightIsSupportedNumberLike = rightType == typeof(double) || rightType == typeof(bool) || rightType == typeof(JavaScriptRuntime.JsNull);

                            return (leftIsSupportedNumberLike && rightIsSupportedNumberLike) ? typeof(double) : null;
                        }
                        case Operator.Subtraction:
                        case Operator.Multiplication:
                        case Operator.Division:
                        {
                            bool leftIsSupportedNumberLike = leftType == typeof(double) || leftType == typeof(bool) || leftType == typeof(JavaScriptRuntime.JsNull);
                            bool rightIsSupportedNumberLike = rightType == typeof(double) || rightType == typeof(bool) || rightType == typeof(JavaScriptRuntime.JsNull);
                            return (leftIsSupportedNumberLike && rightIsSupportedNumberLike) ? typeof(double) : null;
                        }
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

                    return null;
                }
            }

            return null;
        }

        // Include class field initializers: `field = <expr>`.
        foreach (var pdef in classDecl.Body.Body.Where(n => n is PropertyDefinition).Cast<PropertyDefinition>())
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
                    var intrinsicClr = TryInferNewExpressionIntrinsicClrType(init);
                    ProposeClr(name, intrinsicClr ?? InferExpressionClrType(init));
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
                        var intrinsicClr = TryInferNewExpressionIntrinsicClrType(assign.Right);
                        ProposeClr(propName, intrinsicClr ?? InferClassFieldExpressionClrType(assign.Right));
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
        foreach (var method in classDecl.Body.Body.Where(n => n is MethodDefinition).Cast<MethodDefinition>())
        {
            if (method.Value is FunctionExpression fe)
            {
                Walk(fe.Body);
            }
            else if (method.Value is not null)
            {
                // Normal class methods (including getters/setters) are typically FunctionExpression,
                // but walk any other representation to avoid missing assignments.
                Walk(method.Value);
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

    Type? InferExpressionClrType(Node expr, Scope? scope = null, Dictionary<string, Type>? proposedTypes = null)
    {
        static bool IsSupportedNumberLike(Type? t) =>
            t == typeof(double) || t == typeof(bool) || t == typeof(JavaScriptRuntime.JsNull);

        static bool IsIdentifierShadowed(Scope? s, string name)
        {
            var current = s;
            while (current != null)
            {
                if (current.Bindings.ContainsKey(name))
                {
                    return true;
                }
                current = current.Parent;
            }
            return false;
        }

        static bool IsSupportedMathNumberMethod(string? name) =>
            name != null && (name == "abs" || name == "acos" || name == "acosh" || name == "asin" || name == "asinh" ||
                             name == "atan" || name == "atan2" || name == "atanh" || name == "cbrt" || name == "ceil" ||
                             name == "clz32" || name == "cos" || name == "cosh" || name == "exp" || name == "expm1" ||
                             name == "floor" || name == "fround" || name == "hypot" || name == "imul" || name == "log" ||
                             name == "log10" || name == "log1p" || name == "log2" || name == "max" || name == "min" ||
                             name == "pow" || name == "random" || name == "round" || name == "sign" || name == "sin" ||
                             name == "sinh" || name == "sqrt" || name == "tan" || name == "tanh" || name == "trunc");

        Scope? FindEnclosingClassScope(Scope? s)
        {
            var current = s;
            while (current != null)
            {
                if (current.Kind == ScopeKind.Class)
                {
                    return current;
                }
                current = current.Parent;
            }
            return null;
        }

        switch (expr)
        {
            case Identifier id:
            {
                // First check proposedTypes (types being inferred in current pass)
                if (proposedTypes != null && proposedTypes.TryGetValue(id.Name, out var proposedType))
                {
                    return proposedType;
                }
                
                // Then look up the identifier's type from bindings in the current scope or parent scopes
                if (scope != null)
                {
                    var currentScope = scope;
                    while (currentScope != null)
                    {
                        if (currentScope.Bindings.TryGetValue(id.Name, out var binding) && binding.ClrType != null)
                        {
                            return binding.ClrType;
                        }
                        currentScope = currentScope.Parent;
                    }
                }
                return null;
            }
            case NumericLiteral:
                return typeof(double);
            case StringLiteral:
                return typeof(string);
            case BooleanLiteral:
                return typeof(bool);
            case NullLiteral:
                // Treat JavaScript `null` as a distinct known value.
                return typeof(JavaScriptRuntime.JsNull);
            case ArrayExpression:
                // Array literals always compile to the runtime Array implementation.
                return typeof(JavaScriptRuntime.Array);
            case NewExpression ne:
            {
                // new Array(...)
                if (ne.Callee is Identifier ctorId && string.Equals(ctorId.Name, "Array", StringComparison.Ordinal))
                {
                    return typeof(JavaScriptRuntime.Array);
                }

                // new <Intrinsic>(...) (e.g., Int32Array)
                if (ne.Callee is Identifier intrinsicCtorId)
                {
                    return JavaScriptRuntime.IntrinsicObjectRegistry.Get(intrinsicCtorId.Name);
                }

                return null;
            }
            case MemberExpression me:
            {
                // this.<field>
                if (me.Object is ThisExpression && !me.Computed)
                {
                    var fieldName = me.Property switch
                    {
                        Identifier fid => fid.Name,
                        PrivateIdentifier pid => pid.Name,
                        _ => null
                    };

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        var classScope = FindEnclosingClassScope(scope);
                        if (classScope != null && classScope.StableInstanceFieldClrTypes.TryGetValue(fieldName, out var fieldClrType))
                        {
                            return fieldClrType;
                        }
                    }

                    return null;
                }

                // <expr>.length
                if (!me.Computed && me.Property is Identifier propId && string.Equals(propId.Name, "length", StringComparison.Ordinal))
                {
                    var receiverType = InferExpressionClrType(me.Object, scope, proposedTypes);
                    if (receiverType == typeof(JavaScriptRuntime.Array) || receiverType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        return typeof(double);
                    }
                }

                // <typedArray>[index]
                if (me.Computed)
                {
                    var receiverType = InferExpressionClrType(me.Object, scope, proposedTypes);
                    if (receiverType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        var indexType = InferExpressionClrType(me.Property, scope, proposedTypes);
                        // Numeric index required (JS will ToNumber, but we only infer when it's already clearly number-like).
                        if (IsSupportedNumberLike(indexType))
                        {
                            return typeof(double);
                        }
                    }
                }

                return null;
            }
            case CallExpression ce:
            {
                // Math.*(...) numeric helpers
                // (e.g., const q = Math.ceil(Math.sqrt(this.sieveSizeInBits));)
                if (ce.Callee is MemberExpression mathMe &&
                    !mathMe.Computed &&
                    mathMe.Object is Identifier mathId &&
                    string.Equals(mathId.Name, "Math", StringComparison.Ordinal) &&
                    mathMe.Property is Identifier mathMethodId &&
                    !IsIdentifierShadowed(scope, "Math") &&
                    IsSupportedMathNumberMethod(mathMethodId.Name))
                {
                    return typeof(double);
                }

                // Array.of(...) / Array.from(...)
                if (ce.Callee is MemberExpression me && me.Object is Identifier objId && string.Equals(objId.Name, "Array", StringComparison.Ordinal))
                {
                    if (me.Property is Identifier propId)
                    {
                        if (string.Equals(propId.Name, "of", StringComparison.Ordinal))
                        {
                            return typeof(JavaScriptRuntime.Array);
                        }
                        if (string.Equals(propId.Name, "from", StringComparison.Ordinal))
                        {
                            return typeof(JavaScriptRuntime.Array);
                        }
                        if (string.Equals(propId.Name, "isArray", StringComparison.Ordinal))
                        {
                            return typeof(bool);
                        }
                    }
                }

                // Array instance methods - use reflection to get return type
                if (ce.Callee is MemberExpression instanceMe && instanceMe.Property is Identifier methodId)
                {
                    var receiverType = InferExpressionClrType(instanceMe.Object, scope, proposedTypes);
                    if (receiverType == typeof(JavaScriptRuntime.Array))
                    {
                        // Use reflection to determine return type of Array methods
                        // GetMethods to handle overloads - we just need to check if any overload returns a specific type
                        var methods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            .Where(m => string.Equals(m.Name, methodId.Name, StringComparison.Ordinal))
                            .ToList();
                        
                        if (methods.Count > 0)
                        {
                            // Check if all overloads have the same return type
                            var returnType = methods[0].ReturnType;
                            if (methods.All(m => m.ReturnType == returnType) && returnType != typeof(void))
                            {
                                // Only return primitive types that we can safely infer
                                if (returnType == typeof(bool) || returnType == typeof(double))
                                {
                                    return returnType;
                                }
                                // For object return types, check if it's Array
                                else if (returnType == typeof(object) || returnType == receiverType)
                                {
                                    // Some methods like slice, map return Array but are typed as object
                                    // We already handle these cases elsewhere, so skip here
                                }
                            }
                        }
                    }
                }

                return null;
            }
            case NonLogicalBinaryExpression binExpr:
            {

                switch (binExpr.Operator)
                {
                    case Operator.Addition:
                        return InferAddOperatorType(binExpr);
                    case Operator.Subtraction:
                    case Operator.Multiplication:
                    case Operator.Division:
                        return InferNumericBinaryOperatorType(binExpr);
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

        // Only infer numeric `+` when we can prove both sides are number-like.
        // IMPORTANT: `null` here means "unknown/uninferred", not JavaScript null/undefined.
        bool LeftIsSupportedNumberLike = leftType == typeof(double) || leftType == typeof(bool) || leftType == typeof(JavaScriptRuntime.JsNull);
        bool RightIsSupportedNumberLike = rightType == typeof(double) || rightType == typeof(bool) || rightType == typeof(JavaScriptRuntime.JsNull);

        if (LeftIsSupportedNumberLike && RightIsSupportedNumberLike)
        {
            return typeof(double);
        }

        // If we get here, at least one side is an unsupported/unknown non-string type
        // (e.g. Object, Symbol, BigInt or some other Type) - we don't infer the result.
        return null;
    }

    Type? InferNumericBinaryOperatorType(NonLogicalBinaryExpression binaryExpression)
    {
        var leftType = InferExpressionClrType(binaryExpression.Left);
        var rightType = InferExpressionClrType(binaryExpression.Right);

        // Only infer numeric operators when we can prove both sides are number-like.
        // IMPORTANT: `null` here means "unknown/uninferred", not JavaScript null/undefined.
        bool leftIsSupportedNumberLike = leftType == typeof(double) || leftType == typeof(bool) || leftType == typeof(JavaScriptRuntime.JsNull);
        bool rightIsSupportedNumberLike = rightType == typeof(double) || rightType == typeof(bool) || rightType == typeof(JavaScriptRuntime.JsNull);

        if (leftIsSupportedNumberLike && rightIsSupportedNumberLike)
        {
            return typeof(double);
        }

        return null;
    }
}