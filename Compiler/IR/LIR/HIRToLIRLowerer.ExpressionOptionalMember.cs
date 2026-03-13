using System;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerOptionalPropertyAccessExpression(HIROptionalPropertyAccessExpression propAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (!TryLowerExpression(propAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        var boxedObject = EnsureObject(objectTemp);

        int nullishLabel = CreateLabel();
        int endLabel = CreateLabel();

        // Short-circuit if base is undefined (null)
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boxedObject, nullishLabel));

        // Short-circuit if base is explicit JS null (JsNull)
        var isJsNullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), boxedObject, isJsNullTemp));
        DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, nullishLabel));

        // Non-nullish: perform access (as GetItem with string key)
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var boxedKey = EnsureObject(keyTemp);
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, boxedKey, resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Nullish: undefined
        _methodBodyIR.Instructions.Add(new LIRLabel(nullishLabel));
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerOptionalIndexAccessExpression(HIROptionalIndexAccessExpression indexAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (!TryLowerExpression(indexAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        var boxedObject = EnsureObject(objectTemp);

        int nullishLabel = CreateLabel();
        int endLabel = CreateLabel();

        // Short-circuit if base is undefined (null)
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boxedObject, nullishLabel));

        // Short-circuit if base is explicit JS null (JsNull)
        var isJsNullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), boxedObject, isJsNullTemp));
        DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, nullishLabel));

        // Non-nullish: evaluate index expression (must not run when base is nullish)
        if (!TryLowerExpression(indexAccessExpr.Index, out var indexTemp))
        {
            return false;
        }

        var indexStorage = GetTempStorage(indexTemp);
        TempVariable indexForGet = indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double)
            ? indexTemp
            : EnsureObject(indexTemp);

        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, indexForGet, resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Nullish: undefined
        _methodBodyIR.Instructions.Add(new LIRLabel(nullishLabel));
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
