using Acornima;
using Acornima.Ast;
using Jroc.Runtime.Node.Contracts;
using Jroc.Utilities;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    public void AnalyzeNodeModuleContractOverrideSafety(
        IEnumerable<ModuleDefinition> modules)
    {
        var candidates = new Dictionary<BindingInfo, Type>(ReferenceEqualityComparer.Instance);
        var moduleAnalyses = new List<NodeModuleAnalysis>();

        foreach (var module in modules)
        {
            var root = module.SymbolTable?.Root
                ?? throw new InvalidOperationException(
                    $"Module '{module.Path}' does not have a symbol table.");
            var parentMap = BuildParentMap(root.AstNode);
            var scopeMap = BuildNodeScopeMap(root);
            moduleAnalyses.Add(new NodeModuleAnalysis(root, parentMap, scopeMap));

            foreach (var scope in EnumerateScopes(root))
            {
                foreach (var binding in scope.Bindings.Values)
                {
                    binding.CanSkipNodeModuleOverrideGuard = false;

                    if (!binding.IsStableType
                        || binding.ClrType == null
                        || !IsNodeModuleContractType(binding.ClrType)
                        || binding.HasNonInitializationWrite
                        || binding.DeclarationNode is not VariableDeclarator
                        {
                            Id: Identifier declaredIdentifier,
                            Init: CallExpression requireCall
                        }
                        || !string.Equals(
                            declaredIdentifier.Name,
                            binding.Name,
                            StringComparison.Ordinal)
                        || !scopeMap.TryGetValue(requireCall, out var requireScope)
                        || !TryGetRequiredNodeContract(
                            requireCall,
                            requireScope,
                            out var requiredContract)
                        || requiredContract != binding.ClrType)
                    {
                        continue;
                    }

                    candidates[binding] = binding.ClrType;
                }
            }
        }

        if (candidates.Count == 0)
        {
            return;
        }

        var unsafeContracts = new HashSet<Type>();
        var hasDynamicRequire = false;

        foreach (var analysis in moduleAnalyses)
        {
            var candidateNames = new HashSet<string>(
                candidates.Keys.Select(static binding => binding.Name),
                StringComparer.Ordinal);
            var walker = new AstWalker();

            walker.Visit(analysis.Root.AstNode, node =>
            {
                if (!analysis.ScopeMap.TryGetValue(node, out var scope))
                {
                    return;
                }

                if (node is CallExpression { Callee: Identifier { Name: "eval" } })
                {
                    hasDynamicRequire = true;
                    return;
                }

                if (node is ImportExpression importExpression)
                {
                    if (importExpression.Source is Literal { Value: string moduleSpecifier }
                        && JavaScriptRuntime.Node.NodeModuleRegistry.TryGetModuleContractType(
                            moduleSpecifier,
                            out var importedContract)
                        && importedContract != null)
                    {
                        unsafeContracts.Add(importedContract);
                    }
                    else if (importExpression.Source is not Literal { Value: string })
                    {
                        hasDynamicRequire = true;
                    }

                    return;
                }

                if (node is CallExpression call
                    && IsInjectedCommonJsRequireCall(call, scope))
                {
                    if (!TryGetRequiredNodeContract(call, scope, out var contractType))
                    {
                        hasDynamicRequire |= call.Arguments.Count != 1
                            || call.Arguments[0] is not Literal { Value: string };
                    }
                    else if (!IsSafeNodeModuleAcquisition(
                        call,
                        contractType,
                        candidates,
                        analysis.ParentMap))
                    {
                        unsafeContracts.Add(contractType);
                    }

                    return;
                }

                if (node is Identifier { Name: "module" } moduleIdentifier
                    && IsNodeModuleBindingValueReference(
                        moduleIdentifier,
                        analysis.ParentMap)
                    && IsInjectedCommonJsModuleBinding(scope)
                    && IsUnsafeModuleBindingUse(moduleIdentifier, analysis.ParentMap))
                {
                    hasDynamicRequire = true;
                    return;
                }

                if (node is Identifier { Name: "require" } requireIdentifier
                    && IsNodeModuleBindingValueReference(
                        requireIdentifier,
                        analysis.ParentMap)
                    && IsInjectedCommonJsRequireBinding(scope)
                    && (!analysis.ParentMap.TryGetValue(requireIdentifier, out var requireParent)
                        || requireParent is not CallExpression requireCall
                        || !ReferenceEquals(requireCall.Callee, requireIdentifier)))
                {
                    hasDynamicRequire = true;
                    return;
                }

                if (node is not Identifier identifier
                    || !candidateNames.Contains(identifier.Name)
                    || !IsNodeModuleBindingValueReference(identifier, analysis.ParentMap))
                {
                    return;
                }

                var binding = TryResolveBinding(scope, identifier.Name);
                if (binding == null
                    || !candidates.TryGetValue(binding, out var candidateContract))
                {
                    return;
                }

                if (!IsSafeNodeModuleBindingUse(
                    identifier,
                    candidateContract,
                    analysis.ParentMap))
                {
                    unsafeContracts.Add(candidateContract);
                }
            });
        }

        if (hasDynamicRequire)
        {
            return;
        }

        foreach (var (binding, contractType) in candidates)
        {
            binding.CanSkipNodeModuleOverrideGuard = !unsafeContracts.Contains(contractType);
        }
    }

    private static bool IsNodeModuleContractType(Type type)
        => type.IsInterface
            && type.GetCustomAttributes(
                    typeof(NodeModuleInterfaceAttribute),
                    inherit: false)
                .Length == 1;

    private static bool IsInjectedCommonJsRequireCall(
        CallExpression call,
        Scope scope)
        => call.Callee is Identifier { Name: "require" }
            && IsInjectedCommonJsRequireBinding(scope);

    private static bool IsInjectedCommonJsRequireBinding(Scope scope)
        => TryResolveBinding(scope, "require") is { } binding
            && binding.DeclaringScope.Kind == ScopeKind.Global
            && binding.DeclaringScope.Parameters.Contains("require")
            && ReferenceEquals(binding.DeclarationNode, binding.DeclaringScope.AstNode);

    private static bool IsInjectedCommonJsModuleBinding(Scope scope)
        => TryResolveBinding(scope, "module") is { } binding
            && binding.DeclaringScope.Kind == ScopeKind.Global
            && binding.DeclaringScope.Parameters.Contains("module")
            && ReferenceEquals(binding.DeclarationNode, binding.DeclaringScope.AstNode);

    private static bool IsUnsafeModuleBindingUse(
        Identifier identifier,
        Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(identifier, out var parent))
        {
            return true;
        }

        if (parent is not MemberExpression member
            || !ReferenceEquals(member.Object, identifier))
        {
            return true;
        }

        var memberName = member switch
        {
            { Computed: false, Property: Identifier identifierProperty } => identifierProperty.Name,
            { Computed: true, Property: Literal { Value: string propertyName } } => propertyName,
            _ => null
        };

        return memberName == null
            || string.Equals(memberName, "require", StringComparison.OrdinalIgnoreCase)
            || string.Equals(memberName, "parent", StringComparison.OrdinalIgnoreCase)
            || string.Equals(memberName, "children", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetRequiredNodeContract(
        CallExpression call,
        Scope scope,
        out Type contractType)
    {
        contractType = null!;
        return IsInjectedCommonJsRequireCall(call, scope)
            && call.Arguments.Count == 1
            && call.Arguments[0] is Literal { Value: string moduleSpecifier }
            && JavaScriptRuntime.Node.NodeModuleRegistry.TryGetModuleContractType(
                moduleSpecifier,
                out var resolvedContract)
            && resolvedContract != null
            && (contractType = resolvedContract) != null;
    }

    private static bool IsSafeNodeModuleAcquisition(
        CallExpression call,
        Type contractType,
        IReadOnlyDictionary<BindingInfo, Type> candidates,
        Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(call, out var parent))
        {
            return false;
        }

        if (parent is VariableDeclarator declarator
            && ReferenceEquals(declarator.Init, call))
        {
            return candidates.Any(candidate =>
                ReferenceEquals(candidate.Key.DeclarationNode, declarator)
                && candidate.Value == contractType);
        }

        return parent is MemberExpression member
            && ReferenceEquals(member.Object, call)
            && !IsNodeModuleMemberMutation(member, parentMap)
            && !NodeModuleMemberCanReturnContract(member, contractType);
    }

    private static bool IsNodeModuleBindingValueReference(
        Identifier identifier,
        Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(identifier, out var parent))
        {
            return false;
        }

        return parent switch
        {
            VariableDeclarator declarator when ReferenceEquals(declarator.Id, identifier) => false,
            MemberExpression member
                when !member.Computed && ReferenceEquals(member.Property, identifier) => false,
            Property property
                when !property.Computed
                    && ReferenceEquals(property.Key, identifier)
                    && !ReferenceEquals(property.Value, identifier) => false,
            FunctionDeclaration functionDeclaration
                when ReferenceEquals(functionDeclaration.Id, identifier) => false,
            FunctionExpression functionExpression
                when ReferenceEquals(functionExpression.Id, identifier) => false,
            ClassDeclaration classDeclaration
                when ReferenceEquals(classDeclaration.Id, identifier) => false,
            LabeledStatement => false,
            _ => true
        };
    }

    private static bool IsSafeNodeModuleBindingUse(
        Identifier identifier,
        Type contractType,
        Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(identifier, out var parent))
        {
            return false;
        }

        return parent switch
        {
            MemberExpression member when ReferenceEquals(member.Object, identifier)
                => !IsNodeModuleMemberMutation(member, parentMap)
                    && !NodeModuleMemberCanReturnContract(member, contractType),
            NonUpdateUnaryExpression { Operator: Operator.TypeOf } => true,
            _ => false
        };
    }

    private static bool IsNodeModuleMemberMutation(
        MemberExpression member,
        Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(member, out var parent))
        {
            return false;
        }

        return parent switch
        {
            AssignmentExpression assignment when ReferenceEquals(assignment.Left, member) => true,
            UpdateExpression update when ReferenceEquals(update.Argument, member) => true,
            NonUpdateUnaryExpression { Operator: Operator.Delete } => true,
            ForInStatement forIn when ReferenceEquals(forIn.Left, member) => true,
            ForOfStatement forOf when ReferenceEquals(forOf.Left, member) => true,
            _ => IsNestedAssignmentTarget(member, parent, parentMap)
        };
    }

    private static bool NodeModuleMemberCanReturnContract(
        MemberExpression member,
        Type contractType)
    {
        if (!member.Computed && member.Property is Identifier identifier)
        {
            return ContractMemberCanReturnContract(contractType, identifier.Name);
        }

        if (member.Computed
            && member.Property is Literal { Value: string propertyName })
        {
            return ContractMemberCanReturnContract(contractType, propertyName);
        }

        return true;
    }

    private static bool ContractMemberCanReturnContract(
        Type contractType,
        string memberName)
    {
        return contractType
                .GetProperties()
                .Where(property => string.Equals(
                    GetNodeModuleMemberName(property),
                    memberName,
                    StringComparison.Ordinal))
                .Any(property => IsNodeModuleContractType(property.PropertyType))
            || contractType
                .GetMethods()
                .Where(method => string.Equals(
                    GetNodeModuleMemberName(method),
                    memberName,
                    StringComparison.Ordinal))
                .Any(method => IsNodeModuleContractType(method.ReturnType));
    }

    private static string GetNodeModuleMemberName(System.Reflection.MemberInfo member)
        => member
            .GetCustomAttributes(typeof(NodeModuleMemberAttribute), inherit: false)
            .OfType<NodeModuleMemberAttribute>()
            .SingleOrDefault()
            ?.MemberName
            ?? member.Name;

    private static bool IsNestedAssignmentTarget(
        Node target,
        Node parent,
        Dictionary<Node, Node> parentMap)
    {
        var current = target;
        var ancestor = parent;

        while (true)
        {
            var isPatternPath = ancestor switch
            {
                Property property when ReferenceEquals(property.Value, current) => true,
                ArrayPattern => true,
                ObjectPattern => true,
                AssignmentPattern assignmentPattern
                    when ReferenceEquals(assignmentPattern.Left, current) => true,
                RestElement rest when ReferenceEquals(rest.Argument, current) => true,
                _ => false
            };

            if (!isPatternPath)
            {
                return ancestor switch
                {
                    AssignmentExpression assignment
                        when ReferenceEquals(assignment.Left, current) => true,
                    ForInStatement forIn when ReferenceEquals(forIn.Left, current) => true,
                    ForOfStatement forOf when ReferenceEquals(forOf.Left, current) => true,
                    _ => false
                };
            }

            current = ancestor;
            if (!parentMap.TryGetValue(current, out ancestor!))
            {
                return false;
            }
        }
    }

    private sealed record NodeModuleAnalysis(
        Scope Root,
        Dictionary<Node, Node> ParentMap,
        Dictionary<Node, Scope> ScopeMap);
}
