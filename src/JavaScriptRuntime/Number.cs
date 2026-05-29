namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Number intrinsic surface needed by tests/runtime.
    /// </summary>
    [IntrinsicObject("Number")]
    public static class Number
    {
        internal const string NumberDataPropertyName = "[[NumberData]]";

        /// <summary>
        /// ECMAScript: Number.isNaN(x) returns true only when x is a Number and is NaN (no coercion).
        /// </summary>
        public static bool isNaN(object? value)
        {
            return value switch
            {
                double d => double.IsNaN(d),
                float f => float.IsNaN(f),
                _ => false
            };
        }

        public static bool isFinite(object? value)
        {
            return value switch
            {
                double d => double.IsFinite(d),
                float f => float.IsFinite(f),
                int or long or short or byte or sbyte or uint or ulong or ushort => true,
                _ => false
            };
        }

        /// <summary>
        /// ECMAScript: Number.isInteger(x) returns true only for finite integral Number values.
        /// </summary>
        public static bool isInteger(object? value)
        {
            return value switch
            {
                double d => double.IsFinite(d) && double.IsInteger(d),
                float f => float.IsFinite(f) && float.IsInteger(f),
                int or long or short or byte or sbyte or uint or ulong or ushort => true,
                _ => false
            };
        }

        internal static bool IsNumberConstructor(Delegate candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);

            var intrinsic = GlobalThis.Number;
            return ReferenceEquals(candidate, intrinsic)
                || (intrinsic is Delegate intrinsicDelegate && candidate.Method == intrinsicDelegate.Method);
        }

        internal static object Construct(object?[]? args, object? newTarget)
        {
            var value = args != null && args.Length > 0
                ? TypeUtilities.ToNumber(args[0])
                : 0d;

            var wrapper = new JsObject();
            var prototype = GlobalThis.NumberPrototypeValue;

            if (newTarget is not null and not JsNull)
            {
                var candidatePrototype = ObjectRuntime.GetItem(newTarget, "prototype");
                if (candidatePrototype is JsNull || TypeUtilities.IsConstructorReturnOverride(candidatePrototype))
                {
                    prototype = candidatePrototype;
                }
            }

            PrototypeChain.SetPrototype(wrapper, prototype);
            PropertyDescriptorStore.DefineOrUpdate(wrapper, NumberDataPropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = true,
                Value = value
            });

            return wrapper;
        }

        internal static double ThisNumberValue(object? value)
        {
            if (value is double or float or int or long or short or byte or System.Numerics.BigInteger)
            {
                return TypeUtilities.ToNumber(value);
            }

            if (value is not null
                && PropertyDescriptorStore.TryGetOwn(value, NumberDataPropertyName, out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data)
            {
                return TypeUtilities.ToNumber(descriptor.Value);
            }

            throw new TypeError("Number.prototype method called on incompatible receiver");
        }

        internal static bool TryGetWrappedNumberValue(object? value, out double numberValue)
        {
            numberValue = default;

            if (value is null || value is JsNull)
            {
                return false;
            }

            if (!PropertyDescriptorStore.TryGetOwn(value, NumberDataPropertyName, out var descriptor)
                || descriptor.Kind != JsPropertyDescriptorKind.Data)
            {
                return false;
            }

            numberValue = TypeUtilities.ToNumber(descriptor.Value);
            return true;
        }
    }
}
