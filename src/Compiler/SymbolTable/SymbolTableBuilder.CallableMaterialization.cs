using Acornima.Ast;
using Jroc.Utilities;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private static readonly CallableMaterializationReason IdentityObservableReasons =
        CallableMaterializationReason.NonCallRead
        | CallableMaterializationReason.Alias
        | CallableMaterializationReason.Export
        | CallableMaterializationReason.Return
        | CallableMaterializationReason.PropertyStorage
        | CallableMaterializationReason.ArrayStorage
        | CallableMaterializationReason.UnknownArgument
        | CallableMaterializationReason.CallApplyBind
        | CallableMaterializationReason.Reflection
        | CallableMaterializationReason.CapturedValueRead;

    private static readonly CallableMaterializationReason UnknownMaterializationReasons =
        CallableMaterializationReason.MutableOrHoistedBinding
        | CallableMaterializationReason.Reassigned
        | CallableMaterializationReason.AsyncOrGenerator
        | CallableMaterializationReason.NamedFunctionExpression
        | CallableMaterializationReason.InvocationContextRequired
        | CallableMaterializationReason.WithEnvironment
        | CallableMaterializationReason.InitializationNotProven
        | CallableMaterializationReason.OptionalCall
        | CallableMaterializationReason.SpreadCall
        | CallableMaterializationReason.RecursiveReference
        | CallableMaterializationReason.MutuallyRecursiveScc
        | CallableMaterializationReason.UnboundEvaluation;

    private static void AnalyzeCallableMaterialization(Scope globalScope)
    {
        var scopeByAstNode = new Dictionary<Node, Scope>(ReferenceEqualityComparer.Instance);
        var states = new Dictionary<BindingInfo, CallableAnalysisState>();
        CollectScopes(globalScope);

        foreach (var scope in EnumerateCallableAnalysisScopes(globalScope))
        {
            foreach (var binding in scope.Bindings.Values)
            {
                Node? initializer = binding.DeclarationNode switch
                {
                    VariableDeclarator
                    {
                        Id: Identifier declarationId,
                        Init: ArrowFunctionExpression arrow
                    } when string.Equals(
                        declarationId.Name,
                        binding.Name,
                        StringComparison.Ordinal) => arrow,
                    VariableDeclarator
                    {
                        Id: Identifier declarationId,
                        Init: FunctionExpression function
                    } when string.Equals(
                        declarationId.Name,
                        binding.Name,
                        StringComparison.Ordinal) => function,
                    FunctionDeclaration { Id: Identifier declarationId } declaration
                        when binding.Kind == BindingKind.Function
                            && string.Equals(
                                declarationId.Name,
                                binding.Name,
                                StringComparison.Ordinal) => declaration,
                    _ => null
                };
                if (initializer == null)
                {
                    continue;
                }

                var state = new CallableAnalysisState(binding, initializer);
                states.Add(binding, state);

                if (binding.Kind != BindingKind.Const)
                {
                    state.Reasons |= CallableMaterializationReason.MutableOrHoistedBinding;
                }

                if (binding.HasNonInitializationWrite)
                {
                    state.Reasons |= CallableMaterializationReason.Reassigned;
                }

                if (initializer is FunctionDeclaration)
                {
                    state.Reasons |= CallableMaterializationReason.MutableOrHoistedBinding;
                    continue;
                }

                var callableScope = StableDirectCallableEligibility.FindCallableScope(
                    initializer,
                    globalScope);
                if (callableScope == null)
                {
                    state.Reasons |= CallableMaterializationReason.InitializationNotProven;
                    continue;
                }

                state.Reasons |= StableDirectCallableEligibility.GetCallableIneligibilityReason(
                    initializer,
                    callableScope);
            }
        }

        var stateByInitializer = new Dictionary<Node, CallableAnalysisState>(
            ReferenceEqualityComparer.Instance);
        foreach (var state in states.Values)
        {
            stateByInitializer.Add(state.Initializer, state);
        }
        Visit(globalScope.AstNode, globalScope, parent: null, grandParent: null);
        MarkRecursiveComponents(states.Values);

        foreach (var state in states.Values)
        {
            var kind = state.Reasons == CallableMaterializationReason.None
                ? CallableMaterializationKind.DirectOnly
                : (state.Reasons & UnknownMaterializationReasons) != 0
                    ? CallableMaterializationKind.UnknownMaterialize
                    : (state.Reasons & IdentityObservableReasons) != 0
                        ? CallableMaterializationKind.IdentityObservable
                        : CallableMaterializationKind.UnknownMaterialize;
            state.Binding.CallableMaterialization = new CallableMaterializationDecision(
                kind,
                state.Reasons,
                state.RuntimeUseCount,
                state.DirectCallCount);
        }

        return;

        void CollectScopes(Scope scope)
        {
            scopeByAstNode.TryAdd(scope.AstNode, scope);
            foreach (var child in scope.Children)
            {
                CollectScopes(child);
            }
        }

        void Visit(Node node, Scope currentScope, Node? parent, Node? grandParent)
        {
            if (node is ForStatement forStatement)
            {
                var loopScope = forStatement.Init != null
                    && scopeByAstNode.TryGetValue(forStatement.Init, out var mappedForScope)
                        ? mappedForScope
                        : currentScope;

                if (forStatement.Init != null)
                {
                    Visit(forStatement.Init, loopScope, forStatement, parent);
                }
                if (forStatement.Test != null)
                {
                    Visit(forStatement.Test, loopScope, forStatement, parent);
                }
                if (forStatement.Update != null)
                {
                    Visit(forStatement.Update, loopScope, forStatement, parent);
                }
                Visit(forStatement.Body, loopScope, forStatement, parent);
                return;
            }

            if (node is ForOfStatement forOfStatement)
            {
                var loopScope = scopeByAstNode.TryGetValue(
                    (Node)forOfStatement.Left,
                    out var mappedForOfScope)
                        ? mappedForOfScope
                        : currentScope;

                Visit((Node)forOfStatement.Left, loopScope, forOfStatement, parent);
                Visit(forOfStatement.Right, currentScope, forOfStatement, parent);
                Visit(forOfStatement.Body, loopScope, forOfStatement, parent);
                return;
            }

            if (node is ForInStatement forInStatement)
            {
                var loopScope = scopeByAstNode.TryGetValue(
                    (Node)forInStatement.Left,
                    out var mappedForInScope)
                        ? mappedForInScope
                        : currentScope;

                Visit((Node)forInStatement.Left, loopScope, forInStatement, parent);
                Visit(forInStatement.Right, currentScope, forInStatement, parent);
                Visit(forInStatement.Body, loopScope, forInStatement, parent);
                return;
            }

            if (node is WhileStatement whileStatement)
            {
                Visit(whileStatement.Test, currentScope, whileStatement, parent);
                Visit(whileStatement.Body, currentScope, whileStatement, parent);
                return;
            }

            if (node is DoWhileStatement doWhileStatement)
            {
                Visit(doWhileStatement.Body, currentScope, doWhileStatement, parent);
                Visit(doWhileStatement.Test, currentScope, doWhileStatement, parent);
                return;
            }

            if (node is SwitchStatement switchStatement
                && scopeByAstNode.TryGetValue(switchStatement, out var switchScope))
            {
                Visit(
                    switchStatement.Discriminant,
                    currentScope,
                    switchStatement,
                    parent);
                foreach (var switchCase in switchStatement.Cases)
                {
                    if (switchCase.Test != null)
                    {
                        Visit(
                            switchCase.Test,
                            switchScope,
                            switchCase,
                            switchStatement);
                    }
                    foreach (var consequent in switchCase.Consequent)
                    {
                        Visit(
                            consequent,
                            switchScope,
                            switchCase,
                            switchStatement);
                    }
                }
                return;
            }

            if (node is CatchClause catchClause)
            {
                var catchScope = scopeByAstNode.TryGetValue(
                    catchClause.Body,
                    out var mappedCatchScope)
                        ? mappedCatchScope
                        : currentScope;
                if (catchClause.Param != null)
                {
                    Visit(catchClause.Param, catchScope, catchClause, parent);
                }
                Visit(catchClause.Body, catchScope, catchClause, parent);
                return;
            }

            if (scopeByAstNode.TryGetValue(node, out var nodeScope))
            {
                currentScope = nodeScope;
            }

            if (node is Identifier identifier
                && !IsDeclarationIdentifier(identifier, parent)
                && TryResolveBinding(currentScope, identifier.Name) is { } binding
                && states.TryGetValue(binding, out var state))
            {
                AnalyzeUse(identifier, currentScope, parent, grandParent, state);
            }

            foreach (var child in node.ChildNodes)
            {
                Visit(child, currentScope, node, parent);
            }
        }

        void AnalyzeUse(
            Identifier identifier,
            Scope useScope,
            Node? parent,
            Node? grandParent,
            CallableAnalysisState state)
        {
            state.RuntimeUseCount++;

            if (CrossesCallableCaptureBoundary(useScope, state.Binding.DeclaringScope))
            {
                state.Reasons |= CallableMaterializationReason.CapturedValueRead;
            }

            if (parent is CallExpression call
                && ReferenceEquals(call.Callee, identifier))
            {
                var owner = FindOwningCallableState(useScope, stateByInitializer);
                if (owner != null)
                {
                    owner.Dependencies.Add(state);
                    if (ReferenceEquals(owner, state))
                    {
                        state.Reasons |= CallableMaterializationReason.RecursiveReference;
                    }
                }

                if (StableDirectCallableEligibility.TryGetEligibleCall(
                        state.Binding,
                        call,
                        useScope,
                        out _,
                        out _,
                        out var failureReason))
                {
                    state.DirectCallCount++;
                }
                else
                {
                    state.Reasons |= failureReason == CallableMaterializationReason.None
                        ? CallableMaterializationReason.InitializationNotProven
                        : failureReason;
                }
                return;
            }

            if (parent?.GetType().Name.StartsWith("Export", StringComparison.Ordinal) == true)
            {
                state.Reasons |= CallableMaterializationReason.Export;
                return;
            }

            switch (parent)
            {
                case MemberExpression member when ReferenceEquals(member.Object, identifier):
                {
                    var memberName = TryGetStaticMemberName(member);
                    state.Reasons |= memberName is "call" or "apply" or "bind"
                        ? CallableMaterializationReason.CallApplyBind
                        : CallableMaterializationReason.Reflection;
                    return;
                }

                case VariableDeclarator { Init: { } init } when ReferenceEquals(init, identifier):
                    state.Reasons |= CallableMaterializationReason.Alias;
                    return;

                case AssignmentExpression assignment when ReferenceEquals(assignment.Right, identifier):
                    state.Reasons |= IsCommonJsExportTarget(assignment.Left)
                        ? CallableMaterializationReason.Export
                        : assignment.Left is MemberExpression
                            ? CallableMaterializationReason.PropertyStorage
                            : CallableMaterializationReason.Alias;
                    return;

                case ReturnStatement:
                    state.Reasons |= CallableMaterializationReason.Return;
                    return;

                case Property property when ReferenceEquals(property.Value, identifier):
                    state.Reasons |= CallableMaterializationReason.PropertyStorage;
                    return;

                case ArrayExpression:
                    state.Reasons |= CallableMaterializationReason.ArrayStorage;
                    return;

                case CallExpression argumentCall when argumentCall.Arguments.Any(
                    argument => ReferenceEquals(argument, identifier)):
                case NewExpression newExpression when newExpression.Arguments.Any(
                    argument => ReferenceEquals(argument, identifier)):
                    state.Reasons |= CallableMaterializationReason.UnknownArgument;
                    return;

                case SpreadElement when grandParent is CallExpression:
                    state.Reasons |= CallableMaterializationReason.UnknownArgument;
                    return;

                case UnaryExpression:
                    state.Reasons |= CallableMaterializationReason.Reflection;
                    return;

                default:
                    state.Reasons |= CallableMaterializationReason.NonCallRead;
                    return;
            }
        }
    }

    private static IEnumerable<Scope> EnumerateCallableAnalysisScopes(Scope scope)
    {
        yield return scope;
        foreach (var child in scope.Children)
        {
            foreach (var descendant in EnumerateCallableAnalysisScopes(child))
            {
                yield return descendant;
            }
        }
    }

    private static bool CrossesCallableCaptureBoundary(Scope scope, Scope declaringScope)
    {
        var current = scope;
        while (current != null && !ReferenceEquals(current, declaringScope))
        {
            if (current.Kind is ScopeKind.Function or ScopeKind.Class)
            {
                return true;
            }
            current = current.Parent;
        }

        return false;
    }

    private static CallableAnalysisState? FindOwningCallableState(
        Scope scope,
        IReadOnlyDictionary<Node, CallableAnalysisState> stateByInitializer)
    {
        var current = scope;
        while (current != null)
        {
            if (stateByInitializer.TryGetValue(current.AstNode, out var owner))
            {
                return owner;
            }
            current = current.Parent;
        }

        return null;
    }

    private static void MarkRecursiveComponents(
        IEnumerable<CallableAnalysisState> states)
    {
        var allStates = states.ToArray();
        foreach (var state in allStates)
        {
            foreach (var dependency in state.Dependencies)
            {
                if (!ReferenceEquals(state, dependency)
                    && HasDependencyPath(dependency, state, new HashSet<CallableAnalysisState>()))
                {
                    state.Reasons |= CallableMaterializationReason.MutuallyRecursiveScc;
                    dependency.Reasons |= CallableMaterializationReason.MutuallyRecursiveScc;
                }
            }
        }
    }

    private static bool HasDependencyPath(
        CallableAnalysisState current,
        CallableAnalysisState target,
        HashSet<CallableAnalysisState> visited)
    {
        if (!visited.Add(current))
        {
            return false;
        }

        return current.Dependencies.Any(dependency =>
            ReferenceEquals(dependency, target)
            || HasDependencyPath(dependency, target, visited));
    }

    private static bool IsDeclarationIdentifier(Identifier identifier, Node? parent)
        => parent switch
        {
            VariableDeclarator declarator when ReferenceEquals(declarator.Id, identifier) => true,
            FunctionDeclaration function when ReferenceEquals(function.Id, identifier)
                || function.Params.Any(parameter => ReferenceEquals(parameter, identifier)) => true,
            FunctionExpression function when ReferenceEquals(function.Id, identifier)
                || function.Params.Any(parameter => ReferenceEquals(parameter, identifier)) => true,
            ArrowFunctionExpression arrow when arrow.Params.Any(
                parameter => ReferenceEquals(parameter, identifier)) => true,
            CatchClause catchClause when ReferenceEquals(catchClause.Param, identifier) => true,
            MemberExpression member when !member.Computed
                && ReferenceEquals(member.Property, identifier) => true,
            Property property when !property.Computed
                && ReferenceEquals(property.Key, identifier)
                && !ReferenceEquals(property.Value, identifier) => true,
            MethodDefinition method when !method.Computed
                && ReferenceEquals(method.Key, identifier) => true,
            PropertyDefinition property when !property.Computed
                && ReferenceEquals(property.Key, identifier) => true,
            _ => false
        };

    private static string? TryGetStaticMemberName(MemberExpression member)
        => member switch
        {
            { Computed: false, Property: Identifier identifier } => identifier.Name,
            { Computed: true, Property: StringLiteral literal } => literal.Value,
            _ => null
        };

    private static bool IsCommonJsExportTarget(Node node)
    {
        if (node is Identifier { Name: "exports" })
        {
            return true;
        }

        if (node is not MemberExpression member)
        {
            return false;
        }

        if (member.Object is Identifier { Name: "exports" })
        {
            return true;
        }

        if (member.Object is MemberExpression nested
            && nested.Object is Identifier { Name: "module" }
            && TryGetStaticMemberName(nested) == "exports")
        {
            return true;
        }

        return member.Object is Identifier { Name: "module" }
            && TryGetStaticMemberName(member) == "exports";
    }

    private sealed class CallableAnalysisState(
        BindingInfo binding,
        Node initializer)
    {
        public BindingInfo Binding { get; } = binding;
        public Node Initializer { get; } = initializer;
        public CallableMaterializationReason Reasons { get; set; }
        public int RuntimeUseCount { get; set; }
        public int DirectCallCount { get; set; }
        public HashSet<CallableAnalysisState> Dependencies { get; } = new();
    }
}
