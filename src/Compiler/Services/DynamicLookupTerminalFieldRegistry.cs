using Jroc.Utilities.Ecma335;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.Services;

/// <summary>
/// Reserves stable field handles during method compilation, then emits their
/// dedicated owner after all predeclared generated methods are finalized.
/// </summary>
internal sealed class DynamicLookupTerminalFieldRegistry
{
    private readonly List<FieldDefinitionHandle> _fields = [];
    private int? _firstFieldRow;
    private bool _emitted;

    internal FieldDefinitionHandle ReserveField(
        MetadataBuilder metadataBuilder)
    {
        if (_emitted)
        {
            throw new InvalidOperationException(
                "Dynamic lookup terminal fields have already been emitted.");
        }

        _firstFieldRow ??=
            metadataBuilder.GetRowCount(TableIndex.Field) + 1;
        if (metadataBuilder.GetRowCount(TableIndex.Field)
            != _firstFieldRow.Value - 1)
        {
            throw new InvalidOperationException(
                "Field definitions were emitted while dynamic lookup terminal fields were reserved.");
        }

        var handle = MetadataTokens.FieldDefinitionHandle(
            _firstFieldRow.Value + _fields.Count);
        _fields.Add(handle);
        return handle;
    }

    internal void Emit(
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences)
    {
        if (_emitted)
        {
            throw new InvalidOperationException(
                "Dynamic lookup terminal fields have already been emitted.");
        }

        _emitted = true;
        if (_fields.Count == 0)
        {
            return;
        }

        if (metadataBuilder.GetRowCount(TableIndex.Field)
            != _firstFieldRow!.Value - 1)
        {
            throw new InvalidOperationException(
                "Reserved dynamic lookup terminal field rows are no longer available.");
        }

        var typeBuilder = new TypeBuilder(
            metadataBuilder,
            "Jroc.Generated",
            "DynamicLookupSites");
        _ = typeBuilder.AddTypeDefinition(
            TypeAttributes.NotPublic
                | TypeAttributes.Abstract
                | TypeAttributes.Sealed
                | TypeAttributes.BeforeFieldInit,
            bclReferences.ObjectType,
            firstFieldOverride: _fields[0],
            firstMethodOverride: null);

        var fieldSignature = new BlobBuilder();
        new BlobEncoder(fieldSignature)
            .Field()
            .Type()
            .Int32();
        var signatureHandle =
            metadataBuilder.GetOrAddBlob(fieldSignature);

        foreach (var expectedHandle in _fields)
        {
            var fieldRow =
                MetadataTokens.GetRowNumber(expectedHandle);
            var actualHandle = metadataBuilder.AddFieldDefinition(
                FieldAttributes.Assembly
                    | FieldAttributes.Static,
                metadataBuilder.GetOrAddString(
                    $"__jroc_dynamicLookup_{fieldRow}"),
                signatureHandle);
            if (actualHandle != expectedHandle)
            {
                throw new InvalidOperationException(
                    $"Dynamic lookup terminal field row mismatch. Expected 0x{MetadataTokens.GetToken(expectedHandle):X8}, got 0x{MetadataTokens.GetToken(actualHandle):X8}.");
            }
        }
    }
}
