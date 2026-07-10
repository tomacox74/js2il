using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using JavaScriptRuntime;
using Xunit;

namespace Jroc.Tests;

public sealed class ObjectLiteralTypeGenerationGroundworkTests
{
    private sealed class SpecializedJsObjectForTest : JsObject
    {
        public double n;
        public string? text;
    }

    [Fact]
    public void GeneratedAssembly_DeclaresObjectLiteralTypeForEligibleShapeOnly()
    {
        var entryPath = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "ObjectLiteralTypeGenerationGroundwork",
            Guid.NewGuid().ToString("N"),
            "entry.js");

        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath)
        {
            SourceText = """
                const a = { b: 'hello', n: 42, flag: true };
                console.log(a.b, a.n, a.flag);

                const unsafe = { x: 1 };
                leak(unsafe);

                function leak(value) {
                    console.log(value);
                }
                """
        });

        using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
        using var peReader = new PEReader(peStream);
        var reader = peReader.GetMetadataReader();

        // Object literal types are nested: Modules.<Module> -> ObjectLiterals -> ObjectLiteral_L{line}C{col}_{binding}.
        var containerHandle = Assert.Single(
            reader.TypeDefinitions,
            handle => reader.GetString(reader.GetTypeDefinition(handle).Name) == "ObjectLiterals");
        var container = reader.GetTypeDefinition(containerHandle);
        Assert.True(container.IsNested);

        var moduleTypeHandle = container.GetDeclaringType();
        var moduleType = reader.GetTypeDefinition(moduleTypeHandle);
        Assert.Equal("Modules", reader.GetString(moduleType.Namespace));

        var objectLiteralTypeHandles = reader.TypeDefinitions
            .Where(handle =>
            {
                var type = reader.GetTypeDefinition(handle);
                return reader.GetString(type.Name).StartsWith("ObjectLiteral_", StringComparison.Ordinal);
            })
            .ToList();

        var typeHandle = Assert.Single(objectLiteralTypeHandles);
        var type = reader.GetTypeDefinition(typeHandle);
        Assert.Equal("ObjectLiteral_L1C11_a", reader.GetString(type.Name));
        Assert.Equal(containerHandle, type.GetDeclaringType());

        Assert.Equal(HandleKind.TypeReference, type.BaseType.Kind);
        var baseTypeRef = reader.GetTypeReference(MetadataTokens.TypeReferenceHandle(MetadataTokens.GetRowNumber(type.BaseType)));
        Assert.Equal("JavaScriptRuntime", reader.GetString(baseTypeRef.Namespace));
        Assert.Equal("JsObject", reader.GetString(baseTypeRef.Name));
        Assert.Contains(type.GetMethods().Select(reader.GetMethodDefinition), method =>
            reader.GetString(method.Name) == ".ctor"
            && (method.Attributes & MethodAttributes.Public) == MethodAttributes.Public);

        var fieldNames = type.GetFields()
            .Select(reader.GetFieldDefinition)
            .Select(field => reader.GetString(field.Name))
            .ToArray();

        Assert.Equal(new[] { "b", "n", "flag" }, fieldNames);
    }

    [Fact]
    public void JsObjectSubclass_PreservesDictionaryDescriptorEnumerationAndJsonBehavior()
    {
        var obj = new SpecializedJsObjectForTest
        {
            n = 42,
            text = "hello"
        };

        obj.SetNumber("n", obj.n);
        obj.SetString("text", obj.text);

        IDictionary<string, object?> dict = obj;
        Assert.Equal(42d, dict["n"]);
        Assert.Equal("hello", dict["text"]);
        Assert.Equal(new[] { "n", "text" }, dict.Keys.ToArray());
        Assert.True(PropertyDescriptorStore.TryGetOwn(obj, "n", out var descriptor));
        Assert.Equal(42d, descriptor.Value);
        Assert.True(descriptor.Writable);
        Assert.True(descriptor.Enumerable);
        Assert.True(descriptor.Configurable);
        Assert.Equal("""{"n":42,"text":"hello"}""", global::JavaScriptRuntime.JSON.Stringify(obj));
    }
}
