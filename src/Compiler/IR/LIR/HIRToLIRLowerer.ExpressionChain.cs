using Jroc.HIR;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerChainExpression(
        HIRChainExpression chainExpression,
        out TempVariable resultTempVar)
        => TryLowerChainReference(
            chainExpression,
            out resultTempVar,
            out _,
            out _);

    private bool TryLowerChainReference(
        HIRChainExpression chainExpression,
        out TempVariable resultTempVar,
        out TempVariable receiverTempVar)
        => TryLowerChainReference(
            chainExpression,
            out resultTempVar,
            out receiverTempVar,
            out _);

    private bool TryLowerChainReference(
        HIRChainExpression chainExpression,
        out TempVariable resultTempVar,
        out TempVariable receiverTempVar,
        out TempVariable shortCircuitedTempVar)
    {
        resultTempVar = CreateTempVariable();
        receiverTempVar = CreateTempVariable();
        shortCircuitedTempVar = CreateTempVariable();
        var nullishLabel = CreateLabel();
        var endLabel = CreateLabel();
        TempVariable currentValue;
        TempVariable? callReceiver = null;
        var segmentIndex = 0;

        if (chainExpression.BaseExpression is HIRSuperExpression)
        {
            if (chainExpression.Segments.Count == 0)
            {
                return false;
            }

            var firstSegment = chainExpression.Segments[0];
            HIRExpression superAccess = firstSegment switch
            {
                HIRChainCallSegment call =>
                    new HIRCallExpression(
                        chainExpression.BaseExpression,
                        call.Arguments),
                HIRChainPropertySegment property =>
                    new HIRPropertyAccessExpression(
                        chainExpression.BaseExpression,
                        property.PropertyName),
                HIRChainIndexSegment index =>
                    new HIRIndexAccessExpression(
                        chainExpression.BaseExpression,
                        index.Index),
                _ => null!
            };
            if (superAccess == null
                || !TryLowerExpression(superAccess, out currentValue))
            {
                return false;
            }

            if (firstSegment is not HIRChainCallSegment)
            {
                if (!TryLowerExpression(
                        new HIRThisExpression(),
                        out var superReceiver))
                {
                    return false;
                }

                callReceiver = EnsureObject(superReceiver);
            }
            segmentIndex = 1;
        }
        else if (!TryLowerExpression(
                     chainExpression.BaseExpression,
                     out currentValue))
        {
            return false;
        }

        currentValue = EnsureObject(currentValue);

        for (; segmentIndex < chainExpression.Segments.Count; segmentIndex++)
        {
            var segment = chainExpression.Segments[segmentIndex];
            if (segment.Optional)
            {
                var isNullish = EmitIsNullish(currentValue);
                _methodBodyIR.Instructions.Add(
                    new LIRBranchIfTrue(isNullish, nullishLabel));
            }

            switch (segment)
            {
                case HIRChainPropertySegment property:
                {
                    callReceiver = currentValue;
                    var propertyName = EmitConstString(property.PropertyName);
                    var propertyValue = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetItem(
                        currentValue,
                        propertyName,
                        propertyValue));
                    DefineTempStorage(
                        propertyValue,
                        new ValueStorage(
                            ValueStorageKind.Reference,
                            typeof(object)));
                    currentValue = propertyValue;
                    break;
                }

                case HIRChainIndexSegment index:
                {
                    callReceiver = currentValue;
                    if (!TryLowerExpression(index.Index, out var indexValue))
                    {
                        return false;
                    }

                    var propertyValue = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetItem(
                        currentValue,
                        EnsureObject(indexValue),
                        propertyValue));
                    DefineTempStorage(
                        propertyValue,
                        new ValueStorage(
                            ValueStorageKind.Reference,
                            typeof(object)));
                    currentValue = propertyValue;
                    break;
                }

                case HIRChainCallSegment call:
                {
                    if (!TryLowerCallArgumentsToArgsArray(
                            call.Arguments,
                            out var argumentsArray))
                    {
                        return false;
                    }

                    var receiver = callReceiver
                        ?? EmitConstUndefined();
                    var callResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(
                        new LIRCallRuntimeServicesStatic(
                            nameof(JavaScriptRuntime.RuntimeServices.CallWithThis),
                            new[]
                            {
                                currentValue,
                                EnsureObject(receiver),
                                argumentsArray
                            },
                            callResult,
                            new[]
                            {
                                typeof(object),
                                typeof(object),
                                typeof(object[])
                            }));
                    DefineTempStorage(
                        callResult,
                        new ValueStorage(
                            ValueStorageKind.Reference,
                            typeof(object)));
                    currentValue = callResult;
                    callReceiver = null;
                    break;
                }
            }
        }

        _methodBodyIR.Instructions.Add(
            new LIRCopyTemp(currentValue, resultTempVar));
        var finalReceiver = callReceiver ?? EmitConstUndefined();
        _methodBodyIR.Instructions.Add(
            new LIRCopyTemp(finalReceiver, receiverTempVar));
        _methodBodyIR.Instructions.Add(
            new LIRConstBoolean(false, shortCircuitedTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        _methodBodyIR.Instructions.Add(new LIRLabel(nullishLabel));
        ClearNumericRefinementsAtLabel();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(receiverTempVar));
        _methodBodyIR.Instructions.Add(
            new LIRConstBoolean(true, shortCircuitedTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        ClearNumericRefinementsAtLabel();
        DefineTempStorage(
            resultTempVar,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        DefineTempStorage(
            receiverTempVar,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        DefineTempStorage(
            shortCircuitedTempVar,
            new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return true;
    }

    private TempVariable EmitConstUndefined()
    {
        var undefined = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefined));
        DefineTempStorage(
            undefined,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return undefined;
    }
}
