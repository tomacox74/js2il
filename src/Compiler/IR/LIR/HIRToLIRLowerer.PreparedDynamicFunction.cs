using Jroc.HIR;
using Jroc.Services;
using System.Linq;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerPreparedDynamicFunction(
        HIRPreparedDynamicFunctionExpression expression,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;
        TempVariable callee;
        TempVariable receiver;
        if (expression.Callee is HIRPropertyAccessExpression property)
        {
            if (!TryLowerExpression(property.Object, out receiver))
            {
                return false;
            }
            receiver = EnsureObject(receiver);
            callee = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.GetPreparedDynamicFunctionConstructor),
                [receiver],
                callee));
            DefineTempStorage(callee, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else
        {
            if (!TryLowerExpression(expression.Callee, out callee)
                || !TryLowerExpression(new HIRLiteralExpression(JavascriptType.Undefined, null), out receiver))
            {
                return false;
            }
            callee = EnsureObject(callee);
            receiver = EnsureObject(receiver);
        }

        if (!TryLowerCallArgumentsToArgsArray(expression.Arguments, out var arguments)
            || !TryLowerExpression(
                (HIRExpression?)expression.Factory ?? new HIRLiteralExpression(JavascriptType.Undefined, null),
                out var factory)
            || !TryLowerExpression(new HIRLiteralExpression(
                expression.SyntaxError == null ? JavascriptType.Undefined : JavascriptType.String,
                expression.SyntaxError), out var syntaxError))
        {
            return false;
        }
        var sourceTemps = expression.Sources.Select(source => EnsureObject(CreateStringConstant(source))).ToArray();
        var sources = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(sourceTemps, sources));
        DefineTempStorage(sources, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            expression.Construct
                ? nameof(JavaScriptRuntime.RuntimeServices.ConstructPreparedDynamicFunction)
                : nameof(JavaScriptRuntime.RuntimeServices.CallPreparedDynamicFunction),
            [callee, receiver, arguments, EnsureObject(factory), sources, EnsureObject(syntaxError)],
            resultTempVar,
            [typeof(object), typeof(object), typeof(object[]), typeof(object), typeof(object[]), typeof(object)]));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
