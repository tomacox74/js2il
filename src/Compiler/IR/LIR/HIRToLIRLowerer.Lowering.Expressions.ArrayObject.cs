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
    private bool TryLowerArrayExpression(HIRArrayExpression arrayExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Check if there are any spread elements
        bool hasSpreadElements = arrayExpr.Elements.Any(e => e is HIRSpreadElement);
        bool hasArrayHoles = arrayExpr.Elements.Any(e => e is HIRArrayHoleExpression);

        if (hasArrayHoles && !hasSpreadElements)
        {
            _methodBodyIR.Instructions.Add(
                new LIRNewJsArray(
                    new List<TempVariable>(),
                    resultTempVar,
                    CapacityHint: arrayExpr.Elements.Length));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

            var lengthKeyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString("length", lengthKeyTemp));
            DefineTempStorage(lengthKeyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var lengthValueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(arrayExpr.Elements.Length, lengthValueTemp));
            DefineTempStorage(lengthValueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var lengthSetResult = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRSetItem(
                resultTempVar,
                lengthKeyTemp,
                lengthValueTemp,
                lengthSetResult,
                ThrowOnError: true));
            DefineTempStorage(lengthSetResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            for (int i = 0; i < arrayExpr.Elements.Length; i++)
            {
                var element = arrayExpr.Elements[i];
                if (element is HIRArrayHoleExpression)
                {
                    continue;
                }

                if (!TryLowerExpression(element, out var elementTemp))
                {
                    return false;
                }

                var indexTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstNumber(i, indexTemp));
                DefineTempStorage(indexTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

                var setResult = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRSetItem(
                    resultTempVar,
                    indexTemp,
                    elementTemp,
                    setResult,
                    ThrowOnError: true));
                DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }

            return true;
        }

        if (!hasSpreadElements)
        {
            // Simple case: no spread elements, use LIRNewJsArray
            var elementTemps = new List<TempVariable>();
            foreach (var element in arrayExpr.Elements)
            {
                if (!TryLowerExpression(element, out var elementTemp))
                {
                    return false;
                }
                elementTemps.Add(elementTemp);
            }

            // Emit the LIRNewJsArray instruction
            _methodBodyIR.Instructions.Add(new LIRNewJsArray(elementTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
            return true;
        }

        // Complex case: has spread elements.
        // Preserve strict left-to-right evaluation order while enabling a small optimization:
        // seed the array with any leading non-spread elements so the ctor capacity hint is non-zero
        // and we reduce the number of subsequent Add instructions.

        int prefixCount = 0;
        while (prefixCount < arrayExpr.Elements.Length && arrayExpr.Elements[prefixCount] is not HIRSpreadElement)
        {
            prefixCount++;
        }

        var prefixElementTemps = new List<TempVariable>(capacity: prefixCount);
        for (int i = 0; i < prefixCount; i++)
        {
            if (!TryLowerExpression(arrayExpr.Elements[i], out var elementTemp))
            {
                return false;
            }
            prefixElementTemps.Add(elementTemp);
        }

        _methodBodyIR.Instructions.Add(new LIRNewJsArray(prefixElementTemps, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // Process the remaining elements (including the first spread) in order.
        for (int i = prefixCount; i < arrayExpr.Elements.Length; i++)
        {
            var element = arrayExpr.Elements[i];
            if (element is HIRSpreadElement spreadElement)
            {
                if (!TryLowerExpression(spreadElement.Argument, out var spreadArgTemp))
                {
                    return false;
                }
                var boxedSpreadArg = EnsureObject(spreadArgTemp);
                _methodBodyIR.Instructions.Add(new LIRArrayPushRange(resultTempVar, boxedSpreadArg));
                continue;
            }

            if (!TryLowerExpression(element, out var remainingTemp))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRArrayAdd(resultTempVar, remainingTemp));
        }

        return true;
    }

    private bool TryLowerObjectExpression(HIRObjectExpression objectExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();
        var objectTemp = resultTempVar;

        // Fast path: simple object literal with only non-computed properties.
        // Preserve the existing LIRNewJsObject initialization pattern for minimal IL/snapshot churn.
        bool allSimple = objectExpr.Members.All(static member => member is HIRObjectProperty { IsPrototypeMutation: false });

        if (allSimple)
        {
            if (objectExpr.ObjectLiteralShape is { IsEligible: true } shape
                && !shape.GeneratedClrTypeHandle.IsNil
                && shape.Members.Count == objectExpr.Members.Length)
            {
                var inferredProperties = new List<InferredObjectProperty>(shape.Members.Count);
                foreach (HIRObjectProperty prop in objectExpr.Members)
                {
                    if (!TryLowerExpression(prop.Value, out var valueTemp))
                    {
                        return false;
                    }

                    if (prop.IsMethodDefinition)
                    {
                        valueTemp = EmitMarkUndefinedPrototype(valueTemp);
                    }

                    inferredProperties.Add(new InferredObjectProperty(prop.Key, valueTemp));
                }

                _methodBodyIR.Instructions.Add(new LIRNewInferredJsObject(shape, inferredProperties, resultTempVar));
                DefineTempStorage(
                    resultTempVar,
                    new ValueStorage(ValueStorageKind.Reference, TypeHandle: shape.GeneratedClrTypeHandle));
                return true;
            }

            var properties = new List<ObjectProperty>();
            foreach (HIRObjectProperty prop in objectExpr.Members)
            {
                if (!TryLowerExpression(prop.Value, out var valueTemp))
                {
                    return false;
                }

                if (prop.IsMethodDefinition)
                {
                    valueTemp = EmitMarkUndefinedPrototype(valueTemp);
                }

                properties.Add(new ObjectProperty(prop.Key, valueTemp));
            }

            _methodBodyIR.Instructions.Add(new LIRNewJsObject(properties, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.JsObject)));
            return true;
        }

        // Create the object lazily. Object creation itself is not observable, and delaying it avoids
        // losing the target temp when a generator suspends while evaluating the first computed key.
        var targetCreated = false;
        void EnsureObjectTargetCreated()
        {
            if (targetCreated)
            {
                return;
            }

            _methodBodyIR.Instructions.Add(new LIRNewJsObject(new List<ObjectProperty>(), objectTemp));
            DefineTempStorage(objectTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.JsObject)));
            targetCreated = true;
        }

        foreach (var member in objectExpr.Members)
        {
            switch (member)
            {
                case HIRObjectProperty prop:
                {
                    if (!TryLowerExpression(prop.Value, out var valueTemp))
                    {
                        return false;
                    }

                    if (prop.IsMethodDefinition)
                    {
                        valueTemp = EmitMarkUndefinedPrototype(valueTemp);
                    }

                    EnsureObjectTargetCreated();
                    if (prop.IsPrototypeMutation)
                    {
                        EmitSetObjectLiteralPrototype(objectTemp, valueTemp);
                    }
                    else
                    {
                        var keyTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstString(prop.Key, keyTemp));
                        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        EmitDefineObjectLiteralDataProperty(objectTemp, keyTemp, valueTemp);
                    }
                    break;
                }

                case HIRObjectComputedProperty computed:
                {
                    // Evaluate key expression before value expression (ECMA-262 order).
                    if (!TryLowerExpression(computed.KeyExpression, out var keyExprTemp))
                    {
                        return false;
                    }

                    if (!TryLowerExpression(computed.Value, out var valueTemp))
                    {
                        return false;
                    }

                    if (computed.IsMethodDefinition)
                    {
                        valueTemp = EmitMarkUndefinedPrototype(valueTemp);
                    }

                    EnsureObjectTargetCreated();
                    EmitDefineObjectLiteralDataProperty(objectTemp, keyExprTemp, valueTemp);
                    break;
                }

                case HIRObjectAccessorProperty accessor:
                {
                    EnsureObjectTargetCreated();
                    if (!TryLowerObjectLiteralAccessorProperty(objectTemp, accessor.Key, accessor.Getter, accessor.Setter))
                    {
                        return false;
                    }
                    break;
                }

                case HIRObjectComputedAccessorProperty computedAccessor:
                {
                    if (!TryLowerExpression(computedAccessor.KeyExpression, out var accessorKeyTemp))
                    {
                        return false;
                    }

                    EnsureObjectTargetCreated();
                    if (!TryLowerObjectLiteralAccessorProperty(objectTemp, accessorKeyTemp, computedAccessor.Getter, computedAccessor.Setter))
                    {
                        return false;
                    }
                    break;
                }

                case HIRObjectSpreadProperty spread:
                {
                    if (!TryLowerExpression(spread.Argument, out var spreadTemp))
                    {
                        return false;
                    }

                    EnsureObjectTargetCreated();
                    var boxedTarget = EnsureObject(objectTemp);
                    var boxedSource = EnsureObject(spreadTemp);
                    var spreadResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                        IntrinsicName: "Object",
                        MethodName: "SpreadIntoObjectLiteral",
                        Arguments: new List<TempVariable> { boxedTarget, boxedSource },
                        Result: spreadResult));
                    DefineTempStorage(spreadResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                default:
                    throw new NotSupportedException($"Unhandled object literal member type during lowering: {member.GetType().FullName}");
            }
        }

        EnsureObjectTargetCreated();
        return true;
    }

    private bool TryLowerObjectLiteralDataProperty(TempVariable targetTemp, string key, HIRExpression valueExpression)
    {
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(key, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return TryLowerObjectLiteralDataProperty(targetTemp, keyTemp, valueExpression);
    }

    private bool TryLowerObjectLiteralDataProperty(TempVariable targetTemp, TempVariable keyTemp, HIRExpression valueExpression)
    {
        if (!TryLowerExpression(valueExpression, out var valueTemp))
        {
            return false;
        }

        EmitDefineObjectLiteralDataProperty(targetTemp, keyTemp, valueTemp);
        return true;
    }

    private void EmitDefineObjectLiteralDataProperty(TempVariable targetTemp, TempVariable keyTemp, TempVariable valueTemp)
    {
        var boxedTarget = EnsureObject(targetTemp);
        var keyStorage = GetTempStorage(keyTemp);
        var valueStorage = GetTempStorage(valueTemp);
        var keyArg = keyStorage.Kind == ValueStorageKind.Reference && keyStorage.ClrType == typeof(string)
            ? keyTemp
            : EnsureObject(keyTemp);
        var valueArg = keyStorage.Kind == ValueStorageKind.Reference
            && keyStorage.ClrType == typeof(string)
            && valueStorage.Kind == ValueStorageKind.UnboxedValue
            && (valueStorage.ClrType == typeof(double) || valueStorage.ClrType == typeof(bool))
                ? valueTemp
                : EnsureObject(valueTemp);
        var defineResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: "DefineObjectLiteralDataProperty",
            Arguments: new List<TempVariable> { boxedTarget, keyArg, valueArg },
            Result: defineResult));
        DefineTempStorage(defineResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
    }

    private void EmitSetObjectLiteralPrototype(TempVariable targetTemp, TempVariable valueTemp)
    {
        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.SetObjectLiteralPrototype),
            Arguments: new List<TempVariable> { EnsureObject(targetTemp), EnsureObject(valueTemp) },
            Result: setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
    }

    private bool TryLowerObjectLiteralAccessorProperty(TempVariable targetTemp, string key, HIRExpression? getterExpression, HIRExpression? setterExpression)
    {
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(key, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return TryLowerObjectLiteralAccessorProperty(targetTemp, keyTemp, getterExpression, setterExpression);
    }

    private bool TryLowerObjectLiteralAccessorProperty(TempVariable targetTemp, TempVariable keyTemp, HIRExpression? getterExpression, HIRExpression? setterExpression)
    {
        if (!TryLowerOptionalObjectLiteralAccessorExpression(getterExpression, out var getterTemp))
        {
            return false;
        }

        if (!TryLowerOptionalObjectLiteralAccessorExpression(setterExpression, out var setterTemp))
        {
            return false;
        }

        var boxedTarget = EnsureObject(targetTemp);
        var boxedKey = EnsureObject(keyTemp);
        var boxedGetter = EnsureObject(getterTemp);
        var boxedSetter = EnsureObject(setterTemp);
        var defineResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: "DefineObjectLiteralAccessorProperty",
            Arguments: new List<TempVariable> { boxedTarget, boxedKey, boxedGetter, boxedSetter },
            Result: defineResult));
        DefineTempStorage(defineResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerOptionalObjectLiteralAccessorExpression(HIRExpression? accessorExpression, out TempVariable accessorTemp)
    {
        if (accessorExpression is null)
        {
            accessorTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(accessorTemp));
            DefineTempStorage(accessorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        if (!TryLowerExpression(accessorExpression, out accessorTemp))
        {
            return false;
        }

        accessorTemp = EmitMarkUndefinedPrototype(accessorTemp);
        return true;
    }
}
