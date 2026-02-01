using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class DebuggableAttributeTests
{
    [Fact]
    public void CompilingWithEmitPdb_EmitsDebuggableAttribute()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "DebuggableAttribute", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "debuggable.js");
        var dllPath = Path.Combine(outputPath, "debuggable.dll");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, "\"use strict\";\nconsole.log('hi');\n");

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

        using var stream = File.OpenRead(dllPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        var assemblyDef = reader.GetAssemblyDefinition();
        var debuggableCaHandle = assemblyDef.GetCustomAttributes()
            .FirstOrDefault(handle => IsDebuggableAttribute(reader, reader.GetCustomAttribute(handle).Constructor));

        Assert.True(!debuggableCaHandle.IsNil, "Expected DebuggableAttribute custom attribute on assembly when EmitPdb=true.");

        var debuggableCa = reader.GetCustomAttribute(debuggableCaHandle);
        var br = reader.GetBlobReader(debuggableCa.Value);
        var prolog = br.ReadUInt16();
        Assert.Equal(0x0001, prolog);

        // DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
        Assert.True(br.ReadBoolean(), "Expected isJITTrackingEnabled=true");
        Assert.True(br.ReadBoolean(), "Expected isJITOptimizerDisabled=true");

        var namedArgCount = br.ReadUInt16();
        Assert.Equal(0, namedArgCount);
    }

    [Fact]
    public void CompilingWithoutEmitPdb_DoesNotEmitDebuggableAttribute()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "DebuggableAttribute", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "nodebuggable.js");
        var dllPath = Path.Combine(outputPath, "nodebuggable.dll");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, "\"use strict\";\nconsole.log('hi');\n");

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath,
            EmitPdb = false
        };

        var logger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(jsPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");

        using var stream = File.OpenRead(dllPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        var assemblyDef = reader.GetAssemblyDefinition();
        Assert.DoesNotContain(
            assemblyDef.GetCustomAttributes(),
            handle => IsDebuggableAttribute(reader, reader.GetCustomAttribute(handle).Constructor));
    }

    private static bool IsDebuggableAttribute(MetadataReader reader, EntityHandle ctorHandle)
    {
        const string ExpectedNamespace = "System.Diagnostics";
        const string ExpectedName = "DebuggableAttribute";

        if (ctorHandle.Kind == HandleKind.MethodDefinition)
        {
            var method = reader.GetMethodDefinition((MethodDefinitionHandle)ctorHandle);
            var declaringType = reader.GetTypeDefinition(method.GetDeclaringType());
            return string.Equals(reader.GetString(declaringType.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                && string.Equals(reader.GetString(declaringType.Name), ExpectedName, StringComparison.Ordinal);
        }

        if (ctorHandle.Kind == HandleKind.MemberReference)
        {
            var memberRef = reader.GetMemberReference((MemberReferenceHandle)ctorHandle);
            var parent = memberRef.Parent;
            if (parent.Kind == HandleKind.TypeReference)
            {
                var typeRef = reader.GetTypeReference((TypeReferenceHandle)parent);
                return string.Equals(reader.GetString(typeRef.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                    && string.Equals(reader.GetString(typeRef.Name), ExpectedName, StringComparison.Ordinal);
            }
        }

        return false;
    }
}
