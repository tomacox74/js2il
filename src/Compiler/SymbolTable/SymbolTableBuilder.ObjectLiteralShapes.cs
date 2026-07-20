using Acornima;
using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jroc.SymbolTables;

/// <summary>
/// Object literal shape eligibility analysis (issue #1429, parent #1428).
///
/// Decides, per object literal bound to a local/module binding, whether every use of the
/// binding is provably safe for early-bound, strongly-typed CLR member access. The analysis
/// is strictly conservative: any construct it cannot prove safe disqualifies the literal and
/// the literal compiles exactly as today (plain JsObject). This pass performs analysis only;
/// no codegen consumes the result yet.
/// </summary>
public partial class SymbolTableBuilder
{
    /// <summary>
    /// Analyzes all object literals assigned to bindings at their declaration and records an
    /// <see cref="ObjectLiteralShapeInfo"/> on each binding describing eligibility for
    /// early-bound member access.
    /// </summary>
    private void AnalyzeObjectLiteralShapes(Scope root)
    {
        foreach (var scope in EnumerateScopes(root))
        {
            foreach (var binding in scope.Bindings.Values)
            {
                binding.ObjectLiteralShape = null;
            }
        }

        var candidates = CollectObjectLiteralShapeCandidates(root);
        if (candidates.Count == 0)
        {
            return;
        }

        var parentMap = BuildParentMap(root.AstNode);
        var scopeMap = BuildNodeScopeMap(root);

        foreach (var shape in candidates.Values)
        {
            ValidateObjectLiteralStructure(shape);
            CheckDeclarationContext(shape, parentMap);
        }

        // Interprocedural object-literal type inference (issue #1434 phase 6). The coupled
        // fixed point decides which callables may receive a literal shape at a parameter and
        // which literal bindings escape into an unsafe callee use.
        var interprocedural = new ObjectLiteralInterproceduralAnalysis(this, root, candidates, parentMap, scopeMap);
        interprocedural.Prepare();

        AnalyzeObjectLiteralBindingUses(root.AstNode, candidates, parentMap, scopeMap, interprocedural);

        interprocedural.Analyze();

        foreach (var shape in candidates.Values)
        {
            shape.Binding.ObjectLiteralShape = shape;
        }

        interprocedural.AssignParameterShapes();
    }

    private Dictionary<BindingInfo, ObjectLiteralShapeInfo> CollectObjectLiteralShapeCandidates(Scope root)
    {
        var candidates = new Dictionary<BindingInfo, ObjectLiteralShapeInfo>();

        foreach (var scope in EnumerateScopes(root))
        {
            foreach (var binding in scope.Bindings.Values)
            {
                if (binding.DeclarationNode is not VariableDeclarator declarator
                    || declarator.Id is not Identifier id
                    || !string.Equals(id.Name, binding.Name, StringComparison.Ordinal)
                    || declarator.Init is not ObjectExpression literal)
                {
                    continue;
                }

                if (binding.Kind != BindingKind.Const
                    && binding.Kind != BindingKind.Let
                    && binding.Kind != BindingKind.Var)
                {
                    continue;
                }

                var members = new List<ObjectLiteralMemberInfo>();
                foreach (var propertyNode in literal.Properties)
                {
                    if (propertyNode is Property { Kind: PropertyKind.Init, Computed: false } property
                        && property.Key is Identifier keyIdentifier
                        && property.Value is Node valueNode)
                    {
                        var isFunction = valueNode is FunctionExpression or ArrowFunctionExpression;
                        members.Add(new ObjectLiteralMemberInfo(
                            keyIdentifier.Name,
                            valueNode,
                            isFunction ? null : InferObjectLiteralMemberClrType(valueNode),
                            isFunction));
                    }
                    else
                    {
                        // Structure violations are recorded during validation; keep a placeholder
                        // member list so the shape still reports the disqualify reason.
                        members.Clear();
                        break;
                    }
                }

                candidates[binding] = new ObjectLiteralShapeInfo(literal, binding, members);
            }
        }

        return candidates;
    }

    private static void ValidateObjectLiteralStructure(ObjectLiteralShapeInfo shape)
    {
        var literal = shape.Literal;
        if (literal.Properties.Count == 0)
        {
            shape.Disqualify("literal has no members");
            return;
        }

        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var propertyNode in literal.Properties)
        {
            if (propertyNode is SpreadElement)
            {
                shape.Disqualify("spread element in literal");
                return;
            }

            if (propertyNode is not Property property)
            {
                shape.Disqualify($"unsupported literal member node '{propertyNode.Type}'");
                return;
            }

            if (property.Kind != PropertyKind.Init)
            {
                shape.Disqualify("getter/setter member in literal");
                return;
            }

            if (property.Computed)
            {
                shape.Disqualify("computed key in literal");
                return;
            }

            if (property.Key is not Identifier keyIdentifier)
            {
                shape.Disqualify("non-identifier key in literal");
                return;
            }

            if (string.Equals(keyIdentifier.Name, "__proto__", StringComparison.Ordinal))
            {
                shape.Disqualify("__proto__ key in literal");
                return;
            }

            if (!seenNames.Add(keyIdentifier.Name))
            {
                shape.Disqualify($"duplicate key '{keyIdentifier.Name}' in literal");
                return;
            }
        }
    }

    private static void CheckDeclarationContext(ObjectLiteralShapeInfo shape, Dictionary<Node, Node> parentMap)
    {
        // export const a = { ... } — the binding escapes the module.
        var node = shape.Binding.DeclarationNode;
        while (node != null && parentMap.TryGetValue(node, out var parent))
        {
            if (parent is ExportNamedDeclaration or ExportDefaultDeclaration or ExportAllDeclaration)
            {
                shape.Disqualify("binding is exported from the module");
                return;
            }

            if (parent is not VariableDeclaration)
            {
                return;
            }

            node = parent;
        }
    }

    private void AnalyzeObjectLiteralBindingUses(
        Node rootNode,
        Dictionary<BindingInfo, ObjectLiteralShapeInfo> candidates,
        Dictionary<Node, Node> parentMap,
        Dictionary<Node, Scope> scopeMap,
        ObjectLiteralInterproceduralAnalysis interprocedural)
    {
        var candidateNames = new HashSet<string>(candidates.Keys.Select(b => b.Name), StringComparer.Ordinal);
        var walker = new Jroc.Utilities.AstWalker();

        walker.Visit(rootNode, node =>
        {
            if (node is not Identifier identifier || !candidateNames.Contains(identifier.Name))
            {
                return;
            }

            if (!scopeMap.TryGetValue(identifier, out var scope))
            {
                return;
            }

            var binding = TryResolveBinding(scope, identifier.Name);
            if (binding == null || !candidates.TryGetValue(binding, out var shape) || !shape.IsEligible)
            {
                return;
            }

            if (!IsBindingValueReference(identifier, parentMap))
            {
                return;
            }

            AnalyzeObjectLiteralUse(shape, identifier, parentMap, scope, interprocedural);
        });
    }

    /// <summary>
    /// Filters out identifier nodes that are not value references to the binding
    /// (declaration names, member property names, object literal keys, labels).
    /// </summary>
    private static bool IsBindingValueReference(Identifier identifier, Dictionary<Node, Node> parentMap)
    {
        if (!parentMap.TryGetValue(identifier, out var parent))
        {
            return false;
        }

        return parent switch
        {
            // The declaration itself (const a = ...) is not a use.
            VariableDeclarator declarator when ReferenceEquals(declarator.Id, identifier) => false,
            // obj.a — property-name position, not a reference to the binding.
            MemberExpression member when !member.Computed && ReferenceEquals(member.Property, identifier) => false,
            // { a: ... } — key position.
            Property property when !property.Computed && ReferenceEquals(property.Key, identifier) => false,
            FunctionDeclaration functionDeclaration when ReferenceEquals(functionDeclaration.Id, identifier) => false,
            FunctionExpression functionExpression when ReferenceEquals(functionExpression.Id, identifier) => false,
            ClassDeclaration classDeclaration when ReferenceEquals(classDeclaration.Id, identifier) => false,
            LabeledStatement => false,
            _ => true
        };
    }

    /// <summary>
    /// Evaluates one value reference to a candidate binding. Only a small whitelist of
    /// contexts is considered safe; everything else disqualifies the shape.
    /// </summary>
    private void AnalyzeObjectLiteralUse(
        ObjectLiteralShapeInfo shape,
        Identifier identifier,
        Dictionary<Node, Node> parentMap,
        Scope useScope,
        ObjectLiteralInterproceduralAnalysis interprocedural)
    {
        parentMap.TryGetValue(identifier, out var parent);

        switch (parent)
        {
            // obj.p — static member access on the object; validated below.
            case MemberExpression member when ReferenceEquals(member.Object, identifier):
                AnalyzeObjectLiteralMemberAccess(shape, member, parentMap);
                return;

            // typeof obj — always "object"; observes nothing about the members.
            case NonUpdateUnaryExpression { Operator: Operator.TypeOf }:
                return;

            // Reassignment of the binding itself invalidates everything.
            case AssignmentExpression assignment when ReferenceEquals(assignment.Left, identifier):
                shape.Disqualify("binding is reassigned");
                return;

            case UpdateExpression update when ReferenceEquals(update.Argument, identifier):
                shape.Disqualify("binding is reassigned");
                return;

            case CallExpression call when !ReferenceEquals(call.Callee, identifier):
                // Passing the literal binding as a positional argument to a statically-resolved,
                // closed-world callable is a candidate for interprocedural propagation. The final
                // safety decision is deferred to the coupled fixed point; record an obligation.
                if (interprocedural.TryRecordLiteralCallArgument(shape, identifier, call, useScope))
                {
                    return;
                }

                shape.Disqualify("object passed to a call");
                return;

            case NewExpression newExpr when !ReferenceEquals(newExpr.Callee, identifier):
                if (interprocedural.TryRecordLiteralCallArgument(shape, identifier, newExpr, useScope))
                {
                    return;
                }

                shape.Disqualify("object passed to a call");
                return;

            case CallExpression:
            case NewExpression:
                shape.Disqualify("object passed to a call");
                return;

            case ReturnStatement:
            case ArrowFunctionExpression: // concise-body arrow return
                shape.Disqualify("object returned from a function");
                return;

            case Property:
            case ArrayExpression:
                shape.Disqualify("object stored into another object or array");
                return;

            case AssignmentExpression assignment when ReferenceEquals(assignment.Right, identifier):
                shape.Disqualify("object stored through an assignment");
                return;

            case VariableDeclarator declarator when ReferenceEquals(declarator.Init, identifier):
                shape.Disqualify("object aliased to another binding");
                return;

            case SpreadElement:
                shape.Disqualify("object used in a spread");
                return;

            case ForInStatement:
            case ForOfStatement:
                shape.Disqualify("object enumerated by for-in/for-of");
                return;

            case NonUpdateUnaryExpression { Operator: Operator.Delete }:
                shape.Disqualify("delete applied to the binding");
                return;

            case BinaryExpression { Operator: Operator.In } binary when ReferenceEquals(binary.Right, identifier):
                shape.Disqualify("object used with the 'in' operator");
                return;

            case ExportNamedDeclaration:
            case ExportDefaultDeclaration:
            case ExportSpecifier:
                shape.Disqualify("object exported from the module");
                return;

            default:
                shape.Disqualify($"object used in unsupported context '{parent?.Type.ToString() ?? "unknown"}'");
                return;
        }
    }

    private void AnalyzeObjectLiteralMemberAccess(
        ObjectLiteralShapeInfo shape,
        MemberExpression member,
        Dictionary<Node, Node> parentMap)
    {
        if (member.Computed)
        {
            shape.Disqualify("computed member access on the object");
            return;
        }

        if (member.Property is not Identifier propertyIdentifier)
        {
            shape.Disqualify("non-identifier member access on the object");
            return;
        }

        if (!shape.TryGetMember(propertyIdentifier.Name, out var memberInfo))
        {
            shape.Disqualify($"access to undeclared member '{propertyIdentifier.Name}'");
            return;
        }

        parentMap.TryGetValue(member, out var accessParent);

        switch (accessParent)
        {
            // delete obj.p — member disappears; early binding would be incorrect.
            case NonUpdateUnaryExpression { Operator: Operator.Delete }:
                shape.Disqualify($"delete of member '{memberInfo.Name}'");
                return;

            // obj.p = v (including compound assignments, which read then write).
            case AssignmentExpression assignment when ReferenceEquals(assignment.Left, member):
                RecordObjectLiteralMemberWrite(shape, memberInfo, assignment);
                return;

            // obj.p++ / --obj.p — numeric read-modify-write.
            case UpdateExpression:
                RecordObjectLiteralMemberWriteType(memberInfo, typeof(double));
                return;

            // obj.p as a destructuring/iteration assignment target (e.g. [obj.p] = arr,
            // ({ x: obj.p } = src), for (obj.p of arr)). These write through generic paths
            // the early-bound field would not observe, so disqualify conservatively.
            case ArrayPattern:
            case AssignmentPattern:
            case RestElement:
            case ForInStatement forIn when ReferenceEquals(forIn.Left, member):
            case ForOfStatement forOf when ReferenceEquals(forOf.Left, member):
                shape.Disqualify($"member '{memberInfo.Name}' is a destructuring or iteration assignment target");
                return;

            case Property property when ReferenceEquals(property.Value, member)
                && parentMap.TryGetValue(property, out var propertyParent)
                && propertyParent is ObjectPattern:
                shape.Disqualify($"member '{memberInfo.Name}' is a destructuring or iteration assignment target");
                return;

            // obj.p(...) — method call; obj becomes `this` inside the callee.
            case CallExpression call when ReferenceEquals(call.Callee, member):
                AnalyzeObjectLiteralMethodCall(shape, memberInfo);
                return;

            default:
                // Plain read in any expression context is safe: the member value (not the
                // object) is what flows onward.
                return;
        }
    }

    private static void RecordObjectLiteralMemberWrite(
        ObjectLiteralShapeInfo shape,
        ObjectLiteralMemberInfo memberInfo,
        AssignmentExpression assignment)
    {
        RecordObjectLiteralMemberWriteType(memberInfo, InferMemberWriteClrType(assignment));
    }

    /// <summary>
    /// Conservative stable CLR type produced by an assignment to an object-literal member. Simple
    /// assignments take the type of the right-hand side; provably numeric compound assignments
    /// produce <c>double</c>; everything else (including <c>+=</c>, which can concatenate strings)
    /// is unknown. Shared by the per-binding shape analysis and the interprocedural parameter
    /// safe-use parity check (issue #1434).
    /// </summary>
    internal static Type? InferMemberWriteClrType(AssignmentExpression assignment)
    {
        if (assignment.Operator == Operator.Assignment)
        {
            return InferObjectLiteralMemberClrType(assignment.Right);
        }

        // Compound assignments other than string concatenation produce numbers; `+=` can
        // produce strings. Treat only provably numeric operators as double.
        return assignment.Operator switch
        {
            Operator.SubtractionAssignment
                or Operator.MultiplicationAssignment
                or Operator.DivisionAssignment
                or Operator.RemainderAssignment
                or Operator.ExponentiationAssignment
                or Operator.BitwiseAndAssignment
                or Operator.BitwiseOrAssignment
                or Operator.BitwiseXorAssignment
                or Operator.LeftShiftAssignment
                or Operator.RightShiftAssignment
                or Operator.UnsignedRightShiftAssignment => typeof(double),
            _ => null
        };
    }

    private static void RecordObjectLiteralMemberWriteType(ObjectLiteralMemberInfo memberInfo, Type? writtenType)
    {
        // Any write demotes a function member to a plain data member.
        memberInfo.IsFunction = false;

        if (memberInfo.ClrType != null && memberInfo.ClrType != writtenType)
        {
            memberInfo.ClrType = null;
        }
    }

    private void AnalyzeObjectLiteralMethodCall(ObjectLiteralShapeInfo shape, ObjectLiteralMemberInfo memberInfo)
    {
        if (!memberInfo.IsFunction)
        {
            shape.Disqualify($"call of non-function member '{memberInfo.Name}'");
            return;
        }

        // Calling obj.p() passes obj as `this` into the callee. That is only safe when the
        // callee provably never observes `this` (arrows capture lexical `this` instead).
        if (memberInfo.ValueNode is ArrowFunctionExpression)
        {
            return;
        }

        if (FunctionObservesThis(memberInfo.ValueNode))
        {
            shape.Disqualify($"member '{memberInfo.Name}' is called and its body uses 'this'");
        }
    }

    private readonly Dictionary<Node, bool> _functionObservesThisCache = new(ReferenceEqualityComparer.Instance);

    private bool FunctionObservesThis(Node functionNode)
    {
        if (_functionObservesThisCache.TryGetValue(functionNode, out var cached))
        {
            return cached;
        }

        // Conservative: any ThisExpression anywhere in the body disqualifies, including inside
        // nested functions (they could still be invoked with the object as `this` through
        // aliases this pass does not track).
        var observes = false;
        var walker = new Jroc.Utilities.AstWalker();
        walker.Visit(functionNode, node =>
        {
            if (node is ThisExpression)
            {
                observes = true;
            }
        });

        _functionObservesThisCache[functionNode] = observes;
        return observes;
    }

    private static Type? InferObjectLiteralMemberClrType(Node valueNode)
    {
        return valueNode switch
        {
            NumericLiteral => typeof(double),
            BooleanLiteral => typeof(bool),
            StringLiteral => typeof(string),
            NonUpdateUnaryExpression { Operator: Operator.UnaryNegation or Operator.UnaryPlus } unary
                when unary.Argument is NumericLiteral => typeof(double),
            _ => null
        };
    }
}
