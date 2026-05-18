using Js2IL.SymbolTables;

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

public sealed class HIRDefineClassMethodDataPropertyExpression : HIRExpression
{
    public HIRDefineClassMethodDataPropertyExpression(
        HIRExpression target,
        HIRExpression key,
        HIRExpression owner,
        Scope classScope,
        string clrMethodName,
        double length,
        string functionName,
        bool isStatic,
        bool isPrivate,
        bool isGenerator,
        bool isAsync)
    {
        Target = target;
        Key = key;
        Owner = owner;
        ClassScope = classScope;
        ClrMethodName = clrMethodName;
        Length = length;
        FunctionName = functionName;
        IsStatic = isStatic;
        IsPrivate = isPrivate;
        IsGenerator = isGenerator;
        IsAsync = isAsync;
    }

    public HIRExpression Target { get; init; }
    public HIRExpression Key { get; init; }
    public HIRExpression Owner { get; init; }
    public Scope ClassScope { get; init; }
    public string ClrMethodName { get; init; }
    public double Length { get; init; }
    public string FunctionName { get; init; }
    public bool IsStatic { get; init; }
    public bool IsPrivate { get; init; }
    public bool IsGenerator { get; init; }
    public bool IsAsync { get; init; }
}
