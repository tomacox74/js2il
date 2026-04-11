using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Js2IL.Runtime;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Tests.DebugSymbols;

public class JsCallableScopeAbiAttributeTests
{
    [Fact]
    public void CompilingSimpleFunction_EmitsNoScopesAbiAttribute()
    {
        var dllPath = CompileScript(
            "no-scopes",
            """
            "use strict";
            function add(a) {
                return a + 1;
            }
            console.log(add(41));
            """);

        var attributes = ReadCallableAbiAttributes(dllPath);

        var attr = Assert.Single(attributes);
        Assert.Equal(CallableScopeAbiKind.NoScopes, attr.Kind);
        Assert.Equal(0, attr.SingleScopeTypeMetadataToken);
    }

    [Fact]
    public void CompilingGlobalCaptureFunction_EmitsSingleScopeAbiAttribute_AndRuns()
    {
        var dllPath = CompileScript(
            "single-scope",
            """
            "use strict";
            let x = 41;
            function addOne() {
                return x + 1;
            }
            console.log(addOne());
            """);

        var attributes = ReadCallableAbiAttributes(dllPath);

        var attr = Assert.Single(attributes);
        Assert.Equal(CallableScopeAbiKind.SingleScope, attr.Kind);
        Assert.NotEqual(0, attr.SingleScopeTypeMetadataToken);
        Assert.Equal("42", RunAssembly(dllPath));
    }

    [Fact]
    public void CompilingNestedClosure_EmitsScopeArrayAbiAttribute_AndRuns()
    {
        var dllPath = CompileScript(
            "scope-array",
            """
            "use strict";
            let g = 1;
            function outer() {
                let x = 40;
                function inner() {
                    return g + x + 1;
                }
                return inner();
            }
            console.log(outer());
            """);

        var attributes = ReadCallableAbiAttributes(dllPath);

        Assert.Contains(attributes, attr => attr.Kind == CallableScopeAbiKind.ScopeArray);
        Assert.Equal("42", RunAssembly(dllPath));
    }

    [Fact]
    public void CompilingInstanceClassMethods_KeepsNoScopesAbiAttribute()
    {
        var dllPath = CompileScript(
            "instance-method",
            """
            "use strict";
            let offset = 1;
            class Counter {
                inc(x) {
                    return x + offset;
                }
            }
            console.log(new Counter().inc(41));
            """);

        var attributes = ReadCallableAbiAttributes(dllPath);

        var instanceMethod = Assert.Single(attributes, attr => string.Equals(attr.MethodName, "inc", StringComparison.Ordinal));
        Assert.Equal(CallableScopeAbiKind.NoScopes, instanceMethod.Kind);
        Assert.Contains(attributes, attr => attr.Kind == CallableScopeAbiKind.ScopeArray);
        Assert.Equal("42", RunAssembly(dllPath));
    }

    private static string CompileScript(string baseName, string js)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "CallableScopeAbi", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var jsPath = Path.Combine(outputPath, baseName + ".js");
        var dllPath = Path.Combine(outputPath, baseName + ".dll");

        var mockFs = new MockFileSystem();
        mockFs.AddFile(jsPath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath
        };

        var logger = new TestLogger();
        using var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(jsPath), $"Compilation failed. Errors: {logger.Errors}\nWarnings: {logger.Warnings}");
        Assert.True(File.Exists(dllPath), $"Expected DLL at '{dllPath}'.");
        return dllPath;
    }

    private static string RunAssembly(string dllPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = dllPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var proc = Process.Start(psi);
        Assert.NotNull(proc);
        Assert.True(proc!.WaitForExit(30000));

        var stdOut = proc.StandardOutput.ReadToEnd();
        var stdErr = proc.StandardError.ReadToEnd();
        Assert.True(proc.ExitCode == 0, $"dotnet failed: {stdErr}\nSTDOUT:\n{stdOut}");

        return stdOut.Replace("\r\n", "\n").Trim();
    }

    private static List<CallableAbiAttributeData> ReadCallableAbiAttributes(string dllPath)
    {
        using var stream = File.OpenRead(dllPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();
        var results = new List<CallableAbiAttributeData>();

        foreach (var methodHandle in reader.MethodDefinitions)
        {
            var method = reader.GetMethodDefinition(methodHandle);
            foreach (var customAttributeHandle in method.GetCustomAttributes())
            {
                var customAttribute = reader.GetCustomAttribute(customAttributeHandle);
                if (!IsCallableScopeAbiAttribute(reader, customAttribute.Constructor))
                {
                    continue;
                }

                var methodName = reader.GetString(method.Name);
                var declaringType = reader.GetTypeDefinition(method.GetDeclaringType());
                var declaringTypeName = reader.GetString(declaringType.Name);
                results.Add(ReadCallableAbiAttribute(reader, customAttribute, declaringTypeName, methodName));
            }
        }

        return results;
    }

    private static bool IsCallableScopeAbiAttribute(MetadataReader reader, EntityHandle constructorHandle)
    {
        const string ExpectedNamespace = "Js2IL.Runtime";
        const string ExpectedName = "JsCallableScopeAbiAttribute";

        if (constructorHandle.Kind == HandleKind.MemberReference)
        {
            var memberRef = reader.GetMemberReference((MemberReferenceHandle)constructorHandle);
            if (memberRef.Parent.Kind == HandleKind.TypeReference)
            {
                var typeRef = reader.GetTypeReference((TypeReferenceHandle)memberRef.Parent);
                return string.Equals(reader.GetString(typeRef.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                    && string.Equals(reader.GetString(typeRef.Name), ExpectedName, StringComparison.Ordinal);
            }
        }

        if (constructorHandle.Kind == HandleKind.MethodDefinition)
        {
            var method = reader.GetMethodDefinition((MethodDefinitionHandle)constructorHandle);
            var typeDef = reader.GetTypeDefinition(method.GetDeclaringType());
            return string.Equals(reader.GetString(typeDef.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                && string.Equals(reader.GetString(typeDef.Name), ExpectedName, StringComparison.Ordinal);
        }

        return false;
    }

    private static CallableAbiAttributeData ReadCallableAbiAttribute(
        MetadataReader reader,
        CustomAttribute attribute,
        string declaringTypeName,
        string methodName)
    {
        var blob = reader.GetBlobReader(attribute.Value);
        var prolog = blob.ReadUInt16();
        Assert.Equal(0x0001, prolog);

        var kind = (CallableScopeAbiKind)blob.ReadInt32();
        var namedArgCount = blob.ReadUInt16();
        var singleScopeTypeMetadataToken = 0;

        for (var i = 0; i < namedArgCount; i++)
        {
            var argumentKind = blob.ReadByte();
            Assert.Equal((byte)0x54, argumentKind);

            var serializationType = blob.ReadByte();
            Assert.Equal((byte)0x08, serializationType);

            var name = blob.ReadSerializedString();
            if (string.Equals(name, "SingleScopeTypeMetadataToken", StringComparison.Ordinal))
            {
                singleScopeTypeMetadataToken = blob.ReadInt32();
            }
            else
            {
                _ = blob.ReadInt32();
            }
        }

        return new CallableAbiAttributeData(declaringTypeName, methodName, kind, singleScopeTypeMetadataToken);
    }

    private sealed record CallableAbiAttributeData(
        string DeclaringTypeName,
        string MethodName,
        CallableScopeAbiKind Kind,
        int SingleScopeTypeMetadataToken);
}
