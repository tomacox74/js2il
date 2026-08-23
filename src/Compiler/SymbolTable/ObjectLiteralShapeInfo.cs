using Acornima.Ast;
using System;
using System.Collections.Generic;

namespace Jroc.SymbolTables;

/// <summary>
/// Describes a single member of an object literal that is a candidate for
/// early-bound, strongly-typed CLR member access (see issue #1428/#1429).
/// </summary>
public sealed class ObjectLiteralMemberInfo
{
    public ObjectLiteralMemberInfo(string name, Node valueNode, Type? clrType, bool isFunction)
    {
        Name = name;
        ValueNode = valueNode;
        ClrType = clrType;
        IsFunction = isFunction;
    }

    /// <summary>Member name in literal source order.</summary>
    public string Name { get; }

    /// <summary>The AST node of the member's initializer value.</summary>
    public Node ValueNode { get; }

    /// <summary>
    /// Conservative stable CLR type for the member value. In addition to primitive
    /// types, stable function members retain their materialized delegate type.
    /// Null means boxed object.
    /// A member only keeps an unboxed type when every observed write agrees.
    /// </summary>
    public Type? ClrType { get; internal set; }

    /// <summary>
    /// True when the member value is a function expression / arrow function and the
    /// member is never reassigned. Demoted to a plain data member on any write.
    /// </summary>
    public bool IsFunction { get; internal set; }
}

/// <summary>
/// Shared metadata for JavaScript object layouts that may use generated CLR storage.
/// </summary>
public abstract class InferredObjectShapeInfo
{
    protected InferredObjectShapeInfo(
        Node sourceNode,
        BindingInfo binding,
        IReadOnlyList<ObjectLiteralMemberInfo> members)
    {
        SourceNode = sourceNode;
        Binding = binding;
        Members = members;
        IsEligible = true;
    }

    public Node SourceNode { get; }

    /// <summary>The binding that introduces this layout.</summary>
    public BindingInfo Binding { get; }

    /// <summary>Members in literal source order.</summary>
    public IReadOnlyList<ObjectLiteralMemberInfo> Members { get; }

    /// <summary>
    /// True when every use of the binding is provably safe for early binding.
    /// The analysis is strictly conservative: any use it cannot prove safe disqualifies.
    /// </summary>
    public bool IsEligible { get; private set; }

    /// <summary>First reason the literal was disqualified; null while eligible.</summary>
    public string? DisqualifyReason { get; private set; }

    /// <summary>
    /// Deterministic generated CLR type name assigned by TypeGenerator for eligible shapes.
    /// Null until type generation runs, and always null for ineligible shapes.
    /// </summary>
    public string? GeneratedClrTypeName { get; internal set; }

    /// <summary>
    /// Metadata handle for the generated CLR type assigned by TypeGenerator.
    /// Nil until type generation runs, and always nil for ineligible shapes.
    /// </summary>
    public System.Reflection.Metadata.TypeDefinitionHandle GeneratedClrTypeHandle { get; internal set; }

    /// <summary>
    /// Structural signature key over member names + member CLR types + function-ness. The key is
    /// normalized by ordering members by name so two literals that declare the same members in a
    /// different source order produce an identical key and may share a single generated CLR type
    /// (issue #1434 phase 6 canonicalization). Construction and enumeration still use each literal's
    /// own source order (see the object-literal lowering), so observable property order is
    /// unchanged. Members with the same name but different CLR types or function-ness produce
    /// distinct keys, so they never join.
    /// </summary>
    public string GetStructuralSignatureKey()
    {
        var builder = new System.Text.StringBuilder();
        var ordered = new List<ObjectLiteralMemberInfo>(Members);
        ordered.Sort(static (left, right) => string.CompareOrdinal(left.Name, right.Name));
        foreach (var member in ordered)
        {
            builder.Append(member.Name).Append('\u0000');
            if (member.IsFunction)
            {
                builder.Append("fn:").Append(member.ClrType?.FullName ?? "object");
            }
            else
            {
                builder.Append(member.ClrType?.FullName ?? "object");
            }
            builder.Append('\u0001');
        }

        return builder.ToString();
    }

    internal void Disqualify(string reason)
    {
        if (!IsEligible)
        {
            return;
        }

        IsEligible = false;
        DisqualifyReason = reason;
    }

    internal bool TryGetMember(string name, out ObjectLiteralMemberInfo member)
    {
        foreach (var candidate in Members)
        {
            if (string.Equals(candidate.Name, name, StringComparison.Ordinal))
            {
                member = candidate;
                return true;
            }
        }

        member = null!;
        return false;
    }
}

/// <summary>
/// Result of the compile-time eligibility analysis for a single object literal bound
/// to a local/module binding. When <see cref="InferredObjectShapeInfo.IsEligible"/> is
/// true, later phases may generate a specialized CLR type and early-bind member access.
/// </summary>
public sealed class ObjectLiteralShapeInfo : InferredObjectShapeInfo
{
    public ObjectLiteralShapeInfo(
        ObjectExpression literal,
        BindingInfo binding,
        IReadOnlyList<ObjectLiteralMemberInfo> members)
        : base(literal, binding, members)
    {
        Literal = literal;
    }

    /// <summary>The object literal expression this shape describes.</summary>
    public ObjectExpression Literal { get; }
}

/// <summary>
/// Layout inferred from the unconditional initialization prefix of an ES5-style
/// constructor function.
/// </summary>
public sealed class ConstructorShapeInfo : InferredObjectShapeInfo
{
    public ConstructorShapeInfo(
        Node constructorNode,
        Scope constructorScope,
        BindingInfo binding,
        IReadOnlyList<ObjectLiteralMemberInfo> members)
        : base(constructorNode, binding, members)
    {
        ConstructorNode = constructorNode;
        ConstructorScope = constructorScope;
    }

    public Node ConstructorNode { get; }

    public Scope ConstructorScope { get; }
}
