using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    // Minimal typed array implementation to support tests and basic semantics.
    // Note: This is not a full ECMAScript TypedArray, but provides enough for length, construction,
    // reading via index, and set(source[, offset]).
    [IntrinsicObject("Int32Array")]
    public class Int32Array
    {
        private readonly int[] _buffer;

        public Int32Array()
        {
            _buffer = System.Array.Empty<int>();
        }

        // Single dynamic argument ctor used by codegen for `new Int32Array(x)` where x can be a number or array-like
        public Int32Array(object? arg)
        {
            // undefined/null => length 0
            if (arg is null || arg is JsNull)
            {
                _buffer = System.Array.Empty<int>();
                return;
            }

            // number => allocate with given length (ToInt32 semantics simplified)
            if (TryToNumber(arg, out var num))
            {
                int len = ToLength(num);
                if (len < 0) len = 0;
                _buffer = new int[len];
                return;
            }

            // JavaScriptRuntime.Array => copy elements with ToInt32 conversion
            if (arg is Array jsArray)
            {
                int len = jsArray.Count;
                var buf = new int[len];
                for (int i = 0; i < len; i++) buf[i] = ToInt32(jsArray[i]!);
                _buffer = buf;
                return;
            }

            // .NET array or IEnumerable => copy
            if (arg is System.Array arr)
            {
                int len = arr.Length;
                var buf = new int[len];
                for (int i = 0; i < len; i++) buf[i] = ToInt32(arr.GetValue(i)!);
                _buffer = buf;
                return;
            }

            if (arg is IEnumerable en)
            {
                var list = new List<int>();
                foreach (var item in en) list.Add(ToInt32(item!));
                _buffer = list.ToArray();
                return;
            }

            // Fallback: non-numeric, non-array => treat as length 0
            _buffer = System.Array.Empty<int>();
        }

        // JS typed arrays expose a numeric length; keep as double for consistency with runtime helpers
        public double length => _buffer.Length;

        // Indexer aligns with JS semantics: index is dynamic (object) and values are JS numbers (double boxed).
        public object this[object index]
        {
            get
            {
                int i = SafeToInt(index);
                if ((uint)i >= (uint)_buffer.Length) return 0.0; // out-of-bounds reads return 0 as a JS number
                return (double)_buffer[i];
            }
            set
            {
                int i = SafeToInt(index);
                if ((uint)i >= (uint)_buffer.Length) return; // ignore out-of-bounds sets
                _buffer[i] = ToInt32(value!);
            }
        }

        // Int32Array.prototype.set(source[, offset])
        public object set(object[]? args)
        {
            if (args == null || args.Length == 0 || args[0] == null)
            {
                return null!; // undefined
            }
            var source = args[0];
            int offset = 0;
            if (args.Length > 1 && args[1] != null)
            {
                offset = SafeToInt(args[1]);
                if (offset < 0) offset = 0;
                if (offset >= _buffer.Length) return null!; // nothing to copy
            }

            void CopyFromSequence(IEnumerable seq)
            {
                int i = 0;
                foreach (var item in seq)
                {
                    int dst = offset + i;
                    if (dst >= _buffer.Length) break;
                    _buffer[dst] = ToInt32(item!);
                    i++;
                }
            }

            if (source is Array jsArray)
            {
                for (int i = 0; i < jsArray.Count && (offset + i) < _buffer.Length; i++)
                {
                    _buffer[offset + i] = ToInt32(jsArray[i]!);
                }
                return null!;
            }
            if (source is Int32Array i32)
            {
                for (int i = 0; i < i32._buffer.Length && (offset + i) < _buffer.Length; i++)
                {
                    _buffer[offset + i] = i32._buffer[i];
                }
                return null!;
            }
            if (source is System.Array arr)
            {
                for (int i = 0; i < arr.Length && (offset + i) < _buffer.Length; i++)
                {
                    _buffer[offset + i] = ToInt32(arr.GetValue(i)!);
                }
                return null!;
            }
            if (source is IEnumerable en)
            {
                CopyFromSequence(en);
                return null!;
            }

            // Non-arraylike => no-op
            return null!;
        }

        private static bool TryToNumber(object o, out double d)
        {
            switch (o)
            {
                case double dd: d = dd; return true;
                case float ff: d = ff; return true;
                case int ii: d = ii; return true;
                case long ll: d = ll; return true;
                case short ss: d = ss; return true;
                case byte bb: d = bb; return true;
                case bool bo: d = bo ? 1 : 0; return true;
                case string s when double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pd):
                    d = pd; return true;
                default:
                    d = 0; return false;
            }
        }

        // Approximate JS ToInt32 semantics: NaN/±0/±∞ => 0; otherwise truncate toward zero and clamp to int range.
        private static int ToInt32(object o)
        {
            if (!TryToNumber(o, out var d)) return 0;
            if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0) return 0;
            // Truncate toward zero
            try
            {
                if (d > int.MaxValue) return int.MaxValue;
                if (d < int.MinValue) return int.MinValue;
                return (int)d;
            }
            catch { return 0; }
        }

        private static int SafeToInt(object o)
        {
            if (!TryToNumber(o, out var d)) return 0;
            if (double.IsNaN(d) || double.IsInfinity(d)) return 0;
            try { return (int)d; } catch { return 0; }
        }

        private static int ToLength(double d)
        {
            if (double.IsNaN(d) || d <= 0) return 0;
            if (double.IsInfinity(d)) return int.MaxValue;
            if (d > int.MaxValue) return int.MaxValue;
            return (int)d;
        }
    }
}
