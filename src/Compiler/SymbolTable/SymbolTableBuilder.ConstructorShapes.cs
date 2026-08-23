using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void AnalyzeConstructorShapes(Scope root)
    {
        foreach (var scope in EnumerateScopes(root))
        {
            scope.ConstructorShape = null;
            foreach (var binding in scope.Bindings.Values)
            {
                binding.ConstructorShape = null;
                binding.ConstructedShape = null;
                binding.ConstructorShapeCandidates.Clear();
            }
        }

        foreach (var scope in EnumerateScopes(root))
        {
            foreach (var binding in scope.Bindings.Values)
            {
                if (!TryGetConstructorFunction(binding, root, out var constructorNode, out var constructorScope, out var body)
                    || constructorScope.IsAsync
                    || constructorScope.IsGenerator
                    || constructorScope.IsMethodDefinition
                    || binding.HasNonInitializationWrite)
                {
                    continue;
                }

                var shape = AnalyzeConstructorBody(
                    constructorNode,
                    constructorScope,
                    binding,
                    body);
                binding.ConstructorShape = shape;
                constructorScope.ConstructorShape = shape;
            }
        }

        InferConstructorShapeCandidates(root);
    }

    private static bool TryGetConstructorFunction(
        BindingInfo binding,
        Scope root,
        out Node constructorNode,
        out Scope constructorScope,
        out BlockStatement body)
    {
        constructorNode = null!;
        constructorScope = null!;
        body = null!;

        switch (binding.DeclarationNode)
        {
            case FunctionDeclaration function
                when function.Body is BlockStatement functionBody:
                constructorNode = function;
                body = functionBody;
                break;
            case VariableDeclarator
            {
                Init: FunctionExpression function
            } when function.Body is BlockStatement functionBody:
                constructorNode = function;
                body = functionBody;
                break;
            default:
                return false;
        }

        constructorScope = FindScopeByAstNode(root, constructorNode)!;
        return constructorScope != null;
    }

    private static ConstructorShapeInfo AnalyzeConstructorBody(
        Node constructorNode,
        Scope constructorScope,
        BindingInfo binding,
        BlockStatement body)
    {
        var members = new List<ObjectLiteralMemberInfo>();
        var initializerAssignments = new HashSet<AssignmentExpression>(
            ReferenceEqualityComparer.Instance);
        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        var initializationPrefixEnded = false;

        foreach (var statement in body.Body)
        {
            if (TryGetTopLevelThisAssignment(
                    statement,
                    out var assignment,
                    out var memberName,
                    out var value))
            {
                if (initializationPrefixEnded)
                {
                    var lateShape = new ConstructorShapeInfo(
                        constructorNode,
                        constructorScope,
                        binding,
                        members);
                    lateShape.Disqualify(
                        $"member '{memberName}' is initialized after the unconditional top-level prefix");
                    return lateShape;
                }

                if (!seenNames.Add(memberName))
                {
                    var duplicateShape = new ConstructorShapeInfo(
                        constructorNode,
                        constructorScope,
                        binding,
                        members);
                    duplicateShape.Disqualify(
                        $"member '{memberName}' is initialized more than once");
                    return duplicateShape;
                }

                if (ContainsThisOutsideNestedFunctions(value))
                {
                    var escapingShape = new ConstructorShapeInfo(
                        constructorNode,
                        constructorScope,
                        binding,
                        members);
                    escapingShape.Disqualify(
                        $"this escapes while initializing member '{memberName}'");
                    return escapingShape;
                }

                initializerAssignments.Add(assignment);
                members.Add(new ObjectLiteralMemberInfo(
                    memberName,
                    value,
                    clrType: null,
                    isFunction: false));
                continue;
            }

            initializationPrefixEnded = true;
        }

        var shape = new ConstructorShapeInfo(
            constructorNode,
            constructorScope,
            binding,
            members);
        var pending = new Stack<(Node Node, bool InsideArrow)>();
        pending.Push((body, false));
        while (pending.Count > 0 && shape.IsEligible)
        {
            var (node, insideArrow) = pending.Pop();
            if (!ReferenceEquals(node, body)
                && node is FunctionDeclaration
                    or FunctionExpression
                    or ClassDeclaration
                    or ClassExpression)
            {
                continue;
            }

            var childInsideArrow =
                insideArrow || node is ArrowFunctionExpression;

            switch (node)
            {
                case AssignmentExpression assignment
                    when TryGetThisMember(assignment.Left, out var name, out var computed):
                    if (computed)
                    {
                        shape.Disqualify("computed this-member initialization");
                    }
                    else if (!initializerAssignments.Contains(assignment))
                    {
                        shape.Disqualify(
                            $"member '{name}' is initialized conditionally or outside the top-level prefix");
                    }
                    break;

                case UpdateExpression update
                    when TryGetThisMember(update.Argument, out var updateName, out _):
                    shape.Disqualify(
                        $"member '{updateName}' is updated outside supported initialization");
                    break;

                case UnaryExpression
                {
                    Operator: Operator.Delete,
                    Argument: var argument
                } when TryGetThisMember(argument, out var deletedName, out _):
                    shape.Disqualify($"delete this.{deletedName}");
                    break;

                case CallExpression call
                    when IsObjectDefinePropertyOnThis(call):
                    shape.Disqualify("Object.defineProperty(this, ...) is not supported");
                    break;

                case ReturnStatement
                {
                    Argument: not null
                } when !insideArrow:
                    shape.Disqualify("explicit constructor return value may override the receiver");
                    break;
            }

            foreach (var child in node.ChildNodes)
            {
                pending.Push((child, childInsideArrow));
            }
        }

        if (shape.IsEligible && members.Count == 0)
        {
            shape.Disqualify(
                "constructor has no unconditional top-level this-member initializers");
        }

        return shape;
    }

    private static bool TryGetTopLevelThisAssignment(
        Statement statement,
        out AssignmentExpression assignment,
        out string memberName,
        out Node value)
    {
        assignment = null!;
        memberName = string.Empty;
        value = null!;
        if (statement is not ExpressionStatement
            {
                Expression: AssignmentExpression
                {
                    Operator: Operator.Assignment
                } candidate
            }
            || !TryGetThisMember(candidate.Left, out memberName, out var computed)
            || computed)
        {
            return false;
        }

        assignment = candidate;
        value = candidate.Right;
        return true;
    }

    private static bool TryGetThisMember(
        Node node,
        out string memberName,
        out bool computed)
    {
        memberName = string.Empty;
        computed = false;
        if (node is not MemberExpression
            {
                Object: ThisExpression
            } member)
        {
            return false;
        }

        computed = member.Computed;
        if (!member.Computed && member.Property is Identifier identifier)
        {
            memberName = identifier.Name;
        }
        else
        {
            memberName = "<computed>";
        }

        return true;
    }

    private static bool ContainsThisOutsideNestedFunctions(Node node)
    {
        var pending = new Stack<Node>();
        pending.Push(node);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (!ReferenceEquals(current, node)
                && current is FunctionDeclaration
                    or FunctionExpression
                    or ClassDeclaration
                    or ClassExpression)
            {
                continue;
            }

            if (current is ThisExpression)
            {
                return true;
            }

            foreach (var child in current.ChildNodes)
            {
                pending.Push(child);
            }
        }

        return false;
    }

    private static bool IsObjectDefinePropertyOnThis(CallExpression call)
        => call.Callee is MemberExpression
            {
                Computed: false,
                Object: Identifier
                {
                    Name: "Object"
                },
                Property: Identifier
                {
                    Name: "defineProperty"
                }
            }
            && call.Arguments.Count > 0
            && call.Arguments[0] is ThisExpression;

    private void InferConstructorShapeCandidates(Scope root)
    {
        var prototypeMethods = FindPrototypeMethodScopes(root);
        var eligibleShapes = EnumerateScopes(root)
            .SelectMany(static scope => scope.Bindings.Values)
            .Select(static binding => binding.ConstructorShape)
            .Where(static shape => shape is { IsEligible: true })
            .Cast<ConstructorShapeInfo>()
            .Distinct(ReferenceEqualityComparer.Instance)
            .Cast<ConstructorShapeInfo>()
            .ToArray();

        bool changed;
        do
        {
            changed = false;
            foreach (var scope in EnumerateScopes(root))
            {
                VisitScope(scope);
            }
        }
        while (changed);

        void VisitScope(Scope scope)
        {
            var pending = new Stack<(Node Node, bool IsRoot)>();
            pending.Push((scope.AstNode, true));
            while (pending.Count > 0)
            {
                var (node, isRoot) = pending.Pop();
                if (!isRoot
                    && scope.Children.Any(child => ReferenceEquals(child.AstNode, node)))
                {
                    continue;
                }

                switch (node)
                {
                    case VariableDeclarator
                    {
                        Id: Identifier identifier,
                        Init: Expression initializer
                    }:
                        changed |= AddExpressionCandidates(
                            TryResolveBinding(scope, identifier.Name),
                            initializer,
                            scope,
                            allowExact: true);
                        break;

                    case AssignmentExpression
                    {
                        Operator: Operator.Assignment,
                        Left: Identifier identifier,
                        Right: Expression value
                    }:
                        changed |= AddExpressionCandidates(
                            TryResolveBinding(scope, identifier.Name),
                            value,
                            scope,
                            allowExact: false);
                        break;

                    case CallExpression call:
                        if (TryGetCandidateCallTarget(
                                call,
                                scope,
                                prototypeMethods,
                                out var targetScope))
                        {
                            changed |= AddParameterCandidates(
                                call,
                                scope,
                                targetScope);
                        }
                        break;

                    case MemberExpression
                    {
                        Computed: false,
                        Object: Identifier receiver,
                        Property: Identifier property
                    }:
                        var receiverBinding =
                            TryResolveBinding(scope, receiver.Name);
                        if (receiverBinding != null
                            && receiverBinding.DeclaringScope.Parameters.Contains(
                                receiverBinding.Name))
                        {
                            var matches = eligibleShapes
                                .Where(shape => shape.TryGetMember(
                                    property.Name,
                                    out _))
                                .ToArray();
                            if (matches.Length == 1)
                            {
                                changed |= receiverBinding
                                    .ConstructorShapeCandidates
                                    .Add(matches[0]);
                            }
                        }
                        break;
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push((child, false));
                }
            }
        }

        bool AddExpressionCandidates(
            BindingInfo? target,
            Expression expression,
            Scope expressionScope,
            bool allowExact)
        {
            if (target == null)
            {
                return false;
            }

            var candidates = GetExpressionCandidates(
                expression,
                expressionScope)
                .Distinct(ReferenceEqualityComparer.Instance)
                .Cast<ConstructorShapeInfo>()
                .ToArray();
            var added = false;
            foreach (var candidate in candidates)
            {
                added |= target.ConstructorShapeCandidates.Add(candidate);
            }

            if (allowExact
                && !target.HasNonInitializationWrite
                && candidates.Length == 1
                && expression is NewExpression)
            {
                target.ConstructedShape = candidates[0];
            }

            return added;
        }

        IEnumerable<ConstructorShapeInfo> GetExpressionCandidates(
            Expression expression,
            Scope expressionScope)
        {
            if (expression is Identifier identifier
                && TryResolveBinding(expressionScope, identifier.Name)
                    is { } source)
            {
                if (source.ConstructedShape is { IsEligible: true } exact)
                {
                    yield return exact;
                }
                foreach (var candidate in source.ConstructorShapeCandidates)
                {
                    yield return candidate;
                }
                yield break;
            }

            if (expression is NewExpression
                {
                    Callee: Identifier constructor
                }
                && TryResolveBinding(expressionScope, constructor.Name)
                    ?.ConstructorShape is
                    {
                        IsEligible: true
                    } shape)
            {
                yield return shape;
            }
        }

        bool AddParameterCandidates(
            CallExpression call,
            Scope callScope,
            Scope targetScope)
        {
            if (!TryGetSimpleParameterNames(
                    targetScope.AstNode,
                    out var parameterNames))
            {
                return false;
            }

            var added = false;
            for (var index = 0;
                 index < parameterNames.Count
                 && index < call.Arguments.Count;
                 index++)
            {
                if (call.Arguments[index] is not Expression argument
                    || !targetScope.Bindings.TryGetValue(
                        parameterNames[index],
                        out var parameterBinding))
                {
                    continue;
                }

                foreach (var candidate in GetExpressionCandidates(
                             argument,
                             callScope))
                {
                    added |= parameterBinding
                        .ConstructorShapeCandidates
                        .Add(candidate);
                }
            }

            return added;
        }
    }

    private static Dictionary<string, Scope> FindPrototypeMethodScopes(
        Scope root)
    {
        var result = new Dictionary<string, Scope>(StringComparer.Ordinal);
        foreach (var scope in EnumerateScopes(root))
        {
            var pending = new Stack<(Node Node, bool IsRoot)>();
            pending.Push((scope.AstNode, true));
            while (pending.Count > 0)
            {
                var (node, isRoot) = pending.Pop();
                if (!isRoot
                    && scope.Children.Any(child => ReferenceEquals(
                        child.AstNode,
                        node)))
                {
                    continue;
                }

                if (node is AssignmentExpression
                    {
                        Operator: Operator.Assignment,
                        Left: MemberExpression
                        {
                            Computed: false,
                            Object: MemberExpression
                            {
                                Computed: false,
                                Property: Identifier
                                {
                                    Name: "prototype"
                                }
                            },
                            Property: Identifier method
                        },
                        Right: FunctionExpression function
                    })
                {
                    var functionScope = FindScopeByAstNode(root, function);
                    if (functionScope != null)
                    {
                        result.TryAdd(method.Name, functionScope);
                    }
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push((child, false));
                }
            }
        }

        return result;
    }

    private bool TryGetCandidateCallTarget(
        CallExpression call,
        Scope callScope,
        IReadOnlyDictionary<string, Scope> prototypeMethods,
        out Scope targetScope)
    {
        targetScope = null!;
        if (call.Callee is Identifier direct
            && TryResolveBinding(callScope, direct.Name) is { } binding)
        {
            var node = binding.DeclarationNode switch
            {
                FunctionDeclaration function => (Node)function,
                VariableDeclarator
                {
                    Init: FunctionExpression function
                } => function,
                _ => null
            };
            targetScope = node == null
                ? null!
                : FindScopeByAstNode(
                    FindRootScope(callScope)!,
                    node)!;
            return targetScope != null;
        }

        if (call.Callee is MemberExpression
            {
                Computed: false,
                Property: Identifier method
            }
            && prototypeMethods.TryGetValue(
                method.Name,
                out targetScope!))
        {
            return true;
        }

        return false;
    }
}
