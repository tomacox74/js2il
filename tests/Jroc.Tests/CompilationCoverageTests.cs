using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Jroc.DebugSymbols;
using Jroc.IR;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Tests;

public sealed class CompilationCoverageTests
{
    private static JrocInMemoryCompileRequest Request(string source, bool enabled = false)
        => new(Path.Combine(Path.GetTempPath(), "jroc-coverage-tests", "sample.js"))
        {
            SourceText = source,
            AssemblyName = "CoverageSample",
            CollectCompilationCoverage = enabled,
            AssumeUnmodifiedHostGlobals = true
        };

    [Fact]
    public void NumericStatementsUseDirectIl()
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("var x = 1;\nvar y = x + 2;\ny = y * 3;"));
        Assert.NotNull(analysis.Artifact);
        Assert.True(analysis.Report.Complete);
        Assert.NotEmpty(analysis.Report.Sites);
        Assert.All(analysis.Report.Sites, site => Assert.Equal("directIl", site.Mode));
        Assert.Equal(analysis.Report.Sites.Count, analysis.Report.Counts.DirectIl);
        Assert.Equal(100d, analysis.Report.Percentages["directIl"]);
    }

    [Theory]
    [InlineData("function read(receiver, key) {\n  return receiver[key];\n}")]
    [InlineData("function call(receiver) {\n  return receiver.method();\n}")]
    [InlineData("function add(a, b) {\n  return a + b;\n}")]
    public void GenericOperationsIncludeSourceAndReason(string source)
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request(source));
        Assert.True(analysis.Report.CompilationSucceeded);
        var site = Assert.Single(analysis.Report.Sites, site => site.Line == 2 && site.Mode == "runtimeDispatch");
        Assert.Equal(Request(source).EntryFilePath, site.File);
        Assert.Equal(3, site.Column);
        Assert.NotEmpty(site.Reasons);
    }

    [Fact]
    public void StaticMathCallUsesBoundRuntimeIntrinsic()
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("var value = Math.abs(-1);"));
        Assert.True(analysis.Report.CompilationSucceeded);
        Assert.True(analysis.Report.Counts.RuntimeIntrinsic > 0);
        Assert.Equal(0, analysis.Report.Counts.RuntimeDispatch);
    }

    [Fact]
    public void GuardedStringMethodRetainsDynamicFallbackClassification()
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("var value = 'abc';\nvar result = value.indexOf('b');"));
        Assert.True(analysis.Report.Complete);
        Assert.Contains(analysis.Report.Sites,
            site => site.Line == 2 && site.Mode == "runtimeDispatch"
                && site.Reasons.Any(reason => reason.Contains("fallback", StringComparison.Ordinal)));
    }

    [Fact]
    public void UnsupportedSourceProducesPartialReportWithoutInventedDirectSites()
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("eval('var unsupported = 1;');"));
        Assert.Null(analysis.Artifact);
        Assert.False(analysis.Report.Complete);
        Assert.Equal(0, analysis.Report.Counts.DirectIl);
        var site = Assert.Single(analysis.Report.Sites);
        Assert.Equal("unsupported", site.Mode);
        Assert.Equal(1, site.Line);
        Assert.Equal(1, site.Column);
        Assert.Contains("eval", string.Join(" ", site.Reasons));
        Assert.NotEmpty(analysis.Report.Diagnostics);
    }

    [Fact]
    public void InvalidSyntaxIsDiagnosticNotUnsupportedFeature()
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("var = ;"));
        Assert.Null(analysis.Artifact);
        Assert.Equal(0, analysis.Report.Counts.Total);
        Assert.Contains("Failed to parse", string.Join(" ", analysis.Report.Diagnostics));
    }

    [Theory]
    [InlineData("#! comment\nvar value = 1;", true)]
    [InlineData("var yield = 1;", false)]
    public void SingleEntryAnalysisPreservesExplicitModuleParseGoal(string source, bool succeeds)
    {
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(
            Request(source) with { ParseAsModule = true });
        Assert.Equal(succeeds, analysis.Report.CompilationSucceeded);
        Assert.Equal(succeeds, analysis.Report.Complete);
        if (succeeds)
        {
            Assert.NotNull(analysis.Artifact);
            Assert.NotEmpty(analysis.Report.Sites);
        }
        else
        {
            Assert.Null(analysis.Artifact);
            Assert.Contains("Failed to parse JavaScript module", string.Join(" ", analysis.Report.Diagnostics));
        }
    }

    [Fact]
    public void SitesDeduplicateAcrossLoweringExpansionAndKeepWorstMode()
    {
        var collector = new CompilationCoverageCollector();
        var span = new SourceSpan("sample.js", new(1, 1), new(1, 10));
        var body = new MethodBodyIR();
        body.Instructions.Add(new LIRSequencePoint(span));
        body.Instructions.Add(new LIRConstNumber(1, new(0)));
        body.Instructions.Add(new LIRConstNumber(2, new(1)));
        collector.RecordMethod(body);
        collector.RecordMethod(body);
        Assert.Equal(1, collector.CreateReport(true).Counts.Total);
        body.Instructions.Add(new LIRAddDynamic(new(0), new(1), new(2)));
        collector.RecordMethod(body);
        var report = collector.CreateReport(true);
        Assert.Equal(1, report.Counts.RuntimeDispatch);
        Assert.Equal(0, report.Counts.DirectIl);
        Assert.Single(report.Sites);
    }

    [Fact]
    public void DisabledInstrumentationHasNoCollectorOrReportAndPreservesIl()
    {
        using var services = CompilerServices.BuildServiceProvider(new CompilerOptions());
        Assert.Null(services.GetService<CompilationCoverageCollector>());
        const string source = "function add(a, b) { return a + b; } var result = add(1, 2);";
        var ordinary = JrocInMemoryCompiler.Compile(Request(source));
        var measured = JrocInMemoryCompiler.Compile(Request(source, enabled: true));
        Assert.Null(ordinary.CompilationCoverage);
        Assert.NotNull(measured.CompilationCoverage);
        Assert.Equal(MethodBodies(ordinary), MethodBodies(measured));
    }

    [Fact]
    public void JsonCountsAndPercentagesAreConsistentAndEmptyTotalsAreZero()
    {
        var report = JrocInMemoryCompiler.AnalyzeCompilationCoverage(Request("var value = 1;")).Report;
        using var json = JsonDocument.Parse(report.ToJson());
        Assert.Equal(1, json.RootElement.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(report.Sites.Count, json.RootElement.GetProperty("counts").GetProperty("total").GetInt32());
        Assert.Equal(100d, report.Percentages.Values.Sum(), precision: 10);
        Assert.All(new CompilationCoverageCollector().CreateReport(true).Percentages.Values,
            percentage => Assert.Equal(0d, percentage));
    }

    [Fact]
    public void MultiEntryAnalysisIncludesDependenciesAndDistinctFiles()
    {
        var folder = Path.Combine(Path.GetTempPath(), "jroc-coverage-multi");
        var files = new MockFileSystem();
        files.AddFile(Path.Combine(folder, "helper.js"), "exports.value = 1;");
        var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(
            new JrocInMemoryMultiEntryCompileRequest(
            [
                new(Path.Combine(folder, "first.js"), "var value = require('./helper');"),
                new(Path.Combine(folder, "second.js"), "var value = 2;")
            ]) { FileSystem = files });
        Assert.True(analysis.Report.Complete);
        Assert.Equal(3, analysis.Report.Sites.Select(site => site.File).Distinct().Count());
    }

    private static string[] MethodBodies(JrocCompiledAssemblyArtifact artifact)
    {
        using var stream = new MemoryStream(artifact.PeBytes);
        using var pe = new PEReader(stream);
        var metadata = pe.GetMetadataReader();
        return metadata.MethodDefinitions.Select(handle => metadata.GetMethodDefinition(handle))
            .Where(method => method.RelativeVirtualAddress != 0)
            .Select(method => Convert.ToHexString(pe.GetMethodBody(method.RelativeVirtualAddress).GetILContent().AsSpan()))
            .ToArray();
    }
}
