using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.TwoPhaseCompilation;

internal static class MethodDefinitionFinalizer
{
    public static MethodDefinitionHandle EmitMethod(MetadataBuilder metadataBuilder, TypeBuilder typeBuilder, CompiledCallableBody body)
    {
        if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
        if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));
        if (body == null) throw new ArgumentNullException(nameof(body));

        body.Validate();

        // Emit parameter metadata first so we can pass the ParamList handle.
        // Parameter sequence numbers are 1-based.
        ParameterHandle firstParam = default;
        for (ushort i = 0; i < body.ParameterNames.Length; i++)
        {
            var paramName = body.ParameterNames[i] ?? string.Empty;
            var paramHandle = metadataBuilder.AddParameter(
                attributes: ParameterAttributes.None,
                name: metadataBuilder.GetOrAddString(paramName),
                sequenceNumber: (ushort)(i + 1));

            if (i == 0)
            {
                firstParam = paramHandle;
            }
        }

        var handle = typeBuilder.AddMethodDefinition(
            body.Attributes,
            body.MethodName,
            body.Signature,
            body.BodyOffset,
            firstParam);

        if (handle != body.ExpectedMethodDef)
        {
            throw new InvalidOperationException(
                $"[TwoPhase] MethodDef token mismatch for {body.Callable.DisplayName}. " +
                $"Expected 0x{MetadataTokens.GetToken(body.ExpectedMethodDef):X8}, got 0x{MetadataTokens.GetToken(handle):X8}.");
        }

        return handle;
    }
}
