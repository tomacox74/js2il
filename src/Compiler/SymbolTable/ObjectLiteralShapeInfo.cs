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
    /// Conservative stable CLR type for the member value
    /// (typeof(double), typeof(bool) or typeof(string)); null means boxed object.
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
/// Result of the compile-time eligibility analysis for a single object literal bound
/// to a local/module binding. When <see cref="IsEligible"/> is true, later phases may
/// generate a specialized CLR type and early-bind member accesses; otherwise the
/// literal must compile exactly as today (plain JsObject).
/// </summary>
public sealed class ObjectLiteralShapeInfo
{
    public ObjectLiteralShapeInfo(ObjectExpression literal, BindingInfo binding, IReadOnlyList<ObjectLiteralMemberInfo> members)
    {
        Literal = literal;
        Binding = binding;
        Members = members;
        IsEligible = true;
    }

    /// <summary>The object literal expression this shape describes.</summary>
    public ObjectExpression Literal { get; }

    /// <summary>The binding the literal is assigned to at its declaration.</summary>
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
