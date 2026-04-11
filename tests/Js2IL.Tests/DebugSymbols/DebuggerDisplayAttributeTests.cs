using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Js2IL.Tests.DebugSymbols;

public class DebuggerDisplayAttributeTests
{
    [Fact]
    public void CompilingWithEmitPdb_EmitsDebuggerDisplayAttribute_OnScopeTypeWithUserFields()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "DebuggerDisplayAttribute", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "debuggerdisplay.js");
        var dllPath = Path.Combine(outputPath, "debuggerdisplay.dll");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, "\"use strict\";\nlet x = 123;\nfunction f() { return x; }\nconsole.log(f());\n");

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

        var scopeTypeHandle = FindTypeWithFieldNamed(reader, "x");
        Assert.True(!scopeTypeHandle.IsNil, "Expected at least one generated type with a field named 'x'.");

        var typeDef = reader.GetTypeDefinition(scopeTypeHandle);
        var debuggerDisplayCaHandle = typeDef.GetCustomAttributes()
            .FirstOrDefault(handle => IsDebuggerDisplayAttribute(reader, reader.GetCustomAttribute(handle).Constructor));

        Assert.True(!debuggerDisplayCaHandle.IsNil, "Expected DebuggerDisplayAttribute on a scope type when EmitPdb=true.");

        var debuggerDisplay = reader.GetCustomAttribute(debuggerDisplayCaHandle);
        var br = reader.GetBlobReader(debuggerDisplay.Value);
        Assert.Equal(0x0001, br.ReadUInt16());

        var displayString = br.ReadSerializedString();
        Assert.Contains("x={x}", displayString);

        Assert.Equal(0, br.ReadUInt16());
    }

    [Fact]
    public void CompilingWithoutEmitPdb_DoesNotEmitDebuggerDisplayAttribute_OnScopeTypeWithUserFields()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "DebuggerDisplayAttribute", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, "nodebuggerdisplay.js");
        var dllPath = Path.Combine(outputPath, "nodebuggerdisplay.dll");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, "\"use strict\";\nlet x = 123;\nfunction f() { return x; }\nconsole.log(f());\n");

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

        var scopeTypeHandle = FindTypeWithFieldNamed(reader, "x");
        Assert.True(!scopeTypeHandle.IsNil, "Expected at least one generated type with a field named 'x'.");

        var typeDef = reader.GetTypeDefinition(scopeTypeHandle);
        Assert.DoesNotContain(
            typeDef.GetCustomAttributes(),
            handle => IsDebuggerDisplayAttribute(reader, reader.GetCustomAttribute(handle).Constructor));
    }

    private static TypeDefinitionHandle FindTypeWithFieldNamed(MetadataReader reader, string fieldName)
    {
        return reader.TypeDefinitions
            .FirstOrDefault(typeHandle =>
            {
                var typeDef = reader.GetTypeDefinition(typeHandle);
                return typeDef.GetFields().Any(fieldHandle =>
                {
                    var fieldDef = reader.GetFieldDefinition(fieldHandle);
                    return string.Equals(reader.GetString(fieldDef.Name), fieldName, StringComparison.Ordinal);
                });
            });
    }

    private static bool IsDebuggerDisplayAttribute(MetadataReader reader, EntityHandle ctorHandle)
    {
        const string ExpectedNamespace = "System.Diagnostics";
        const string ExpectedName = "DebuggerDisplayAttribute";

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
