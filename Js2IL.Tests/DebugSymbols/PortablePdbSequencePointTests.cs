using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class PortablePdbSequencePointTests
{
    [Fact]
    public void SequencePoints_AreDecodedWithExpectedLineAndZeroBasedColumns()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "PortablePdbSequencePoints", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "seqpoints.js");
        var dllPath = Path.Combine(outputPath, "seqpoints.dll");
        var pdbPath = Path.Combine(outputPath, "seqpoints.pdb");

        // Keep JS simple + stable locations.
        var js = "\"use strict\";\n" +
             "console.log('a');\n" +
             "console.log('b');\n";

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath,
            EmitPdb = true
        };

        var logger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(jsPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");
        Assert.True(File.Exists(pdbPath), $"Expected PDB at '{pdbPath}'.");

        // Sanity: PE has CodeView entry.
        using (var peStream = File.OpenRead(dllPath))
        using (var peReader = new PEReader(peStream))
        {
            var debugDirectory = peReader.ReadDebugDirectory();
            Assert.Contains(debugDirectory, e => e.Type == DebugDirectoryEntryType.CodeView);
        }

        using var pdbStream = File.OpenRead(pdbPath);
        using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdbReader = provider.GetMetadataReader();

        // Find at least one method that has sequence points.
        var methodWithPoints = pdbReader.MethodDebugInformation
            .Select(h => pdbReader.GetMethodDebugInformation(h))
            .FirstOrDefault(mdi => !mdi.SequencePointsBlob.IsNil && !mdi.Document.IsNil);

        Assert.False(methodWithPoints.SequencePointsBlob.IsNil, "Expected at least one method to have a sequence points blob.");

        // Decode sequence points and check for expected coordinates.
        var document = pdbReader.GetDocument(methodWithPoints.Document);
        var docName = pdbReader.GetString(document.Name);

        // We prefer the real file path as document name when available.
        Assert.True(
            string.Equals(docName, jsPath, StringComparison.OrdinalIgnoreCase) || docName.Contains("seqpoints", StringComparison.OrdinalIgnoreCase),
            $"Unexpected PDB document name '{docName}'.");

        var points = DecodeSequencePoints(pdbReader, methodWithPoints);
        Assert.NotEmpty(points);

        // Columns in Portable PDB are 0-based.
        Assert.Contains(points, p => p.StartLine == 1 && p.StartColumn == 0);

        // Expect at least one non-prologue statement to have a sequence point.
        // Note: not every statement necessarily maps to a unique IL offset, and Portable PDB
        // does not allow duplicate offsets, so some statements can be dropped.
        Assert.Contains(points, p => p.StartLine == 3 && p.StartColumn == 0);

        // Ensure IL offsets are non-decreasing.
        for (int i = 1; i < points.Count; i++)
        {
            Assert.True(points[i].IlOffset >= points[i - 1].IlOffset, "Sequence point IL offsets must be non-decreasing.");
        }
    }

    private static List<DecodedSequencePoint> DecodeSequencePoints(MetadataReader pdbReader, MethodDebugInformation mdi)
    {
        var blobReader = pdbReader.GetBlobReader(mdi.SequencePointsBlob);

        // header: local signature row id (ignored for now)
        _ = blobReader.ReadCompressedInteger();

        var results = new List<DecodedSequencePoint>();

        int ilOffset = 0;
        int previousNonHiddenStartLine = 0;
        int previousNonHiddenStartColumn = 0;

        while (blobReader.RemainingBytes > 0)
        {
            ilOffset += blobReader.ReadCompressedInteger();

            int deltaLines = blobReader.ReadCompressedInteger();
            int deltaColumns;

            if (deltaLines == 0)
            {
                deltaColumns = blobReader.ReadCompressedInteger();
                if (deltaColumns == 0)
                {
                    // hidden-sequence-point-record
                    continue;
                }
            }
            else
            {
                deltaColumns = blobReader.ReadCompressedSignedInteger();
            }

            int startLine;
            int startColumn;

            if (previousNonHiddenStartLine == 0)
            {
                startLine = blobReader.ReadCompressedInteger();
                startColumn = blobReader.ReadCompressedInteger();
            }
            else
            {
                startLine = previousNonHiddenStartLine + blobReader.ReadCompressedSignedInteger();
                startColumn = previousNonHiddenStartColumn + blobReader.ReadCompressedSignedInteger();
            }

            previousNonHiddenStartLine = startLine;
            previousNonHiddenStartColumn = startColumn;

            int endLine = startLine + deltaLines;
            int endColumn = startColumn + deltaColumns;

            results.Add(new DecodedSequencePoint(ilOffset, startLine, startColumn, endLine, endColumn));
        }

        return results;
    }

    private readonly record struct DecodedSequencePoint(int IlOffset, int StartLine, int StartColumn, int EndLine, int EndColumn);
}
