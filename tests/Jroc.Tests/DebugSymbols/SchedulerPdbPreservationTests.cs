using Jroc.IL;
using Jroc.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Jroc.Tests.DebugSymbols;

public sealed class SchedulerPdbPreservationTests
{
    [Fact]
    public void DisabledAndIdentityModes_HaveEquivalentDecodedSymbols()
    {
        const string source = """
            "use strict";
            function calc(a, b) {
              const x = a * 2 + 1;
              const y = b - 3;
              return x + y;
            }
            console.log(calc(4, 8));
            """;

        var disabled = Compile(
            source,
            LIRStackSchedulerMode.Disabled,
            emitPdb: true);
        var identity = Compile(
            source,
            LIRStackSchedulerMode.Identity,
            emitPdb: true);

        var disabledSymbols = ReadSymbols(disabled);
        var identitySymbols = ReadSymbols(identity);

        Assert.Equal(disabledSymbols.Documents, identitySymbols.Documents);
        Assert.Equal(
            disabledSymbols.Methods.Select(method => method.SemanticIdentity),
            identitySymbols.Methods.Select(method => method.SemanticIdentity));
        Assert.Equal(
            disabledSymbols.LocalScopes.Select(scope => scope.SemanticIdentity),
            identitySymbols.LocalScopes.Select(scope => scope.SemanticIdentity));

        var calcScopes = identitySymbols.LocalScopes
            .Where(scope =>
                scope.Locals.Any(local => local.Name == "x")
                && scope.Locals.Any(local => local.Name == "y"))
            .ToArray();
        var calcScope = Assert.Single(calcScopes);
        Assert.Equal(new[] { "x", "y" }, calcScope.Locals.Select(local => local.Name));
        Assert.Equal(new[] { 0, 1 }, calcScope.Locals.Select(local => local.Index));
        Assert.Equal(0, calcScope.StartOffset);
        Assert.Equal(calcScope.MethodIlLength, calcScope.Length);
    }

    [Fact]
    public void AsyncAndGeneratorStatements_RetainOriginalSourceLines()
    {
        const string source = """
            "use strict";
            async function af(value) {
              const resumed = await Promise.resolve(value);
              return resumed + 1;
            }
            function* gen(value) {
              const resumed = yield value + 1;
              return resumed + 2;
            }
            af(1);
            gen(1).next();
            """;

        var artifact = Compile(
            source,
            LIRStackSchedulerMode.Identity,
            emitPdb: true);
        var symbols = ReadSymbols(artifact);
        var nonHiddenLines = symbols.Methods
            .SelectMany(method => method.Points)
            .Where(point => !point.IsHidden)
            .Select(point => point.StartLine)
            .ToHashSet();

        Assert.Contains(3, nonHiddenLines);
        Assert.Contains(4, nonHiddenLines);
        Assert.Contains(7, nonHiddenLines);
        Assert.Contains(8, nonHiddenLines);
    }

    [Fact]
    public void SchedulerCompilesWithAndWithoutPdbEmission()
    {
        const string source = """
            "use strict";
            function calc(a) {
              const x = a * 2 + 1;
              return x;
            }
            console.log(calc(4));
            """;

        var withPdb = Compile(
            source,
            LIRStackSchedulerMode.Identity,
            emitPdb: true);
        var withoutPdb = Compile(
            source,
            LIRStackSchedulerMode.Identity,
            emitPdb: false);

        Assert.NotNull(withPdb.PdbBytes);
        Assert.Null(withoutPdb.PdbBytes);
        Assert.NotEmpty(withPdb.PeBytes);
        Assert.NotEmpty(withoutPdb.PeBytes);
    }

    [Fact]
    public void LiteralScheduling_PreservesDecodedSourceSpansAndSourceLocals()
    {
        const string source = """
            "use strict";
            function make(a, b) {
              const label = "values";
              return [a * 2, b + 3, label];
            }
            make(2, 4);
            """;

        var previous = ReadSymbols(Compile(
            source,
            LIRStackSchedulerMode.ConversionsAndStableLoads,
            emitPdb: true));
        var scheduled = ReadSymbols(Compile(
            source,
            LIRStackSchedulerMode.LiteralAndArguments,
            emitPdb: true));

        Assert.Equal(previous.Documents, scheduled.Documents);
        Assert.Equal(
            previous.Methods.SelectMany(method => method.Points)
                .Select(point => point.SemanticIdentity),
            scheduled.Methods.SelectMany(method => method.Points)
                .Select(point => point.SemanticIdentity));
        Assert.Equal(
            previous.LocalScopes.SelectMany(scope => scope.Locals)
                .Select(local => (local.Name, local.Attributes)),
            scheduled.LocalScopes.SelectMany(scope => scope.Locals)
                .Select(local => (local.Name, local.Attributes)));
    }

    [Fact]
    public void CallResultScheduling_PreservesDecodedSourceSpansAndSourceLocals()
    {
        const string source = """
            "use strict";
            function calculate(value) {
              const result = Math.floor(value) + Math.sqrt(value);
              return result;
            }
            calculate(9);
            """;

        var previous = ReadSymbols(Compile(
            source,
            LIRStackSchedulerMode.LiteralAndArguments,
            emitPdb: true));
        var scheduled = ReadSymbols(Compile(
            source,
            LIRStackSchedulerMode.CallResults,
            emitPdb: true));

        Assert.Equal(previous.Documents, scheduled.Documents);
        Assert.Equal(
            previous.Methods.SelectMany(method => method.Points)
                .Select(point => point.SemanticIdentity),
            scheduled.Methods.SelectMany(method => method.Points)
                .Select(point => point.SemanticIdentity));
        Assert.Equal(
            previous.LocalScopes.SelectMany(scope => scope.Locals)
                .Select(local => (local.Name, local.Attributes)),
            scheduled.LocalScopes.SelectMany(scope => scope.Locals)
                .Select(local => (local.Name, local.Attributes)));
    }

    private static JrocCompiledAssemblyArtifact Compile(
        string source,
        LIRStackSchedulerMode mode,
        bool emitPdb)
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "SchedulerPdbPreservation");
        var entryPath = Path.Combine(root, "scheduler-pdb.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, source);
        var options = new CompilerOptions
        {
            OutputDirectory = root,
            EmitPdb = emitPdb,
            LIRStackSchedulerMode = mode
        };
        var logger = new TestLogger();
        using var services = CompilerServices.BuildServiceProvider(
            options,
            fileSystem,
            logger);
        var compiler = services.GetRequiredService<Compiler>();
        return compiler.CompileToArtifact(entryPath)
            ?? throw new InvalidOperationException(
                $"Compilation failed. Errors: {logger.Errors}\n"
                + $"Warnings: {logger.Warnings}");
    }

    private static SymbolSnapshot ReadSymbols(
        JrocCompiledAssemblyArtifact artifact)
    {
        Assert.NotNull(artifact.PdbBytes);
        using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
        using var peReader = new PEReader(peStream);
        var peMetadata = peReader.GetMetadataReader();
        using var pdbStream = new MemoryStream(
            artifact.PdbBytes!,
            writable: false);
        using var pdbProvider =
            MetadataReaderProvider.FromPortablePdbStream(pdbStream);
        var pdbMetadata = pdbProvider.GetMetadataReader();

        var documents = pdbMetadata.Documents
            .Select(handle =>
            {
                var document = pdbMetadata.GetDocument(handle);
                return new DocumentSnapshot(
                    MetadataTokens.GetRowNumber(handle),
                    pdbMetadata.GetString(document.Name),
                    document.Hash.IsNil
                        ? string.Empty
                        : Convert.ToHexString(
                            pdbMetadata.GetBlobBytes(document.Hash)),
                    document.HashAlgorithm.IsNil
                        ? Guid.Empty
                        : pdbMetadata.GetGuid(document.HashAlgorithm),
                    document.Language.IsNil
                        ? Guid.Empty
                        : pdbMetadata.GetGuid(document.Language));
            })
            .ToArray();

        var methods = new List<MethodSnapshot>();
        foreach (var methodHandle in peMetadata.MethodDefinitions)
        {
            var row = MetadataTokens.GetRowNumber(methodHandle);
            var method = peMetadata.GetMethodDefinition(methodHandle);
            var name = peMetadata.GetString(method.Name);
            var debugHandle = MetadataTokens.MethodDebugInformationHandle(row);
            var debugInfo = pdbMetadata.GetMethodDebugInformation(debugHandle);
            var points = debugInfo.SequencePointsBlob.IsNil
                ? System.Array.Empty<SequencePointSnapshot>()
                : debugInfo.GetSequencePoints()
                    .Select(point => new SequencePointSnapshot(
                        point.Offset,
                        point.IsHidden,
                        point.StartLine,
                        point.StartColumn,
                        point.EndLine,
                        point.EndColumn,
                        MetadataTokens.GetRowNumber(
                            point.Document.IsNil
                                ? debugInfo.Document
                                : point.Document)))
                    .ToArray();
            var ilLength = method.RelativeVirtualAddress == 0
                ? 0
                : peReader.GetMethodBody(method.RelativeVirtualAddress)
                    .GetILBytes()?.Length ?? 0;
            methods.Add(new MethodSnapshot(row, name, ilLength, points));
        }

        var methodByRow = methods.ToDictionary(method => method.Row);
        var localScopes = pdbMetadata.LocalScopes
            .Select(handle =>
            {
                var scope = pdbMetadata.GetLocalScope(handle);
                var methodRow = MetadataTokens.GetRowNumber(scope.Method);
                var locals = scope.GetLocalVariables()
                    .Select(localHandle =>
                    {
                        var local = pdbMetadata.GetLocalVariable(localHandle);
                        return new LocalSnapshot(
                            local.Index,
                            pdbMetadata.GetString(local.Name),
                            local.Attributes);
                    })
                    .ToArray();
                return new LocalScopeSnapshot(
                    methodByRow[methodRow].Name,
                    methodByRow[methodRow].IlLength,
                    scope.StartOffset,
                    scope.Length,
                    locals);
            })
            .ToArray();

        return new SymbolSnapshot(documents, methods.ToArray(), localScopes);
    }

    private sealed record SymbolSnapshot(
        DocumentSnapshot[] Documents,
        MethodSnapshot[] Methods,
        LocalScopeSnapshot[] LocalScopes);

    private readonly record struct DocumentSnapshot(
        int Row,
        string Name,
        string Hash,
        Guid HashAlgorithm,
        Guid Language);

    private sealed record MethodSnapshot(
        int Row,
        string Name,
        int IlLength,
        SequencePointSnapshot[] Points)
    {
        internal string SemanticIdentity => string.Join(
            "|",
            Row,
            Name,
            IlLength,
            string.Join(",", Points.Select(point => point.SemanticIdentity)));
    }

    private readonly record struct SequencePointSnapshot(
        int Offset,
        bool IsHidden,
        int StartLine,
        int StartColumn,
        int EndLine,
        int EndColumn,
        int DocumentRow)
    {
        internal string SemanticIdentity => string.Join(
            ":",
            IsHidden,
            StartLine,
            StartColumn,
            EndLine,
            EndColumn,
            DocumentRow);
    }

    private sealed record LocalScopeSnapshot(
        string MethodName,
        int MethodIlLength,
        int StartOffset,
        int Length,
        LocalSnapshot[] Locals)
    {
        internal string SemanticIdentity => string.Join(
            "|",
            MethodName,
            MethodIlLength,
            StartOffset,
            Length,
            string.Join(
                ",",
                Locals.Select(local => string.Join(
                    ":",
                    local.Index,
                    local.Name,
                    (int)local.Attributes))));
    }

    private readonly record struct LocalSnapshot(
        int Index,
        string Name,
        LocalVariableAttributes Attributes);
}
