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
                // %IteratorPrototype% already supplies [Symbol.iterator]; the spec gives this
                // prototype only `next` and its @@toStringTag.
                PrototypeChain.SetPrototype(
                    prototype,
                    Iterator.Prototype);
                DefineIteratorMethod(
                    prototype,
                    "next",
                    SegmentIteratorNext,
                    "next");
                DefineToStringTag(prototype, "Segmenter String Iterator");
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
            private const int SegmentSlot = 0;
            private const int IndexSlot = 1;
            private const int InputSlot = 2;

            private object? _segment;
            private object? _index;
            private object? _input;
            // Bit per inline slot; once deleted, the key falls back to ordinary storage so a
            // later re-add lands at the end of the key order like any fresh property.
            private byte _deletedSlots;

            public SegmentData(
                string segment,
                double index,
                string input)
            {
                _segment = segment;
                _index = index;
                _input = input;
            }

            private static int SlotOf(string key)
                => key switch
                {
                    "segment" => SegmentSlot,
                    "index" => IndexSlot,
                    "input" => InputSlot,
                    _ => -1
                };

            private static string KeyOf(int slot)
                => slot switch
                {
                    SegmentSlot => "segment",
                    IndexSlot => "index",
                    _ => "input"
                };

            private bool IsInlineSlot(int slot)
                => slot >= 0 && (_deletedSlots & (1 << slot)) == 0;

            private ref object? SlotRef(int slot)
            {
                switch (slot)
                {
                    case SegmentSlot:
                        return ref _segment;
                    case IndexSlot:
                        return ref _index;
                    default:
                        return ref _input;
                }
            }

            internal override bool TryGetOwnPropertyValue(
                string key,
                out object? value)
            {
                var slot = SlotOf(key);
                if (IsInlineSlot(slot))
                {
                    value = SlotRef(slot);
                    return true;
                }

                return base.TryGetOwnPropertyValue(key, out value);
            }

            internal override bool HasOwnPropertyValue(string key)
                => IsInlineSlot(SlotOf(key)) || base.HasOwnPropertyValue(key);

            internal override bool SetOwnPropertyValue(
                string key,
                object? value)
            {
                var slot = SlotOf(key);
                if (IsInlineSlot(slot))
                {
                    SlotRef(slot) = value;
                    return true;
                }

                return base.SetOwnPropertyValue(key, value);
            }

            internal override bool DeleteOwnProperty(string key)
            {
                var slot = SlotOf(key);
                if (IsInlineSlot(slot))
                {
                    _deletedSlots |= (byte)(1 << slot);
                    SlotRef(slot) = null;
                    return true;
                }

                return base.DeleteOwnProperty(key);
            }

            internal override IEnumerable<string> GetOwnPropertyKeys()
            {
                var seen = new HashSet<string>(StringComparer.Ordinal);
                for (var slot = SegmentSlot; slot <= InputSlot; slot++)
                {
                    if (IsInlineSlot(slot))
                    {
                        var key = KeyOf(slot);
                        seen.Add(key);
                        yield return key;
                    }
                }

                foreach (var key in base.GetOwnPropertyKeys())
                {
                    if (seen.Add(key))
                    {
                        yield return key;
                    }
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

    private static void DefineToStringTag(object target, string tag)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
        PropertyDescriptorStore.DefineOrUpdate(
            target,
            Symbol.toStringTag.DebugId,
            new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = tag
            });
    }
}
