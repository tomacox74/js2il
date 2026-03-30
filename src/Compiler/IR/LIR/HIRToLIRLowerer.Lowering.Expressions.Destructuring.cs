using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerDestructuringPattern(HIRPattern pattern, TempVariable sourceValue, DestructuringWriteMode writeMode, string? sourceNameForError)
    {
        sourceValue = EnsureObject(sourceValue);

        switch (pattern)
        {
            case HIRIdentifierPattern id:
                switch (writeMode)
                {
                    case DestructuringWriteMode.Declaration:
                        return TryDeclareBinding(id.Symbol, sourceValue);

                    case DestructuringWriteMode.Assignment:
                        // Assignment to const is a runtime TypeError.
                        if (id.Symbol.BindingInfo.Kind == BindingKind.Const)
                        {
                            _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
                            return true;
                        }
                        return TryStoreToBinding(id.Symbol.BindingInfo, sourceValue, out _);

                    case DestructuringWriteMode.ForDeclarationBindingInitialization:
                        // Loop-head ForDeclaration bindings are initialized each iteration.
                        // This must be allowed for const bindings as part of a fresh iteration environment.
                        return TryStoreToBinding(id.Symbol.BindingInfo, sourceValue, out _);

                    default:
                        return false;
                }

            case HIRDefaultPattern def:
                {
                    // Apply default only when the incoming value is undefined (null).
                    var notNullLabel = CreateLabel();
                    var endLabel = CreateLabel();

                    var selected = CreateTempVariable();
                    DefineTempStorage(selected, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(sourceValue, notNullLabel));

                    if (!TryLowerExpression(def.Default, out var defaultTemp))
                    {
                        return false;
                    }
                    defaultTemp = EnsureObject(defaultTemp);
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(defaultTemp, selected));
                    _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

                    _methodBodyIR.Instructions.Add(new LIRLabel(notNullLabel));
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(sourceValue, selected));
                    _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

                    return TryLowerDestructuringPattern(def.Target, selected, writeMode, sourceNameForError);
                }

            case HIRRestPattern rest:
                // Rest patterns are materialized by the containing object/array pattern.
                return TryLowerDestructuringPattern(rest.Target, sourceValue, writeMode, sourceNameForError);

            case HIRObjectPattern obj:
                {
                    EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(obj));

                    // Collect excluded keys for object rest.
                    var excludedKeyTemps = new List<TempVariable>(obj.Properties.Count);

                    foreach (var prop in obj.Properties)
                    {
                        var keyTemp = EmitConstString(prop.Key);
                        excludedKeyTemps.Add(keyTemp);
                        var getResult = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceValue, EnsureObject(keyTemp), getResult));
                        DefineTempStorage(getResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(prop.Value, getResult, writeMode, prop.Key))
                        {
                            return false;
                        }
                    }

                    if (obj.Rest != null)
                    {
                        var excludedArray = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRBuildArray(excludedKeyTemps, excludedArray));
                        DefineTempStorage(excludedArray, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                        var restObj = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                            IntrinsicName: "Object",
                            MethodName: nameof(JavaScriptRuntime.Object.Rest),
                            Arguments: new[] { EnsureObject(sourceValue), excludedArray },
                            Result: restObj));
                        DefineTempStorage(restObj, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(obj.Rest.Target, restObj, writeMode, "rest"))
                        {
                            return false;
                        }
                    }

                    return true;
                }

            case HIRArrayPattern arr:
                {
                    EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(arr));

                    for (int i = 0; i < arr.Elements.Count; i++)
                    {
                        var elementPattern = arr.Elements[i];
                        if (elementPattern == null)
                        {
                            continue;
                        }

                        var indexTemp = EmitConstNumber(i);
                        var getResult = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceValue, indexTemp, getResult));
                        DefineTempStorage(getResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(elementPattern, getResult, writeMode, i.ToString()))
                        {
                            return false;
                        }
                    }

                    if (arr.Rest != null)
                    {
                        if (!TryBuildArrayRest(sourceValue, arr.Elements.Count, out var restArray))
                        {
                            return false;
                        }
                        if (!TryLowerDestructuringPattern(arr.Rest.Target, restArray, writeMode, "rest"))
                        {
                            return false;
                        }
                    }

                    return true;
                }

            default:
                return false;
        }
    }

    private bool TryBuildArrayRest(TempVariable sourceObject, int startIndex, out TempVariable restArray)
    {
        restArray = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(System.Array.Empty<TempVariable>(), restArray));
        DefineTempStorage(restArray, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // len = Object.GetLength(source)
        var lenTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetLength(sourceObject, lenTemp));
        DefineTempStorage(lenTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        // NOTE: temp-local allocation is linear and does not account for loop back-edges.
        // Pin loop-carry temps to stable variable slots so values remain correct across iterations.
        SetTempVariableSlot(lenTemp, CreateAnonymousVariableSlot("$arrayRest_len", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

        var idxTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber((double)startIndex, idxTemp));
        DefineTempStorage(idxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        SetTempVariableSlot(idxTemp, CreateAnonymousVariableSlot("$arrayRest_idx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

        var loopLabel = CreateLabel();
        var endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRLabel(loopLabel));

        var condTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(idxTemp, lenTemp, condTemp));
        DefineTempStorage(condTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(condTemp, endLabel));

        var itemTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceObject, idxTemp, itemTemp));
        DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRArrayAdd(restArray, EnsureObject(itemTemp)));

        // idx = idx + 1
        var oneTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, oneTemp));
        DefineTempStorage(oneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var updatedIdx = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRAddNumber(idxTemp, oneTemp, updatedIdx));
        DefineTempStorage(updatedIdx, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(updatedIdx, idxTemp));

        _methodBodyIR.Instructions.Add(new LIRBranch(loopLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

        return true;
    }

}
