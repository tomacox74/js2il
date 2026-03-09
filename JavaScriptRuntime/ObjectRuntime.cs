using System;

namespace JavaScriptRuntime
{
    [IntrinsicObject("ObjectRuntime")]
    /// <summary>
    /// Low-level object runtime surface.
    /// Runtime helpers live here while JavaScript Object built-ins stay on <see cref="Object"/>.
    /// </summary>
    public static class ObjectRuntime
    {
        public static object? CallMember(object receiver, string methodName, object[]? args)
            => Object.CallMember(receiver, methodName, args);

        public static IJavaScriptIterator GetIterator(object? iterable)
            => Object.GetIterator(iterable);

        public static object? GetProperty(object obj, string name)
            => Object.GetProperty(obj, name);

        public static object? SetProperty(object obj, string name, object? value)
            => Object.SetProperty(obj, name, value);

        public static bool HasPropertyIn(object? key, object? obj)
            => Object.HasPropertyIn(key, obj);

        /// <summary>
        /// Implements the JavaScript <c>delete obj.prop</c> runtime semantics for this
        /// strict-mode-only runtime. Returns true if the deletion succeeds or the property
        /// is not present, and throws for non-configurable own properties.
        /// </summary>
        public static bool DeleteProperty(object? receiver, object? propName)
        {
            if (receiver is null || receiver is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot convert undefined or null to object");
            }

            var key = DotNet2JSConversions.ToString(propName);

            if (PropertyDescriptorStore.TryGetOwn(receiver, key, out var ownDescriptor)
                && !ownDescriptor.Configurable)
            {
                throw new JavaScriptRuntime.TypeError($"Cannot delete property '{key}' of object");
            }

            if (receiver is System.Dynamic.ExpandoObject exp)
            {
                var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                dict.Remove(key);
                PropertyDescriptorStore.Delete(receiver, key);
                return true;
            }

            if (receiver is System.Collections.Generic.IDictionary<string, object?> dictGeneric)
            {
                dictGeneric.Remove(key);
                PropertyDescriptorStore.Delete(receiver, key);
                return true;
            }

            if (receiver is System.Collections.IDictionary dictObj)
            {
                if (dictObj.Contains(key))
                {
                    dictObj.Remove(key);
                    return true;
                }

                object? match = null;
                foreach (var k in dictObj.Keys)
                {
                    if (string.Equals(DotNet2JSConversions.ToString(k), key, StringComparison.Ordinal))
                    {
                        match = k;
                        break;
                    }
                }
                if (match != null)
                {
                    dictObj.Remove(match);
                }

                PropertyDescriptorStore.Delete(receiver, key);
                return true;
            }

            // Arrays/typed arrays/strings and CLR objects: deletion is a no-op in this runtime for now.
            // (Full JS semantics for array holes and non-configurable properties are not modeled.)
            return true;
        }

        /// <summary>
        /// Implements the JavaScript <c>delete obj[index]</c> runtime semantics (minimal).
        /// </summary>
        public static bool DeleteItem(object? receiver, object? index)
        {
            return DeleteProperty(receiver, DotNet2JSConversions.ToString(index));
        }

        // Determines whether a computed key should be treated as an array index.
        // We intentionally require a *canonical* decimal representation for string keys:
        //  - "0", "1", ... are indices
        //  - "01", "1.0", "-1", "length", "true" are properties
        internal static bool TryGetCanonicalArrayIndex(object index, string propName, out int intIndex)
        {
            intIndex = 0;

            switch (index)
            {
                case int ii:
                    if (ii < 0) return false;
                    intIndex = ii;
                    return true;
                case long ll:
                    if (ll < 0 || ll > int.MaxValue) return false;
                    intIndex = (int)ll;
                    return true;
                case short ss:
                    if (ss < 0) return false;
                    intIndex = ss;
                    return true;
                case byte bb:
                    intIndex = bb;
                    return true;
                case double dd:
                    if (double.IsNaN(dd) || double.IsInfinity(dd)) return false;
                    if (dd < 0 || dd > int.MaxValue) return false;
                    if (!double.IsInteger(dd)) return false;
                    intIndex = (int)dd;
                    return true;
                case float ff:
                    if (float.IsNaN(ff) || float.IsInfinity(ff)) return false;
                    if (ff < 0 || ff > int.MaxValue) return false;
                    if (!float.IsInteger(ff)) return false;
                    intIndex = (int)ff;
                    return true;
                case string s:
                    return TryParseCanonicalIndexString(s, out intIndex);
            }

            // Fallback: try the ToPropertyKey string representation.
            return TryParseCanonicalIndexString(propName, out intIndex);
        }

        internal static bool TryParseCanonicalIndexString(string s, out int parsed)
        {
            parsed = 0;
            if (string.IsNullOrEmpty(s)) return false;
            if (!int.TryParse(s, global::System.Globalization.NumberStyles.None, global::System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return false;
            }
            if (parsed < 0) return false;
            return parsed.ToString(global::System.Globalization.CultureInfo.InvariantCulture) == s;
        }

        public static object GetItem(object obj, object index)
        {
            var propName = Object.ToPropertyKeyString(index);

            // Proxy get trap: treat item access as property access using ToPropertyKey
            if (obj is JavaScriptRuntime.Proxy)
            {
                return GetProperty(obj, propName)!;
            }

            // Symbol keys are always properties.
            if (index is Symbol)
            {
                return GetProperty(obj, propName)!;
            }

            bool isIndex = TryGetCanonicalArrayIndex(index, propName, out int intIndex);

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (!isIndex)
                {
                    return GetProperty(obj, propName)!;
                }
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): numeric index coerces to property name string per JS ToPropertyKey
            if (obj is System.Dynamic.ExpandoObject)
            {
                return GetProperty(obj, propName)!;
            }

            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return GetProperty(array, propName)!;
                }
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is TypedArrayBase typedArray)
            {
                if (!isIndex)
                {
                    return GetProperty(typedArray, propName)!;
                }

                return typedArray[(double)intIndex];
            }
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return GetProperty(buffer, propName)!;
                }
                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[(double)intIndex]!;
            }
            else
            {
                // Generic object index access: treat index as a property key (JS ToPropertyKey -> string)
                // and fall back to dynamic property lookup (public fields/properties and ExpandoObject).
                return GetProperty(obj, propName)!;
            }
        }

        public static object GetItem(object obj, double index)
        {
            // Proxy get trap: numeric index coerces to property key
            if (obj is JavaScriptRuntime.Proxy)
            {
                var propName = Object.ToPropertyKeyString(index);
                return GetProperty(obj, propName)!;
            }

            // Coerce index to int (JS ToInt32-ish truncation)
            int intIndex = (int)index;

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): numeric index coerces to property name string per JS ToPropertyKey
            if (obj is System.Dynamic.ExpandoObject)
            {
                var propName = Object.ToPropertyKeyString(index);
                return GetProperty(obj, propName)!;
            }

            if (obj is Array array)
            {
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is TypedArrayBase typedArray)
            {
                return typedArray[index];
            }
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (double.IsNaN(index) || double.IsInfinity(index) || index % 1.0 != 0.0)
                {
                    var propName = Object.ToPropertyKeyString(index);
                    return GetProperty(buffer, propName)!;
                }

                if (index < 0 || index > int.MaxValue)
                {
                    return null!;
                }

                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[index]!;
            }
            else
            {
                // Generic object index access: treat index as a property key (JS ToPropertyKey -> string)
                // and fall back to dynamic property lookup (public fields/properties and ExpandoObject).
                var propName = Object.ToPropertyKeyString(index);
                return GetProperty(obj, propName)!;
            }
        }

        /// <summary>
        /// Fast-path overload for string key reads.
        /// Avoids boxing at the call site when the compiler has proven the key is a string.
        /// Skips ToPropertyKeyString/Symbol checks and directly tests the canonical index string path.
        /// </summary>
        public static object GetItem(object obj, string key)
        {
            // Proxy get trap: treat item access as property access
            if (obj is JavaScriptRuntime.Proxy)
            {
                return GetProperty(obj, key)!;
            }

            bool isIndex = TryParseCanonicalIndexString(key, out int intIndex);

            // String: return character at index as a 1-length string
            if (obj is string str)
            {
                if (!isIndex)
                {
                    return GetProperty(obj, key)!;
                }
                if (intIndex < 0 || intIndex >= str.Length)
                {
                    return null!; // undefined
                }
                return str[intIndex].ToString();
            }

            // ExpandoObject (object literal): key is already a string property
            if (obj is System.Dynamic.ExpandoObject)
            {
                return GetProperty(obj, key)!;
            }

            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return GetProperty(array, key)!;
                }
                // Bounds check: return undefined (null) when OOB to mimic JS behavior
                if (intIndex < 0 || intIndex >= array.Count)
                {
                    return null!; // undefined
                }
                return array[intIndex]!;
            }
            else if (obj is TypedArrayBase typedArray)
            {
                if (!isIndex)
                {
                    return GetProperty(typedArray, key)!;
                }

                return typedArray[(double)intIndex];
            }
            else if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return GetProperty(buffer, key)!;
                }
                // Reads outside bounds return undefined per Node.js Buffer semantics
                return buffer[(double)intIndex]!;
            }
            else
            {
                return GetProperty(obj, key)!;
            }
        }

        /// <summary>
        /// Gets an item from an object and converts the result to a number (double).
        /// Provides a fast path for typed-array receivers that avoids boxing the element value.
        /// For all other receivers, falls back to TypeUtilities.ToNumber(GetItem(obj, index)).
        /// </summary>
        public static double GetItemAsNumber(object obj, double index)
        {
            if (obj is TypedArrayBase typedArray)
            {
                return typedArray[index];
            }

            return TypeUtilities.ToNumber(GetItem(obj, index));
        }

        /// <summary>
        /// Gets an item from an object and converts the result to a number (double).
        /// Provides a fast path for typed-array receivers that avoids boxing the element value.
        /// For all other receivers, falls back to TypeUtilities.ToNumber(GetItem(obj, index)).
        /// </summary>
        public static double GetItemAsNumber(object obj, object index)
        {
            if (obj is TypedArrayBase typedArray && index is double d)
            {
                return typedArray[d];
            }

            return TypeUtilities.ToNumber(GetItem(obj, index));
        }

        /// <summary>
        /// Fast-path overload for string key reads that returns an unboxed number.
        /// Avoids boxing at the call site when the compiler has proven the key is a string
        /// and the result is consumed as a number.
        /// </summary>
        public static double GetItemAsNumber(object obj, string key)
        {
            return TypeUtilities.ToNumber(GetItem(obj, key));
        }

        /// <summary>
        /// Sets an item on an object by index/key.
        /// Used by codegen for computed member assignment and property assignment.
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetItem(object? obj, object index, object? value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            var propName = Object.ToPropertyKeyString(index);
            bool isIndex = TryGetCanonicalArrayIndex(index, propName, out int intIndex);

            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy)
            {
                return SetProperty(obj, propName, value);
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            if (obj is System.Dynamic.ExpandoObject)
            {
                return SetProperty(obj, propName, value);
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (!isIndex)
                {
                    // Non-index keys behave like properties in JS (e.g. "length").
                    return SetProperty(array, propName, value);
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value!;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                // Extend with undefined (null) up to the index, then add.
                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: coerce and store when in-bounds
            if (obj is TypedArrayBase typedArray)
            {
                if (!isIndex)
                {
                    return SetProperty(typedArray, propName, value);
                }

                typedArray[(double)intIndex] = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return value;
            }

            // Buffer: coerce and store when in-bounds
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return SetProperty(buffer, propName, value);
                }
                // Buffer indexer expects numeric value
                buffer[(double)intIndex] = value;
                return value;
            }

            // Generic object: treat as property assignment (ToPropertyKey -> string)
            return SetProperty(obj, propName, value);
        }

        /// <summary>
        /// Fast-path overload for string key writes.
        /// Avoids boxing at the call site when the compiler has proven the key is a string.
        /// Skips ToPropertyKeyString/Symbol checks and directly tests the canonical index string path.
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetItem(object? obj, string key, object? value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy)
            {
                return SetProperty(obj, key, value);
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            bool isIndex = TryParseCanonicalIndexString(key, out int intIndex);

            if (obj is System.Dynamic.ExpandoObject)
            {
                return SetProperty(obj, key, value);
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (!isIndex)
                {
                    return SetProperty(array, key, value);
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value!;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                // Extend with undefined (null) up to the index, then add.
                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: coerce and store when in-bounds
            if (obj is TypedArrayBase typedArray)
            {
                if (!isIndex)
                {
                    return SetProperty(typedArray, key, value);
                }

                typedArray[(double)intIndex] = JavaScriptRuntime.TypeUtilities.ToNumber(value);
                return value;
            }

            // Buffer: coerce and store when in-bounds
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return SetProperty(buffer, key, value);
                }
                // Buffer indexer expects numeric value
                buffer[(double)intIndex] = value;
                return value;
            }

            // Generic object: treat as property assignment
            return SetProperty(obj, key, value);
        }

        /// <summary>
        /// Fast-path overload for string key writes with numeric values.
        /// Avoids boxing at the call site when the compiler has an unboxed double.
        /// Returns the assigned value to match JavaScript assignment expression semantics.
        /// </summary>
        public static object? SetItem(object? obj, string key, double value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            // Proxy set trap
            if (obj is JavaScriptRuntime.Proxy)
            {
                return SetProperty(obj, key, value);
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            bool isIndex = TryParseCanonicalIndexString(key, out int intIndex);

            if (obj is System.Dynamic.ExpandoObject)
            {
                return SetProperty(obj, key, value);
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (!isIndex)
                {
                    // Common hot path in benchmarks: ret.length = i
                    if (string.Equals(key, "length", StringComparison.Ordinal))
                    {
                        array.length = value;
                        return value;
                    }

                    return SetProperty(array, key, value);
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                // Extend with undefined (null) up to the index, then add.
                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: value is already numeric.
            if (obj is TypedArrayBase typedArray)
            {
                if (!isIndex)
                {
                    return SetProperty(typedArray, key, value);
                }

                typedArray[(double)intIndex] = value;
                return value;
            }

            // Buffer: value is already numeric.
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                if (!isIndex)
                {
                    return SetProperty(buffer, key, value);
                }
                buffer[(double)intIndex] = value;
                return value;
            }

            // Generic object: treat as property assignment
            return SetProperty(obj, key, value);
        }

        /// <summary>
        /// Fast-path overload for numeric index and numeric value.
        /// Avoids boxing at the call site when the compiler has unboxed doubles.
        /// Returns the assigned value (boxed) to match JavaScript assignment expression semantics.
        /// </summary>
        public static object SetItem(object? obj, double index, double value)
        {
            if (obj is null)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null or undefined");
            }

            if (obj is JsNull)
            {
                throw new JavaScriptRuntime.TypeError("Cannot set properties of null");
            }

            int intIndex;
            if (double.IsNaN(index) || double.IsInfinity(index))
            {
                intIndex = 0;
            }
            else
            {
                try { intIndex = (int)index; }
                catch { intIndex = 0; }
            }

            // Strings are immutable in JS; silently ignore and return value.
            if (obj is string)
            {
                return value;
            }

            // JS Array index assignment
            if (obj is Array array)
            {
                if (intIndex < 0)
                {
                    // Negative indices behave like properties in JS; treat as property for host safety.
                    return SetProperty(array, intIndex.ToString(System.Globalization.CultureInfo.InvariantCulture), value) ?? value;
                }

                if (intIndex < array.Count)
                {
                    array[intIndex] = value;
                    return value;
                }

                if (intIndex == array.Count)
                {
                    array.Add(value);
                    return value;
                }

                while (array.Count < intIndex)
                {
                    array.Add(null);
                }
                array.Add(value);
                return value;
            }

            // Typed arrays: only use element write path if index is finite, integer, and in-bounds.
            // Otherwise treat as no-op (typed arrays do not store non-integer-index properties).
            if (obj is TypedArrayBase typedArray)
            {
                if (!double.IsNaN(index) && !double.IsInfinity(index) && (index % 1.0 == 0.0))
                {
                    if (index >= 0 && index <= int.MaxValue)
                    {
                        var typedArrayIndex = (int)index;
                        if (typedArrayIndex < (int)typedArray.length)
                        {
                            typedArray.SetFromDouble(typedArrayIndex, value);
                        }
                    }
                }
                return value;
            }

            // Buffer: only use element write path if index is finite, integer, and in-bounds.
            if (obj is JavaScriptRuntime.Node.Buffer buffer)
            {
                // Check if index is a valid integer index
                if (!double.IsNaN(index) && !double.IsInfinity(index) && (index % 1.0 == 0.0))
                {
                    // Validate index is within int32 range before casting
                    if (index >= 0 && index <= int.MaxValue)
                    {
                        int bufferIndex = (int)index;
                        // Only write if in bounds [0, length)
                        if (bufferIndex < (int)buffer.length)
                        {
                            buffer[(double)bufferIndex] = value;
                        }
                        // Out-of-bounds: no-op (buffers don't expand)
                    }
                    // Negative or too large: no-op
                }
                // NaN/Infinity/fractional: no-op (do not treat as element 0 or property)
                return value;
            }

            // Fallback: treat numeric index as a property key string.
            return SetProperty(obj, DotNet2JSConversions.ToString(index), value) ?? value;
        }
    }
}
