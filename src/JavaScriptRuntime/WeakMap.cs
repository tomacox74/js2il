using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakMap")]
    public sealed class WeakMap
    {
        internal static readonly object Prototype = CreatePrototype();
        // ConditionalWeakTable allows keys to be garbage collected when no other references exist
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();

        public WeakMap()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public WeakMap(object? iterable)
            : this()
        {
            if (iterable is null || iterable is JsNull)
            {
                return;
            }

            AddEntriesFromIterable(iterable);
        }

        private void AddEntriesFromIterable(object iterable)
        {
            var iterator = ObjectRuntime.GetIterator(iterable);
            var completedNormally = false;
            try
            {
                while (true)
                {
                    var step = JavaScriptRuntime.Object.IteratorNext(iterator);
                    if (JavaScriptRuntime.Object.IteratorResultDone(step))
                    {
                        break;
                    }

                    var (key, value) = ExtractEntry(JavaScriptRuntime.Object.IteratorResultValue(step));
                    JavaScriptRuntime.Object.CallMember2(this, "set", key, value);
                }

                completedNormally = true;
            }
            finally
            {
                if (!completedNormally)
                {
                    JavaScriptRuntime.Object.IteratorClose(iterator);
                }
            }
        }

        private static (object? Key, object? Value) ExtractEntry(object? entry)
        {
            if (entry is null || entry is JsNull)
            {
                throw new TypeError("Iterator value must be an object");
            }

            var entryType = TypeUtilities.Typeof(entry);
            if (entryType != "object" && entryType != "function")
            {
                throw new TypeError("Iterator value is not an entry object");
            }

            if (entry is JavaScriptRuntime.Array arrayEntry)
            {
                return (
                    arrayEntry.Count > 0 ? arrayEntry[0] : null,
                    arrayEntry.Count > 1 ? arrayEntry[1] : null
                );
            }

            if (entry is System.Collections.IList listEntry)
            {
                return (
                    listEntry.Count > 0 ? listEntry[0] : null,
                    listEntry.Count > 1 ? listEntry[1] : null
                );
            }

            return (ObjectRuntime.GetItem(entry, 0.0), ObjectRuntime.GetItem(entry, 1.0));
        }

        public object set(object? key, object? value)
        {
            if (!TypeUtilities.CanBeHeldWeakly(key))
            {
                throw new TypeError("Invalid value used as weak map key");
            }

            _table.AddOrUpdate(key!, value!);
            return this;
        }

        public object? get(object? key)
        {
            if (!TypeUtilities.CanBeHeldWeakly(key))
            {
                return null; // JavaScript undefined
            }

            if (_table.TryGetValue(key!, out var value))
            {
                return value;
            }
            return null; // JavaScript undefined
        }

        public bool has(object? key)
        {
            if (!TypeUtilities.CanBeHeldWeakly(key))
            {
                return false;
            }

            return _table.TryGetValue(key!, out _);
        }

        public bool delete(object? key)
        {
            if (!TypeUtilities.CanBeHeldWeakly(key))
            {
                return false;
            }

            return _table.Remove(key!);
        }

        private static object CreatePrototype()
        {
            var prototype = new JsObject();
            DefinePrototypeMethod(prototype, "delete", PrototypeDelete);
            DefinePrototypeMethod(prototype, "get", PrototypeGet);
            DefinePrototypeMethod(prototype, "has", PrototypeHas);
            DefinePrototypeMethod(prototype, "set", PrototypeSet);
            return prototype;
        }

        private static void DefinePrototypeMethod(object prototype, string name, Func<object[], object?[]?, object?> method)
        {
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = method
            });
        }

        private static WeakMap GetThisWeakMap(string memberName)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is not WeakMap weakMap)
            {
                throw new TypeError($"WeakMap.prototype.{memberName} called on non-WeakMap");
            }

            return weakMap;
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("delete");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.delete(key);
        }

        private static object? PrototypeGet(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("get");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.get(key);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("has");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.has(key);
        }

        private static object? PrototypeSet(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("set");
            var key = args != null && args.Length > 0 ? args[0] : null;
            var value = args != null && args.Length > 1 ? args[1] : null;
            return weakMap.set(key, value);
        }
    }
}
