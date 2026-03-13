using System.Collections.Generic;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Utilities;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private static bool TryGetDynamicFunctionLiteralArguments(IReadOnlyList<HIRExpression> arguments, out List<string> literalArgs)
    {
        literalArgs = new List<string>(arguments.Count);
        foreach (var argument in arguments)
        {
            if (argument is not HIRLiteralExpression { Kind: JavascriptType.String, Value: string literalValue })
            {
                literalArgs.Clear();
                return false;
            }

            literalArgs.Add(literalValue);
        }

        return true;
    }

    private static bool TryGetDynamicFunctionSyntaxErrorMessage(IReadOnlyList<HIRExpression> arguments, out string? errorMessage)
    {
        errorMessage = null;

        if (!TryGetDynamicFunctionLiteralArguments(arguments, out var literalArgs))
        {
            return false;
        }

        var parser = new JavaScriptParser();
        return !DynamicFunctionSupport.TryParseFunctionExpression(
            parser,
            "dynamic-function",
            literalArgs,
            out _,
            out errorMessage);
    }

    private bool TryEmitThrownBuiltInError(string errorTypeName, string message, out TempVariable resultTempVar)
    {
        var messageTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(message, messageTemp));
        DefineTempStorage(messageTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var errorTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(errorTypeName, messageTemp, errorTemp));
        DefineTempStorage(errorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        _methodBodyIR.Instructions.Add(new LIRThrow(errorTemp));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
