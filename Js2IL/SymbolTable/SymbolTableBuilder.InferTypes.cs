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
            scope.StableReturnArrayElementClrType = null;

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
        bool canInferStableArrayReturn = callableScope.Parent?.Kind != ScopeKind.Class;

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

        // Multi-return array inference (non-class callables only):
        // require a final top-level return and all observed returns to resolve to Array.
        if (returns.Count > 1
            && canInferStableArrayReturn
            && body.Body.Count > 0
            && body.Body.Last() is ReturnStatement lastTopLevelReturn
            && lastTopLevelReturn.Argument != null)
        {
            var finalType = InferExpressionClrType(lastTopLevelReturn.Argument, callableScope);
            if (finalType == typeof(JavaScriptRuntime.Array)
                && returns.All(r =>
                    r.Argument != null
                    && InferExpressionClrType(r.Argument, callableScope) == typeof(JavaScriptRuntime.Array)))
            {
                callableScope.StableReturnArrayElementClrType = InferCommonStableArrayElementClrType(
                    returns
                        .Where(r => r.Argument != null)
                        .Select(r => r.Argument!),
                    callableScope);
                return typeof(JavaScriptRuntime.Array);
            }
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

        bool IsIdentifierForcedNumericBeforeReturn(string name)
        {
            bool forcedNumeric = false;

            void Walk(Node? node)
            {
                if (node == null)
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

                if (node is UpdateExpression ue && ue.Argument is Identifier uid && string.Equals(uid.Name, name, StringComparison.Ordinal))
                {
                    // ++/-- forces ToNumber and the variable becomes a number afterwards.
                    forcedNumeric = true;
                    return;
                }

                if (node is AssignmentExpression ae && ae.Left is Identifier aid && string.Equals(aid.Name, name, StringComparison.Ordinal))
                {
                    var rhs = InferExpressionClrType(ae.Right, callableScope);
                    if (rhs == typeof(double))
                    {
                        forcedNumeric = true;
                        return;
                    }

                    // Any clearly non-numeric assignment makes it unsafe to infer.
                    if (rhs != null && rhs != typeof(double))
                    {
                        forcedNumeric = false;
                        return;
                    }
                }

                foreach (var child in node.ChildNodes)
                {
                    Walk(child);
                }
            }

            // Walk everything except the return itself.
            foreach (var stmt in body.Body)
            {
                if (ReferenceEquals(stmt, onlyReturn))
                {
                    break;
                }
                Walk(stmt);
            }

            return forcedNumeric;
        }

        if (inferred == null && onlyReturn.Argument is Identifier rid)
        {
            // Common Prime-style pattern in class methods:
            // searchBitFalse(index) { while (...) { index++; } return index; }
            // Parameters are object-typed, but update expressions force them numeric.
            if (IsIdentifierForcedNumericBeforeReturn(rid.Name))
            {
                inferred = typeof(double);
            }
        }

        // Only allow a small, well-understood value-like primitive set.
        if (inferred == typeof(double) || inferred == typeof(bool))
        {
            return inferred;
        }

        if (canInferStableArrayReturn && inferred == typeof(JavaScriptRuntime.Array))
        {
            callableScope.StableReturnArrayElementClrType = InferExpressionArrayElementClrType(
                onlyReturn.Argument,
                callableScope);
            return inferred;
        }

        // Allow string return typing for stable String.fromCharCode(...) call sites.
        // This keeps string-return ABI specialization targeted and predictable.
        if (inferred == typeof(string)
            && onlyReturn.Argument is CallExpression returnCall
            && returnCall.Callee is MemberExpression returnMember
            && !returnMember.Computed
            && returnMember.Object is Identifier returnObject
            && returnMember.Property is Identifier returnProperty
            && string.Equals(returnObject.Name, "String", StringComparison.Ordinal)
            && string.Equals(returnProperty.Name, "fromCharCode", StringComparison.Ordinal))
        {
            return typeof(string);
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

        foreach (var binding in scope.Bindings.Values)
        {
            binding.StableElementClrType = null;
        }

        var proposedClrTypes = new Dictionary<string, Type>();
        var unitializedClrTypes = new HashSet<string>();

        // Type inference is currently conservative:
        // - We infer stable types for uncaptured variables.
        // - Additionally, we infer stable types for captured const bindings (safe: cannot be reassigned).
        foreach (var binding in scope.Bindings.Values)
        {
            if (binding.IsCaptured && binding.Kind != BindingKind.Const)
            {
                if (TryInferStableCapturedBindingClrType(scope, binding, proposedClrTypes, out var capturedType)
                    && capturedType != null)
                {
                    proposedClrTypes[binding.Name] = capturedType;
                }

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

        // For uninitialized uncaptured bindings, also consider assignments in nested blocks/scopes.
        // This mirrors the captured-binding write analysis but is limited to reference types.
        foreach (var uninitializedBindingName in unitializedClrTypes.ToArray())
        {
            if (!scope.Bindings.TryGetValue(uninitializedBindingName, out var binding))
            {
                continue;
            }

            if (binding.IsCaptured && binding.Kind != BindingKind.Const)
            {
                continue;
            }

            var assignedType = InferCapturedBindingReferenceTypeFromWrites(scope, binding, proposedClrTypes);
            if (assignedType == null || assignedType.IsValueType)
            {
                continue;
            }

            if (!AreCapturedBindingWritesCompatible(scope, binding, assignedType, proposedClrTypes))
            {
                continue;
            }

            proposedClrTypes[uninitializedBindingName] = assignedType;
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

        InferStableArrayElementClrTypesForScope(scope);
    }

    private void InferStableArrayElementClrTypesForScope(Scope scope)
    {
        var proposedClrTypes = scope.Bindings
            .Where(kvp => kvp.Value.ClrType != null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ClrType!);

        foreach (var binding in scope.Bindings.Values)
        {
            if (!binding.IsStableType || binding.ClrType != typeof(JavaScriptRuntime.Array))
            {
                binding.StableElementClrType = null;
                continue;
            }

            binding.StableElementClrType = InferStableArrayElementClrTypeFromWrites(scope, binding, proposedClrTypes);
        }
    }

    private bool TryInferStableCapturedBindingClrType(
        Scope declaringScope,
        BindingInfo binding,
        Dictionary<string, Type> proposedClrTypes,
        out Type? inferredType)
    {
        inferredType = null;

        if (!binding.IsCaptured || binding.Kind == BindingKind.Const)
        {
            return false;
        }

        if (binding.DeclarationNode is not VariableDeclarator declarator)
        {
            return false;
        }

        if (declarator.Init == null)
        {
            var assignedType = InferCapturedBindingReferenceTypeFromWrites(declaringScope, binding, proposedClrTypes);
            if (assignedType == null)
            {
                return false;
            }

            if (!AreCapturedBindingWritesCompatible(declaringScope, binding, assignedType, proposedClrTypes))
            {
                return false;
            }

            inferredType = assignedType;
            return true;
        }

        var initializerType = InferExpressionClrType(declarator.Init, declaringScope, proposedClrTypes);
        if (initializerType == null || initializerType.IsValueType)
        {
            return false;
        }

        if (!AreCapturedBindingWritesCompatible(declaringScope, binding, initializerType, proposedClrTypes))
        {
            return false;
        }

        inferredType = initializerType;
        return true;
    }

    private Type? InferCapturedBindingReferenceTypeFromWrites(
        Scope scope,
        BindingInfo targetBinding,
        Dictionary<string, Type> proposedClrTypes)
    {
        Type? inferred = null;
        bool sawWrite = false;
        bool isCompatible = true;

        void WalkScope(Scope currentScope)
        {
            if (!isCompatible)
            {
                return;
            }
            // If this scope resolves the name to a different binding (shadowing), this subtree cannot
            // write to the captured binding we are validating.
            if (!ReferenceEquals(TryResolveBinding(currentScope, targetBinding.Name), targetBinding))
            {
                return;
            }

            bool IsChildScopeRoot(Node node)
            {
                foreach (var childScope in currentScope.Children)
                {
                    if (ReferenceEquals(childScope.AstNode, node))
                    {
                        return true;
                    }
                }

                return false;
            }

            void WalkNode(Node? node)
            {
                if (node == null || !isCompatible)
                {
                    return;
                }

                // Nested scope roots are validated in their own scope context.
                if (!ReferenceEquals(node, currentScope.AstNode) && IsChildScopeRoot(node))
                {
                    return;
                }

                if (node is AssignmentExpression assignExpr
                    && assignExpr.Left is Identifier id
                    && ReferenceEquals(TryResolveBinding(currentScope, id.Name), targetBinding))
                {
                    if (assignExpr.Operator != Operator.Assignment)
                    {
                        isCompatible = false;
                        return;
                    }

                    var rightType = InferExpressionClrType(assignExpr.Right, currentScope, proposedClrTypes);
                    if (rightType == null || rightType.IsValueType)
                    {
                        isCompatible = false;
                        return;
                    }

                    sawWrite = true;
                    if (inferred == null)
                    {
                        inferred = rightType;
                        return;
                    }

                    if (inferred != rightType)
                    {
                        isCompatible = false;
                        return;
                    }
                }
                else if (node is UpdateExpression updateExpr
                    && updateExpr.Argument is Identifier updateId
                    && ReferenceEquals(TryResolveBinding(currentScope, updateId.Name), targetBinding))
                {
                    isCompatible = false;
                    return;
                }

                foreach (var child in node.ChildNodes)
                {
                    WalkNode(child);
                }
            }

            WalkNode(currentScope.AstNode);

            foreach (var childScope in currentScope.Children)
            {
                WalkScope(childScope);
            }
        }

        WalkScope(scope);
        return isCompatible && sawWrite ? inferred : null;
    }

    private bool AreCapturedBindingWritesCompatible(
        Scope scope,
        BindingInfo targetBinding,
        Type expectedType,
        Dictionary<string, Type> proposedClrTypes)
    {
        bool isCompatible = true;

        void WalkScope(Scope currentScope)
        {
            if (!isCompatible)
            {
                return;
            }

            // If this scope resolves the name to a different binding (shadowing), this subtree cannot
            // write to the captured binding we are validating.
            if (!ReferenceEquals(TryResolveBinding(currentScope, targetBinding.Name), targetBinding))
            {
                return;
            }

            bool IsChildScopeRoot(Node node)
            {
                foreach (var childScope in currentScope.Children)
                {
                    if (ReferenceEquals(childScope.AstNode, node))
                    {
                        return true;
                    }
                }

                return false;
            }

            void WalkNode(Node? node)
            {
                if (node == null || !isCompatible)
                {
                    return;
                }

                // Nested scope roots are validated in their own scope context.
                if (!ReferenceEquals(node, currentScope.AstNode) && IsChildScopeRoot(node))
                {
                    return;
                }

                if (node is AssignmentExpression assignExpr
                    && assignExpr.Left is Identifier id
                    && ReferenceEquals(TryResolveBinding(currentScope, id.Name), targetBinding))
                {
                    if (assignExpr.Operator != Operator.Assignment)
                    {
                        isCompatible = false;
                        return;
                    }

                    var rightType = InferExpressionClrType(assignExpr.Right, currentScope, proposedClrTypes);
                    if (rightType != expectedType)
                    {
                        isCompatible = false;
                        return;
                    }
                }
                else if (node is UpdateExpression updateExpr
                    && updateExpr.Argument is Identifier updateId
                    && ReferenceEquals(TryResolveBinding(currentScope, updateId.Name), targetBinding))
                {
                    isCompatible = false;
                    return;
                }

                foreach (var child in node.ChildNodes)
                {
                    WalkNode(child);
                }
            }

            WalkNode(currentScope.AstNode);

            foreach (var childScope in currentScope.Children)
            {
                WalkScope(childScope);
            }
        }

        WalkScope(scope);
        return isCompatible;
    }

    private Type? InferCommonStableArrayElementClrType(IEnumerable<Node> returnExpressions, Scope scope)
    {
        Type? common = null;
        foreach (var returnExpr in returnExpressions)
        {
            var candidate = InferExpressionArrayElementClrType(returnExpr, scope);
            if (candidate == null)
            {
                return null;
            }

            if (common == null)
            {
                common = candidate;
                continue;
            }

            if (common != candidate)
            {
                return null;
            }
        }

        return common;
    }

    private Type? InferExpressionArrayElementClrType(Node expr, Scope? scope = null, Dictionary<string, Type>? proposedTypes = null)
    {
        static Scope? FindRootScope(Scope? s)
        {
            var current = s;
            while (current?.Parent != null)
            {
                current = current.Parent;
            }

            return current;
        }

        static Scope? FindScopeByAstNodeRecursive(Scope scopeNode, Node astNode)
        {
            if (ReferenceEquals(scopeNode.AstNode, astNode))
            {
                return scopeNode;
            }

            foreach (var child in scopeNode.Children)
            {
                var found = FindScopeByAstNodeRecursive(child, astNode);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        switch (expr)
        {
            case Identifier id when scope != null:
            {
                var binding = TryResolveBinding(scope, id.Name);
                if (binding?.IsStableType == true
                    && binding.ClrType == typeof(JavaScriptRuntime.Array))
                {
                    return binding.StableElementClrType;
                }

                return null;
            }

            case ArrayExpression arrayExpr:
            {
                Type? common = null;
                foreach (var elementNode in arrayExpr.Elements)
                {
                    if (elementNode is not Node element)
                    {
                        // Holes in array literals are not considered stable element evidence.
                        return null;
                    }

                    var elementType = InferExpressionClrType(element, scope, proposedTypes);
                    if (elementType == null || elementType.IsValueType)
                    {
                        return null;
                    }

                    if (common == null)
                    {
                        common = elementType;
                        continue;
                    }

                    if (common != elementType)
                    {
                        return null;
                    }
                }

                return common;
            }

            case CallExpression callExpr:
            {
                if (callExpr.Callee is Identifier calleeId && scope != null)
                {
                    var calleeBinding = TryResolveBinding(scope, calleeId.Name);
                    if (calleeBinding?.Kind == BindingKind.Function && calleeBinding.DeclarationNode != null)
                    {
                        var root = FindRootScope(scope);
                        if (root != null)
                        {
                            var calleeScope = FindScopeByAstNodeRecursive(root, calleeBinding.DeclarationNode);
                            if (calleeScope?.StableReturnClrType == typeof(JavaScriptRuntime.Array))
                            {
                                return calleeScope.StableReturnArrayElementClrType;
                            }
                        }
                    }
                }

                // Preserve element type for array copy-like operations.
                if (callExpr.Callee is MemberExpression member
                    && !member.Computed
                    && member.Property is Identifier methodId
                    && string.Equals(methodId.Name, "slice", StringComparison.Ordinal)
                    && InferExpressionClrType(member.Object, scope, proposedTypes) == typeof(JavaScriptRuntime.Array))
                {
                    return InferExpressionArrayElementClrType(member.Object, scope, proposedTypes);
                }

                return null;
            }
        }

        return null;
    }

    private Type? InferStableArrayElementClrTypeFromWrites(
        Scope scope,
        BindingInfo targetBinding,
        Dictionary<string, Type> proposedClrTypes)
    {
        Type? inferred = null;
        bool sawEvidence = false;
        bool isCompatible = true;
        var aliasBindings = new HashSet<BindingInfo> { targetBinding };

        static bool IsSupportedNumberLike(Type? t) =>
            t == typeof(double) || t == typeof(bool) || t == typeof(JavaScriptRuntime.JsNull);

        bool IsNeutralArrayInitialization(Node rhs, Scope currentScope, Dictionary<string, Type> proposed)
        {
            if (rhs is ArrayExpression arr)
            {
                return arr.Elements.Count == 0;
            }

            if (rhs is NewExpression ne
                && ne.Callee is Identifier ctorId
                && string.Equals(ctorId.Name, "Array", StringComparison.Ordinal))
            {
                if (ne.Arguments.Count == 0)
                {
                    return true;
                }

                if (ne.Arguments.Count == 1)
                {
                    var arg0 = ne.Arguments[0] as Node;
                    var arg0Type = arg0 != null ? InferExpressionClrType(arg0, currentScope, proposed) : null;
                    return IsSupportedNumberLike(arg0Type);
                }
            }

            return false;
        }

        void MergeCandidate(Type? candidate)
        {
            if (candidate != typeof(string))
            {
                isCompatible = false;
                return;
            }

            sawEvidence = true;
            if (inferred == null)
            {
                inferred = candidate;
                return;
            }

            if (inferred != candidate)
            {
                isCompatible = false;
            }
        }

        bool IsTargetOrAliasBinding(BindingInfo? binding) =>
            binding != null && aliasBindings.Contains(binding);

        void TrackAliasBindingFromAssignment(BindingInfo? leftBinding, Node? rightExpr, Scope currentScope)
        {
            if (leftBinding == null || rightExpr == null || ReferenceEquals(leftBinding, targetBinding))
            {
                return;
            }

            if (rightExpr is not Identifier rightIdentifier)
            {
                return;
            }

            var rightBinding = TryResolveBinding(currentScope, rightIdentifier.Name);
            if (IsTargetOrAliasBinding(rightBinding))
            {
                aliasBindings.Add(leftBinding);
            }
        }

        void ProcessArrayAssignmentRhs(Node rhs, Scope currentScope)
        {
            var rhsType = InferExpressionClrType(rhs, currentScope, proposedClrTypes);
            if (rhsType != typeof(JavaScriptRuntime.Array))
            {
                isCompatible = false;
                return;
            }

            var rhsElementType = InferExpressionArrayElementClrType(rhs, currentScope, proposedClrTypes);
            if (rhsElementType == null)
            {
                if (IsNeutralArrayInitialization(rhs, currentScope, proposedClrTypes))
                {
                    return;
                }

                isCompatible = false;
                return;
            }

            MergeCandidate(rhsElementType);
        }

        void WalkScope(Scope currentScope)
        {
            if (!isCompatible)
            {
                return;
            }

            // If this scope resolves the name to a different binding (shadowing), this subtree cannot
            // read/write the target binding.
            if (!ReferenceEquals(TryResolveBinding(currentScope, targetBinding.Name), targetBinding))
            {
                return;
            }

            bool IsChildScopeRoot(Node node)
            {
                foreach (var childScope in currentScope.Children)
                {
                    if (ReferenceEquals(childScope.AstNode, node))
                    {
                        return true;
                    }
                }

                return false;
            }

            void WalkNode(Node? node)
            {
                if (node == null || !isCompatible)
                {
                    return;
                }

                // Nested scope roots are validated in their own scope context.
                if (!ReferenceEquals(node, currentScope.AstNode) && IsChildScopeRoot(node))
                {
                    return;
                }

                if (node is VariableDeclarator declarator
                    && declarator.Id is Identifier declId
                    && declarator.Init != null)
                {
                    var declBinding = TryResolveBinding(currentScope, declId.Name);
                    TrackAliasBindingFromAssignment(declBinding, declarator.Init, currentScope);
                    if (ReferenceEquals(declBinding, targetBinding))
                    {
                        ProcessArrayAssignmentRhs(declarator.Init, currentScope);
                    }
                }
                else if (node is AssignmentExpression assignExpr)
                {
                    var leftIdentifierBinding = assignExpr.Left is Identifier assignIdentifier
                        ? TryResolveBinding(currentScope, assignIdentifier.Name)
                        : null;

                    if (assignExpr.Operator == Operator.Assignment)
                    {
                        TrackAliasBindingFromAssignment(leftIdentifierBinding, assignExpr.Right, currentScope);
                    }

                    if (assignExpr.Operator != Operator.Assignment)
                    {
                        // Compound assignments against the target are not stable for element typing.
                        if (ReferenceEquals(leftIdentifierBinding, targetBinding))
                        {
                            isCompatible = false;
                            return;
                        }

                        if (assignExpr.Left is MemberExpression compoundMember
                            && compoundMember.Object is Identifier compoundMemberObjectId
                            && ReferenceEquals(TryResolveBinding(currentScope, compoundMemberObjectId.Name), targetBinding))
                        {
                            isCompatible = false;
                            return;
                        }
                    }

                    if (ReferenceEquals(leftIdentifierBinding, targetBinding))
                    {
                        ProcessArrayAssignmentRhs(assignExpr.Right, currentScope);
                    }
                    else if (assignExpr.Left is MemberExpression memberAssign
                        && memberAssign.Object is Identifier memberObjectId
                        && IsTargetOrAliasBinding(TryResolveBinding(currentScope, memberObjectId.Name)))
                    {
                        if (!memberAssign.Computed)
                        {
                            // Non-computed writes like arr.length mutate array shape; treat as unstable.
                            isCompatible = false;
                            return;
                        }

                        var indexType = InferExpressionClrType(memberAssign.Property, currentScope, proposedClrTypes);
                        if (memberAssign.Property is StringLiteral || indexType == typeof(string))
                        {
                            isCompatible = false;
                            return;
                        }

                        var rhsType = InferExpressionClrType(assignExpr.Right, currentScope, proposedClrTypes);
                        MergeCandidate(rhsType);
                    }
                }
                else if (node is UpdateExpression updateExpr)
                {
                    if (updateExpr.Argument is Identifier updateId
                        && ReferenceEquals(TryResolveBinding(currentScope, updateId.Name), targetBinding))
                    {
                        isCompatible = false;
                        return;
                    }

                    if (updateExpr.Argument is MemberExpression memberUpdate
                        && memberUpdate.Object is Identifier updateObjectId
                        && IsTargetOrAliasBinding(TryResolveBinding(currentScope, updateObjectId.Name)))
                    {
                        isCompatible = false;
                        return;
                    }
                }

                foreach (var child in node.ChildNodes)
                {
                    WalkNode(child);
                }
            }

            WalkNode(currentScope.AstNode);

            foreach (var childScope in currentScope.Children)
            {
                WalkScope(childScope);
            }
        }

        WalkScope(scope);
        return isCompatible && sawEvidence ? inferred : null;
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
                case Literal regexLiteral when regexLiteral.Raw != null && regexLiteral.Raw.TrimStart().StartsWith("/", StringComparison.Ordinal):
                    return typeof(JavaScriptRuntime.RegExp);
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

        static Scope? FindRootScope(Scope? s)
        {
            var current = s;
            while (current?.Parent != null)
            {
                current = current.Parent;
            }
            return current;
        }

        static Scope? FindScopeByAstNode(Scope scopeNode, Node astNode)
        {
            if (ReferenceEquals(scopeNode.AstNode, astNode))
            {
                return scopeNode;
            }

            foreach (var child in scopeNode.Children)
            {
                var found = FindScopeByAstNode(child, astNode);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        static Scope? FindClassScopeRecursive(Scope scope, string className)
        {
            if (scope.Kind == ScopeKind.Class && string.Equals(scope.Name, className, StringComparison.Ordinal))
            {
                return scope;
            }

            foreach (var child in scope.Children)
            {
                var found = FindClassScopeRecursive(child, className);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        static Scope? FindScopeByAstNodeRecursive(Scope scope, Node astNode)
        {
            if (ReferenceEquals(scope.AstNode, astNode))
            {
                return scope;
            }

            foreach (var child in scope.Children)
            {
                var found = FindScopeByAstNodeRecursive(child, astNode);
                if (found != null)
                {
                    return found;
                }
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
            case Literal regexLiteral when regexLiteral.Raw != null && regexLiteral.Raw.TrimStart().StartsWith("/", StringComparison.Ordinal):
                return typeof(JavaScriptRuntime.RegExp);
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

                    if (receiverType == typeof(JavaScriptRuntime.Array))
                    {
                        var indexType = InferExpressionClrType(me.Property, scope, proposedTypes);
                        if (IsSupportedNumberLike(indexType))
                        {
                            return InferExpressionArrayElementClrType(me.Object, scope, proposedTypes);
                        }
                    }
                }

                return null;
            }
            case CallExpression ce:
            {
                // Direct call to a known function declaration/expression binding.
                // If the callee has a stable inferred return type, propagate it to this expression.
                if (ce.Callee is Identifier calleeId && scope != null)
                {
                    var calleeBinding = TryResolveBinding(scope, calleeId.Name);
                    if (calleeBinding?.Kind == BindingKind.Function && calleeBinding.DeclarationNode != null)
                    {
                        var root = FindRootScope(scope);
                        if (root != null)
                        {
                            var calleeScope = FindScopeByAstNodeRecursive(root, calleeBinding.DeclarationNode);
                            var stableReturn = calleeScope?.StableReturnClrType;
                            if (stableReturn == typeof(JavaScriptRuntime.Array))
                            {
                                return stableReturn;
                            }
                        }
                    }
                }

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

                // Calls to function declarations with stable inferred primitive return types.
                if (ce.Callee is Identifier functionId && scope != null)
                {
                    var resolvedBinding = TryResolveBinding(scope, functionId.Name);
                    if (resolvedBinding?.DeclarationNode is FunctionDeclaration functionDecl)
                    {
                        var root = FindRootScope(scope);
                        var functionScope = root != null ? FindScopeByAstNode(root, functionDecl) : null;
                        var stableReturnType = functionScope?.StableReturnClrType;
                        if (stableReturnType == typeof(double) || stableReturnType == typeof(bool) || stableReturnType == typeof(string))
                        {
                            return stableReturnType;
                        }
                    }
                }

                // this.<field>.<method>(...) where <field> is a user-class instance
                // and the method has a stable inferred primitive return type.
                if (ce.Callee is MemberExpression userMethodMe
                    && !userMethodMe.Computed
                    && userMethodMe.Property is Identifier userMethodId
                    && userMethodMe.Object is MemberExpression receiverFieldMe
                    && receiverFieldMe.Object is ThisExpression
                    && !receiverFieldMe.Computed
                    && receiverFieldMe.Property is Identifier receiverFieldId)
                {
                    var classScope = FindEnclosingClassScope(scope);
                    if (classScope != null
                        && classScope.StableInstanceFieldUserClassNames.TryGetValue(receiverFieldId.Name, out var receiverUserClassName)
                        && !string.IsNullOrEmpty(receiverUserClassName))
                    {
                        var root = FindRootScope(classScope);
                        if (root != null)
                        {
                            var receiverClassScope = FindClassScopeRecursive(root, receiverUserClassName);
                            if (receiverClassScope != null)
                            {
                                var methodScope = receiverClassScope.Children.FirstOrDefault(s =>
                                    s.Kind == ScopeKind.Function &&
                                    s.Parent?.Kind == ScopeKind.Class &&
                                    string.Equals(s.Name, userMethodId.Name, StringComparison.Ordinal));

                                if (methodScope?.StableReturnClrType == typeof(double) || methodScope?.StableReturnClrType == typeof(bool))
                                {
                                    return methodScope.StableReturnClrType;
                                }
                            }
                        }
                    }
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

                // String.fromCharCode(...)
                if (ce.Callee is MemberExpression stringMe
                    && !stringMe.Computed
                    && stringMe.Object is Identifier stringId
                    && string.Equals(stringId.Name, "String", StringComparison.Ordinal)
                    && stringMe.Property is Identifier stringMethodId
                    && string.Equals(stringMethodId.Name, "fromCharCode", StringComparison.Ordinal)
                    && !IsIdentifierShadowed(scope, "String"))
                {
                    return typeof(string);
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
                                // Some array methods (slice/map/filter/concat/...) return Array.
                                // Preserve that to unlock downstream typed array calls/indexing.
                                else if (returnType == receiverType)
                                {
                                    return receiverType;
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
                        return InferAddOperatorType(binExpr, scope, proposedTypes);
                    case Operator.Subtraction:
                    case Operator.Multiplication:
                    case Operator.Division:
                        return InferNumericBinaryOperatorType(binExpr, scope, proposedTypes);
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

    Type? InferAddOperatorType(NonLogicalBinaryExpression binaryExpression, Scope? scope, Dictionary<string, Type>? proposedTypes)
    {
        var leftType = InferExpressionClrType(binaryExpression.Left, scope, proposedTypes);
        var rightType = InferExpressionClrType(binaryExpression.Right, scope, proposedTypes);

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

    Type? InferNumericBinaryOperatorType(NonLogicalBinaryExpression binaryExpression, Scope? scope, Dictionary<string, Type>? proposedTypes)
    {
        var leftType = InferExpressionClrType(binaryExpression.Left, scope, proposedTypes);
        var rightType = InferExpressionClrType(binaryExpression.Right, scope, proposedTypes);

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
