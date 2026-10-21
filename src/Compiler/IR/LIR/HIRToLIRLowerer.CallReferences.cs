using Jroc.HIR;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerCallReference(HIRExpression expression, out TempVariable function, out TempVariable receiver)
    {
        function = default;
        receiver = default;
        if (expression is HIRChainExpression chain)
        {
            return TryLowerChainReference(chain, out function, out receiver);
        }
        if (expression is HIRPropertyAccessExpression property)
        {
            if (property.Object is HIRSuperExpression)
            {
                if (!TryLowerSuperPropertyReference(null, property.PropertyName, read: true, out var reference))
                {
                    return false;
                }
                receiver = reference.Receiver;
                function = EmitGetSuperProperty(reference);
                return true;
            }
            if (!TryLowerExpression(property.Object, out receiver))
            {
                return false;
            }
            receiver = RequireObjectCoercible(receiver);
            function = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(receiver, EmitConstString(property.PropertyName), function));
            DefineTempStorage(function, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }
        if (expression is HIRIndexAccessExpression index)
        {
            if (index.Object is HIRSuperExpression)
            {
                if (!TryLowerSuperPropertyReference(index.Index, null, read: true, out var reference))
                {
                    return false;
                }
                receiver = reference.Receiver;
                function = EmitGetSuperProperty(reference);
                return true;
            }
            if (!TryLowerExpression(index.Object, out receiver)
                || !TryLowerExpression(index.Index, out var key))
            {
                return false;
            }
            receiver = RequireObjectCoercible(receiver);
            function = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(receiver, EnsureObject(key), function));
            DefineTempStorage(function, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }
        if (expression is HIRVariableExpression variable
            && variable.Name.Kind == BindingKind.Global
            && (_activeWithObjects.Count > 0 || _scope?.MayUseBoundWithObject == true))
        {
            EmitWithGlobalCallReference(variable.Name.Name, out function, out receiver);
            return true;
        }
        receiver = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(receiver));
        DefineTempStorage(receiver, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return TryLowerExpression(expression, out function);
    }

    private void EmitWithGlobalCallReference(string name, out TempVariable function, out TempVariable receiver)
    {
        function = CreateTempVariable();
        receiver = CreateTempVariable();
        var resultFunction = function;
        var resultReceiver = receiver;
        var nameTemp = EmitConstString(name);
        var end = CreateLabel();
        if (EmitWithBindingProbe(name) is { } probe)
        {
            var bound = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(probe.HasBinding, bound));
            EmitReference(probe.WithObject);
            _methodBodyIR.Instructions.Add(new LIRBranch(end));
            _methodBodyIR.Instructions.Add(new LIRLabel(bound));
            ClearNumericRefinementsAtLabel();
        }
        var boundObject = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.GetBoundWithBindingObject), [nameTemp], boundObject));
        DefineTempStorage(boundObject, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        var global = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boundObject, global));
        EmitReference(boundObject);
        _methodBodyIR.Instructions.Add(new LIRBranch(end));
        _methodBodyIR.Instructions.Add(new LIRLabel(global));
        ClearNumericRefinementsAtLabel();
        var globalFunction = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime), nameof(JavaScriptRuntime.ObjectRuntime.GetGlobalBindingValue),
            [nameTemp], globalFunction));
        DefineTempStorage(globalFunction, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(globalFunction, resultFunction));
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultReceiver));
        _methodBodyIR.Instructions.Add(new LIRLabel(end));
        ClearNumericRefinementsAtLabel();
        DefineTempStorage(function, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        DefineTempStorage(receiver, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        void EmitReference(TempVariable withObject)
        {
            var value = EmitGetWithBindingValue(withObject, nameTemp);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(value, resultFunction));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(withObject, resultReceiver));
        }
    }
}
