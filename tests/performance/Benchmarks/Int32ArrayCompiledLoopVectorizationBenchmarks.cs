using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks;

/// <summary>
/// Compares two compiled JavaScript loops, not direct calls to the Int32Array runtime helper.
/// Compilation and module loading are excluded; each invocation includes the identical
/// local array allocation and two-element result check.
/// </summary>
[MemoryDiagnoser]
[JsonExporterAttribute.FullCompressed]
[Config(typeof(Int32ArrayCompiledLoopVectorizationConfig))]
public class Int32ArrayCompiledLoopVectorizationBenchmarks
{
    private const double ExpectedChecksum = 10;
    private const string SourceTemplate = """
        "use strict";
        function run() {
            const a = new Int32Array(4096);
            for (let i = 0; i < 4096; i++) {
                __OR_STATEMENT__
            }
            return a[0] + a[4095];
        }
        module.exports = { run };
        """;

    private JrocLoadedAssembly? _canonicalAssembly;
    private JrocLoadedAssembly? _scalarAssembly;
    private ILoopExports? _canonical;
    private ILoopExports? _scalar;

    public interface ILoopExports : IDisposable
    {
        double Run();
    }

    [GlobalSetup]
    public void Setup()
    {
        try
        {
            (_canonicalAssembly, _canonical) = Prepare("a[i] |= 5;", "canonical");
            (_scalarAssembly, _scalar) = Prepare("a[i] = a[i] | 5;", "scalar");

            var canonicalChecksum = _canonical.Run();
            var scalarChecksum = _scalar.Run();
            if (canonicalChecksum != ExpectedChecksum || scalarChecksum != canonicalChecksum)
            {
                throw new InvalidOperationException(
                    $"Compiled loop checksums differ: canonical={canonicalChecksum}, scalar={scalarChecksum}, expected={ExpectedChecksum}.");
            }
        }
        catch
        {
            Cleanup();
            throw;
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _canonical?.Dispose();
        _scalar?.Dispose();
        _canonicalAssembly?.Dispose();
        _scalarAssembly?.Dispose();
        _canonical = null;
        _scalar = null;
        _canonicalAssembly = null;
        _scalarAssembly = null;
    }

    [Benchmark(Description = "Compiled scalar syntax control", Baseline = true)]
    public double ScalarControl() => _scalar!.Run();

    [Benchmark(Description = "Compiled local-const OR loop")]
    public double CanonicalOrLoop() => _canonical!.Run();

    private static (JrocLoadedAssembly Assembly, ILoopExports Exports) Prepare(
        string statement, string variant)
    {
        var artifact = JrocInMemoryCompiler.Compile(
            new JrocInMemoryCompileRequest($"int32array-{variant}.js")
            {
                SourceText = SourceTemplate.Replace("__OR_STATEMENT__", statement, StringComparison.Ordinal)
            });

        if (ReferencesVectorHelper(artifact) != (variant == "canonical"))
        {
            throw new InvalidOperationException(
                $"The {variant} fixture has an unexpected vector OR helper reference.");
        }

        var assembly = JrocInMemoryAssemblyLoader.Load(artifact);
        try
        {
            return (assembly, JsEngine.LoadModule<ILoopExports>(assembly.Assembly, artifact.ModuleIds.Single()));
        }
        catch
        {
            assembly.Dispose();
            throw;
        }
    }

    private static bool ReferencesVectorHelper(JrocCompiledAssemblyArtifact artifact)
    {
        using var pe = new PEReader(new MemoryStream(artifact.PeBytes));
        var metadata = pe.GetMetadataReader();
        foreach (var handle in metadata.MemberReferences)
        {
            if (metadata.GetString(metadata.GetMemberReference(handle).Name)
                == nameof(JavaScriptRuntime.RuntimeServices.TryVectorOrRange))
            {
                return true;
            }
        }

        return false;
    }
}

public sealed class Int32ArrayCompiledLoopVectorizationConfig : ManualConfig
{
    public Int32ArrayCompiledLoopVectorizationConfig()
    {
        AddJob(Job.ShortRun.WithId("IntrinsicsOn"));
        AddJob(Job.ShortRun.WithId("IntrinsicsOff")
            .WithEnvironmentVariable("DOTNET_EnableHWIntrinsic", "0"));
    }
}
