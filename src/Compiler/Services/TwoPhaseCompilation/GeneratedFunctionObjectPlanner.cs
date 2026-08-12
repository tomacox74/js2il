using Acornima.Ast;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;

namespace Jroc.Services.TwoPhaseCompilation;

internal static class GeneratedFunctionObjectPlanner
{
    public static GeneratedFunctionObjectPlan CreatePlan(
        CallableId callable,
        CallableSignature signature,
        SymbolTable symbolTable)
    {
        var callableScope = ResolveCallableScope(callable, symbolTable);
        var classScope = ResolveClassScope(callable, symbolTable, callableScope);
        var captures = BuildCapturePlan(callableScope, signature, out var slotCount);
        var returnKind = GetReturnKind(callable);

        return new GeneratedFunctionObjectPlan
        {
            Callable = callable,
            Signature = signature,
            Namespace = string.Empty,
            ModuleName = symbolTable.Root.Name,
            TypeName = callable.Kind == CallableKind.Arrow
                ? GeneratedFunctionObjectNaming.WrapperTypeName
                : BuildTypeName(callable),
            CanonicalOwnerTypeName = ResolveCanonicalOwnerTypeName(
                callable,
                callableScope,
                classScope),
            Captures = captures,
            StateFields = BuildStatePlan(
                callable,
                callableScope,
                signature),
            ScopeChainSlotCount = slotCount,
            IsConstructable = IsConstructable(callable),
            RequiresInvocationContext =
                callable.NeedsArgumentsObject
                || callable.HasRestParameters
                || callable.Semantics.IsNamedFunctionExpression
                || callableScope?.MayUseBoundWithObject == true
                || callable.Semantics.HasNestedArrowLexicalContext
                || callable.Semantics.UsesThis
                || callable.Semantics.UsesNewTarget
                || callable.Semantics.UsesSuper
                || returnKind is GeneratedFunctionReturnKind.Generator
                    or GeneratedFunctionReturnKind.AsyncGenerator,
            UsesNonStrictThisBinding =
                callable.Kind is CallableKind.FunctionDeclaration
                    or CallableKind.FunctionExpression
                && !callable.HasRestrictedFunctionProperties,
            RequiresArrayCallAdapter =
                callable.NeedsArgumentsObject
                || callable.HasRestParameters
                || HasDirectSpreadCall(callable, callableScope),
            ReturnKind = returnKind
        };
    }

    private static bool HasDirectSpreadCall(
        CallableId callable,
        Scope? callableScope)
    {
        for (var scope = callableScope?.Parent; scope is not null; scope = scope.Parent)
        {
            var binding = scope.Bindings.Values.FirstOrDefault(candidate =>
                ReferenceEquals(candidate.DeclarationNode, callable.AstNode)
                || candidate.DeclarationNode is VariableDeclarator
                {
                    Init: { } initializer
                } && ReferenceEquals(initializer, callable.AstNode));
            if (binding is null)
            {
                continue;
            }

            return binding.CallableMaterialization?.Reasons.HasFlag(
                CallableMaterializationReason.SpreadCall) == true;
        }

        return false;
    }

    private static Scope? ResolveCallableScope(CallableId callable, SymbolTable symbolTable)
    {
        if (callable.AstNode == null)
        {
            return null;
        }

        var scope = symbolTable.FindScopeByAstNode(callable.AstNode);
        if (scope == null && callable.AstNode is MethodDefinition methodDefinition)
        {
            scope = symbolTable.FindScopeByAstNode(methodDefinition.Value);
        }

        return scope;
    }

    private static Scope? ResolveClassScope(
        CallableId callable,
        SymbolTable symbolTable,
        Scope? callableScope)
    {
        var current = callableScope;
        while (current != null)
        {
            if (current.Kind == ScopeKind.Class)
            {
                return current;
            }
            current = current.Parent;
        }

        if (callable.Kind is not (CallableKind.ClassConstructor
            or CallableKind.ClassMethod
            or CallableKind.ClassGetter
            or CallableKind.ClassSetter
            or CallableKind.ClassStaticMethod
            or CallableKind.ClassStaticGetter
            or CallableKind.ClassStaticSetter))
        {
            return null;
        }

        return Find(symbolTable.Root);

        Scope? Find(Scope scope)
        {
            if (scope.Kind == ScopeKind.Class
                && IsCallableMemberOfClass(callable.AstNode, scope.AstNode))
            {
                return scope;
            }

            foreach (var child in scope.Children)
            {
                var found = Find(child);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }

    private static bool IsCallableMemberOfClass(Node? callableNode, Node classNode)
    {
        var body = classNode switch
        {
            ClassDeclaration declaration => declaration.Body,
            ClassExpression expression => expression.Body,
            _ => null
        };
        if (body == null || callableNode == null)
        {
            return false;
        }

        if (ReferenceEquals(body, callableNode))
        {
            return true;
        }

        return body.Body.Any(member =>
            ReferenceEquals(member, callableNode)
            || member is MethodDefinition method
                && ReferenceEquals(method.Value, callableNode));
    }

    private static IReadOnlyList<GeneratedFunctionCapturePlan> BuildCapturePlan(
        Scope? callableScope,
        CallableSignature signature,
        out int slotCount)
    {
        slotCount = 0;
        if (callableScope == null
            || signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.NoScopes)
        {
            return Array.Empty<GeneratedFunctionCapturePlan>();
        }

        var layout = new EnvironmentLayoutBuilder(
                new Services.VariableBindings.ScopeMetadataRegistry())
            .Build(callableScope, ScopesAbi.CallableKind.Function);
        slotCount = layout.ScopeChain.Slots.Count;

        if (signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.SingleScope
            && !string.IsNullOrWhiteSpace(signature.SingleScopeScopeName))
        {
            var slot = layout.ScopeChain.FindSlot(signature.SingleScopeScopeName!);
            return
            [
                new GeneratedFunctionCapturePlan(
                    BuildEnvironmentFieldName(slot?.Index ?? 0, signature.SingleScopeScopeName!),
                    signature.SingleScopeScopeName!,
                    slot?.Index ?? 0)
            ];
        }

        var requiredIndices = layout.GetRequiredParentScopeIndices();
        return layout.ScopeChain.Slots
            .Where(slot => requiredIndices.Contains(slot.Index))
            .Select(slot => new GeneratedFunctionCapturePlan(
                BuildEnvironmentFieldName(slot.Index, slot.ScopeName),
                slot.ScopeName,
                slot.Index))
            .ToArray();
    }

    private static IReadOnlyList<GeneratedFunctionStatePlan> BuildStatePlan(
        CallableId callable,
        Scope? callableScope,
        CallableSignature signature)
    {
        var state = new List<GeneratedFunctionStatePlan>();
        if (signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.ScopeArray)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_transitionalScopes",
                GeneratedFunctionStateKind.TransitionalScopeArray));
        }
        else if (callable.Kind is CallableKind.ClassMethod
                or CallableKind.ClassGetter
                or CallableKind.ClassSetter
            && callableScope != null
            && (callableScope.ReferencesParentScopeVariables
                || callableScope.HasDescendantCallableReferencingParentScopeVariables
                || callableScope.Children.Any(child =>
                    child.Kind is ScopeKind.Function or ScopeKind.Class)))
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_transitionalScopes",
                GeneratedFunctionStateKind.TransitionalScopeArray));
        }

        if (callable.Kind == CallableKind.Arrow
            && (callable.Semantics.UsesThis || callable.Semantics.UsesSuper))
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalThis",
                GeneratedFunctionStateKind.LexicalThis));
        }

        if (callable.Kind == CallableKind.Arrow && callable.Semantics.UsesNewTarget)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalNewTarget",
                GeneratedFunctionStateKind.LexicalNewTarget));
        }

        if (callable.Semantics.UsesSuper)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_homeObject",
                GeneratedFunctionStateKind.HomeObject));
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalSuperScopes",
                GeneratedFunctionStateKind.LexicalSuperScopes));
        }

        if (callable.Semantics.UsesPrivateNames)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_privateBrand",
                GeneratedFunctionStateKind.PrivateBrand));
        }

        return state;
    }

    private static bool IsConstructable(CallableId callable)
        => callable.Semantics.IsConstructable;

    private static GeneratedFunctionReturnKind GetReturnKind(CallableId callable)
    {
        if (callable.Kind == CallableKind.ClassConstructor)
        {
            return GeneratedFunctionReturnKind.Constructor;
        }

        return callable.Semantics.IsAsync && callable.Semantics.IsGenerator
            ? GeneratedFunctionReturnKind.AsyncGenerator
            : callable.Semantics.IsAsync
                ? GeneratedFunctionReturnKind.Promise
                : callable.Semantics.IsGenerator
                    ? GeneratedFunctionReturnKind.Generator
                    : GeneratedFunctionReturnKind.Value;
    }

    private static string BuildTypeName(CallableId callable)
    {
        var identity = callable.Name
            ?? callable.Location?.ToString()
            ?? "anonymous";
        return $"FunctionObject_{callable.Kind}_{StableSuffix(callable.UniqueKey)}_{Sanitize(identity)}";
    }

    private static string ResolveCanonicalOwnerTypeName(
        CallableId callable,
        Scope? callableScope,
        Scope? resolvedClassScope)
    {
        if (callable.Kind is CallableKind.FunctionExpression or CallableKind.Arrow)
        {
            return callableScope?.Name
                ?? callable.Location?.ToString()
                ?? "anonymous";
        }

        if (callable.Kind == CallableKind.FunctionDeclaration)
        {
            return callable.Name ?? "anonymous";
        }

        var classScope = resolvedClassScope ?? callableScope;
        while (classScope != null && classScope.Kind != ScopeKind.Class)
        {
            classScope = classScope.Parent;
        }

        if (classScope != null)
        {
            var @namespace = classScope.DotNetNamespace ?? "Classes";
            var typeName = classScope.DotNetTypeName ?? classScope.Name;
            return $"{@namespace}.{typeName}";
        }

        return callable.Name ?? "anonymous";
    }

    private static string BuildEnvironmentFieldName(int index, string scopeName)
        => $"_environment{index}_{Sanitize(scopeName.Split('/').Last())}";

    private static string Sanitize(string value)
    {
        var chars = value.Select(character =>
                char.IsLetterOrDigit(character) || character == '_'
                    ? character
                    : '_')
            .ToArray();
        return new string(chars);
    }

    private static string StableSuffix(string value)
    {
        uint hash = 2166136261;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= 16777619;
        }

        return hash.ToString("X8", System.Globalization.CultureInfo.InvariantCulture);
    }
}
