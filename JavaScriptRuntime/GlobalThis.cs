using System;
using System.Collections.Generic;
using System.Dynamic;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Holds global intrinsic variables for the current program (Node-like today, extensible later).
    /// Minimal surface for js2il codegen: __dirname, __filename, and process.exitCode.
    /// </summary>
    public static class GlobalThis
    {
        private static readonly ThreadLocal<ServiceContainer?> _serviceProvider = new(() => null);

        // Per-"realm" (thread) global object. This backs the ECMAScript globalThis value.
        // We represent it as an ExpandoObject so member get/set and delegate-valued properties
        // work with the existing Object.GetProperty/SetProperty/CallMember dispatch.
        private static readonly ThreadLocal<ExpandoObject?> _globalObject = new(() => null);

        // Some ECMAScript globals are callable (e.g., Boolean(x)). When used in expression position
        // (e.g., arr.filter(Boolean)), we expose them as function values (delegates) so the compiler
        // can bind them as intrinsic globals.
        private static readonly Func<object[], object?, bool> _booleanFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToBoolean(value);

        private static readonly Func<object[], object?, string> _stringFunctionValue = static (_, value) =>
            JavaScriptRuntime.DotNet2JSConversions.ToString(value);

        private static readonly Func<object[], object?, double> _numberFunctionValue = static (_, value) =>
            JavaScriptRuntime.TypeUtilities.ToNumber(value);

        private static readonly Func<object[], object?, Delegate> _functionConstructorValue = static (_, __) =>
            throw new NotSupportedException("The Function constructor is not supported yet.");

        internal static ServiceContainer? ServiceProvider
        {
            get => _serviceProvider.Value;
            set => _serviceProvider.Value = value;
        }

        /// <summary>
        /// ECMA-262 globalThis value.
        /// Returns the global object for the current execution context.
        /// </summary>
        /// <remarks>
        /// JS2IL models the global object as a dynamic bag (ExpandoObject) seeded with common globals.
        /// This allows libraries to read/write properties via globalThis (e.g., globalThis.window = ...).
        /// </remarks>
        public static object globalThis => GetOrCreateGlobalObject();

        private static ExpandoObject GetOrCreateGlobalObject()
        {
            var obj = _globalObject.Value;
            if (obj == null)
            {
                obj = new ExpandoObject();
                _globalObject.Value = obj;
            }

            SeedGlobalObjectIfMissing(obj);
            return obj;
        }

        private static void SeedGlobalObjectIfMissing(ExpandoObject obj)
        {
            var dict = (IDictionary<string, object?>)obj;

            // Self reference.
            dict["globalThis"] = obj;

            // Seed common globals without overwriting user overrides.
            dict.TryAdd("console", console);
            dict.TryAdd("process", process);
            dict.TryAdd("Infinity", Infinity);
            dict.TryAdd("NaN", NaN);
            dict.TryAdd("Boolean", Boolean);
            dict.TryAdd("String", String);
            dict.TryAdd("Number", Number);
            dict.TryAdd("Function", Function);

            // Global functions exposed as delegates.
            dict.TryAdd("setTimeout", (Func<object, object, object[], object>)setTimeout);
            dict.TryAdd("clearTimeout", (Func<object, object?>)clearTimeout);
            dict.TryAdd("setImmediate", (Func<object, object[], object>)setImmediate);
            dict.TryAdd("clearImmediate", (Func<object, object?>)clearImmediate);
            dict.TryAdd("setInterval", (Func<object, object, object[], object>)setInterval);
            dict.TryAdd("clearInterval", (Func<object, object?>)clearInterval);
            dict.TryAdd("parseInt", (Func<object?, object?, double>)parseInt);
            dict.TryAdd("parseFloat", (Func<object?, double>)parseFloat);
            dict.TryAdd("isFinite", (Func<object?, bool>)isFinite);
        }

        /// <summary>
        /// Minimal process global with writable exitCode.
        /// </summary>
        /// <remarks>Expand as needed in the future.</remarks>
        public static JavaScriptRuntime.Node.Process process
        {
            get => _serviceProvider.Value!.Resolve<JavaScriptRuntime.Node.Process>();
        }

        /// <summary>
        /// Global console object (lowercase) to mirror JS global. Provides access to log/error/warn via the Console intrinsic.
        /// Backed by a single shared instance.
        /// </summary>
        public static JavaScriptRuntime.Console console 
        {
            get => _serviceProvider.Value!.Resolve<JavaScriptRuntime.Console>();
        }

        /// <summary>
        /// ECMAScript global Boolean conversion function value.
        /// This enables patterns like <c>array.filter(Boolean)</c>.
        /// </summary>
        public static Func<object[], object?, bool> Boolean => _booleanFunctionValue;

        /// <summary>
        /// ECMAScript global String conversion function value.
        /// This enables patterns like <c>array.map(String)</c> and type-marker comparisons (e.g., <c>x === String</c>).
        /// </summary>
        public static Func<object[], object?, string> String => _stringFunctionValue;

        /// <summary>
        /// ECMAScript global Number conversion function value.
        /// This enables patterns like <c>array.map(Number)</c> and type-marker comparisons (e.g., <c>x === Number</c>).
        /// </summary>
        public static Func<object[], object?, double> Number => _numberFunctionValue;

        /// <summary>
        /// ECMAScript global Function constructor value (placeholder).
        /// Currently exposed as a callable function value so libraries can reference it as a global identifier.
        /// Invoking it will throw until Function constructor semantics are implemented.
        /// </summary>
        public static Func<object[], object?, Delegate> Function => _functionConstructorValue;

        /// <summary>
        /// ECMAScript global Infinity value (+∞).
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double Infinity => double.PositiveInfinity;

        /// <summary>
        /// ECMAScript global NaN value.
        /// Exposed as a static property so identifiers bind at compile-time.
        /// </summary>
        public static double NaN => double.NaN;

        public static object setTimeout(object callback, object delay, params object[] args)
        {
            return GetTimers().setTimeout(callback, delay, args);
        }

        public static object? clearTimeout(object handle)
        {
            return GetTimers().clearTimeout(handle);
        }

        public static object setImmediate(object callback, params object[] args)
        {
            return GetTimers().setImmediate(callback, args);
        }

        public static object setInterval(object callback, object delay, params object[] args)
        {
            return GetTimers().setInterval(callback, delay, args);
        }

        public static object? clearImmediate(object handle)
        {
            return GetTimers().clearImmediate(handle);
        }

        public static object? clearInterval(object handle)
        {
            return GetTimers().clearInterval(handle);
        }

        /// <summary>
        /// Minimal parseInt implementation for numeric strings (radix 2-36).
        /// Returns NaN on invalid input.
        /// </summary>
        public static double parseInt(object? input, object? radix = null)
        {
            if (input == null) return double.NaN;

            var text = DotNet2JSConversions.ToString(input).TrimStart();
            if (text.Length == 0) return double.NaN;

            int sign = 1;
            if (text[0] == '+')
            {
                text = text.Substring(1);
            }
            else if (text[0] == '-')
            {
                sign = -1;
                text = text.Substring(1);
            }

            int radixValue = 0;
            if (radix != null)
            {
                try
                {
                    radixValue = Convert.ToInt32(radix, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    return double.NaN;
                }
            }

            if (radixValue == 0)
            {
                if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    radixValue = 16;
                    text = text.Substring(2);
                }
                else
                {
                    radixValue = 10;
                }
            }

            // Per spec, if radix is 16, an optional 0x/0X prefix is allowed and must be stripped.
            if (radixValue == 16 && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(2);
            }

            if (radixValue < 2 || radixValue > 36)
            {
                return double.NaN;
            }

            long value = 0;
            int digits = 0;
            foreach (var ch in text)
            {
                int d = ch switch
                {
                    >= '0' and <= '9' => ch - '0',
                    >= 'a' and <= 'z' => ch - 'a' + 10,
                    >= 'A' and <= 'Z' => ch - 'A' + 10,
                    _ => -1
                };

                if (d < 0 || d >= radixValue)
                {
                    break;
                }

                value = (value * radixValue) + d;
                digits++;
            }

            if (digits == 0)
            {
                return double.NaN;
            }

            return (double)(sign * value);
        }

        /// <summary>
        /// Minimal parseFloat implementation.
        /// Accepts leading whitespace, an optional sign, decimals, and an optional exponent.
        /// Stops parsing at the first invalid character; returns NaN if no valid prefix.
        /// </summary>
        public static double parseFloat(object? input)
        {
            if (input == null) return double.NaN;

            var text = DotNet2JSConversions.ToString(input).TrimStart();
            if (text.Length == 0) return double.NaN;

            // Infinity tokens
            if (text.StartsWith("Infinity", StringComparison.Ordinal)) return double.PositiveInfinity;
            if (text.StartsWith("+Infinity", StringComparison.Ordinal)) return double.PositiveInfinity;
            if (text.StartsWith("-Infinity", StringComparison.Ordinal)) return double.NegativeInfinity;

            int i = 0;
            if (text[i] == '+' || text[i] == '-')
            {
                i++;
                if (i >= text.Length) return double.NaN;
            }

            bool sawDigit = false;
            while (i < text.Length && char.IsDigit(text[i]))
            {
                sawDigit = true;
                i++;
            }

            if (i < text.Length && text[i] == '.')
            {
                i++;
                while (i < text.Length && char.IsDigit(text[i]))
                {
                    sawDigit = true;
                    i++;
                }
            }

            // Optional exponent
            if (sawDigit && i < text.Length && (text[i] == 'e' || text[i] == 'E'))
            {
                int expStart = i;
                i++;
                if (i < text.Length && (text[i] == '+' || text[i] == '-'))
                {
                    i++;
                }

                int expDigits = 0;
                while (i < text.Length && char.IsDigit(text[i]))
                {
                    expDigits++;
                    i++;
                }

                if (expDigits == 0)
                {
                    // Roll back; exponent marker not followed by digits.
                    i = expStart;
                }
            }

            if (!sawDigit)
            {
                return double.NaN;
            }

            var prefix = text.Substring(0, i);
            return double.TryParse(
                prefix,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var parsed)
                ? parsed
                : double.NaN;
        }

        /// <summary>
        /// Minimal global isFinite implementation.
        /// Coerces to number and returns true only when the result is a finite IEEE754 double.
        /// </summary>
        public static bool isFinite(object? number)
        {
            var d = TypeUtilities.ToNumber(number);
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        private static Timers GetTimers()
        {
            return _serviceProvider.Value!.Resolve<Timers>();
        }
    }
}
