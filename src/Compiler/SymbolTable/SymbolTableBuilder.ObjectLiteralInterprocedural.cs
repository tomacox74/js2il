using Acornima;
using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jroc.SymbolTables;

/// <summary>
/// Interprocedural object-literal type inference (issue #1434, parent #1428, phase 6).
///
/// Extends the per-binding object-literal shape analysis across call boundaries so an inferred
/// literal type can flow to a callable parameter based on call-site evidence. The dependency is
/// circular (a literal is only eligible if the callee uses its parameter safely, and the
/// parameter type depends on the argument shapes at every call site), so the analysis is a
/// coupled fixed point:
///
///   Step D — escape/eligibility: assume every closed-world parameter uses its value safely,
///            then cascade-invalidate any parameter whose body lets the object escape (monotone).
///   Step E — disqualify literal bindings whose only "unsafe" use was passing the object into a
///            parameter that turned out to escape.
///   Step F — early-binding: join the argument shape across every call site of a parameter using
///            a worklist (no fixed iteration cap); a parameter is early-bound only when all sites
///            agree on one structural shape, its body uses stay safe, and every feeding literal is
///            eligible. Structurally identical literals share one signature and therefore one
///            generated CLR type at the join.
///
/// The parameter ABI stays <c>object</c>; only a stable shape token is propagated so lowering can
/// cast to the generated literal type on member access.
/// </summary>
public partial class SymbolTableBuilder
{
    private sealed class ObjectLiteralInterproceduralAnalysis
    {
        private readonly SymbolTableBuilder _builder;
        private readonly Scope _root;
        private readonly Dictionary<BindingInfo, ObjectLiteralShapeInfo> _candidates;
        private readonly Dictionary<Node, Node> _parentMap;
        private readonly Dictionary<Node, Scope> _scopeMap;

        // Closed-world callable scopes keyed by their callable binding.
        private readonly Dictionary<BindingInfo, Scope> _closedWorldByBinding = new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<Scope> _closedWorldScopes = new(ReferenceEqualityComparer.Instance);

        // Eligible parameter slots keyed by (callee scope, parameter index) and by parameter binding.
        private readonly Dictionary<(Scope, int), Slot> _slots = new();
        private readonly Dictionary<BindingInfo, Slot> _slotByBinding = new(ReferenceEqualityComparer.Instance);

        // Every statically-resolved call/new to a closed-world callable.
        private readonly List<CallSite> _callSites = new();
        private readonly Dictionary<Scope, List<CallSite>> _callSitesByCallee = new(ReferenceEqualityComparer.Instance);

        // Literal-binding → parameter obligations recorded during literal use analysis.
        private readonly List<LiteralObligation> _literalObligations = new();

        // Final signature → representative eligible literal shape mapping (canonicalization).
        private readonly Dictionary<string, ObjectLiteralShapeInfo> _representativeBySignature =
            new(StringComparer.Ordinal);

        public ObjectLiteralInterproceduralAnalysis(
            SymbolTableBuilder builder,
            Scope root,
            Dictionary<BindingInfo, ObjectLiteralShapeInfo> candidates,
            Dictionary<Node, Node> parentMap,
            Dictionary<Node, Scope> scopeMap)
        {
            _builder = builder;
            _root = root;
            _candidates = candidates;
            _parentMap = parentMap;
            _scopeMap = scopeMap;
        }

        private sealed class Slot
        {
            public Slot(Scope callee, int index, BindingInfo binding)
            {
                Callee = callee;
                Index = index;
                Binding = binding;
            }

            public Scope Callee { get; }
            public int Index { get; }
            public BindingInfo Binding { get; }

            // Step D — escape analysis.
            public bool DirectUnsafe { get; set; }
            public bool SafeUse { get; set; } = true;
            public List<(Scope Callee, int Index)> Edges { get; } = new();
            public HashSet<string> AccessedMembers { get; } = new(StringComparer.Ordinal);

            // Deferred member-write obligations (issue #1434 safe-use parity). A write through a
            // typed parameter is only sound when the slot is early-bound to a shape whose member
            // accepts the written value through the generated setter, so the write is recorded here
            // during body analysis and validated once the representative shape is known.
            public List<MemberWrite> MemberWrites { get; } = new();

            // Step F — early binding.
            public string? SignatureKey { get; set; }
            public bool Generic { get; set; }
        }

        private readonly record struct MemberWrite(string Member, Type? WrittenType);

        private readonly record struct CallSite(Scope Callee, Scope CallScope, NodeList<Expression> Arguments, int ParameterCount);

        private readonly record struct LiteralObligation(ObjectLiteralShapeInfo Shape, Scope Callee, int Index);

        private enum Evidence
        {
            Pending,
            Generic,
            Concrete
        }

        // ------------------------------------------------------------------
        // Preparation
        // ------------------------------------------------------------------

        public void Prepare()
        {
            BuildClosedWorldCallables();
            BuildEligibleSlots();
            CollectCallSites();
        }

        private void BuildClosedWorldCallables()
        {
            var callableByBinding = new Dictionary<BindingInfo, Scope>(ReferenceEqualityComparer.Instance);

            foreach (var scope in EnumerateScopes(_root))
            {
                if (!IsEligibleCallableShape(scope))
                {
                    continue;
                }

                var binding = TryGetCallableBinding(scope);
                if (binding == null)
                {
                    continue;
                }

                callableByBinding[binding] = scope;
            }

            if (callableByBinding.Count == 0)
            {
                return;
            }

            // A callable is closed-world only when every reference to its binding is the callee of
            // a direct call/new. Any other reference (assigned, passed as a value, returned) means
            // it can be invoked with arbitrary arguments this analysis cannot see.
            var escaping = new HashSet<BindingInfo>(ReferenceEqualityComparer.Instance);
            var walker = new Jroc.Utilities.AstWalker();
            walker.Visit(_root.AstNode, node =>
            {
                if (node is not Identifier id)
                {
                    return;
                }

                if (!_scopeMap.TryGetValue(id, out var scope))
                {
                    return;
                }

                var binding = TryResolveBinding(scope, id.Name);
                if (binding == null || !callableByBinding.ContainsKey(binding))
                {
                    return;
                }

                _parentMap.TryGetValue(id, out var parent);

                if (IsCallableDeclarationName(id, parent))
                {
                    return;
                }

                if (parent is CallExpression call && ReferenceEquals(call.Callee, id))
                {
                    return;
                }

                if (parent is NewExpression newExpr && ReferenceEquals(newExpr.Callee, id))
                {
                    return;
                }

                escaping.Add(binding);
            });

            foreach (var (binding, scope) in callableByBinding)
            {
                if (escaping.Contains(binding))
                {
                    continue;
                }

                _closedWorldByBinding[binding] = scope;
                _closedWorldScopes.Add(scope);
            }
        }

        private static bool IsCallableDeclarationName(Identifier id, Node? parent)
            => parent switch
            {
                FunctionDeclaration functionDeclaration when ReferenceEquals(functionDeclaration.Id, id) => true,
                FunctionExpression functionExpression when ReferenceEquals(functionExpression.Id, id) => true,
                VariableDeclarator declarator when ReferenceEquals(declarator.Id, id) => true,
                _ => false
            };

        private bool IsEligibleCallableShape(Scope scope)
        {
            if (scope.Kind != ScopeKind.Function)
            {
                return false;
            }

            if (scope.IsAsync || scope.IsGenerator || scope.NeedsArgumentsObject || scope.HasRestParameters)
            {
                return false;
            }

            // Phase 6 targets free functions and const/let/var function bindings; class methods
            // and Function-constructor callables are out of scope.
            if (scope.AstNode is FunctionDeclaration)
            {
                return true;
            }

            if (scope.AstNode is ArrowFunctionExpression or FunctionExpression
                && _parentMap.TryGetValue(scope.AstNode, out var parent)
                && parent is VariableDeclarator declarator
                && declarator.Id is Identifier
                && ReferenceEquals(declarator.Init, scope.AstNode))
            {
                return true;
            }

            return false;
        }

        private BindingInfo? TryGetCallableBinding(Scope scope)
        {
            if (scope.AstNode is FunctionDeclaration { Id: Identifier funcId })
            {
                return scope.Parent != null ? TryResolveBinding(scope.Parent, funcId.Name) : null;
            }

            if ((scope.AstNode is ArrowFunctionExpression or FunctionExpression)
                && _parentMap.TryGetValue(scope.AstNode, out var parent)
                && parent is VariableDeclarator { Id: Identifier declId })
            {
                var declaratorScope = _scopeMap.TryGetValue(parent, out var s) ? s : scope.Parent;
                return declaratorScope != null ? TryResolveBinding(declaratorScope, declId.Name) : null;
            }

            return null;
        }

        private void BuildEligibleSlots()
        {
            foreach (var scope in _closedWorldScopes)
            {
                if (!TryGetSimpleParameterNames(scope.AstNode, out var parameterNames) || parameterNames.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < parameterNames.Count; i++)
                {
                    if (!scope.Bindings.TryGetValue(parameterNames[i], out var binding))
                    {
                        continue;
                    }

                    if (binding.HasWrite || binding.IsCaptured)
                    {
                        continue;
                    }

                    // A parameter that already carries a stable primitive type receives primitive
                    // arguments, never a literal shape.
                    if (binding.IsStableType && binding.ClrType != null)
                    {
                        continue;
                    }

                    var slot = new Slot(scope, i, binding);
                    _slots[(scope, i)] = slot;
                    _slotByBinding[binding] = slot;
                }
            }
        }

        private void CollectCallSites()
        {
            if (_closedWorldScopes.Count == 0)
            {
                return;
            }

            var walker = new Jroc.Utilities.AstWalker();
            walker.Visit(_root.AstNode, node =>
            {
                switch (node)
                {
                    case CallExpression call:
                        TryAddCallSite(call.Callee, call.Arguments, node);
                        break;
                    case NewExpression newExpr:
                        TryAddCallSite(newExpr.Callee, newExpr.Arguments, node);
                        break;
                }
            });
        }

        private void TryAddCallSite(Node callee, NodeList<Expression> arguments, Node callNode)
        {
            var calleeScope = TryResolveClosedWorldCallee(callee, callNode);
            if (calleeScope == null)
            {
                return;
            }

            var callScope = _scopeMap.TryGetValue(callNode, out var s) ? s : _root;
            TryGetSimpleParameterNames(calleeScope.AstNode, out var parameterNames);
            var callSite = new CallSite(calleeScope, callScope, arguments, parameterNames.Count);
            _callSites.Add(callSite);

            if (!_callSitesByCallee.TryGetValue(calleeScope, out var list))
            {
                list = new List<CallSite>();
                _callSitesByCallee[calleeScope] = list;
            }

            list.Add(callSite);
        }

        private Scope? TryResolveClosedWorldCallee(Node callee, Node callNode)
        {
            if (callee is not Identifier calleeId)
            {
                return null;
            }

            var callScope = _scopeMap.TryGetValue(callNode, out var s) ? s : _root;
            var binding = TryResolveBinding(callScope, calleeId.Name);
            if (binding == null)
            {
                return null;
            }

            return _closedWorldByBinding.TryGetValue(binding, out var calleeScope) ? calleeScope : null;
        }

        // ------------------------------------------------------------------
        // Literal call-argument obligations (invoked during literal use analysis)
        // ------------------------------------------------------------------

        public bool TryRecordLiteralCallArgument(
            ObjectLiteralShapeInfo shape,
            Identifier identifier,
            Node callNode,
            Scope useScope)
        {
            NodeList<Expression> arguments;
            Node callee;
            switch (callNode)
            {
                case CallExpression call:
                    arguments = call.Arguments;
                    callee = call.Callee;
                    break;
                case NewExpression newExpr:
                    arguments = newExpr.Arguments;
                    callee = newExpr.Callee;
                    break;
                default:
                    return false;
            }

            var calleeScope = TryResolveClosedWorldCallee(callee, callNode);
            if (calleeScope == null)
            {
                return false;
            }

            var index = IndexOfArgument(arguments, identifier);
            if (index < 0)
            {
                return false;
            }

            if (!_slots.ContainsKey((calleeScope, index)))
            {
                return false;
            }

            _literalObligations.Add(new LiteralObligation(shape, calleeScope, index));
            return true;
        }

        private static int IndexOfArgument(NodeList<Expression> arguments, Identifier identifier)
        {
            for (var i = 0; i < arguments.Count; i++)
            {
                // A spread element before this position expands to an unknown number of arguments,
                // so every later positional argument maps to an unknown parameter index. The
                // syntactic index is therefore not a stable parameter index, and any literal /
                // forwarded parameter at or after the first spread must fall back to the generic
                // path (issue #1434 spread-shift bug).
                if (arguments[i] is SpreadElement)
                {
                    return -1;
                }

                if (ReferenceEquals(arguments[i], identifier))
                {
                    return i;
                }
            }

            return -1;
        }

        // ------------------------------------------------------------------
        // Analysis
        // ------------------------------------------------------------------

        public void Analyze()
        {
            if (_slots.Count == 0)
            {
                return;
            }

            AnalyzeParameterBodyUses();

            // Coupled fixed point. A member write through a typed parameter is only sound when the
            // slot is early-bound to a shape whose member accepts the written value through the
            // generated setter (the typed backing field stays in sync with JsObject storage);
            // otherwise the object would be mutated through the generic path and desynchronize, so
            // the write demotes the slot to unsafe and disqualifies its feeding literals. Marking a
            // slot unsafe and disqualifying literals are both monotone, so the loop converges.
            do
            {
                ResetTransientState();
                ComputeSafeUse();
                DisqualifyEscapingLiterals();
                BuildRepresentatives();
                ComputeEarlyBinding();
            }
            while (ValidateMemberObligations());
        }

        private void ResetTransientState()
        {
            foreach (var slot in _slots.Values)
            {
                // DirectUnsafe is monotone (set by body analysis and by member-obligation
                // validation) and is intentionally preserved; every other per-iteration
                // conclusion is recomputed from the current DirectUnsafe flags and the current
                // set of still-eligible literals.
                slot.SafeUse = true;
                slot.Generic = false;
                slot.SignatureKey = null;
            }

            _representativeBySignature.Clear();
        }

        private void AnalyzeParameterBodyUses()
        {
            foreach (var slot in _slots.Values)
            {
                AnalyzeParameterBody(slot);
            }
        }

        private void AnalyzeParameterBody(Slot slot)
        {
            var paramBinding = slot.Binding;
            var walker = new Jroc.Utilities.AstWalker();
            walker.Visit(slot.Callee.AstNode, node =>
            {
                if (node is not Identifier id || !string.Equals(id.Name, paramBinding.Name, StringComparison.Ordinal))
                {
                    return;
                }

                if (!_scopeMap.TryGetValue(id, out var scope))
                {
                    return;
                }

                var binding = TryResolveBinding(scope, id.Name);
                if (!ReferenceEquals(binding, paramBinding))
                {
                    return;
                }

                if (!IsBindingValueReference(id, _parentMap))
                {
                    return;
                }

                // Skip the parameter's own declaration identifier in the callable's parameter list.
                _parentMap.TryGetValue(id, out var idParent);
                if (idParent is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
                {
                    return;
                }

                AnalyzeParameterUse(slot, id, scope);
            });
        }

        private void AnalyzeParameterUse(Slot slot, Identifier identifier, Scope useScope)
        {
            _parentMap.TryGetValue(identifier, out var parent);

            switch (parent)
            {
                case MemberExpression member when ReferenceEquals(member.Object, identifier):
                    AnalyzeParameterMemberAccess(slot, member);
                    return;

                case NonUpdateUnaryExpression { Operator: Operator.TypeOf }:
                    return;

                case CallExpression call when !ReferenceEquals(call.Callee, identifier):
                    RecordParameterCallArgument(slot, identifier, call.Callee, call.Arguments, call, useScope);
                    return;

                case NewExpression newExpr when !ReferenceEquals(newExpr.Callee, identifier):
                    RecordParameterCallArgument(slot, identifier, newExpr.Callee, newExpr.Arguments, newExpr, useScope);
                    return;

                default:
                    // Any other use (return, alias, store, spread, call of the object, for-in/of,
                    // 'in', delete, export, ...) lets the object escape the callee.
                    slot.DirectUnsafe = true;
                    return;
            }
        }

        private void AnalyzeParameterMemberAccess(Slot slot, MemberExpression member)
        {
            if (member.Computed || member.Property is not Identifier propertyId)
            {
                slot.DirectUnsafe = true;
                return;
            }

            slot.AccessedMembers.Add(propertyId.Name);

            _parentMap.TryGetValue(member, out var accessParent);

            switch (accessParent)
            {
                // obj.p = v (including compound assignments, which read then write). A member write
                // is safe only when the slot is ultimately early-bound to a shape whose member
                // accepts the written value through the generated setter (the typed backing field
                // stays in sync with JsObject storage). That is not known until the fixed point
                // resolves, so the write is recorded and validated later (issue #1434 safe-use
                // parity); an unsatisfiable write demotes the slot to unsafe.
                case AssignmentExpression assignment when ReferenceEquals(assignment.Left, member):
                    slot.MemberWrites.Add(new MemberWrite(propertyId.Name, InferMemberWriteClrType(assignment)));
                    return;

                // obj.p++ / --obj.p — numeric read-modify-write.
                case UpdateExpression:
                    slot.MemberWrites.Add(new MemberWrite(propertyId.Name, typeof(double)));
                    return;

                // obj.p(...) — method call. Passing the object as `this` into a callee that could
                // observe it through the generic path risks desynchronizing the typed backing
                // fields, so interprocedural method-call parity is conservatively out of scope
                // (issue #1434); the call is treated as an escaping use. Same-scope literals still
                // allow safe method calls via AnalyzeObjectLiteralMethodCall.
                case CallExpression call when ReferenceEquals(call.Callee, member):
                    slot.DirectUnsafe = true;
                    return;

                // A member write through a destructuring/iteration target or delete bypasses the
                // typed backing field's setter, so it is treated as an escaping use.
                case NonUpdateUnaryExpression { Operator: Operator.Delete }:
                case ArrayPattern:
                case AssignmentPattern:
                case RestElement:
                    slot.DirectUnsafe = true;
                    return;

                case ForInStatement forIn when ReferenceEquals(forIn.Left, member):
                case ForOfStatement forOf when ReferenceEquals(forOf.Left, member):
                    slot.DirectUnsafe = true;
                    return;

                case Property property when ReferenceEquals(property.Value, member)
                    && _parentMap.TryGetValue(property, out var propertyParent)
                    && propertyParent is ObjectPattern:
                    slot.DirectUnsafe = true;
                    return;

                default:
                    // Plain member read: safe, the member value (not the object) flows onward.
                    return;
            }
        }

        private void RecordParameterCallArgument(
            Slot slot,
            Identifier identifier,
            Node callee,
            NodeList<Expression> arguments,
            Node callNode,
            Scope useScope)
        {
            var calleeScope = TryResolveClosedWorldCallee(callee, callNode);
            if (calleeScope == null)
            {
                slot.DirectUnsafe = true;
                return;
            }

            var index = IndexOfArgument(arguments, identifier);
            if (index < 0 || !_slots.ContainsKey((calleeScope, index)))
            {
                slot.DirectUnsafe = true;
                return;
            }

            slot.Edges.Add((calleeScope, index));
        }

        private void ComputeSafeUse()
        {
            // Optimistic + invalidation. Every slot starts SafeUse = true; a slot becomes unsafe
            // when it has a direct escaping use or forwards the object into an unsafe slot.
            // Monotone (true → false only), so iterating to a fixed point terminates.
            bool changed;
            do
            {
                changed = false;
                foreach (var slot in _slots.Values)
                {
                    if (!slot.SafeUse)
                    {
                        continue;
                    }

                    if (slot.DirectUnsafe || HasUnsafeEdge(slot))
                    {
                        slot.SafeUse = false;
                        changed = true;
                    }
                }
            }
            while (changed);
        }

        private bool HasUnsafeEdge(Slot slot)
        {
            foreach (var edge in slot.Edges)
            {
                if (!_slots.TryGetValue(edge, out var target) || !target.SafeUse)
                {
                    return true;
                }
            }

            return false;
        }

        private void DisqualifyEscapingLiterals()
        {
            foreach (var obligation in _literalObligations)
            {
                if (!obligation.Shape.IsEligible)
                {
                    continue;
                }

                if (!_slots.TryGetValue((obligation.Callee, obligation.Index), out var slot) || !slot.SafeUse)
                {
                    obligation.Shape.Disqualify("object passed to a call whose parameter use is unsafe");
                }
            }
        }

        private void BuildRepresentatives()
        {
            foreach (var shape in _candidates.Values)
            {
                if (!shape.IsEligible)
                {
                    continue;
                }

                var key = shape.GetStructuralSignatureKey();
                if (!_representativeBySignature.TryGetValue(key, out var existing)
                    || IsBetterRepresentative(shape, existing))
                {
                    _representativeBySignature[key] = shape;
                }
            }
        }

        private static bool IsBetterRepresentative(ObjectLiteralShapeInfo candidate, ObjectLiteralShapeInfo current)
        {
            var candidateLoc = candidate.Literal.Location.Start;
            var currentLoc = current.Literal.Location.Start;
            if (candidateLoc.Line != currentLoc.Line)
            {
                return candidateLoc.Line < currentLoc.Line;
            }

            if (candidateLoc.Column != currentLoc.Column)
            {
                return candidateLoc.Column < currentLoc.Column;
            }

            return string.CompareOrdinal(candidate.Binding.Name, current.Binding.Name) < 0;
        }

        private void ComputeEarlyBinding()
        {
            // Phase F1 — forward propagation (worklist, no fixed iteration cap). Deferred "pending"
            // evidence (an argument sourced from a not-yet-resolved parameter slot) is skipped so
            // mutually-recursive chains rooted at a literal converge.
            bool changed;
            do
            {
                changed = false;
                foreach (var slot in _slots.Values)
                {
                    if (slot.Generic || !slot.SafeUse)
                    {
                        continue;
                    }

                    if (PropagateSlotSignature(slot))
                    {
                        changed = true;
                    }
                }
            }
            while (changed);

            // Phase F2 — validation. Any tentatively-bound slot with a call site whose argument is
            // not provably the same shape (a never-resolved or generic parameter source, or a
            // conflicting/absent shape) becomes generic; this cascades to its consumers.
            do
            {
                changed = false;
                foreach (var slot in _slots.Values)
                {
                    if (slot.Generic || slot.SignatureKey == null)
                    {
                        continue;
                    }

                    if (!ValidateSlot(slot))
                    {
                        slot.Generic = true;
                        changed = true;
                    }
                }
            }
            while (changed);

            // Final consistency: drop bindings whose accessed members are not covered by the shape.
            foreach (var slot in _slots.Values)
            {
                if (slot.Generic || slot.SignatureKey == null || !slot.SafeUse)
                {
                    slot.SignatureKey = null;
                    continue;
                }

                if (!MembersCoveredBySignature(slot))
                {
                    slot.Generic = true;
                    slot.SignatureKey = null;
                }
            }
        }

        private bool PropagateSlotSignature(Slot slot)
        {
            if (!_callSitesByCallee.TryGetValue(slot.Callee, out var sites))
            {
                return false;
            }

            var changed = false;
            foreach (var site in sites)
            {
                var evidence = ResolveArgumentEvidence(site, slot.Index, allowPending: true, out var key);
                switch (evidence)
                {
                    case Evidence.Generic:
                        if (!slot.Generic)
                        {
                            slot.Generic = true;
                            slot.SignatureKey = null;
                            changed = true;
                        }
                        return changed;

                    case Evidence.Concrete:
                        if (slot.SignatureKey == null)
                        {
                            slot.SignatureKey = key;
                            changed = true;
                        }
                        else if (!string.Equals(slot.SignatureKey, key, StringComparison.Ordinal))
                        {
                            slot.Generic = true;
                            slot.SignatureKey = null;
                            changed = true;
                            return changed;
                        }
                        break;

                    case Evidence.Pending:
                        break;
                }
            }

            return changed;
        }

        private bool ValidateSlot(Slot slot)
        {
            if (!_callSitesByCallee.TryGetValue(slot.Callee, out var sites))
            {
                // A slot with a signature but no call sites cannot happen (signatures come from
                // sites); treat defensively as unproven.
                return false;
            }

            foreach (var site in sites)
            {
                var evidence = ResolveArgumentEvidence(site, slot.Index, allowPending: false, out var key);
                if (evidence == Evidence.Generic)
                {
                    return false;
                }

                if (evidence == Evidence.Concrete
                    && !string.Equals(slot.SignatureKey, key, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private Evidence ResolveArgumentEvidence(CallSite site, int index, bool allowPending, out string? key)
        {
            key = null;

            // Missing argument (arity shortfall) — the parameter is undefined at this site.
            if (index >= site.Arguments.Count)
            {
                return Evidence.Generic;
            }

            // A spread element at or before this parameter's positional slot shifts arguments by an
            // unknown amount, so the syntactic argument at `index` is not this parameter's value.
            // No positional evidence is available for this or any later parameter (issue #1434
            // spread-shift bug).
            for (var i = 0; i <= index; i++)
            {
                if (site.Arguments[i] is SpreadElement)
                {
                    return Evidence.Generic;
                }
            }

            var arg = site.Arguments[index];
            if (arg is not Identifier argId)
            {
                return Evidence.Generic;
            }

            var binding = TryResolveBinding(site.CallScope, argId.Name);
            if (binding == null)
            {
                return Evidence.Generic;
            }

            if (_candidates.TryGetValue(binding, out var literalShape))
            {
                if (!literalShape.IsEligible)
                {
                    return Evidence.Generic;
                }

                key = literalShape.GetStructuralSignatureKey();
                return Evidence.Concrete;
            }

            if (_slotByBinding.TryGetValue(binding, out var sourceSlot))
            {
                if (sourceSlot.Generic || sourceSlot.SignatureKey == null)
                {
                    return allowPending && !sourceSlot.Generic ? Evidence.Pending : Evidence.Generic;
                }

                key = sourceSlot.SignatureKey;
                return Evidence.Concrete;
            }

            return Evidence.Generic;
        }

        private bool MembersCoveredBySignature(Slot slot)
        {
            if (slot.AccessedMembers.Count == 0)
            {
                return true;
            }

            if (slot.SignatureKey == null
                || !_representativeBySignature.TryGetValue(slot.SignatureKey, out var representative))
            {
                return false;
            }

            foreach (var name in slot.AccessedMembers)
            {
                if (!representative.TryGetMember(name, out _))
                {
                    return false;
                }
            }

            return true;
        }

        // ------------------------------------------------------------------
        // Member-write obligation validation (issue #1434 safe-use parity)
        // ------------------------------------------------------------------

        private bool ValidateMemberObligations()
        {
            var changed = false;
            foreach (var slot in _slots.Values)
            {
                if (slot.DirectUnsafe || slot.MemberWrites.Count == 0)
                {
                    continue;
                }

                if (!AreMemberWritesSafe(slot))
                {
                    // The write cannot be lowered through a typed setter for this slot, so the
                    // object would be mutated via the generic path and desynchronize. Demote the
                    // slot to an escaping use; ComputeSafeUse/DisqualifyEscapingLiterals will
                    // disqualify its feeding literals on the next iteration.
                    slot.DirectUnsafe = true;
                    changed = true;
                }
            }

            return changed;
        }

        private bool AreMemberWritesSafe(Slot slot)
        {
            // Writes are only sound when the slot is early-bound to a concrete, eligible shape: the
            // generated setter keeps the typed backing field in sync with JsObject storage.
            if (!slot.SafeUse || slot.Generic || slot.SignatureKey == null
                || !_representativeBySignature.TryGetValue(slot.SignatureKey, out var representative)
                || !representative.IsEligible)
            {
                return false;
            }

            foreach (var write in slot.MemberWrites)
            {
                if (!representative.TryGetMember(write.Member, out var member))
                {
                    return false;
                }

                // A write to a function member could overwrite the method; keep it out of scope.
                if (member.IsFunction)
                {
                    return false;
                }

                // An object-typed field accepts any value through its setter; a primitively-typed
                // field only accepts a write whose type provably matches (a conflicting write would
                // otherwise be silently coerced or throw), so fall back conservatively.
                if (member.ClrType != null && write.WrittenType != member.ClrType)
                {
                    return false;
                }
            }

            return true;
        }

        // ------------------------------------------------------------------
        // Commit
        // ------------------------------------------------------------------

        public void AssignParameterShapes()
        {
            foreach (var slot in _slots.Values)
            {
                if (slot.Generic || slot.SignatureKey == null || !slot.SafeUse)
                {
                    continue;
                }

                if (!_representativeBySignature.TryGetValue(slot.SignatureKey, out var representative)
                    || !representative.IsEligible)
                {
                    continue;
                }

                slot.Binding.ObjectLiteralShape = representative;
            }
        }
    }
}
