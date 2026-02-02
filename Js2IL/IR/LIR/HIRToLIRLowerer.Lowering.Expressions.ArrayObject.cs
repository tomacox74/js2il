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

        // Complex case: has spread elements
        // First, collect all non-spread elements for initial capacity hint
        // Then emit array creation + individual Add/PushRange calls

        // Create the array with capacity 1 (minimum - will grow as needed)
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(Array.Empty<TempVariable>(), resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // Process each element
        foreach (var element in arrayExpr.Elements)
        {
            if (element is HIRSpreadElement spreadElement)
            {
                // Lower the spread argument
                if (!TryLowerExpression(spreadElement.Argument, out var spreadArgTemp))
                {
                    return false;
                }
                var boxedSpreadArg = EnsureObject(spreadArgTemp);
                // Emit PushRange to spread the elements
                _methodBodyIR.Instructions.Add(new LIRArrayPushRange(resultTempVar, boxedSpreadArg));
            }
            else
            {
                // Lower regular element
                if (!TryLowerExpression(element, out var elementTemp))
                {
                    return false;
                }
                var boxedElement = EnsureObject(elementTemp);
                // Emit Add for single element
                _methodBodyIR.Instructions.Add(new LIRArrayAdd(resultTempVar, boxedElement));
            }
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

                var boxedValue = EnsureObject(valueTemp);
                properties.Add(new ObjectProperty(prop.Key, boxedValue));
            }

            _methodBodyIR.Instructions.Add(new LIRNewJsObject(properties, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(System.Dynamic.ExpandoObject)));
            return true;
        }

        // Create an empty object first, then apply members in source evaluation order.
        // This preserves side-effect order for computed keys and spread members.
        _methodBodyIR.Instructions.Add(new LIRNewJsObject(new List<ObjectProperty>(), resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(System.Dynamic.ExpandoObject)));

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
