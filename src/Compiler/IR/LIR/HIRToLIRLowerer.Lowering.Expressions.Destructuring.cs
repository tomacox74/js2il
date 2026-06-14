using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerDestructuringPattern(HIRPattern pattern, TempVariable sourceValue, DestructuringWriteMode writeMode, string? sourceNameForError)
        => TryLowerDestructuringPattern(pattern, sourceValue, writeMode, sourceNameForError, preparedTarget: null);

    private sealed record PreparedDestructuringTarget(TempVariable Object, TempVariable Key);

    private void EmitWithBindingProbe(string name)
    {
        if (_activeWithObjects.Count == 0)
        {
            return;
        }

        var nameTemp = EmitConstString(name);
        var probeResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.HasPropertyIn),
            new[] { EnsureObject(nameTemp), _activeWithObjects.Peek() },
            probeResult));
        DefineTempStorage(probeResult, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
    }

    private bool TryLowerDestructuringPattern(
        HIRPattern pattern,
        TempVariable sourceValue,
        DestructuringWriteMode writeMode,
        string? sourceNameForError,
        PreparedDestructuringTarget? preparedTarget)
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

            case HIRPropertyTargetPattern propertyTarget:
                if (writeMode != DestructuringWriteMode.Assignment)
                {
                    return false;
                }

                if (preparedTarget != null)
                {
                    return TryStorePreparedDestructuringTarget(preparedTarget, sourceValue);
                }

                return TryLowerPropertyAssignmentTarget(propertyTarget.Object, propertyTarget.PropertyName, sourceValue, out _);

            case HIRIndexTargetPattern indexTarget:
                if (writeMode != DestructuringWriteMode.Assignment)
                {
                    return false;
                }

                if (preparedTarget != null)
                {
                    return TryStorePreparedDestructuringTarget(preparedTarget, sourceValue);
                }

                return TryLowerIndexAssignmentTarget(indexTarget.Object, indexTarget.Index, sourceValue, out _);

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

                    return TryLowerDestructuringPattern(def.Target, selected, writeMode, sourceNameForError, preparedTarget);
                }

            case HIRRestPattern rest:
                // Rest patterns are materialized by the containing object/array pattern.
                return TryLowerDestructuringPattern(rest.Target, sourceValue, writeMode, sourceNameForError, preparedTarget);

            case HIRObjectPattern obj:
                {
                    EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(obj));

                    // Collect excluded keys for object rest.
                    var excludedKeyTemps = new List<TempVariable>(obj.Properties.Count);

                    foreach (var prop in obj.Properties)
                    {
                        if (!TryLowerObjectPatternKey(prop, out var keyTemp))
                        {
                            return false;
                        }

                        excludedKeyTemps.Add(keyTemp);

                        if (!TryPrepareDestructuringAssignmentTarget(prop.Value, writeMode, out var nestedPreparedTarget))
                        {
                            return false;
                        }

                        var getResult = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceValue, EnsureObject(keyTemp), getResult));
                        DefineTempStorage(getResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(prop.Value, getResult, writeMode, prop.Key ?? "<computed>", nestedPreparedTarget))
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
                return TryLowerArrayDestructuringPattern(arr, sourceValue, writeMode, sourceNameForError);

            default:
                return false;
        }
    }

    private bool TryLowerObjectPatternKey(HIRObjectPatternProperty prop, out TempVariable keyTemp)
    {
        if (prop.ComputedKey == null)
        {
            keyTemp = EmitConstString(prop.Key!);
            return true;
        }

        if (!TryLowerExpression(prop.ComputedKey, out var rawKey))
        {
            keyTemp = default;
            return false;
        }

        keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.ToPropertyKeyString),
            new[] { EnsureObject(rawKey) },
            keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return true;
    }

    private bool TryPrepareDestructuringAssignmentTarget(
        HIRPattern pattern,
        DestructuringWriteMode writeMode,
        out PreparedDestructuringTarget? preparedTarget)
    {
        preparedTarget = null;
        if (pattern is HIRDefaultPattern defaultPattern)
        {
            return TryPrepareDestructuringAssignmentTarget(defaultPattern.Target, writeMode, out preparedTarget);
        }

        if (pattern is HIRRestPattern restPattern)
        {
            return TryPrepareDestructuringAssignmentTarget(restPattern.Target, writeMode, out preparedTarget);
        }

        if (pattern is HIRIdentifierPattern identifierPattern)
        {
            EmitWithBindingProbe(identifierPattern.Symbol.Name);
            return true;
        }

        if (writeMode != DestructuringWriteMode.Assignment)
        {
            return true;
        }

        if (pattern is HIRPropertyTargetPattern propertyTarget)
        {
            if (!TryLowerExpression(propertyTarget.Object, out var objectTemp))
            {
                return false;
            }

            preparedTarget = new PreparedDestructuringTarget(EnsureObject(objectTemp), EnsureObject(EmitConstString(propertyTarget.PropertyName)));
            return true;
        }

        if (pattern is HIRIndexTargetPattern indexTarget)
        {
            if (!TryLowerExpression(indexTarget.Object, out var objectTemp))
            {
                return false;
            }

            if (!TryLowerExpression(indexTarget.Index, out var indexTemp))
            {
                return false;
            }

            var indexStorage = GetTempStorage(indexTemp);
            var keyTemp = indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double)
                ? indexTemp
                : EnsureObject(indexTemp);
            preparedTarget = new PreparedDestructuringTarget(EnsureObject(objectTemp), keyTemp);
            return true;
        }

        return true;
    }

    private bool TryStorePreparedDestructuringTarget(PreparedDestructuringTarget target, TempVariable valueToStore)
    {
        var keyStorage = GetTempStorage(target.Key);
        var valueStorage = GetTempStorage(valueToStore);
        var canUseNumericSetItem = keyStorage.Kind == ValueStorageKind.UnboxedValue && keyStorage.ClrType == typeof(double);
        var canUseStringKeyDoubleValueSetItem =
            keyStorage.Kind == ValueStorageKind.Reference &&
            keyStorage.ClrType == typeof(string) &&
            valueStorage.Kind == ValueStorageKind.UnboxedValue &&
            valueStorage.ClrType == typeof(double);

        if (!canUseNumericSetItem && !canUseStringKeyDoubleValueSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }

        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(target.Object, target.Key, valueToStore, setResult, UsesStrictAssignmentSemantics()));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerArrayDestructuringPattern(
        HIRArrayPattern arr,
        TempVariable sourceValue,
        DestructuringWriteMode writeMode,
        string? sourceNameForError)
    {
        EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(arr));

        var iterator = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.Object),
            nameof(JavaScriptRuntime.Object.GetIterator),
            new[] { EnsureObject(sourceValue) },
            iterator));
        DefineTempStorage(iterator, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptIterator)));
        SetTempVariableSlot(iterator, CreateAnonymousVariableSlot("$destructuring_iter", new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptIterator))));

        var completed = CreateTempVariable();
        DefineTempStorage(completed, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        SetTempVariableSlot(completed, CreateAnonymousVariableSlot("$destructuring_completed", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

        var iteratorDone = CreateTempVariable();
        DefineTempStorage(iteratorDone, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        SetTempVariableSlot(iteratorDone, CreateAnonymousVariableSlot("$destructuring_done", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

        var falseTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
        DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(falseTemp, completed));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(falseTemp, iteratorDone));

        var trueTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
        DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        if (_isGenerator && !_methodBodyIR.LeafScopeId.IsNil)
        {
            return TryLowerArrayDestructuringPatternInGenerator(arr, iterator, completed, iteratorDone, trueTemp, writeMode);
        }

        var tryStart = CreateLabel();
        var tryEnd = CreateLabel();
        var finallyStart = CreateLabel();
        var finallyEnd = CreateLabel();
        var end = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRLabel(tryStart));

        for (int i = 0; i < arr.Elements.Count; i++)
        {
            var elementPattern = arr.Elements[i];
            if (elementPattern == null)
            {
                if (!TryEmitIteratorDestructuringStep(iterator, iteratorDone, trueTemp, out _, needValue: false))
                {
                    return false;
                }
                continue;
            }

            if (!TryPrepareDestructuringAssignmentTarget(elementPattern, writeMode, out var preparedTarget))
            {
                return false;
            }

            if (!TryEmitIteratorDestructuringStep(iterator, iteratorDone, trueTemp, out var elementValue, needValue: true))
            {
                return false;
            }

            if (!TryLowerDestructuringPattern(elementPattern, elementValue, writeMode, i.ToString(), preparedTarget))
            {
                return false;
            }
        }

        if (arr.Rest != null)
        {
            if (!TryPrepareDestructuringAssignmentTarget(arr.Rest.Target, writeMode, out var preparedRestTarget))
            {
                return false;
            }

            if (!TryBuildArrayRestFromIterator(iterator, iteratorDone, trueTemp, out var restArray))
            {
                return false;
            }

            if (!TryLowerDestructuringPattern(arr.Rest.Target, restArray, writeMode, "rest", preparedRestTarget))
            {
                return false;
            }
        }

        if (arr.Rest == null)
        {
            var skipNormalClose = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, skipNormalClose));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, completed));
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                nameof(JavaScriptRuntime.Object),
                nameof(JavaScriptRuntime.Object.IteratorClose),
                new[] { EnsureObject(iterator) }));
            _methodBodyIR.Instructions.Add(new LIRLabel(skipNormalClose));
        }

        _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, completed));
        _methodBodyIR.Instructions.Add(new LIRLeave(end));
        _methodBodyIR.Instructions.Add(new LIRLabel(tryEnd));

        _methodBodyIR.Instructions.Add(new LIRLabel(finallyStart));
        var skipClose = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(completed, skipClose));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, skipClose));
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
            nameof(JavaScriptRuntime.Object),
            nameof(JavaScriptRuntime.Object.IteratorCloseForThrowCompletion),
            new[] { EnsureObject(iterator) }));
        _methodBodyIR.Instructions.Add(new LIRLabel(skipClose));
        _methodBodyIR.Instructions.Add(new LIREndFinally());
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyEnd));
        _methodBodyIR.Instructions.Add(new LIRLabel(end));

        _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
            ExceptionRegionKind.Finally,
            TryStartLabelId: tryStart,
            TryEndLabelId: tryEnd,
            HandlerStartLabelId: finallyStart,
            HandlerEndLabelId: finallyEnd));

        return true;
    }

    private bool TryLowerArrayDestructuringPatternInGenerator(
        HIRArrayPattern arr,
        TempVariable iterator,
        TempVariable completed,
        TempVariable iteratorDone,
        TempVariable trueTemp,
        DestructuringWriteMode writeMode)
    {
        var scopeName = _methodBodyIR.LeafScopeId.Name;
        var finallyEntry = CreateLabel();
        var finallyExit = CreateLabel();
        var end = CreateLabel();
        var ctx = CreateGeneratorFinallyContext(finallyEntry, finallyExit);

        EmitResetGeneratorPendingCompletions(scopeName);
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIterator), EnsureObject(iterator)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringCompleted), completed));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIteratorDone), iteratorDone));
        _generatorTryCatchFinallyStack.Push(ctx);
        try
        {
            for (int i = 0; i < arr.Elements.Count; i++)
            {
                var elementPattern = arr.Elements[i];
                if (elementPattern == null)
                {
                    if (!TryEmitIteratorDestructuringStep(iterator, iteratorDone, trueTemp, out _, needValue: false))
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIteratorDone), iteratorDone));
                    continue;
                }

                if (!TryPrepareDestructuringAssignmentTarget(elementPattern, writeMode, out var preparedTarget))
                {
                    return false;
                }

                if (!TryEmitIteratorDestructuringStep(iterator, iteratorDone, trueTemp, out var elementValue, needValue: true))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIteratorDone), iteratorDone));

                if (!TryLowerDestructuringPattern(elementPattern, elementValue, writeMode, i.ToString(), preparedTarget))
                {
                    return false;
                }
            }

            if (arr.Rest != null)
            {
                if (!TryPrepareDestructuringAssignmentTarget(arr.Rest.Target, writeMode, out var preparedRestTarget))
                {
                    return false;
                }

                if (!TryBuildArrayRestFromIterator(iterator, iteratorDone, trueTemp, out var restArray))
                {
                    return false;
                }

                if (!TryLowerDestructuringPattern(arr.Rest.Target, restArray, writeMode, "rest", preparedRestTarget))
                {
                    return false;
                }
            }

            if (arr.Rest == null)
            {
                var skipNormalClose = CreateLabel();
                _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, skipNormalClose));
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, completed));
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringCompleted), completed));
                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                    nameof(JavaScriptRuntime.Object),
                    nameof(JavaScriptRuntime.Object.IteratorClose),
                    new[] { EnsureObject(iterator) }));
                _methodBodyIR.Instructions.Add(new LIRLabel(skipNormalClose));
            }

            _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, completed));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringCompleted), completed));
            _methodBodyIR.Instructions.Add(new LIRBranch(end));

            _methodBodyIR.Instructions.Add(new LIRLabel(finallyEntry));
            _generatorTryCatchFinallyStack.Pop();
            _generatorTryCatchFinallyStack.Push(ctx with { IsInFinally = true });

            var persistedIterator = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIterator), persistedIterator));
            DefineTempStorage(persistedIterator, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var persistedCompleted = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringCompleted), persistedCompleted));
            DefineTempStorage(persistedCompleted, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var persistedDone = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._destructuringIteratorDone), persistedDone));
            DefineTempStorage(persistedDone, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var skipClose = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(persistedCompleted, skipClose));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(persistedDone, skipClose));

            var hasPendingException = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, GeneratorHasPendingExceptionField, hasPendingException));
            DefineTempStorage(hasPendingException, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            var normalClose = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasPendingException, normalClose));
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                nameof(JavaScriptRuntime.Object),
                nameof(JavaScriptRuntime.Object.IteratorCloseForThrowCompletion),
                new[] { EnsureObject(persistedIterator) }));
            _methodBodyIR.Instructions.Add(new LIRBranch(skipClose));

            _methodBodyIR.Instructions.Add(new LIRLabel(normalClose));
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                nameof(JavaScriptRuntime.Object),
                nameof(JavaScriptRuntime.Object.IteratorClose),
                new[] { EnsureObject(persistedIterator) }));
            _methodBodyIR.Instructions.Add(new LIRLabel(skipClose));
            _methodBodyIR.Instructions.Add(new LIRBranch(finallyExit));

            _methodBodyIR.Instructions.Add(new LIRLabel(finallyExit));
            EmitDispatchGeneratorPendingCompletions(scopeName, end);
            _methodBodyIR.Instructions.Add(new LIRLabel(end));
            return true;
        }
        finally
        {
            if (_generatorTryCatchFinallyStack.Count > 0)
            {
                _generatorTryCatchFinallyStack.Pop();
            }
        }
    }

    private bool TryEmitIteratorDestructuringStep(
        TempVariable iterator,
        TempVariable iteratorDone,
        TempVariable trueTemp,
        out TempVariable value,
        bool needValue)
    {
        value = CreateTempVariable();
        DefineTempStorage(value, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        var alreadyDoneLabel = CreateLabel();
        var stepDoneLabel = CreateLabel();
        var endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, alreadyDoneLabel));

        var step = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.Object),
            nameof(JavaScriptRuntime.Object.IteratorDestructuringStep),
            new[] { EnsureObject(iterator) },
            step));
        DefineTempStorage(step, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(step, stepDoneLabel));

        if (needValue)
        {
            var iterValue = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.Object),
                nameof(JavaScriptRuntime.Object.IteratorDestructuringStepValue),
                new[] { EnsureObject(step) },
                iterValue));
            DefineTempStorage(iterValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(EnsureObject(iterValue), value));
        }

        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(stepDoneLabel));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, iteratorDone));
        _methodBodyIR.Instructions.Add(new LIRLabel(alreadyDoneLabel));
        var undefinedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefinedTemp));
        DefineTempStorage(undefinedTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(undefinedTemp, value));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        return true;
    }

    private bool TryBuildArrayRestFromIterator(
        TempVariable iterator,
        TempVariable iteratorDone,
        TempVariable trueTemp,
        out TempVariable restArray)
    {
        restArray = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(System.Array.Empty<TempVariable>(), restArray));
        DefineTempStorage(restArray, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
        SetTempVariableSlot(restArray, CreateAnonymousVariableSlot("$destructuring_rest", new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array))));

        var loopLabel = CreateLabel();
        var endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRLabel(loopLabel));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, endLabel));

        if (!TryEmitIteratorDestructuringStep(iterator, iteratorDone, trueTemp, out var itemValue, needValue: true))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(iteratorDone, endLabel));
        _methodBodyIR.Instructions.Add(new LIRArrayAdd(restArray, EnsureObject(itemValue)));
        _methodBodyIR.Instructions.Add(new LIRBranch(loopLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        return true;
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
