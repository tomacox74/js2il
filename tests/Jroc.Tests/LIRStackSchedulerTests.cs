using Jroc.DebugSymbols;
using Jroc.IL;
using Jroc.IR;
using Jroc.Services.TwoPhaseCompilation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Jroc.Tests;

public sealed class LIRStackSchedulerTests
{
    [Fact]
    public void Identity_EmptyBody_ReturnsEmptyLegacyDelegatingPlan()
    {
        var schedule = LIRStackScheduler.Identity(new MethodBodyIR());

        Assert.Equal(LIRStackSchedulerMode.Identity, schedule.Mode);
        Assert.Empty(schedule.Operations);
        Assert.Empty(schedule.Regions);
        Assert.Empty(schedule.TempResidencies);
        Assert.Empty(schedule.OwnedTemps);
        Assert.Empty(schedule.EffectiveLastUses);
        Assert.Equal(0, schedule.MaxStackDepth);
        Assert.Equal(default, schedule.Metrics);
    }

    [Fact]
    public void Identity_StraightLineBody_PreservesOrderAndRawLastUses()
    {
        var body = new MethodBodyIR();
        var left = AddTemp(body);
        var right = AddTemp(body);
        var result = AddTemp(body);

        body.Instructions.Add(new LIRLoadParameter(1, left));
        body.Instructions.Add(new LIRConstNumber(2, right));
        body.Instructions.Add(new LIRMulNumber(left, right, result));
        body.Instructions.Add(new LIRReturn(result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(
            new[] { 0, 1, 2, 3 },
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.All(schedule.Operations, operation =>
        {
            Assert.Equal(1, operation.InstructionCount);
            Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition);
        });
        Assert.Equal(
            new[] { TempResidency.MaterializedLocal, TempResidency.MaterializedLocal, TempResidency.MaterializedLocal },
            schedule.TempResidencies);
        Assert.All(schedule.OwnedTemps, Assert.False);
        Assert.Equal(new[] { 2, 2, 3 }, schedule.EffectiveLastUses);
        var region = Assert.Single(schedule.Regions);
        Assert.Equal(0, region.StartLirIndex);
        Assert.Equal(3, region.EndLirIndexExclusive);
        Assert.Equal(0, region.StartOperationIndex);
        Assert.Equal(3, region.OperationCount);
        Assert.Equal(1, schedule.Metrics.ScheduledRegionCount);
    }

    [Fact]
    public void Identity_ControlFlowBody_PreservesEverySourceIndex()
    {
        var body = new MethodBodyIR();
        var condition = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRLoadParameter(1, condition));
        body.Instructions.Add(new LIRBranchIfFalse(condition, 1));
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new LIRReturn(value));
        body.Instructions.Add(new LIRLabel(1));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(
            Enumerable.Range(0, body.Instructions.Count),
            schedule.Operations.Select(operation => operation.StartLirIndex));
        Assert.Collection(
            schedule.Regions,
            region =>
            {
                Assert.Equal(0, region.StartLirIndex);
                Assert.Equal(1, region.EndLirIndexExclusive);
            },
            region =>
            {
                Assert.Equal(2, region.StartLirIndex);
                Assert.Equal(3, region.EndLirIndexExclusive);
            });
    }

    [Fact]
    public void Identity_SequencePointAndScopeCreation_SplitRegions()
    {
        var body = new MethodBodyIR();
        var first = AddTemp(body);
        var second = AddTemp(body);

        body.Instructions.Add(new LIRConstNumber(1, first));
        body.Instructions.Add(new LIRSequencePoint(SourceSpan.Hidden("source.js")));
        body.Instructions.Add(new LIRConstNumber(2, second));
        body.Instructions.Add(new LIRCreateLeafScopeInstance(new ScopeId("block")));
        body.Instructions.Add(new LIRCopyTemp(second, first));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Regions,
            region =>
            {
                Assert.Equal(0, region.StartLirIndex);
                Assert.Equal(1, region.EndLirIndexExclusive);
            },
            region =>
            {
                Assert.Equal(2, region.StartLirIndex);
                Assert.Equal(3, region.EndLirIndexExclusive);
            },
            region =>
            {
                Assert.Equal(4, region.StartLirIndex);
                Assert.Equal(5, region.EndLirIndexExclusive);
            });
    }

    [Fact]
    public void Identity_InternalControlFlowAndUnknownInstructions_AreOpaqueBoundaries()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(body);
        var result = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRConstUndefined(exception));
        body.Instructions.Add(new LIRUnwrapCatchException(exception, result));
        body.Instructions.Add(new LIRConstNumber(1, value));
        body.Instructions.Add(new UnknownInstruction());
        body.Instructions.Add(new LIRCopyTemp(value, result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Regions,
            region => Assert.Equal((0, 1), (region.StartLirIndex, region.EndLirIndexExclusive)),
            region => Assert.Equal((2, 3), (region.StartLirIndex, region.EndLirIndexExclusive)),
            region => Assert.Equal((4, 5), (region.StartLirIndex, region.EndLirIndexExclusive)));
    }

    [Fact]
    public void Identity_ExceptionOperands_ContributeToRawLastUses()
    {
        var body = new MethodBodyIR();
        var exception = AddTemp(body);
        var value = AddTemp(body);

        body.Instructions.Add(new LIRUnwrapCatchException(exception, value));
        body.Instructions.Add(new LIRThrow(value));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(new[] { 0, 1 }, schedule.EffectiveLastUses);
    }

    [Fact]
    public void Identity_IntrinsicConstructorFieldStore_GroupsAtomicFusionCandidate()
    {
        var body = new MethodBodyIR();
        var result = AddTemp(body);

        body.Instructions.Add(new LIRNewIntrinsicObject(
            "Int32Array",
            System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Example",
            "buffer",
            IsPrivateField: false,
            result));
        body.Instructions.Add(new LIRReturnUndefinedImmediate());

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Collection(
            schedule.Operations,
            operation =>
            {
                Assert.Equal(0, operation.StartLirIndex);
                Assert.Equal(2, operation.InstructionCount);
                Assert.Equal(InstructionDisposition.FusedIntoEmissionUnit, operation.Disposition);
                Assert.Equal(0, operation.GetLirInstructionIndex(0));
                Assert.Equal(1, operation.GetLirInstructionIndex(1));
            },
            operation =>
            {
                Assert.Equal(2, operation.StartLirIndex);
                Assert.Equal(1, operation.InstructionCount);
                Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition);
            });
    }

    [Fact]
    public void Identity_UserConstructorFieldStore_GroupsOnlyEligibleStructuralCandidate()
    {
        var body = new MethodBodyIR();
        var result = AddTemp(body);
        var callable = new CallableId
        {
            Kind = CallableKind.ClassConstructor,
            DeclaringScopeName = "module",
            Name = "Child",
            JsParamCount = 0
        };

        body.Instructions.Add(new LIRNewUserClass(
            "Child",
            "Child",
            callable,
            NeedsScopes: false,
            ScopesArray: null,
            MinArgCount: 0,
            MaxArgCount: 0,
            IsDerivedConstructor: false,
            ParameterClrTypes: System.Array.Empty<Type?>(),
            Arguments: System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Parent",
            "child",
            IsPrivateField: false,
            result));

        var schedule = LIRStackScheduler.Identity(body);

        var operation = Assert.Single(schedule.Operations);
        Assert.Equal(2, operation.InstructionCount);
        Assert.Equal(InstructionDisposition.FusedIntoEmissionUnit, operation.Disposition);
    }

    [Fact]
    public void Identity_DerivedUserConstructor_DoesNotGroupFusionCandidate()
    {
        var body = new MethodBodyIR();
        var result = AddTemp(body);
        var callable = new CallableId
        {
            Kind = CallableKind.ClassConstructor,
            DeclaringScopeName = "module",
            Name = "Derived",
            JsParamCount = 0
        };

        body.Instructions.Add(new LIRNewUserClass(
            "Derived",
            "Derived",
            callable,
            NeedsScopes: false,
            ScopesArray: null,
            MinArgCount: 0,
            MaxArgCount: 0,
            IsDerivedConstructor: true,
            ParameterClrTypes: System.Array.Empty<Type?>(),
            Arguments: System.Array.Empty<TempVariable>(),
            result));
        body.Instructions.Add(new LIRStoreUserClassInstanceField(
            "Parent",
            "child",
            IsPrivateField: false,
            result));

        var schedule = LIRStackScheduler.Identity(body);

        Assert.Equal(2, schedule.Operations.Length);
        Assert.All(
            schedule.Operations,
            operation => Assert.Equal(InstructionDisposition.EmitNormally, operation.Disposition));
    }

    [Fact]
    public void Build_DisabledMode_RejectsScheduleConstruction()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            LIRStackScheduler.Build(
                new MethodBodyIR(),
                new LIRStackSchedulerOptions(LIRStackSchedulerMode.Disabled)));

        Assert.Contains("bypasses schedule construction", exception.Message);
    }

    [Fact]
    public void Build_UnimplementedCoverageMode_FailsExplicitly()
    {
        var exception = Assert.Throws<NotSupportedException>(() =>
            LIRStackScheduler.Build(
                new MethodBodyIR(),
                new LIRStackSchedulerOptions(LIRStackSchedulerMode.TypedNumeric)));

        Assert.Contains(nameof(LIRStackSchedulerMode.TypedNumeric), exception.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Compiler_IdentityMode_ProducesByteIdenticalArtifactToDisabledMode(bool emitPdb)
    {
        const string source = """
            "use strict";
            function numeric(a, b) { return a * 2 + b; }
            function control(a) { if (a) return 1; return 2; }
            function call(a) { return Math.floor(a); }
            function eh(a) {
              try { return a + 1; }
              catch (e) { return 0; }
              finally { Math.floor(a); }
            }
            function* gen(a) { yield a + 1; return a + 2; }
            async function af(a) { return (await a) + 1; }
            class Bar { constructor(value) { this.value = value; } }
            class Foo {
              constructor() {
                this.bar = new Bar(5);
                this.buffer = new Int32Array(3);
              }
            }
            class Getter { get value() { return 7; } }
            new Foo();
            Math.floor(1.5);
            new Getter().value;
            +true;
            console.log(numeric(4, 3), control(0), call(3.5));
            """;

        var disabled = Compile(source, LIRStackSchedulerMode.Disabled, emitPdb);
        var identity = Compile(source, LIRStackSchedulerMode.Identity, emitPdb);

        AssertEquivalentMethodBodies(disabled.PeBytes, identity.PeBytes);
        AssertEquivalentPortablePdb(disabled.PdbBytes, identity.PdbBytes);
    }

    private static TempVariable AddTemp(MethodBodyIR body)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        return temp;
    }

    private static JrocCompiledAssemblyArtifact Compile(
        string source,
        LIRStackSchedulerMode mode,
        bool emitPdb)
    {
        var root = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "LIRStackSchedulerIdentity");
        var entryPath = Path.Combine(root, "identity.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, source);

        var options = new CompilerOptions
        {
            OutputDirectory = root,
            EmitPdb = emitPdb,
            LIRStackSchedulerMode = mode
        };

        var logger = new TestLogger();
        using var services = CompilerServices.BuildServiceProvider(options, fileSystem, logger);
        var compiler = services.GetRequiredService<Compiler>();
        return compiler.CompileToArtifact(entryPath)
            ?? throw new InvalidOperationException(
                $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
    }

    private sealed record UnknownInstruction : LIRInstruction;

    private static void AssertEquivalentMethodBodies(byte[] expectedPe, byte[] actualPe)
    {
        using var expectedStream = new MemoryStream(expectedPe, writable: false);
        using var actualStream = new MemoryStream(actualPe, writable: false);
        using var expectedReader = new PEReader(expectedStream);
        using var actualReader = new PEReader(actualStream);
        var expectedMetadata = expectedReader.GetMetadataReader();
        var actualMetadata = actualReader.GetMetadataReader();

        Assert.Equal(
            expectedMetadata.MethodDefinitions.Count,
            actualMetadata.MethodDefinitions.Count);

        foreach (var expectedHandle in expectedMetadata.MethodDefinitions)
        {
            var actualHandle = MetadataTokens.MethodDefinitionHandle(
                MetadataTokens.GetRowNumber(expectedHandle));
            var expectedDefinition = expectedMetadata.GetMethodDefinition(expectedHandle);
            var actualDefinition = actualMetadata.GetMethodDefinition(actualHandle);

            Assert.Equal(
                expectedMetadata.GetString(expectedDefinition.Name),
                actualMetadata.GetString(actualDefinition.Name));
            Assert.Equal(
                expectedDefinition.RelativeVirtualAddress == 0,
                actualDefinition.RelativeVirtualAddress == 0);

            if (expectedDefinition.RelativeVirtualAddress == 0)
            {
                continue;
            }

            var expectedBody = expectedReader.GetMethodBody(expectedDefinition.RelativeVirtualAddress);
            var actualBody = actualReader.GetMethodBody(actualDefinition.RelativeVirtualAddress);

            Assert.Equal(expectedBody.GetILBytes(), actualBody.GetILBytes());
            Assert.Equal(expectedBody.MaxStack, actualBody.MaxStack);
            AssertEquivalentLocalSignature(
                expectedMetadata,
                expectedBody.LocalSignature,
                actualMetadata,
                actualBody.LocalSignature);
            Assert.Equal(
                expectedBody.LocalVariablesInitialized,
                actualBody.LocalVariablesInitialized);
            Assert.Equal(
                expectedBody.ExceptionRegions.ToArray(),
                actualBody.ExceptionRegions.ToArray());
        }
    }

    private static void AssertEquivalentLocalSignature(
        MetadataReader expectedMetadata,
        StandaloneSignatureHandle expectedHandle,
        MetadataReader actualMetadata,
        StandaloneSignatureHandle actualHandle)
    {
        Assert.Equal(expectedHandle.IsNil, actualHandle.IsNil);
        if (expectedHandle.IsNil || actualHandle.IsNil)
        {
            return;
        }

        var expectedSignature = expectedMetadata.GetStandaloneSignature(expectedHandle);
        var actualSignature = actualMetadata.GetStandaloneSignature(actualHandle);
        Assert.Equal(
            expectedMetadata.GetBlobBytes(expectedSignature.Signature),
            actualMetadata.GetBlobBytes(actualSignature.Signature));
    }

    private static void AssertEquivalentPortablePdb(byte[]? expectedPdb, byte[]? actualPdb)
    {
        Assert.Equal(expectedPdb is null, actualPdb is null);
        if (expectedPdb is null || actualPdb is null)
        {
            return;
        }

        using var expectedStream = new MemoryStream(expectedPdb, writable: false);
        using var actualStream = new MemoryStream(actualPdb, writable: false);
        using var expectedProvider = MetadataReaderProvider.FromPortablePdbStream(expectedStream);
        using var actualProvider = MetadataReaderProvider.FromPortablePdbStream(actualStream);
        var expectedReader = expectedProvider.GetMetadataReader();
        var actualReader = actualProvider.GetMetadataReader();

        Assert.Equal(
            expectedReader.MethodDebugInformation.Count,
            actualReader.MethodDebugInformation.Count);

        foreach (var expectedHandle in expectedReader.MethodDebugInformation)
        {
            var actualHandle = MetadataTokens.MethodDebugInformationHandle(
                MetadataTokens.GetRowNumber(expectedHandle));
            var expectedInfo = expectedReader.GetMethodDebugInformation(expectedHandle);
            var actualInfo = actualReader.GetMethodDebugInformation(actualHandle);

            Assert.Equal(
                MetadataTokens.GetToken(expectedInfo.Document),
                MetadataTokens.GetToken(actualInfo.Document));
            Assert.Equal(
                expectedInfo.SequencePointsBlob.IsNil
                    ? System.Array.Empty<byte>()
                    : expectedReader.GetBlobBytes(expectedInfo.SequencePointsBlob),
                actualInfo.SequencePointsBlob.IsNil
                    ? System.Array.Empty<byte>()
                    : actualReader.GetBlobBytes(actualInfo.SequencePointsBlob));
        }

        Assert.Equal(
            GetDocuments(expectedReader),
            GetDocuments(actualReader));
        Assert.Equal(
            GetLocalScopes(expectedReader),
            GetLocalScopes(actualReader));
    }

    private static string[] GetDocuments(MetadataReader reader)
        => reader.Documents
            .Select(handle =>
            {
                var document = reader.GetDocument(handle);
                var hash = document.Hash.IsNil
                    ? string.Empty
                    : Convert.ToHexString(reader.GetBlobBytes(document.Hash));
                return string.Join(
                    "|",
                    reader.GetString(document.Name),
                    reader.GetGuid(document.HashAlgorithm),
                    hash,
                    reader.GetGuid(document.Language));
            })
            .ToArray();

    private static string[] GetLocalScopes(MetadataReader reader)
        => reader.LocalScopes
            .Select(handle =>
            {
                var scope = reader.GetLocalScope(handle);
                var locals = scope.GetLocalVariables()
                    .Select(localHandle =>
                    {
                        var local = reader.GetLocalVariable(localHandle);
                        return string.Join(
                            ":",
                            local.Index,
                            (int)local.Attributes,
                            reader.GetString(local.Name));
                    });

                return string.Join(
                    "|",
                    MetadataTokens.GetToken(scope.Method),
                    scope.StartOffset,
                    scope.Length,
                    string.Join(",", locals));
            })
            .ToArray();
}
