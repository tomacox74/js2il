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
        var requirements = InspectOwnCallableBody(callable.AstNode);

        return new GeneratedFunctionObjectPlan
        {
            Callable = callable,
            Signature = signature,
            Namespace =
                $"FunctionObjects.{Sanitize(symbolTable.Root.Name)}_{StableSuffix(symbolTable.Root.Name)}",
            ModuleName = symbolTable.Root.Name,
            TypeName = BuildTypeName(callable),
            CanonicalOwnerTypeName = ResolveCanonicalOwnerTypeName(
                callable,
                callableScope,
                classScope),
            Captures = captures,
            StateFields = BuildStatePlan(callable, signature, requirements),
            ScopeChainSlotCount = slotCount,
            IsConstructable = IsConstructable(callable),
            RequiresInvocationContext =
                callable.NeedsArgumentsObject
                || callable.HasRestParameters
                || callable.AstNode is FunctionExpression { Id: not null }
                || callableScope?.MayUseBoundWithObject == true
                || HasNestedArrowLexicalContext(callable.AstNode)
                || requirements.UsesThis
                || requirements.UsesNewTarget
                || requirements.UsesSuper,
            UsesNonStrictThisBinding =
                callable.Kind is CallableKind.FunctionDeclaration
                    or CallableKind.FunctionExpression
                && !callable.HasRestrictedFunctionProperties,
            ReturnKind = GetReturnKind(callable)
        };
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
        CallableSignature signature,
        CallableRequirements requirements)
    {
        var state = new List<GeneratedFunctionStatePlan>();
        if (signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.ScopeArray)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_transitionalScopes",
                GeneratedFunctionStateKind.TransitionalScopeArray));
        }

        if (callable.Kind == CallableKind.Arrow
            && (requirements.UsesThis || requirements.UsesSuper))
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalThis",
                GeneratedFunctionStateKind.LexicalThis));
        }

        if (callable.Kind == CallableKind.Arrow && requirements.UsesNewTarget)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalNewTarget",
                GeneratedFunctionStateKind.LexicalNewTarget));
        }

        if (requirements.UsesSuper)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_homeObject",
                GeneratedFunctionStateKind.HomeObject));
            state.Add(new GeneratedFunctionStatePlan(
                "_lexicalSuperScopes",
                GeneratedFunctionStateKind.LexicalSuperScopes));
        }

        if (requirements.UsesPrivateNames)
        {
            state.Add(new GeneratedFunctionStatePlan(
                "_privateBrand",
                GeneratedFunctionStateKind.PrivateBrand));
        }

        return state;
    }

    private static bool IsConstructable(CallableId callable)
    {
        if (callable.Kind == CallableKind.ClassConstructor)
        {
            return true;
        }

        if (callable.Kind is not (CallableKind.FunctionDeclaration or CallableKind.FunctionExpression))
        {
            return false;
        }

        return callable.AstNode switch
        {
            FunctionDeclaration function => !function.Async && !function.Generator,
            FunctionExpression function => !function.Async && !function.Generator,
            _ => false
        };
    }

    private static GeneratedFunctionReturnKind GetReturnKind(CallableId callable)
    {
        if (callable.Kind == CallableKind.ClassConstructor)
        {
            return GeneratedFunctionReturnKind.Constructor;
        }

        return callable.AstNode switch
        {
            FunctionDeclaration { Async: true, Generator: true }
                or FunctionExpression { Async: true, Generator: true }
                => GeneratedFunctionReturnKind.AsyncGenerator,
            FunctionDeclaration { Async: true }
                or FunctionExpression { Async: true }
                or ArrowFunctionExpression { Async: true }
                => GeneratedFunctionReturnKind.Promise,
            FunctionDeclaration { Generator: true }
                or FunctionExpression { Generator: true }
                => GeneratedFunctionReturnKind.Generator,
            _ => GeneratedFunctionReturnKind.Value
        };
    }

    private static CallableRequirements InspectOwnCallableBody(Node? root)
    {
        var requirements = new CallableRequirements();
        if (root == null)
        {
            return requirements;
        }

        Visit(root, isRoot: true);
        return requirements;

        void Visit(Node node, bool isRoot)
        {
            if (!isRoot && node is FunctionDeclaration
                or FunctionExpression
                or ArrowFunctionExpression
                or ClassDeclaration
                or ClassExpression)
            {
                return;
            }

            requirements = node switch
            {
                ThisExpression => requirements with { UsesThis = true },
                MetaProperty => requirements with { UsesNewTarget = true },
                Super => requirements with { UsesSuper = true },
                PrivateIdentifier => requirements with { UsesPrivateNames = true },
                CallExpression { Callee: Identifier { Name: "eval" } } =>
                    requirements with
                    {
                        UsesThis = true,
                        UsesNewTarget = true
                    },
                _ => requirements
            };

            foreach (var child in node.ChildNodes)
            {
                Visit(child, isRoot: false);
            }
        }

    }

    private static bool HasNestedArrowLexicalContext(Node? root)
    {
        if (root is null)
        {
            return false;
        }

        return Visit(root, isRoot: true);

        static bool Visit(Node node, bool isRoot)
        {
            if (!isRoot && node is FunctionDeclaration or FunctionExpression)
            {
                return false;
            }

            if (!isRoot
                && node is ArrowFunctionExpression arrow
                && ArrowUsesLexicalContext(arrow))
            {
                return true;
            }

            return node.ChildNodes.Any(child => Visit(child, isRoot: false));
        }

        static bool ArrowUsesLexicalContext(ArrowFunctionExpression arrow)
        {
            return VisitArrowBody(arrow.Body);

            static bool VisitArrowBody(Node node)
            {
                if (node is FunctionDeclaration or FunctionExpression)
                {
                    return false;
                }

                if (node is ThisExpression or MetaProperty or Super)
                {
                    return true;
                }

                return node.ChildNodes.Any(VisitArrowBody);
            }
        }
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

    private readonly record struct CallableRequirements(
        bool UsesThis = false,
        bool UsesNewTarget = false,
        bool UsesSuper = false,
        bool UsesPrivateNames = false);
}
