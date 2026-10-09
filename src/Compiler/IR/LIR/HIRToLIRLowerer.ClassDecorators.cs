using Jroc.HIR;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private sealed record PreparedClassDecoration(
        TempVariable Decorators, TempVariable Name, TempVariable State);

    private readonly Dictionary<string, PreparedClassDecoration> _preparedClassDecorations = new(StringComparer.Ordinal);

    private bool TryLowerClassDecorationApplication(HIRClassDecorationApplicationStatement statement)
    {
        if (!_preparedClassDecorations.TryGetValue(statement.RegistryClassName, out var decoration)
            || !_classInitializationOwnerTempsByRegistryName.TryGetValue(statement.RegistryClassName, out var owner))
        {
            throw new InvalidOperationException("Class decoration requires its prepared decorators and original constructor.");
        }

        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.ApplyClassDecorators),
            [EnsureObject(owner), decoration.Decorators, decoration.Name],
            decoration.State,
            [typeof(object), typeof(object[]), typeof(object)]));
        return true;
    }

    private bool TryLowerDecoratedClass(
        HIRInitializedUserClassTypeExpression expression,
        out TempVariable result)
    {
        result = default;
        var decorators = new List<TempVariable>();
        var inferredName = _pendingAnonymousClassExpressionInferredName;
        _pendingAnonymousClassExpressionInferredName = null;
        try
        {
            foreach (var decorator in expression.Decorators)
            {
                if (!TryLowerExpression(decorator, out var value))
                {
                    return false;
                }
                decorators.Add(EnsureObject(value));
            }
        }
        finally
        {
            _pendingAnonymousClassExpressionInferredName = inferredName;
        }

        var decoratorArray = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(decorators, decoratorArray));
        DefineTempStorage(decoratorArray, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        var original = new HIRInitializedUserClassTypeExpression(
            expression.RegistryClassName, expression.ClassScope,
            expression.InitializationStatements, expression.SuperClass,
            expression.IsClassExpression, expression.ExplicitName);
        var className = _pendingAnonymousClassExpressionInferredName ?? expression.ExplicitName;
        TempVariable name;
        if (className == null)
        {
            name = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(name));
            DefineTempStorage(name, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else
        {
            name = EnsureObject(CreateStringConstant(className));
        }
        var state = CreateTempVariable();
        DefineTempStorage(state, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _preparedClassDecorations.Add(expression.RegistryClassName, new(decoratorArray, name, state));
        try
        {
            if (!TryLowerExpression(original, out _))
            {
                return false;
            }
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.CompleteClassDecorators),
                [state],
                result,
                [typeof(object)]));
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }
        finally
        {
            _preparedClassDecorations.Remove(expression.RegistryClassName);
        }
    }
}
