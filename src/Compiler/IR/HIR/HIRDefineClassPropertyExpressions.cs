using Js2IL.SymbolTables;
using System.Collections.Generic;

namespace Js2IL.HIR;

public sealed class HIRDefineClassDataPropertyExpression : HIRExpression
{
    public HIRDefineClassDataPropertyExpression(HIRExpression target, HIRExpression key, HIRExpression value)
    {
        Target = target;
        Key = key;
        Value = value;
    }

    public HIRExpression Target { get; init; }
    public HIRExpression Key { get; init; }
    public HIRExpression Value { get; init; }
}

public sealed class HIRDefineClassAccessorPropertyExpression : HIRExpression
{
    public HIRDefineClassAccessorPropertyExpression(HIRExpression target, HIRExpression key, HIRExpression? getter, HIRExpression? setter)
    {
        Target = target;
        Key = key;
        Getter = getter;
        Setter = setter;
    }

    public HIRExpression Target { get; init; }
    public HIRExpression Key { get; init; }
    public HIRExpression? Getter { get; init; }
    public HIRExpression? Setter { get; init; }
}

public sealed record HIRClassMethodDataPropertyDefinition(
    string PropertyKey,
    string ClrMethodName,
    double Length,
    string FunctionName,
    bool IsStatic,
    bool IsPrivate,
    bool IsGenerator,
    bool IsAsync);

public sealed class HIRDefineClassMethodDataPropertiesExpression : HIRExpression
{
    public HIRDefineClassMethodDataPropertiesExpression(
        HIRExpression owner,
        Scope classScope,
        List<HIRClassMethodDataPropertyDefinition> methodDefinitions)
    {
        Owner = owner;
        ClassScope = classScope;
        MethodDefinitions = methodDefinitions;
    }

    public HIRExpression Owner { get; init; }
    public Scope ClassScope { get; init; }
    public List<HIRClassMethodDataPropertyDefinition> MethodDefinitions { get; init; }
}
