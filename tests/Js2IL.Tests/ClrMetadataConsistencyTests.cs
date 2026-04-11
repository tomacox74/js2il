using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests;

public sealed class ClrMetadataConsistencyTests
{
    [Fact]
    public void NestedTypeRelationshipRegistry_ShouldThrow_WhenEnclosingAfterNested()
    {
        var registry = new NestedTypeRelationshipRegistry();

        // Simulate a nested type created earlier than its enclosing type.
        // TypeDef token encoding: 0x0200 + RID.
        var nested = MetadataTokens.TypeDefinitionHandle(0x00C8);
        var enclosing = MetadataTokens.TypeDefinitionHandle(0x00CB);

        var ex = Assert.Throws<InvalidOperationException>(() => registry.Add(nested, enclosing));
        Assert.Contains("enclosing TypeDef must be declared before nested", ex.Message);
        Assert.Contains("0x020000C8", ex.Message);
        Assert.Contains("0x020000CB", ex.Message);
    }

    [Fact]
    public void FormatTypeLoadFailureMessage_ShouldIncludeLoaderExceptionMessages()
    {
        var ex = new ReflectionTypeLoadException(
            System.Array.Empty<Type>(),
            new Exception[]
            {
                new FileNotFoundException("Missing JavaScriptRuntime.dll"),
                new BadImageFormatException("Broken dependency image"),
                new FileNotFoundException("Missing JavaScriptRuntime.dll")
            });

        var message = ClrMetadataConsistencyValidator.FormatTypeLoadFailureMessage(ex);

        Assert.Contains("Loader exceptions:", message);
        Assert.Contains("Missing JavaScriptRuntime.dll", message);
        Assert.Contains("Broken dependency image", message);
    }
}
