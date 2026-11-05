using System.Numerics;
using JavaScriptRuntime;

namespace Jroc.Tests;

// Share the existing nonparallel collection: prototype edits must not invalidate
// a dense-array cache in the middle of another test's allocation measurement.
[Collection(ArrayReceiverSpecializationRuntimeTestsCollection.CollectionName)]
public sealed class TaggedNumericRuntimeTests
{
    [Theory]
    [InlineData("string")]
    [InlineData("object")]
    [InlineData("numeric-string")]
    [InlineData("number")]
    [InlineData("array")]
    [InlineData("array-runtime")]
    public void TaggedNumericReads_DoNotBoxWarmedNumericStorage(string path)
    {
        using var scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        var target = new JsObject();
        target.SetNumber("value", 7.25d);
        target.SetNumber("0", 7.25d);
        var array = new JavaScriptRuntime.Array(32);
        array.AddNumber(7.25d);
        object key = "value";
        for (var index = 0; index < 1_024; index++)
        {
            _ = Read().AsNumber;
            if (path == "number")
                _ = DotNet2JSConversions.ToString(0d);
        }

        long keyConversionAllocated = 0;
        if (path == "number")
        {
            var keyLength = 0;
            var beforeKeys = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < 10_000; index++)
                keyLength += DotNet2JSConversions.ToString(0d).Length;
            keyConversionAllocated = GC.GetAllocatedBytesForCurrentThread() - beforeKeys;
            Assert.Equal(10_000, keyLength);
        }

        var sum = 0d;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < 10_000; index++)
            sum += Read().AsNumber;
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(72_500d, sum);
        // A double key incurs canonical key conversion. Permit that measured
        // cost, but reject an additional numeric payload box on every read.
        if (path == "number")
            Assert.InRange(allocated, 0L, keyConversionAllocated + 1_024L);
        else
            Assert.Equal(0L, allocated);

        JsValue Read() => path switch
        {
            "string" => ObjectRuntime.GetItemValue(target, "value"),
            "object" => ObjectRuntime.GetItemValue(target, key),
            "numeric-string" => ObjectRuntime.GetItemValue(target, "0"),
            "number" => ObjectRuntime.GetItemValue(target, 0d),
            "array" => array.GetItemValue(0d),
            "array-runtime" => ObjectRuntime.GetItemValue(array, 0d),
            _ => throw new InvalidOperationException(path)
        };
    }

    [Fact]
    public void TaggedNumericKeys_PreserveCanonicalPropertyNames()
    {
        using var scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        var target = new JsObject();
        target.SetObject("0", "zero");
        target.SetObject("1.5", "fraction");
        target.SetObject("-1", "negative");
        target.SetObject("NaN", "nan");
        target.SetObject("Infinity", "infinity");
        Assert.Equal("zero", JsValue.ToObject(ObjectRuntime.GetItemValue(target, 0d)));
        Assert.Equal("zero", JsValue.ToObject(ObjectRuntime.GetItemValue(target, -0d)));
        Assert.Equal("fraction", JsValue.ToObject(ObjectRuntime.GetItemValue(target, 1.5d)));
        Assert.Equal("negative", JsValue.ToObject(ObjectRuntime.GetItemValue(target, -1d)));
        Assert.Equal("nan", JsValue.ToObject(ObjectRuntime.GetItemValue(target, double.NaN)));
        Assert.Equal("infinity", JsValue.ToObject(ObjectRuntime.GetItemValue(target, double.PositiveInfinity)));

        var boxedString = new JsObject();
        boxedString.SetString(JavaScriptRuntime.String.StringDataPropertyName, "12");
        boxedString.SetNumber("0", 99d);
        boxedString.SetNumber("01", 42d);
        Assert.Equal("1", JsValue.ToObject(ObjectRuntime.GetItemValue(boxedString, "0")));
        Assert.Equal("1", JsValue.ToObject(ObjectRuntime.GetItemValue(boxedString, 0d)));
        Assert.Equal(42d, ObjectRuntime.GetItemValue(boxedString, "01").AsNumber);
    }

    [Fact]
    public void TaggedOwnReads_RetainTypesIdentityAndNumberBits()
    {
        var target = new JsObject();
        var reference = new JsObject();
        var symbol = new Symbol("tagged");
        object?[] values = ["12", true, false, JsNull.Null, null, reference, symbol, new BigInteger(7)];
        foreach (var value in values)
        {
            target.SetObject("value", value);
            var tagged = ObjectRuntime.GetItemValue(target, "value");
            var raw = JsValue.ToObject(tagged);
            if (value is bool boolean)
                Assert.Equal(boolean, Assert.IsType<bool>(raw));
            else if (value is JsNull)
                Assert.Equal(JsNull.Null, Assert.IsType<JsNull>(raw));
            else
                Assert.Same(value, raw);
        }

        foreach (var number in new[] { 0d, -0d, double.NaN, double.PositiveInfinity, -12.75d })
        {
            target.SetNumber("value", number);
            var tagged = ObjectRuntime.GetItemValue(target, "value");
            Assert.True(tagged.IsNumber);
            Assert.Equal(BitConverter.DoubleToInt64Bits(number), BitConverter.DoubleToInt64Bits(tagged.AsNumber));
            Assert.Equal(BitConverter.DoubleToInt64Bits(number),
                BitConverter.DoubleToInt64Bits(Assert.IsType<double>(JsValue.ToObject(tagged))));
        }
        Assert.True(ObjectRuntime.GetItemValue(target, "missing").IsUndefined);
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemValue(null!, "value"));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemValue(JsNull.Null, "value"));
    }

    [Fact]
    public void TaggedAccessorRead_DefersCoercionAndRetainsReceiver()
    {
        using var scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        var target = new JsObject();
        var prototype = new JsObject();
        var value = new JsObject();
        var events = new List<string>();
        object? receiver = null;
        ObjectRuntime.SetProperty(value, "valueOf",
            new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
            {
                events.Add("convert");
                return 17.5d;
            })));
        ObjectRuntime.DefineObjectLiteralAccessorProperty(prototype, "value",
            (BuiltinFunction0)(self =>
            {
                receiver = self;
                events.Add("get");
                return value;
            }), null);
        JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

        var tagged = ObjectRuntime.GetItemValue(target, "value");
        Assert.Same(value, JsValue.ToObject(tagged));
        Assert.Same(target, receiver);
        Assert.Equal(new[] { "get" }, events);
        Assert.Equal(17.5d, JsValue.ToNumber(tagged));
        Assert.Equal(new[] { "get", "convert" }, events);
    }

    [Fact]
    public void TaggedArrayReads_PreserveHolesAccessorsInheritedAndNonIndexValues()
    {
        using var scope = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider()).EnterAsRoot();
        var array = new JavaScriptRuntime.Array(32);
        array.AddNumber(3d);
        array.AddNumber(99d);
        array.AddNumber(5d);
        Assert.Equal(3d, array.GetItemValue(-0d).AsNumber);
        Assert.True(array.GetItemValue(3d).IsUndefined);
        Assert.True(array.GetItemValue(-1d).IsUndefined);
        Assert.True(array.GetItemValue(double.NaN).IsUndefined);
        Assert.True(array.GetItemValue(double.PositiveInfinity).IsUndefined);
        var value = new JsObject();
        var reads = 0;
        object? receiver = null;
        ObjectRuntime.DefineObjectLiteralAccessorProperty(array, "1",
            (BuiltinFunction0)(self =>
            {
                receiver = self;
                reads++;
                return value;
            }), null);
        Assert.Same(value, JsValue.ToObject(array.GetItemValue(1d)));
        Assert.Same(array, receiver);
        Assert.Equal(1, reads);
        Assert.True(ObjectRuntime.DeleteProperty(array, "2"));
        Assert.True(array.GetItemValue(2d).IsUndefined);
        ObjectRuntime.SetProperty(JavaScriptRuntime.Array.Prototype, "2", "inherited");
        Assert.Equal("inherited", JsValue.ToObject(array.GetItemValue(2d)));
        ObjectRuntime.SetProperty(array, "1.5", "fraction");
        Assert.Equal("fraction", JsValue.ToObject(array.GetItemValue(1.5d)));
    }

    [Fact]
    public void TaggedToNumber_PreservesFallbackConversionsAndErrors()
    {
        Assert.Equal(12.5d, JsValue.ToNumber(JsValue.FromString("12.5")));
        Assert.Equal(1d, JsValue.ToNumber(JsValue.FromBoolean(true)));
        Assert.Equal(0d, JsValue.ToNumber(JsValue.FromBoolean(false)));
        Assert.Equal(0d, JsValue.ToNumber(JsValue.Null));
        Assert.True(double.IsNaN(JsValue.ToNumber(JsValue.Undefined)));
        Assert.Throws<TypeError>(() => JsValue.ToNumber(JsValue.FromObject(new BigInteger(7))));
        Assert.Throws<TypeError>(() => JsValue.ToNumber(JsValue.FromObject(new Symbol("number"))));
    }

    [Fact]
    public void TaggedExoticRead_UsesOriginalLookupOnceInsteadOfOwnNumericSlot()
    {
        var target = new CountingExoticObject();
        target.SetNumber("value", 99d);
        var tagged = ObjectRuntime.GetItemValue(target, "value");
        Assert.Equal("12", JsValue.ToObject(tagged));
        Assert.Equal(1, target.Reads);
    }

    [Theory]
    [InlineData("object")]
    [InlineData("array")]
    public void TaggedSharedIntrinsicPrototypeReads_RespectRealmOverrideAndDeletion(string kind)
    {
        JsObject prototype = kind == "array" ? new JavaScriptRuntime.Array(32) : new JsObject();
        var key = kind == "array" ? "0" : "value";
        if (prototype is JavaScriptRuntime.Array array)
            array.AddNumber(7d);
        else
            prototype.SetNumber(key, 7d);
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(prototype, key, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 7d,
                Writable = true,
                Enumerable = false,
                Configurable = true
            });
        }

        var firstRealm = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        var secondRealm = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        using (firstRealm.EnterAsRoot())
        {
            Assert.Equal(7d, Read().AsNumber);
            ObjectRuntime.SetPropertyNumber(prototype, key, 11d, true);
            Assert.Equal(11d, Read().AsNumber);
            var instance = new JsObject();
            JavaScriptRuntime.Object.setPrototypeOf(instance, prototype);
            Assert.Equal(11d, ObjectRuntime.GetItemValue(instance, key).AsNumber);
            Assert.True(ObjectRuntime.DeleteProperty(prototype, key));
            Assert.True(Read().IsUndefined);
            Assert.True(ObjectRuntime.GetItemValue(instance, key).IsUndefined);
        }
        using (secondRealm.EnterAsRoot())
        {
            Assert.Equal(7d, Read().AsNumber);
            ObjectRuntime.SetPropertyNumber(prototype, key, 13d, true);
            Assert.Equal(13d, Read().AsNumber);
        }
        using (firstRealm.EnterAsRoot())
            Assert.True(Read().IsUndefined);
        using (secondRealm.EnterAsRoot())
            Assert.Equal(13d, Read().AsNumber);

        JsValue Read() => prototype is JavaScriptRuntime.Array dense
            ? dense.GetItemValue(0d)
            : ObjectRuntime.GetItemValue(prototype, key);
    }

    private sealed class CountingExoticObject : JsObject, IExoticJsObject
    {
        public int Reads { get; private set; }

        internal override bool TryGetBoxedValue(string key, object receiverForAccessors, out object? value)
        {
            Reads++;
            return base.TryGetBoxedValue(key, receiverForAccessors, out value);
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
        {
            value = key == "value" ? "12" : null;
            return key == "value";
        }
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    [InlineData("zero")]
    [InlineData("nan-left")]
    [InlineData("nan-right")]
    [InlineData("strings")]
    [InlineData("numeric-string")]
    [InlineData("undefined")]
    [InlineData("null")]
    [InlineData("big-above")]
    [InlineData("big-equal")]
    [InlineData("big-below")]
    public void TaggedRelationalOverloads_PreserveExactOrdering(string label)
    {
        const double boundary = 9_007_199_254_740_992d;
        (object? left, object? right, (bool, bool, bool, bool) expected) = label switch
        {
            "ascending" => ((object?)2d, (object?)3d, (true, false, true, false)),
            "descending" => (3d, 2d, (false, true, false, true)),
            "zero" => (-0d, 0d, (false, false, true, true)),
            "nan-left" => (double.NaN, 1d, (false, false, false, false)),
            "nan-right" => (1d, double.NaN, (false, false, false, false)),
            "strings" => ("10", "2", (true, false, true, false)),
            "numeric-string" => ("10", 2d, (false, true, false, true)),
            "undefined" => (null, 0d, (false, false, false, false)),
            "null" => (JsNull.Null, 0d, (false, false, true, true)),
            "big-above" => (new BigInteger(boundary) + 1, boundary, (false, true, false, true)),
            "big-equal" => (new BigInteger(boundary), boundary, (false, false, true, true)),
            "big-below" => (new BigInteger(boundary) - 1, boundary, (true, false, true, false)),
            _ => throw new InvalidOperationException(label)
        };
        var a = JsValue.FromObject(left);
        var b = JsValue.FromObject(right);
        Assert.Equal(expected, (Operators.LessThan(a, b), Operators.GreaterThan(a, b),
            Operators.LessThanOrEqual(a, b), Operators.GreaterThanOrEqual(a, b)));
        // Keep the original object fallback covered independently, not as the oracle.
        Assert.Equal(expected, (Operators.LessThan(left, right), Operators.GreaterThan(left, right),
            Operators.LessThanOrEqual(left, right), Operators.GreaterThanOrEqual(left, right)));
        if (right is double numberRight)
        {
            Assert.Equal(expected, (Operators.LessThan(a, numberRight), Operators.GreaterThan(a, numberRight),
                Operators.LessThanOrEqual(a, numberRight), Operators.GreaterThanOrEqual(a, numberRight)));
            var reversed = (expected.Item2, expected.Item1, expected.Item4, expected.Item3);
            Assert.Equal(reversed, (Operators.LessThan(numberRight, a), Operators.GreaterThan(numberRight, a),
                Operators.LessThanOrEqual(numberRight, a), Operators.GreaterThanOrEqual(numberRight, a)));
        }
        if (left is double numberLeft)
            Assert.Equal(expected, (Operators.LessThan(numberLeft, b), Operators.GreaterThan(numberLeft, b),
                Operators.LessThanOrEqual(numberLeft, b), Operators.GreaterThanOrEqual(numberLeft, b)));
    }
}
