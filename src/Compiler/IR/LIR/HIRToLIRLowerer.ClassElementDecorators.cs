using Jroc.HIR;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private TempVariable EmitDecoratedClassField(string owner, TempVariable receiver, string name, TempVariable? value = null)
    {
        var type = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetUserClassType(owner, type));
        DefineTempStorage(type, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
        var args = new List<TempVariable> { type, EnsureObject(receiver), CreateStringConstant(name) };
        var types = new List<Type> { typeof(Type), typeof(object), typeof(string) };
        if (value.HasValue)
        {
            args.Add(EnsureObject(value.Value));
            types.Add(typeof(object));
        }
        var result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            value.HasValue ? nameof(JavaScriptRuntime.RuntimeServices.TransformDecoratedClassField)
                : nameof(JavaScriptRuntime.RuntimeServices.CompleteDecoratedClassField),
            args, result, types.ToArray()));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return result;
    }

    private bool TryLowerClassElementDecorators(HIRClassElementDecoratorsExpression expression, out TempVariable result)
    {
        result = default;
        if (!TryLowerExpression(new HIRUserClassTypeExpression(expression.RegistryClassName), out var owner))
            return false;
        var decorators = new List<TempVariable>();
        foreach (var decorator in expression.Decorators)
        {
            if (!TryLowerExpression(decorator, out var value)) return false;
            decorators.Add(EnsureObject(value));
        }
        var array = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(decorators, array));
        DefineTempStorage(array, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.PrepareClassElementDecorators),
            [EnsureObject(owner), CreateStringConstant(expression.Name), CreateStringConstant(expression.Kind),
                CreateBooleanConstant(expression.IsStatic), array],
            result, [typeof(object), typeof(string), typeof(string), typeof(bool), typeof(object[])]));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
