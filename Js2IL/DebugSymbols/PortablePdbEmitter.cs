using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace Js2IL.DebugSymbols;

internal static class PortablePdbEmitter
{
    public static (BlobContentId pdbContentId, ushort portablePdbVersion) Emit(
        MetadataBuilder assemblyMetadata,
        DebugSymbolRegistry debugRegistry,
        string pdbPath,
        MethodDefinitionHandle entryPoint)
    {
        ArgumentNullException.ThrowIfNull(assemblyMetadata);
        ArgumentNullException.ThrowIfNull(debugRegistry);
        ArgumentNullException.ThrowIfNull(pdbPath);

        var pdbMetadata = new MetadataBuilder();

        // Documents
        var documentHandleById = new Dictionary<string, DocumentHandle>(StringComparer.Ordinal);

        DocumentHandle GetOrAddDocument(string documentId)
        {
            documentId ??= string.Empty;
            if (documentHandleById.TryGetValue(documentId, out var existing))
            {
                return existing;
            }

            // Portable PDB document name blob encoding is handled by MetadataBuilder.
            var nameHandle = pdbMetadata.GetOrAddDocumentName(documentId);

            // Hash, language, and hash algorithm are currently unspecified.
            var docHandle = pdbMetadata.AddDocument(
                name: nameHandle,
                hashAlgorithm: default,
                hash: default,
                language: default);

            documentHandleById[documentId] = docHandle;
            return docHandle;
        }

        static (int startLine, int startColumn, int endLine, int endColumn) NormalizeSpan(SourceSpan span)
        {
            int startLine = span.Start.Line;
            // Portable PDB uses 0-based columns; our SourceSpan uses 1-based columns for human readability.
            int startColumn = span.Start.Column - 1;
            int endLine = span.End.Line;
            int endColumn = span.End.Column - 1;

            if (startLine < 0) startLine = 0;
            if (startColumn < 0) startColumn = 0;
            if (endLine < 0) endLine = 0;
            if (endColumn < 0) endColumn = 0;

            if (endLine < startLine)
            {
                endLine = startLine;
            }

            if (endLine == startLine && endColumn <= startColumn)
            {
                endColumn = startColumn + 1;
            }

            // Avoid the special "hidden" line value unless we explicitly want a hidden sequence point.
            if (startLine == SequencePoint.HiddenLine) startLine = SequencePoint.HiddenLine - 1;
            if (endLine == SequencePoint.HiddenLine) endLine = SequencePoint.HiddenLine - 1;

            return (startLine, startColumn, endLine, endColumn);
        }

        static BlobBuilder EncodeSequencePointsBlob(MethodSequencePoint[] points, int localSignatureRowId)
        {
            var builder = new BlobBuilder();

            // header: LocalSignature (StandAloneSig row id)
            builder.WriteCompressedInteger(localSignatureRowId);

            // We only support single-document method bodies right now (Document column on MethodDebugInformation is non-nil).
            int previousOffset = 0;
            int previousNonHiddenStartLine = 0;
            int previousNonHiddenStartColumn = 0;

            foreach (var sp in points)
            {
                int offsetDelta = sp.IlOffset - previousOffset;
                builder.WriteCompressedInteger(offsetDelta);
                previousOffset = sp.IlOffset;

                var (startLine, startColumn, endLine, endColumn) = NormalizeSpan(sp.Span);

                bool isHidden = startLine == SequencePoint.HiddenLine &&
                                endLine == SequencePoint.HiddenLine &&
                                startColumn == 0 &&
                                endColumn == 0;

                if (isHidden)
                {
                    // hidden-sequence-point-record
                    builder.WriteCompressedInteger(0);
                    builder.WriteCompressedInteger(0);
                    continue;
                }

                // sequence-point-record
                int deltaLines = endLine - startLine;
                int deltaColumns = endColumn - startColumn;

                builder.WriteCompressedInteger(deltaLines);

                if (deltaLines == 0)
                {
                    // unsigned compressed, non-zero per spec (we normalize spans to ensure this)
                    builder.WriteCompressedInteger(deltaColumns);
                }
                else
                {
                    // signed compressed
                    builder.WriteCompressedSignedInteger(deltaColumns);
                }

                if (previousNonHiddenStartLine == 0)
                {
                    builder.WriteCompressedInteger(startLine);
                    builder.WriteCompressedInteger(startColumn);
                }
                else
                {
                    builder.WriteCompressedSignedInteger(startLine - previousNonHiddenStartLine);
                    builder.WriteCompressedSignedInteger(startColumn - previousNonHiddenStartColumn);
                }

                previousNonHiddenStartLine = startLine;
                previousNonHiddenStartColumn = startColumn;
            }

            return builder;
        }

        // Method debug information rows are expected to align with MethodDef row ids.
        // Emit one MethodDebugInformation row per MethodDef (including methods without sequence points).
        int methodCount = assemblyMetadata.GetRowCount(TableIndex.MethodDef);
        for (int rowId = 1; rowId <= methodCount; rowId++)
        {
            var methodDef = MetadataTokens.MethodDefinitionHandle(rowId);

            if (!debugRegistry.TryGetMethodDebugInfo(methodDef, out var debugInfo))
            {
                pdbMetadata.AddMethodDebugInformation(document: default, sequencePoints: default);
                continue;
            }

            var points = debugInfo.SequencePoints ?? Array.Empty<MethodSequencePoint>();
            int localSignatureRowId = debugInfo.LocalSignature.IsNil ? 0 : MetadataTokens.GetRowNumber(debugInfo.LocalSignature);

            // Emit LocalVariable + LocalScope so debuggers can show source locals.
            // Keep scope broad (whole method) for now.
            LocalVariableHandle firstLocalVar = default;
            if (debugInfo.Locals is { Length: > 0 })
            {
                for (int i = 0; i < debugInfo.Locals.Length; i++)
                {
                    var local = debugInfo.Locals[i];
                    var handle = pdbMetadata.AddLocalVariable(
                        attributes: LocalVariableAttributes.None,
                        index: local.Index,
                        name: pdbMetadata.GetOrAddString(local.Name));

                    if (i == 0)
                    {
                        firstLocalVar = handle;
                    }
                }

                int length = debugInfo.IlLength;
                if (length <= 0)
                {
                    length = 1;
                }

                pdbMetadata.AddLocalScope(
                    method: methodDef,
                    importScope: default,
                    variableList: firstLocalVar,
                    constantList: default,
                    startOffset: 0,
                    length: length);
            }

            // Emit method debug info (sequence points) with correct local signature row id.
            // The Portable PDB sequence points blob requires strict ordering and no duplicate IL offsets.
            var ordered = points
                .Where(p => p.IlOffset >= 0)
                .OrderBy(p => p.IlOffset)
                .ToArray();

            if (ordered.Length == 0)
            {
                // Still emit a sequence points blob if we have a local signature to associate.
                if (localSignatureRowId != 0)
                {
                    var minimal = EncodeSequencePointsBlob(Array.Empty<MethodSequencePoint>(), localSignatureRowId);
                    var minimalHandle = pdbMetadata.GetOrAddBlob(minimal);
                    pdbMetadata.AddMethodDebugInformation(document: default, sequencePoints: minimalHandle);
                }
                else
                {
                    pdbMetadata.AddMethodDebugInformation(document: default, sequencePoints: default);
                }
                continue;
            }

            var primaryDocumentId = ordered[0].Span.Document ?? string.Empty;
            var documentHandle = GetOrAddDocument(primaryDocumentId);

            var deduped = new List<MethodSequencePoint>(ordered.Length);
            int lastOffset = -1;
            foreach (var sp in ordered)
            {
                if (sp.IlOffset == lastOffset)
                {
                    continue;
                }
                deduped.Add(sp);
                lastOffset = sp.IlOffset;
            }

            var seqBlob = EncodeSequencePointsBlob(deduped.ToArray(), localSignatureRowId);
            var seqHandle = pdbMetadata.GetOrAddBlob(seqBlob);
            pdbMetadata.AddMethodDebugInformation(documentHandle, seqHandle);
        }

        // Serialize the Portable PDB metadata.
        ImmutableArray<int> rowCounts = assemblyMetadata.GetRowCounts();
        var pdbBuilder = new PortablePdbBuilder(pdbMetadata, rowCounts, entryPoint);

        var pdbBlob = new BlobBuilder();
        BlobContentId pdbContentId = pdbBuilder.Serialize(pdbBlob);

        Directory.CreateDirectory(Path.GetDirectoryName(pdbPath) ?? ".");
        var pdbBytes = pdbBlob.ToArray();
        const int maxAttempts = 20;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                File.WriteAllBytes(pdbPath, pdbBytes);
                break;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(25 * attempt);
            }
        }

        return (pdbContentId, pdbBuilder.FormatVersion);
    }
}
