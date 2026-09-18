using System.Globalization;

namespace JavaScriptRuntime;

public sealed class IntlNumberFormat
{
    public string format(object? value)
    {
        var number = TypeUtilities.ToNumber(value);

        if (double.IsNaN(number))
        {
            return "NaN";
        }

        if (double.IsPositiveInfinity(number))
        {
            return "Infinity";
        }

        if (double.IsNegativeInfinity(number))
        {
            return "-Infinity";
        }

        if (number == System.Math.Truncate(number))
        {
            return number.ToString("#,0", CultureInfo.InvariantCulture);
        }

        return number.ToString("#,0.###", CultureInfo.InvariantCulture);
    }
}

public sealed class IntlSegmenter
{
    private static readonly BuiltinFunction0 SegmentsIteratorFactory =
        CreateSegmentsIterator;
    private static readonly BuiltinFunction0 SegmentIteratorNext =
        InvokeSegmentIteratorNext;
    private static readonly BuiltinFunction0 SegmentIteratorSelf =
        ReturnSegmentIterator;

    private static JsObject SegmentsPrototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.IntlSegmentsPrototype,
            static () => new JsObject(),
            static prototype =>
            {
                PrototypeChain.SetPrototype(
                    prototype,
                    GlobalThis.ObjectPrototypeValue);
                DefineIteratorMethod(
                    prototype,
                    Symbol.iterator.DebugId,
                    SegmentsIteratorFactory,
                    "[Symbol.iterator]");
            });

    private static JsObject SegmentIteratorPrototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.IntlSegmentIteratorPrototype,
            static () => new JsObject(),
            static prototype =>
            {
                PrototypeChain.SetPrototype(
                    prototype,
                    Iterator.Prototype);
                DefineIteratorMethod(
                    prototype,
                    "next",
                    SegmentIteratorNext,
                    "next");
                DefineIteratorMethod(
                    prototype,
                    Symbol.iterator.DebugId,
                    SegmentIteratorSelf,
                    "[Symbol.iterator]");
            });

    public Segments segment(object? input)
    {
        return new Segments(DotNet2JSConversions.ToString(input) ?? string.Empty);
    }

    public sealed class Segments : JsObject
    {
        private readonly string _input;

        internal Segments(string input)
        {
            _input = input;
            PrototypeChain.InitializePrototype(this, SegmentsPrototype);
        }

        internal string Input => _input;

        internal sealed class SegmentIterator : JsObject, IJavaScriptIterator
        {
            private readonly string _input;
            private readonly TextElementEnumerator _enumerator;
            private bool _isClosed;

            internal SegmentIterator(string input)
            {
                _input = input;
                _enumerator = StringInfo.GetTextElementEnumerator(input);
                PrototypeChain.InitializePrototype(
                    this,
                    SegmentIteratorPrototype);
            }

            public bool HasReturn => false;

            public IteratorResultObject Next()
            {
                if (_isClosed || !_enumerator.MoveNext())
                {
                    _isClosed = true;
                    return new IteratorResultObject(null, done: true);
                }

                var data = new SegmentData(
                    _enumerator.GetTextElement(),
                    _enumerator.ElementIndex,
                    _input);
                return new IteratorResultObject(data, done: false);
            }

            public void Return()
            {
                _isClosed = true;
            }
        }

        private sealed class SegmentData : JsObject, IExoticJsObject
        {
            private object? _segment;
            private object? _index;
            private object? _input;

            public SegmentData(
                string segment,
                double index,
                string input)
            {
                _segment = segment;
                _index = index;
                _input = input;
            }

            internal override bool TryGetOwnPropertyValue(
                string key,
                out object? value)
            {
                value = key switch
                {
                    "segment" => _segment,
                    "index" => _index,
                    "input" => _input,
                    _ => null
                };
                return key is "segment" or "index" or "input";
            }

            internal override bool HasOwnPropertyValue(string key)
                => key is "segment" or "index" or "input"
                    && !PropertyDescriptorStore.IsDeleted(this, key);

            internal override bool SetOwnPropertyValue(
                string key,
                object? value)
            {
                switch (key)
                {
                    case "segment":
                        _segment = value;
                        return true;
                    case "index":
                        _index = value;
                        return true;
                    case "input":
                        _input = value;
                        return true;
                    default:
                        return base.SetOwnPropertyValue(key, value);
                }
            }

            internal override IEnumerable<string> GetOwnPropertyKeys()
            {
                var seen = new HashSet<string>(StringComparer.Ordinal);
                foreach (var key in base.GetOwnPropertyKeys())
                {
                    if (seen.Add(key))
                    {
                        yield return key;
                    }
                }

                if (seen.Add("segment")
                    && !PropertyDescriptorStore.IsDeleted(this, "segment"))
                {
                    yield return "segment";
                }
                if (seen.Add("index")
                    && !PropertyDescriptorStore.IsDeleted(this, "index"))
                {
                    yield return "index";
                }
                if (seen.Add("input")
                    && !PropertyDescriptorStore.IsDeleted(this, "input"))
                {
                    yield return "input";
                }
            }
        }
    }

    private static object CreateSegmentsIterator(object? thisArgument)
    {
        if (thisArgument is not Segments segments)
        {
            throw new TypeError(
                "Intl.Segmenter Segments iterator called on incompatible receiver");
        }

        return new Segments.SegmentIterator(segments.Input);
    }

    private static object InvokeSegmentIteratorNext(object? thisArgument)
    {
        if (thisArgument is not Segments.SegmentIterator iterator)
        {
            throw new TypeError(
                "Intl.Segmenter iterator next called on incompatible receiver");
        }

        return iterator.Next();
    }

    private static object ReturnSegmentIterator(object? thisArgument)
    {
        if (thisArgument is not Segments.SegmentIterator)
        {
            throw new TypeError(
                "Intl.Segmenter iterator called on incompatible receiver");
        }

        return thisArgument;
    }

    private static void DefineIteratorMethod(
        object target,
        string key,
        Delegate value,
        string name)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
        Function.InitializeFunctionInstance(value, 0d, name);
        PropertyDescriptorStore.DefineOrUpdate(
            target,
            key,
            new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
    }
}
