using JavaScriptRuntime;
using NodeFs = JavaScriptRuntime.Node.FS;
using NodePath = JavaScriptRuntime.Node.Path;
using NodeQueryString = JavaScriptRuntime.Node.QueryString;
using NodeUtil = JavaScriptRuntime.Node.Util;
using Xunit;

namespace Jroc.Tests.Node;

public class NodeUtilityObjectRepresentationTests
{
    [Fact]
    public void PathParse_ReturnsJsObject()
    {
        var path = new NodePath();

        var result = Assert.IsType<JsObject>(path.parse("directory/file.txt"));

        Assert.Equal("file.txt", ObjectRuntime.GetProperty(result, "base"));
        Assert.Equal(".txt", ObjectRuntime.GetProperty(result, "ext"));
        Assert.Equal("file", ObjectRuntime.GetProperty(result, "name"));
    }

    [Fact]
    public void QueryStringParse_ReturnsJsObject_WithOrderedRepeatedValues()
    {
        var queryString = new NodeQueryString();

        var result = Assert.IsType<JsObject>(queryString.parse("first=1&second=2&first=3"));
        var repeatedValues = Assert.IsType<JavaScriptRuntime.Array>(ObjectRuntime.GetProperty(result, "first"));

        Assert.Equal(new[] { "first", "second" }, result.GetOwnPropertyNames());
        Assert.Equal(2d, repeatedValues.length);
        Assert.Equal("1", repeatedValues[0d]);
        Assert.Equal("3", repeatedValues[1d]);
    }

    [Fact]
    public void QueryStringParse_DoesNotInternUntrustedKeys()
    {
        var queryString = new NodeQueryString();
        var key = $"query-key-{Guid.NewGuid():N}";

        Assert.Null(string.IsInterned(key));

        _ = queryString.parse($"{key}=value");

        Assert.Null(string.IsInterned(key));
    }

    [Fact]
    public void UtilTypesAndSynthesizedPrototype_AreJsObjects()
    {
        var util = new NodeUtil();
        Action child = static () => { };
        Action parent = static () => { };

        util.inherits(child, parent);

        Assert.IsType<JsObject>(util.types);
        Assert.IsType<JsObject>(ObjectRuntime.GetProperty(child, "prototype"));
    }

    [Fact]
    public void FsConstants_AreJsObjects()
    {
        var fs = new NodeFs();

        var constants = Assert.IsType<JsObject>(fs.constants);

        Assert.Equal(0d, ObjectRuntime.GetProperty(constants, "F_OK"));
    }
}
