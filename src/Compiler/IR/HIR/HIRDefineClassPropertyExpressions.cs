using Jroc.SymbolTables;
using System.Collections.Generic;

namespace Jroc.HIR;

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

public sealed class HIRDefineClassAccessorMethodPropertyExpression : HIRExpression
{
    public HIRDefineClassAccessorMethodPropertyExpression(
        HIRExpression target,
        HIRExpression owner,
        HIRExpression key,
        Scope classScope,
        string clrMethodName,
        double length,
        string functionName,
        bool isStatic,
        bool isPrivate,
        bool isSetter,
        bool isGenerator,
        bool isAsync)
    {
        Target = target;
        Owner = owner;
        Key = key;
        ClassScope = classScope;
        ClrMethodName = clrMethodName;
        Length = length;
        FunctionName = functionName;
        IsStatic = isStatic;
        IsPrivate = isPrivate;
        IsSetter = isSetter;
        IsGenerator = isGenerator;
        IsAsync = isAsync;
    }

    public HIRExpression Target { get; }
    public HIRExpression Owner { get; }
    public HIRExpression Key { get; }
    public Scope ClassScope { get; }
    public string ClrMethodName { get; }
    public double Length { get; }
    public string FunctionName { get; }
    public bool IsStatic { get; }
    public bool IsPrivate { get; }
    public bool IsSetter { get; }
    public bool IsGenerator { get; }
    public bool IsAsync { get; }
}

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
