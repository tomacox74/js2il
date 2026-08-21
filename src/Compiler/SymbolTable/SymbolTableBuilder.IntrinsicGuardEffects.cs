using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferIntrinsicGuardEffects(Scope root)
    {
        var scopes = EnumerateScopes(root).ToArray();
        var localEffects = new Dictionary<Scope, IntrinsicGuardEffectSummary>();
        var callees = new Dictionary<Scope, HashSet<Scope>>();

        foreach (var scope in scopes)
        {
            var effects = IntrinsicGuardEffects.None;
            var targets = new HashSet<Scope>();
            var pending = new Stack<(Node Node, bool IsScopeRoot)>();
            pending.Push((scope.AstNode, true));

            while (pending.Count > 0)
            {
                var (node, isScopeRoot) = pending.Pop();
                if (!isScopeRoot
                    && scope.Children.Any(
                        child => ReferenceEquals(child.AstNode, node)))
                {
                    continue;
                }

                switch (node)
                {
                    case MemberExpression:
                        effects |= IntrinsicGuardEffects
                            .InvokesUnknownOrEscapedCode;
                        break;

                    case AwaitExpression:
                    case YieldExpression:
                        effects |= IntrinsicGuardEffects.MaySuspendOrYield;
                        break;

                    case AssignmentExpression
                    {
                        Left: MemberExpression assignedMember
                    }:
                        effects |=
                            IntrinsicGuardEffects
                                .DefinesDeletesOrReconfiguresProperties
                            | IntrinsicGuardEffects
                                .InvokesUnknownOrEscapedCode;
                        if (MayTargetIntrinsicPrototype(
                                assignedMember,
                                scope))
                        {
                            effects |= IntrinsicGuardEffects
                                .MutatesIntrinsicPrototypeOrLink;
                        }
                        break;

                    case UpdateExpression
                    {
                        Argument: MemberExpression updatedMember
                    }:
                        effects |=
                            IntrinsicGuardEffects
                                .DefinesDeletesOrReconfiguresProperties
                            | IntrinsicGuardEffects
                                .InvokesUnknownOrEscapedCode;
                        if (MayTargetIntrinsicPrototype(
                                updatedMember,
                                scope))
                        {
                            effects |= IntrinsicGuardEffects
                                .MutatesIntrinsicPrototypeOrLink;
                        }
                        break;

                    case NonUpdateUnaryExpression
                    {
                        Operator: Operator.Delete,
                        Argument: MemberExpression deletedMember
                    }:
                        effects |=
                            IntrinsicGuardEffects
                                .DefinesDeletesOrReconfiguresProperties
                            | IntrinsicGuardEffects
                                .InvokesUnknownOrEscapedCode;
                        if (MayTargetIntrinsicPrototype(
                                deletedMember,
                                scope))
                        {
                            effects |= IntrinsicGuardEffects
                                .MutatesIntrinsicPrototypeOrLink;
                        }
                        break;

                    case ReturnStatement { Argument: { } argument }
                        when MayEscapeGuardedValue(argument, scope):
                        effects |=
                            IntrinsicGuardEffects.EscapesGuardedValue;
                        break;

                    case CallExpression call:
                        if (TryGetDirectEffectTarget(
                                call,
                                scope,
                                root,
                                out var target))
                        {
                            targets.Add(target);
                        }
                        else
                        {
                            effects |= IntrinsicGuardEffects
                                .InvokesUnknownOrEscapedCode;
                            if (call.Arguments.Count > 0)
                            {
                                effects |= IntrinsicGuardEffects
                                    .EscapesGuardedValue;
                            }
                        }

                        if (IsPropertyMutationCall(call, scope))
                        {
                            effects |=
                                IntrinsicGuardEffects
                                    .MutatesIntrinsicPrototypeOrLink
                                | IntrinsicGuardEffects
                                    .DefinesDeletesOrReconfiguresProperties;
                        }
                        break;

                    case NewExpression:
                        effects |=
                            IntrinsicGuardEffects
                                .InvokesUnknownOrEscapedCode
                            | IntrinsicGuardEffects.EscapesGuardedValue;
                        break;
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push((child, false));
                }
            }

            localEffects.Add(
                scope,
                new IntrinsicGuardEffectSummary(effects));
            callees.Add(scope, targets);
            scope.IntrinsicGuardEffects =
                new IntrinsicGuardEffectSummary(effects);
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var scope in scopes)
            {
                var summary = localEffects[scope];
                foreach (var lexicalChild in scope.Children
                             .Where(
                                 static child =>
                                     child.Kind is
                                         ScopeKind.Block
                                         or ScopeKind.Class)
                             .OrderBy(
                                 static child =>
                                     child.GetQualifiedName(),
                                 StringComparer.Ordinal))
                {
                    summary = summary.Union(
                        lexicalChild.IntrinsicGuardEffects);
                }

                foreach (var target in callees[scope]
                             .OrderBy(
                                 static target =>
                                     target.GetQualifiedName(),
                                 StringComparer.Ordinal))
                {
                    summary = summary.Union(
                        target.IntrinsicGuardEffects);
                }

                if (summary != scope.IntrinsicGuardEffects)
                {
                    scope.IntrinsicGuardEffects = summary;
                    changed = true;
                }
            }
        }
        while (changed);
    }

    private static bool MayTargetIntrinsicPrototype(
        MemberExpression member,
        Scope scope)
    {
        if (member.Object is MemberExpression
            {
                Computed: false,
                Object: Identifier intrinsic,
                Property: Identifier { Name: "prototype" }
            })
        {
            return IsUnshadowedGuardedIntrinsic(intrinsic, scope);
        }

        if (!member.Computed
            && member.Object is Identifier intrinsicObject
            && member.Property is Identifier { Name: "prototype" })
        {
            return IsUnshadowedGuardedIntrinsic(
                intrinsicObject,
                scope);
        }

        return !member.Computed
            && member.Property is Identifier { Name: "__proto__" };
    }

    private static bool MayEscapeGuardedValue(
        Expression expression,
        Scope scope)
        => expression switch
        {
            StringLiteral or ArrayExpression
                or FunctionExpression
                or ArrowFunctionExpression => true,
            Identifier identifier =>
                TryResolveBinding(scope, identifier.Name) is { } binding
                && binding.ReceiverCandidateClrTypes.Count > 0,
            NewExpression
            {
                Callee: Identifier intrinsic
            } => IsUnshadowedGuardedIntrinsic(intrinsic, scope),
            _ => false
        };

    private static bool IsPropertyMutationCall(
        CallExpression call,
        Scope scope)
    {
        if (call.Callee is not MemberExpression
            {
                Computed: false,
                Object: Identifier owner,
                Property: Identifier method
            })
        {
            return false;
        }

        if (owner.Name is "Object" or "Reflect"
            && method.Name is
                "defineProperty"
                or "defineProperties"
                or "setPrototypeOf"
                or "deleteProperty")
        {
            return !IsIdentifierShadowed(scope, owner.Name);
        }

        return false;
    }

    private static bool IsUnshadowedGuardedIntrinsic(
        Identifier identifier,
        Scope scope)
        => identifier.Name is
                "String"
                or "Array"
                or "Int8Array"
                or "Uint8Array"
                or "Uint8ClampedArray"
                or "Int16Array"
                or "Uint16Array"
                or "Int32Array"
                or "Uint32Array"
                or "Float32Array"
                or "Float64Array"
                or "BigInt64Array"
                or "BigUint64Array"
            && !IsIdentifierShadowed(scope, identifier.Name);

    private bool TryGetDirectEffectTarget(
        CallExpression call,
        Scope callScope,
        Scope root,
        out Scope targetScope)
    {
        targetScope = null!;
        if (call.Optional
            || call.Arguments.Any(
                static argument => argument is SpreadElement))
        {
            return false;
        }

        if (call.Callee is FunctionExpression
            or ArrowFunctionExpression)
        {
            var directScope = FindScopeByAstNode(root, call.Callee);
            if (directScope == null
                || StableDirectCallableEligibility
                    .GetCallableIneligibilityReason(
                        call.Callee,
                        directScope)
                    != CallableMaterializationReason.None)
            {
                return false;
            }

            targetScope = directScope;
            return true;
        }

        if (call.Callee is not Identifier identifier
            || TryResolveBinding(
                callScope,
                identifier.Name) is not { } binding
            || !StableDirectCallableEligibility.TryGetEligibleCall(
                binding,
                call,
                callScope,
                out _,
                out var eligibleScope,
                out _)
            || eligibleScope == null)
        {
            return false;
        }

        targetScope = eligibleScope;
        return true;
    }
}
