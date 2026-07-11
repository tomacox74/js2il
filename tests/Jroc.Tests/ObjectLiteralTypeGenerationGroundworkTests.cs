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

        // Object literal types are nested: Modules.<Module> -> ObjectLiterals -> L{line}C{col}_{binding}.
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
                return reader.GetString(type.Name).StartsWith("L", StringComparison.Ordinal)
                    && type.GetDeclaringType() == containerHandle;
            })
            .ToList();

        var typeHandle = Assert.Single(objectLiteralTypeHandles);
        var type = reader.GetTypeDefinition(typeHandle);
        Assert.Equal("L1C11_a", reader.GetString(type.Name));
        Assert.Equal(containerHandle, type.GetDeclaringType());

        Assert.Equal(HandleKind.TypeReference, type.BaseType.Kind);
        var baseTypeRef = reader.GetTypeReference(MetadataTokens.TypeReferenceHandle(MetadataTokens.GetRowNumber(type.BaseType)));
        Assert.Equal("JavaScriptRuntime", reader.GetString(baseTypeRef.Namespace));
        Assert.Equal("JsObject", reader.GetString(baseTypeRef.Name));
        Assert.Contains(type.GetMethods().Select(reader.GetMethodDefinition), method =>
            reader.GetString(method.Name) == ".ctor"
            && (method.Attributes & MethodAttributes.Public) == MethodAttributes.Public);

        // Stable members are exposed via private backing fields plus public get_/set_ accessors.
        var fields = type.GetFields()
            .Select(reader.GetFieldDefinition)
            .ToArray();

        Assert.Equal(new[] { "_b", "_n", "_flag" }, fields.Select(field => reader.GetString(field.Name)).ToArray());
        Assert.All(fields, field =>
            Assert.Equal(FieldAttributes.Private, field.Attributes & FieldAttributes.FieldAccessMask));

        var methods = type.GetMethods()
            .Select(reader.GetMethodDefinition)
            .ToArray();

        foreach (var member in new[] { "b", "n", "flag" })
        {
            Assert.Contains(methods, method =>
                reader.GetString(method.Name) == $"get_{member}"
                && (method.Attributes & MethodAttributes.Public) == MethodAttributes.Public);
            Assert.Contains(methods, method =>
                reader.GetString(method.Name) == $"set_{member}"
                && (method.Attributes & MethodAttributes.Public) == MethodAttributes.Public);
        }

        // ctor + getter/setter per member
        Assert.Equal(1 + 2 * 3, methods.Length);
    }

    [Fact]
    public void GeneratedAssembly_SharesOneTypeForReorderedSameShapeLiterals()
    {
        // Two literals with the same member names/types in a different source order canonicalize to
        // one structural signature and must therefore share a single generated CLR type
        // (issue #1434 phase 6 canonicalization), while each literal keeps its own source order.
        var entryPath = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "ObjectLiteralTypeGenerationCanonicalization",
            Guid.NewGuid().ToString("N"),
            "entry.js");

        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entryPath)
        {
            SourceText = """
                const a = { x: 1, y: 'left' };
                const b = { y: 'right', x: 2 };
                console.log(a.x, a.y, b.x, b.y);
                """
        });

        using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
        using var peReader = new PEReader(peStream);
        var reader = peReader.GetMetadataReader();

        var containerHandle = Assert.Single(
            reader.TypeDefinitions,
            handle => reader.GetString(reader.GetTypeDefinition(handle).Name) == "ObjectLiterals");

        var objectLiteralTypeHandles = reader.TypeDefinitions
            .Where(handle =>
            {
                var type = reader.GetTypeDefinition(handle);
                return reader.GetString(type.Name).StartsWith("L", StringComparison.Ordinal)
                    && type.GetDeclaringType() == containerHandle;
            })
            .ToList();

        // Both reordered literals join onto one representative type.
        var typeHandle = Assert.Single(objectLiteralTypeHandles);
        var type = reader.GetTypeDefinition(typeHandle);

        // The representative (first literal, `a`) drives the field layout.
        var fields = type.GetFields()
            .Select(reader.GetFieldDefinition)
            .Select(field => reader.GetString(field.Name))
            .ToArray();
        Assert.Equal(new[] { "_x", "_y" }, fields);
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
