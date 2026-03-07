using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services;

internal static class CallableScopeAbiAttributeEmitter
{
    private const byte PropertyNamedArgument = 0x54;
    private const byte Int32SerializationType = 0x08;

    public static void Emit(
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        EntityHandle parent,
        Js2IL.Runtime.CallableScopeAbiKind kind,
        int singleScopeTypeMetadataToken = 0)
    {
        ArgumentNullException.ThrowIfNull(metadataBuilder);
        ArgumentNullException.ThrowIfNull(bclReferences);

        var blob = new BlobBuilder();
        blob.WriteUInt16(0x0001);
        blob.WriteInt32((int)kind);

        if (kind == Js2IL.Runtime.CallableScopeAbiKind.SingleScope && singleScopeTypeMetadataToken != 0)
        {
            blob.WriteUInt16(1);
            blob.WriteByte(PropertyNamedArgument);
            blob.WriteByte(Int32SerializationType);
            WriteSerString(blob, "SingleScopeTypeMetadataToken");
            blob.WriteInt32(singleScopeTypeMetadataToken);
        }
        else
        {
            blob.WriteUInt16(0);
        }

        metadataBuilder.AddCustomAttribute(
            parent: parent,
            constructor: bclReferences.JsCallableScopeAbiAttribute_Ctor_Ref,
            value: metadataBuilder.GetOrAddBlob(blob));
    }

    private static void WriteSerString(BlobBuilder blob, string value)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(value);
        WriteCompressedUInt32(blob, (uint)utf8.Length);
        blob.WriteBytes(utf8);
    }

    private static void WriteCompressedUInt32(BlobBuilder blob, uint value)
    {
        if (value <= 0x7Fu)
        {
            blob.WriteByte((byte)value);
            return;
        }

        if (value <= 0x3FFFu)
        {
            blob.WriteByte((byte)((value >> 8) | 0x80u));
            blob.WriteByte((byte)(value & 0xFFu));
            return;
        }

        if (value <= 0x1FFFFFFFu)
        {
            blob.WriteByte((byte)((value >> 24) | 0xC0u));
            blob.WriteByte((byte)((value >> 16) & 0xFFu));
            blob.WriteByte((byte)((value >> 8) & 0xFFu));
            blob.WriteByte((byte)(value & 0xFFu));
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(value), "Value too large for compressed integer encoding.");
    }
}
