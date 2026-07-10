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

        var objectLiteralTypes = reader.TypeDefinitions
            .Select(reader.GetTypeDefinition)
            .Where(type => reader.GetString(type.Namespace) == "ObjectLiterals")
            .ToList();

        var type = Assert.Single(objectLiteralTypes);
        Assert.Contains("_a", reader.GetString(type.Name), StringComparison.Ordinal);

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
