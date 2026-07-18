using System.Collections.Generic;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class ObjectRuntimeOrdinaryObjectTests
{
    private sealed class ExoticObjectForTest : JsObject, IExoticJsObject
    {
        private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

        public bool RejectDefinitions { get; set; }

        public bool RejectDeletions { get; set; }

        public int PropertyReadCount { get; private set; }

        public void Seed(string key, object? value)
            => _values[key] = value;

        internal override bool TryGetBoxedValue(
            string key,
            object receiverForAccessors,
            out object? value)
        {
            PropertyReadCount++;
            return base.TryGetBoxedValue(key, receiverForAccessors, out value);
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
            => _values.TryGetValue(key, out value);

        internal override bool SetOwnPropertyValue(string key, object? value)
        {
            _values[key] = value;
            return true;
        }

        internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
            => !RejectDefinitions && base.DefineOwnProperty(key, descriptor);

        internal override bool DeleteOwnProperty(string key)
        {
            if (RejectDeletions)
            {
                return false;
            }

            _values.Remove(key);
            PropertyDescriptorStore.Delete(this, key);
            return true;
        }

        internal override IEnumerable<string> GetOwnPropertyKeys()
        {
            foreach (var key in _values.Keys)
            {
                yield return key;
            }

            foreach (var key in PropertyDescriptorStore.GetOwnKeys(this))
            {
                if (!_values.ContainsKey(key))
                {
                    yield return key;
                }
            }
        }
    }

    [Fact]
    public void CoreDispatch_PreservesJsObjectBehavior()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
            var prototype = new JsObject();

            ObjectRuntime.SetProperty(target, "second", 2d);
            ObjectRuntime.SetProperty(target, "1", "index");
            ObjectRuntime.SetProperty(target, "first", 1d);
            ObjectRuntime.SetProperty(prototype, "inherited", 3d);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal(2d, ObjectRuntime.GetProperty(target, "second"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "inherited"));
            Assert.True(JavaScriptRuntime.Object.hasOwn(target, "first"));
            Assert.True(Operators.In("inherited", target));

            Assert.Equal(
                new object?[] { "1", "second", "first" },
                Assert.IsType<JavaScriptRuntime.Array>(JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            PropertyDescriptorStore.DefineOrUpdate(target, "descriptorOnly", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 4d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
            Assert.Equal(4d, ObjectRuntime.GetProperty(target, "descriptorOnly"));

            Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "second"));

            JavaScriptRuntime.Object.freeze(target);
            Assert.True(JavaScriptRuntime.Object.isFrozen(target));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(target, "first", 4d));

            var rejectingTarget = new ExoticObjectForTest { RejectDefinitions = true };
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(rejectingTarget, "rejected", 1d));
            Assert.Equal(
                1d,
                ObjectRuntime.SetProperty(rejectingTarget, "rejected", 1d, throwOnError: false));
            Assert.False(JavaScriptRuntime.Object.hasOwn(rejectingTarget, "rejected"));

            var deleteRejectingTarget = new ExoticObjectForTest();
            ObjectRuntime.SetProperty(deleteRejectingTarget, "retained", 1d);
            deleteRejectingTarget.RejectDeletions = true;
            Assert.Throws<TypeError>(() =>
                ObjectRuntime.DeleteProperty(deleteRejectingTarget, "retained"));
            Assert.False(ObjectRuntime.DeletePropertyNonStrict(deleteRejectingTarget, "retained"));
            Assert.True(JavaScriptRuntime.Object.hasOwn(deleteRejectingTarget, "retained"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void PropertyReadContract_PreservesInheritedAccessorReceiver()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var target = new JsObject();
            var prototype = new JsObject();

            ObjectRuntime.SetProperty(target, "marker", "target");
            Func<object[], object?[]?, object?> getter = static (_, _) =>
                ObjectRuntime.GetProperty(RuntimeServices.GetCurrentThis()!, "marker");
            ObjectRuntime.DefineObjectLiteralAccessorProperty(
                prototype,
                "computed",
                getter,
                null);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal("target", ObjectRuntime.GetProperty(target, "computed"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void ProxyFallback_PreservesJsObjectTargetSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
            var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());

            ObjectRuntime.SetProperty(proxy, "value", 42d);

            Assert.Equal(42d, ObjectRuntime.GetProperty(proxy, "value"));
            Assert.True(Operators.In("value", proxy));
            Assert.True(ObjectRuntime.DeleteProperty(proxy, "value"));
            Assert.False(Operators.In("value", proxy));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void DeletedIntrinsicProperty_IsMaskedAcrossCoreDispatch()
    {
        object target = new JsObject();
        ObjectRuntime.TrySetOwnValue(target, "hidden", 42d);
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "hidden", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 42d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            Assert.True(ObjectRuntime.DeleteProperty(target, "hidden"));
            Assert.Null(ObjectRuntime.GetProperty(target, "hidden"));
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, "hidden"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "hidden"));
            Assert.False(Operators.In("hidden", target));
            Assert.Empty(Assert.IsType<JavaScriptRuntime.Array>(
                JavaScriptRuntime.Object.getOwnPropertyNames(target)));
            Assert.True(JavaScriptRuntime.Object.CreateForInIterator(target).Next().done);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeCreatedOrdinaryRecords_AreJsObjects()
    {
        var moduleId = $"module-{Guid.NewGuid():N}.js";
        var importMeta = Assert.IsType<JsObject>(RuntimeServices.GetImportMeta(moduleId));
        var revocable = Assert.IsType<JsObject>(
            JavaScriptRuntime.Proxy.revocable(new JsObject(), new JsObject()));

        Assert.Same(importMeta, RuntimeServices.GetImportMeta(moduleId));
        Assert.IsType<string>(ObjectRuntime.GetProperty(importMeta, "url"));
        Assert.IsType<JavaScriptRuntime.Proxy>(ObjectRuntime.GetProperty(revocable, "proxy"));
        Assert.IsAssignableFrom<Delegate>(ObjectRuntime.GetProperty(revocable, "revoke"));
    }

    [Fact]
    public void ExternalDictionary_RemainsAHostObject()
    {
        var hostObject = new Dictionary<string, object?>();

        ObjectRuntime.SetProperty(hostObject, "value", 42d);

        Assert.Equal(42d, ObjectRuntime.GetProperty(hostObject, "value"));
        Assert.True(JavaScriptRuntime.Object.hasOwn(hostObject, "value"));
        Assert.Equal(42d, hostObject["value"]);
    }

    [Fact]
    public void ExoticSubclass_UsesInternalOperationsAcrossGenericObjectSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var target = new ExoticObjectForTest();
            var prototype = new JsObject();

            target.Seed("seed", 0d);
            ObjectRuntime.SetProperty(target, "second", 2d);
            ObjectRuntime.SetProperty(target, "1", "index");
            ObjectRuntime.SetProperty(target, "first", 1d);
            ObjectRuntime.SetProperty(prototype, "inherited", 3d);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal(2d, ObjectRuntime.GetProperty(target, "second"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "inherited"));
            Assert.True(target.PropertyReadCount >= 2);
            Assert.True(JavaScriptRuntime.Object.hasOwn(target, "first"));
            Assert.True(Operators.In("inherited", target));
            Assert.Equal(
                new object?[] { "1", "seed", "second", "first" },
                Assert.IsType<JavaScriptRuntime.Array>(
                    JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            var descriptor = Assert.IsType<JsObject>(
                JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, "first"));
            Assert.Equal(1d, ObjectRuntime.GetProperty(descriptor, "value"));

            Func<object[], object?[]?, object?> getter = static (_, _) => "accessor";
            ObjectRuntime.DefineObjectLiteralAccessorProperty(
                target,
                "computed",
                getter,
                null);
            Assert.Equal("accessor", ObjectRuntime.GetProperty(target, "computed"));

            ObjectRuntime.DefineObjectLiteralDataProperty(target, "helper", 5d);
            JavaScriptRuntime.Object.SetPropertyNumber(target, "typed", 6d);
            Assert.Equal(5d, ObjectRuntime.GetProperty(target, "helper"));
            Assert.Equal(6d, ObjectRuntime.GetProperty(target, "typed"));

            PropertyDescriptorStore.DefineOrUpdate(target, "descriptorOnly", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 7d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
            Assert.Contains(
                "descriptorOnly",
                Assert.IsType<JavaScriptRuntime.Array>(
                    JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());
            Assert.Equal(1d, ObjectRuntime.GetProperty(proxy, "first"));

            Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "second"));

            JavaScriptRuntime.Object.freeze(target);
            Assert.True(JavaScriptRuntime.Object.isFrozen(target));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(target, "first", 4d));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Theory]
    [InlineData("0", true, 0u)]
    [InlineData("2147483648", true, 2147483648u)]
    [InlineData("4294967294", true, 4294967294u)]
    [InlineData("4294967295", false, 0u)]
    [InlineData("01", false, 0u)]
    [InlineData("-0", false, 0u)]
    [InlineData("1.0", false, 0u)]
    public void CanonicalArrayIndex_UsesEcma262PropertyKeyRange(
        string key,
        bool expected,
        uint expectedIndex)
    {
        Assert.Equal(expected, CanonicalArrayIndex.TryParse(key, out var index));
        Assert.Equal(expectedIndex, index);
    }
}
