using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class JsShapeTransitionTests
{
    [Fact]
    public void NewShape_DoesNotAllocateTransitionCache()
    {
        var shape = new JsShape();

        Assert.False(shape.HasTransitionCache);
        Assert.Equal(0, shape.TransitionCacheCount);
    }

    [Fact]
    public void TransitionTo_AllocatesTransitionCacheOnlyOnParentAndReusesLiveChild()
    {
        var parent = new JsShape();

        var first = parent.TransitionTo("value");
        var second = parent.TransitionTo("value");

        Assert.Same(first, second);
        Assert.True(parent.HasTransitionCache);
        Assert.Equal(1, parent.TransitionCacheCount);
        Assert.False(first.HasTransitionCache);
        Assert.Equal(0, first.TransitionCacheCount);
    }

    [Fact]
    public void TransitionToUncached_DoesNotAllocateTransitionCache()
    {
        var parent = new JsShape();

        var child = parent.TransitionToUncached("value");

        Assert.False(parent.HasTransitionCache);
        Assert.Equal(0, parent.TransitionCacheCount);
        Assert.False(child.HasTransitionCache);
        Assert.Equal(0, child.TransitionCacheCount);
    }
}
