using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakMap")]
    public sealed class WeakMap : JsObject
    {
        /// <summary>Realm-owned <c>WeakMap.prototype</c> intrinsic (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.WeakMapPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));
        // ConditionalWeakTable allows keys to be garbage collected when no other references exist
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();

        public WeakMap()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
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
            var adder = GetCallableAdder("set");
            var iterator = ObjectRuntime.GetIterator(iterable);
            var completedNormally = false;
            try
            {
                while (true)
                {
                    var step = JavaScriptRuntime.ObjectRuntime.IteratorNext(iterator);
                    if (JavaScriptRuntime.ObjectRuntime.IteratorResultDone(step))
                    {
                        break;
                    }

                    var (key, value) = ExtractEntry(JavaScriptRuntime.ObjectRuntime.IteratorResultValue(step));
                    CallAdder(adder, key, value);
                }

                completedNormally = true;
            }
            finally
            {
                if (!completedNormally)
                {
                    JavaScriptRuntime.ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                }
            }
        }

        private object GetCallableAdder(string name)
        {
            var adder = ObjectRuntime.GetProperty(Prototype, name);
            if (!CallableOperations.IsCallable(adder))
            {
                throw new TypeError($"WeakMap.prototype.{name} is not callable");
            }

            return adder!;
        }

        private object? CallAdder(object adder, object? key, object? value)
            => CallableOperations.Call2(adder, this, key, value);

        private static (object? Key, object? Value) ExtractEntry(object? entry)
        {
            if (entry is null || entry is JsNull)
            {
                throw new TypeError("Iterator value must be an object or function");
            }

            var entryType = TypeUtilities.Typeof(entry);
            if (entryType != "object" && entryType != "function")
            {
                throw new TypeError("Iterator value is not an entry object");
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

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            DefinePrototypeMethod(prototype, "delete", (BuiltinFunction1)PrototypeDelete);
            DefinePrototypeMethod(prototype, "get", (BuiltinFunction1)PrototypeGet);
            DefinePrototypeMethod(prototype, "has", (BuiltinFunction1)PrototypeHas);
            DefinePrototypeMethod(prototype, "set", (BuiltinFunction2)PrototypeSet);
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "WeakMap"
            });
        }

        private static void DefinePrototypeMethod(object prototype, string name, Delegate method)
        {
            Function.InitializeFunctionInstance(
                method,
                Function.GetLength(method),
                name,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(method));
            Function.MarkUndefinedPrototype(method);
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = method
            });
        }

        private static WeakMap GetThisWeakMap(object? thisArgument, string memberName)
        {
            if (thisArgument is not WeakMap weakMap)
            {
                throw new TypeError($"WeakMap.prototype.{memberName} called on non-WeakMap");
            }

            return weakMap;
        }

        private static object? PrototypeDelete(object? thisArgument, object? key)
        {
            var weakMap = GetThisWeakMap(thisArgument, "delete");
            return weakMap.delete(key);
        }

        private static object? PrototypeGet(object? thisArgument, object? key)
        {
            var weakMap = GetThisWeakMap(thisArgument, "get");
            return weakMap.get(key);
        }

        private static object? PrototypeHas(object? thisArgument, object? key)
        {
            var weakMap = GetThisWeakMap(thisArgument, "has");
            return weakMap.has(key);
        }

        private static object? PrototypeSet(object? thisArgument, object? key, object? value)
        {
            var weakMap = GetThisWeakMap(thisArgument, "set");
            return weakMap.set(key, value);
        }
    }
}
