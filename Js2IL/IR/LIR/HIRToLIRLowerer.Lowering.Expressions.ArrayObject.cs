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
    private bool TryLowerArrayExpression(HIRArrayExpression arrayExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Check if there are any spread elements
        bool hasSpreadElements = arrayExpr.Elements.Any(e => e is HIRSpreadElement);

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
                // Ensure each element is boxed as object for the array
                elementTemps.Add(EnsureObject(elementTemp));
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
            prefixElementTemps.Add(EnsureObject(elementTemp));
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

            var boxedRemaining = EnsureObject(remainingTemp);
            _methodBodyIR.Instructions.Add(new LIRArrayAdd(resultTempVar, boxedRemaining));
        }

        return true;
    }

    private bool TryLowerObjectExpression(HIRObjectExpression objectExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Fast path: simple object literal with only non-computed properties.
        // Preserve the existing LIRNewJsObject initialization pattern for minimal IL/snapshot churn.
        bool allSimple = objectExpr.Members.All(static member => member is HIRObjectProperty);

        if (allSimple)
        {
            var properties = new List<ObjectProperty>();
            foreach (HIRObjectProperty prop in objectExpr.Members)
            {
                if (!TryLowerExpression(prop.Value, out var valueTemp))
                {
                    return false;
                }

                // Do not force-box the value; typed temps are passed directly so that the IL
                // emitter can use void-returning typed setters (SetPropertyNumber/Boolean/String)
                // and avoid a 'box' instruction for numeric/bool/string properties.
                properties.Add(new ObjectProperty(prop.Key, valueTemp));
            }

            _methodBodyIR.Instructions.Add(new LIRNewJsObject(properties, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.JsObject)));
            return true;
        }

        // Create an empty object first, then apply members in source evaluation order.
        // This preserves side-effect order for computed keys and spread members.
        _methodBodyIR.Instructions.Add(new LIRNewJsObject(new List<ObjectProperty>(), resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.JsObject)));

        foreach (var member in objectExpr.Members)
        {
            switch (member)
            {
                case HIRObjectProperty prop:
                {
                    var keyTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString(prop.Key, keyTemp));
                    DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    if (!TryLowerExpression(prop.Value, out var valueTemp))
                    {
                        return false;
                    }

                    var boxedKey = EnsureObject(keyTemp);
                    var boxedValue = EnsureObject(valueTemp);
                    var setResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRSetItem(resultTempVar, boxedKey, boxedValue, setResult));
                    DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
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

                    var boxedKey = EnsureObject(keyExprTemp);
                    var boxedValue = EnsureObject(valueTemp);
                    var setResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRSetItem(resultTempVar, boxedKey, boxedValue, setResult));
                    DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                case HIRObjectSpreadProperty spread:
                {
                    if (!TryLowerExpression(spread.Argument, out var spreadTemp))
                    {
                        return false;
                    }

                    var boxedTarget = EnsureObject(resultTempVar);
                    var boxedSource = EnsureObject(spreadTemp);
                    var spreadResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                        IntrinsicName: "Object",
                        MethodName: "SpreadInto",
                        Arguments: new List<TempVariable> { boxedTarget, boxedSource },
                        Result: spreadResult));
                    DefineTempStorage(spreadResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                default:
                    throw new NotSupportedException($"Unhandled object literal member type during lowering: {member.GetType().FullName}");
            }
        }
        return true;
    }

}
