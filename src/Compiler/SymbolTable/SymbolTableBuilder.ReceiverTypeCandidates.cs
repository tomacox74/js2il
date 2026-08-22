using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferReceiverTypeCandidates(Scope root)
    {
        var stringObjectCandidateBindings = new HashSet<BindingInfo>();
        Dictionary<Node, Node>? parentMap = null;
        var capturedBindingsByScope =
            new Dictionary<Scope, IReadOnlyList<BindingInfo>>();
        var receiverCallTargets =
            new Dictionary<CallExpression, Scope?>(
                ReferenceEqualityComparer.Instance);
        var receiverTargetScopes = new HashSet<Scope>();

        foreach (var scope in EnumerateScopes(root))
        {
            scope.ReceiverParameterTypeSummaries.Clear();
            scope.ReceiverCapturedEntryTypeSummaries.Clear();
            scope.ReceiverReturnTypeSummary = ReceiverTypeSummary.Empty;
            scope.ReceiverThisTypeSummary = ReceiverTypeSummary.Empty;

            foreach (var (index, type) in scope.StableParameterClrTypes)
            {
                if (IsReceiverCandidateType(type))
                {
                    scope.ReceiverParameterTypeSummaries[index] =
                        ReceiverTypeSummary.ForCandidate(type);
                }
            }

            if (IsReceiverCandidateType(scope.StableReturnClrType))
            {
                scope.ReceiverReturnTypeSummary =
                    ReceiverTypeSummary.ForCandidate(scope.StableReturnClrType!);
            }

            foreach (var binding in scope.Bindings.Values)
            {
                binding.ReceiverCandidateClrTypes.Clear();
                AddStableReceiverCandidate(binding);
            }
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var scope in EnumerateScopes(root))
            {
                VisitScope(scope);
            }

            foreach (var scope in EnumerateScopes(root))
            {
                changed |= ApplyParameterCandidates(scope);
                changed |= UpdateReturnSummary(scope);
            }
        }
        while (changed);

        void VisitScope(Scope scope)
        {
            var pending = new Stack<(Node Node, bool IsScopeRoot)>();
            pending.Push((scope.AstNode, true));

            while (pending.Count > 0)
            {
                var (node, isScopeRoot) = pending.Pop();
                if (!isScopeRoot
                    && scope.Children.Any(child => ReferenceEquals(child.AstNode, node)))
                {
                    continue;
                }

                switch (node)
                {
                    case VariableDeclarator
                    {
                        Id: Identifier identifier,
                        Init: { } initializer
                    }:
                        AddAssignmentCandidates(
                            TryResolveBinding(scope, identifier.Name),
                            initializer,
                            scope);
                        break;

                    case AssignmentExpression
                    {
                        Left: Identifier identifier
                    } assignment:
                    {
                        var binding = TryResolveBinding(scope, identifier.Name);
                        if (assignment.Operator == Operator.Assignment)
                        {
                            AddAssignmentCandidates(
                                binding,
                                assignment.Right,
                                scope);
                        }
                        else if (assignment.Operator == Operator.AdditionAssignment
                            && binding != null
                            && (binding.ReceiverCandidateClrTypes.Contains(typeof(string))
                                || stringObjectCandidateBindings.Contains(binding)
                                || ContainsStringCandidate(assignment.Right, scope)))
                        {
                            changed |= binding.ReceiverCandidateClrTypes.Add(typeof(string));
                        }
                        break;
                    }

                    case CallExpression call
                        when TryGetReceiverCallTarget(
                            call,
                            scope,
                            out var targetScope):
                        RecordParameterInputs(call, scope, targetScope);
                        RecordCapturedInputs(call, scope, targetScope);
                        break;

                    case AssignmentExpression
                    {
                        Operator: Operator.Assignment,
                        Left: MemberExpression member,
                        Right: FunctionExpression function
                    }
                    when TryGetIntrinsicPrototypeReceiverType(
                        member,
                        scope,
                        out var receiverType):
                    {
                        var functionScope =
                            FindScopeByAstNode(root, function);
                        if (functionScope != null)
                        {
                            var candidate =
                                new ReceiverTypeSummary(
                                    includesUnknown: true,
                                    includesNonCandidate: true,
                                    [receiverType]);
                            var merged =
                                functionScope.ReceiverThisTypeSummary
                                    .Union(candidate);
                            if (!merged.Equals(
                                    functionScope
                                        .ReceiverThisTypeSummary))
                            {
                                functionScope
                                    .ReceiverThisTypeSummary = merged;
                                changed = true;
                            }
                        }
                        break;
                    }
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push((child, false));
                }
            }
        }

        void AddAssignmentCandidates(
            BindingInfo? binding,
            Expression expression,
            Scope scope)
        {
            AddCandidates(binding, expression, scope);
            if (binding != null
                && IsStringObjectCandidate(expression, scope))
            {
                changed |= stringObjectCandidateBindings.Add(binding);
            }
        }

        void AddCandidates(BindingInfo? binding, Expression expression, Scope scope)
        {
            if (binding == null)
            {
                return;
            }

            foreach (var type in GetReceiverCandidates(expression, scope))
            {
                changed |= binding.ReceiverCandidateClrTypes.Add(type);
            }
        }

        IEnumerable<Type> GetReceiverCandidates(Expression expression, Scope scope)
        {
            if (expression is Identifier identifier
                && TryResolveBinding(scope, identifier.Name) is { } sourceBinding)
            {
                foreach (var candidate in sourceBinding.ReceiverCandidateClrTypes)
                {
                    yield return candidate;
                }

                yield break;
            }

            switch (expression)
            {
                case StringLiteral:
                    yield return typeof(string);
                    yield break;

                case ArrayExpression:
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case CallExpression
                    {
                        Callee: Identifier { Name: "String" }
                    }
                    when !IsIdentifierShadowed(scope, "String"):
                case CallExpression
                    {
                        Callee: MemberExpression
                        {
                            Computed: false,
                            Object: Identifier { Name: "String" },
                            Property: Identifier { Name: "fromCharCode" }
                        }
                    }
                    when !IsIdentifierShadowed(scope, "String"):
                    yield return typeof(string);
                    yield break;

                case NewExpression
                    {
                        Callee: Identifier constructor
                    }
                    when string.Equals(constructor.Name, "Array", StringComparison.Ordinal)
                         && !IsIdentifierShadowed(scope, "Array"):
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case CallExpression call
                    when TryGetReceiverCallTarget(
                        call,
                        scope,
                        out var targetScope):
                    foreach (var candidate in targetScope
                                 .ReceiverReturnTypeSummary
                                 .CandidateClrTypes)
                    {
                        yield return candidate;
                    }
                    yield break;

                case NewExpression
                    {
                        Callee: Identifier constructor
                    }
                    when _runtimeIntrinsicCatalog.TryGetIntrinsicObject(constructor.Name, out var intrinsic)
                         && intrinsic != null:
                    if (IsReceiverCandidateType(intrinsic.Type))
                    {
                        yield return intrinsic.Type;
                    }
                    yield break;

                case CallExpression
                    {
                        Callee: MemberExpression
                        {
                            Computed: false,
                            Object: Identifier { Name: "Array" },
                            Property: Identifier { Name: "of" or "from" }
                        }
                    }
                    when !IsIdentifierShadowed(scope, "Array"):
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case NonLogicalBinaryExpression
                    {
                        Operator: Operator.Addition
                    } addition
                    when ContainsStringCandidate(addition, scope):
                    yield return typeof(string);
                    yield break;
            }
        }

        bool IsStringObjectCandidate(Expression expression, Scope scope)
            => expression switch
            {
                NewExpression
                {
                    Callee: Identifier { Name: "String" }
                } => !IsIdentifierShadowed(scope, "String"),
                Identifier identifier => TryResolveBinding(scope, identifier.Name) is { } sourceBinding
                    && stringObjectCandidateBindings.Contains(sourceBinding),
                _ => false
            };

        bool ContainsStringCandidate(Expression expression, Scope scope)
        {
            var pending = new Stack<Expression>();
            pending.Push(expression);

            while (pending.Count > 0)
            {
                switch (pending.Pop())
                {
                    case StringLiteral:
                    case NewExpression
                        {
                            Callee: Identifier { Name: "String" }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                    case CallExpression
                        {
                            Callee: Identifier { Name: "String" }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                    case CallExpression
                        {
                            Callee: MemberExpression
                            {
                                Computed: false,
                                Object: Identifier { Name: "String" },
                                Property: Identifier { Name: "fromCharCode" }
                            }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                        return true;

                    case Identifier identifier
                        when TryResolveBinding(scope, identifier.Name) is { } binding
                            && (binding.ReceiverCandidateClrTypes.Contains(typeof(string))
                                || stringObjectCandidateBindings.Contains(binding)):
                        return true;

                    case NonLogicalBinaryExpression
                        {
                            Operator: Operator.Addition
                        } addition:
                        pending.Push(addition.Left);
                        pending.Push(addition.Right);
                        break;
                }
            }

            return false;
        }

        void RecordParameterInputs(
            CallExpression call,
            Scope callScope,
            Scope targetScope)
        {
            if (!TryGetSimpleParameterNames(
                    targetScope.AstNode,
                    out var parameterNames))
            {
                return;
            }

            for (var index = 0; index < parameterNames.Count; index++)
            {
                var summary = index < call.Arguments.Count
                    && call.Arguments[index] is Expression argument
                        ? GetReceiverSummary(argument, callScope)
                        : ReceiverTypeSummary.NonCandidate;
                changed |= UnionSummary(
                    targetScope.ReceiverParameterTypeSummaries,
                    index,
                    summary);
            }
        }

        void RecordCapturedInputs(
            CallExpression call,
            Scope callScope,
            Scope targetScope)
        {
            if (!capturedBindingsByScope.TryGetValue(
                    targetScope,
                    out var capturedBindings))
            {
                capturedBindings = FindCapturedBindings(targetScope);
                capturedBindingsByScope.Add(targetScope, capturedBindings);
            }

            foreach (var binding in capturedBindings)
            {
                ReceiverTypeSummary summary;
                if (binding.IsStableType
                    && IsReceiverCandidateType(binding.ClrType))
                {
                    summary = ReceiverTypeSummary.ForCandidate(
                        binding.ClrType!);
                }
                else if (!TryGetImmediatelyPrecedingAssignment(
                             call,
                             binding,
                             callScope,
                             out var assignedExpression,
                             out var assignmentScope))
                {
                    summary = ReceiverTypeSummary.Unknown;
                }
                else
                {
                    summary = GetReceiverSummary(
                        assignedExpression,
                        assignmentScope);
                }

                changed |= UnionSummary(
                    targetScope.ReceiverCapturedEntryTypeSummaries,
                    binding,
                    summary);
            }
        }

        IReadOnlyList<BindingInfo> FindCapturedBindings(Scope targetScope)
        {
            var possibleBindings = new List<BindingInfo>();
            var current = targetScope.Parent;
            while (current != null)
            {
                possibleBindings.AddRange(
                    current.Bindings.Values.Where(
                        static binding => binding.IsCaptured));
                current = current.Parent;
            }

            if (possibleBindings.Count == 0)
            {
                return [];
            }

            var referencedNames = new HashSet<string>(StringComparer.Ordinal);
            var pending = new Stack<Node>();
            pending.Push(targetScope.AstNode);
            while (pending.Count > 0)
            {
                var node = pending.Pop();
                if (!ReferenceEquals(node, targetScope.AstNode)
                    && node is FunctionDeclaration
                        or FunctionExpression
                        or ArrowFunctionExpression)
                {
                    continue;
                }

                if (node is Identifier identifier
                    && GetParentMap().TryGetValue(
                        identifier,
                        out var parent)
                    && !IsIdentifierDeclarationName(identifier, parent))
                {
                    referencedNames.Add(identifier.Name);
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push(child);
                }
            }

            return possibleBindings
                .Where(binding => referencedNames.Contains(binding.Name))
                .ToArray();
        }

        bool TryGetImmediatelyPrecedingAssignment(
            CallExpression call,
            BindingInfo binding,
            Scope callScope,
            out Expression expression,
            out Scope assignmentScope)
        {
            expression = null!;
            assignmentScope = null!;
            Node statement = call;
            var parents = GetParentMap();
            while (parents.TryGetValue(statement, out var parent)
                   && parent is not Acornima.Ast.Program
                       and not BlockStatement
                       and not SwitchCase)
            {
                statement = parent;
            }

            if (!parents.TryGetValue(statement, out var statementList))
            {
                return false;
            }

            IReadOnlyList<Node> statements = statementList switch
            {
                Acornima.Ast.Program program =>
                    program.Body.Cast<Node>().ToArray(),
                BlockStatement block =>
                    block.Body.Cast<Node>().ToArray(),
                SwitchCase switchCase =>
                    switchCase.Consequent.Cast<Node>().ToArray(),
                _ => []
            };
            var statementIndex = statements
                .Select((candidate, index) => (candidate, index))
                .FirstOrDefault(pair => ReferenceEquals(
                    pair.candidate,
                    statement))
                .index;
            if (statementIndex <= 0)
            {
                return false;
            }

            var preceding = statements[statementIndex - 1];
            assignmentScope = callScope;

            if (preceding is ExpressionStatement
                {
                    Expression: AssignmentExpression
                    {
                        Operator: Operator.Assignment,
                        Left: Identifier assignedIdentifier,
                        Right: var assignedValue
                    }
                }
                && ReferenceEquals(
                    TryResolveBinding(
                        assignmentScope,
                        assignedIdentifier.Name),
                    binding))
            {
                expression = assignedValue;
                return true;
            }

            if (preceding is VariableDeclaration declaration)
            {
                foreach (var declarator in declaration.Declarations)
                {
                    if (declarator is
                        {
                            Id: Identifier declaredIdentifier,
                            Init: { } initializer
                        }
                        && ReferenceEquals(
                            TryResolveBinding(
                                assignmentScope,
                                declaredIdentifier.Name),
                            binding))
                    {
                        expression = initializer;
                        return true;
                    }
                }
            }

            return false;
        }

        bool ApplyParameterCandidates(Scope scope)
        {
            if (!TryGetSimpleParameterNames(
                    scope.AstNode,
                    out var parameterNames))
            {
                return false;
            }

            var added = false;
            foreach (var (index, summary) in
                     scope.ReceiverParameterTypeSummaries)
            {
                if (index < 0
                    || index >= parameterNames.Count
                    || !scope.Bindings.TryGetValue(
                        parameterNames[index],
                        out var binding))
                {
                    continue;
                }

                foreach (var candidate in summary.CandidateClrTypes)
                {
                    added |= binding.ReceiverCandidateClrTypes.Add(candidate);
                }
            }

            return added;
        }

        bool UpdateReturnSummary(Scope scope)
        {
            if (scope.Kind != ScopeKind.Function
                || scope.IsAsync
                || scope.IsGenerator
                || !receiverTargetScopes.Contains(scope))
            {
                return false;
            }

            var inferred = GetCallableReturnSummary(scope);
            var merged = scope.ReceiverReturnTypeSummary.Union(inferred);
            if (merged.Equals(scope.ReceiverReturnTypeSummary))
            {
                return false;
            }

            scope.ReceiverReturnTypeSummary = merged;
            return true;
        }

        ReceiverTypeSummary GetCallableReturnSummary(Scope scope)
        {
            if (scope.AstNode is ArrowFunctionExpression
                {
                    Body: Expression expressionBody
                })
            {
                return GetReceiverSummary(expressionBody, scope);
            }

            BlockStatement? body = scope.AstNode switch
            {
                FunctionDeclaration function => function.Body,
                FunctionExpression function => function.Body,
                MethodDefinition { Value.Body: BlockStatement methodBody } =>
                    methodBody,
                ArrowFunctionExpression { Body: BlockStatement arrowBody } =>
                    arrowBody,
                _ => null
            };
            if (body == null)
            {
                return ReceiverTypeSummary.Unknown;
            }

            var returns = new List<ReturnStatement>();
            var pending = new Stack<Node>();
            pending.Push(body);
            while (pending.Count > 0)
            {
                var node = pending.Pop();
                if (!ReferenceEquals(node, body)
                    && node is FunctionDeclaration
                        or FunctionExpression
                        or ArrowFunctionExpression)
                {
                    continue;
                }

                if (node is ReturnStatement returnStatement)
                {
                    returns.Add(returnStatement);
                    continue;
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push(child);
                }
            }

            if (returns.Count == 1
                && body.Body.Count > 0
                && ReferenceEquals(body.Body.Last(), returns[0]))
            {
                return returns[0].Argument is Expression returnedExpression
                    ? GetReceiverSummary(returnedExpression, scope)
                    : ReceiverTypeSummary.NonCandidate;
            }

            var summary = ReceiverTypeSummary.Unknown;
            foreach (var returnStatement in returns)
            {
                summary = summary.Union(
                    returnStatement.Argument is Expression returnedExpression
                        ? GetReceiverSummary(returnedExpression, scope)
                        : ReceiverTypeSummary.NonCandidate);
            }

            return summary;
        }

        ReceiverTypeSummary GetReceiverSummary(
            Expression expression,
            Scope scope)
        {
            if (expression is Identifier identifier
                && TryResolveBinding(scope, identifier.Name) is
                    { } binding)
            {
                if (!binding.HasWrite
                    && TryGetSimpleParameterNames(
                        binding.DeclaringScope.AstNode,
                        out var parameterNames))
                {
                    var parameterIndex = parameterNames
                        .Select((name, index) => (name, index))
                        .FirstOrDefault(pair => string.Equals(
                            pair.name,
                            binding.Name,
                            StringComparison.Ordinal))
                        .index;
                    if (parameterIndex >= 0
                        && parameterIndex < parameterNames.Count
                        && string.Equals(
                            parameterNames[parameterIndex],
                            binding.Name,
                            StringComparison.Ordinal)
                        && binding.DeclaringScope
                            .ReceiverParameterTypeSummaries
                            .TryGetValue(
                                parameterIndex,
                                out var parameterSummary))
                    {
                        return parameterSummary;
                    }
                }

                if (binding.IsStableType
                    && IsReceiverCandidateType(binding.ClrType))
                {
                    return ReceiverTypeSummary.ForCandidate(
                        binding.ClrType!);
                }

                if (binding.ReceiverCandidateClrTypes.Count > 0)
                {
                    return new ReceiverTypeSummary(
                        includesUnknown: true,
                        includesNonCandidate: true,
                        binding.ReceiverCandidateClrTypes);
                }

                return ReceiverTypeSummary.Unknown;
            }

            if (expression is NonLogicalBinaryExpression
                {
                    Operator: Operator.Addition
                } addition
                && ContainsStringCandidate(addition, scope))
            {
                return new ReceiverTypeSummary(
                    includesUnknown: true,
                    includesNonCandidate: true,
                    [typeof(string)]);
            }

            var candidates = GetReceiverCandidates(expression, scope)
                .Distinct()
                .ToArray();
            if (candidates.Length > 0)
            {
                return ReceiverTypeSummary.ForCandidate(candidates[0])
                    .Union(new ReceiverTypeSummary(
                        includesUnknown: false,
                        includesNonCandidate: false,
                        candidates.Skip(1)));
            }

            return expression is NumericLiteral
                or BooleanLiteral
                or NullLiteral
                ? ReceiverTypeSummary.NonCandidate
                : ReceiverTypeSummary.Unknown;
        }

        bool TryGetReceiverCallTarget(
            CallExpression call,
            Scope callScope,
            out Scope targetScope)
        {
            targetScope = null!;
            if (receiverCallTargets.TryGetValue(
                    call,
                    out var cachedTarget))
            {
                targetScope = cachedTarget!;
                if (cachedTarget != null)
                {
                    receiverTargetScopes.Add(cachedTarget);
                }
                return cachedTarget != null;
            }

            if (call.Optional
                || call.Arguments.Any(
                    argument => argument is SpreadElement))
            {
                receiverCallTargets.Add(call, null);
                return false;
            }

            if (call.Callee is FunctionExpression
                or ArrowFunctionExpression)
            {
                var directScope = FindScopeByAstNode(
                    root,
                    call.Callee);
                if (directScope == null
                    || StableDirectCallableEligibility
                        .GetCallableIneligibilityReason(
                            call.Callee,
                            directScope)
                        != CallableMaterializationReason.None)
                {
                    receiverCallTargets.Add(call, null);
                    return false;
                }

                targetScope = directScope;
                receiverCallTargets.Add(call, targetScope);
                receiverTargetScopes.Add(targetScope);
                return true;
            }

            if (call.Callee is not Identifier calleeIdentifier
                || TryResolveBinding(
                    callScope,
                    calleeIdentifier.Name) is not { } callableBinding
                || callableBinding.CallableMaterialization?.Kind
                    != CallableMaterializationKind.DirectOnly
                || !StableDirectCallableEligibility.TryGetEligibleCall(
                    callableBinding,
                    call,
                    callScope,
                    out _,
                    out var eligibleScope,
                    out _)
                || eligibleScope == null)
            {
                receiverCallTargets.Add(call, null);
                return false;
            }

            targetScope = eligibleScope;
            receiverCallTargets.Add(call, targetScope);
            receiverTargetScopes.Add(targetScope);
            return true;
        }

        Dictionary<Node, Node> GetParentMap()
            => parentMap ??= BuildParentMap(root.AstNode);

        bool TryGetIntrinsicPrototypeReceiverType(
            MemberExpression assignedMember,
            Scope assignmentScope,
            out Type receiverType)
        {
            receiverType = null!;
            if (assignedMember.Object is not MemberExpression
                {
                    Computed: false,
                    Object: Identifier constructor,
                    Property: Identifier
                    {
                        Name: "prototype"
                    }
                }
                || IsIdentifierShadowed(
                    assignmentScope,
                    constructor.Name))
            {
                return false;
            }

            receiverType = constructor.Name switch
            {
                "Array" => typeof(JavaScriptRuntime.Array),
                _ => null!
            };
            return receiverType != null;
        }

        static bool UnionSummary<TKey>(
            Dictionary<TKey, ReceiverTypeSummary> summaries,
            TKey key,
            ReceiverTypeSummary incoming)
            where TKey : notnull
        {
            if (!summaries.TryGetValue(key, out var existing))
            {
                summaries.Add(key, incoming);
                return true;
            }

            var merged = existing.Union(incoming);
            if (merged.Equals(existing))
            {
                return false;
            }

            summaries[key] = merged;
            return true;
        }
    }

    private static void AddStableReceiverCandidate(BindingInfo binding)
    {
        if (IsReceiverCandidateType(binding.ClrType))
        {
            binding.ReceiverCandidateClrTypes.Add(binding.ClrType!);
        }
    }

    private static bool IsReceiverCandidateType(Type? type)
        => type == typeof(string)
           || type is
           {
               IsAbstract: false,
               IsSealed: false,
               Namespace: { } namespaceName
           }
           && namespaceName.StartsWith("JavaScriptRuntime", StringComparison.Ordinal);
}
