using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakSet")]
    public sealed class WeakSet : JsObject
    {
        /// <summary>Realm-owned <c>WeakSet.prototype</c> intrinsic (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.WeakSetPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));
        // Use ConditionalWeakTable with a dummy value to track membership
        // The presence of a key in the table indicates it's in the set
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();
        private static readonly object _dummyValue = new object();

        public WeakSet()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
        }

        public WeakSet(object? iterable)
            : this()
        {
            if (iterable is null || iterable is JsNull)
            {
                return;
            }

            AddValuesFromIterable(iterable);
        }

        private void AddValuesFromIterable(object iterable)
        {
            var adder = GetCallableAdder("add");
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

                    CallAdder(adder, JavaScriptRuntime.ObjectRuntime.IteratorResultValue(step));
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
                throw new TypeError($"WeakSet.prototype.{name} is not callable");
            }

            return adder!;
        }

        private object? CallAdder(object adder, object? value)
            => CallableOperations.Call1(adder, this, value);

        public object add(object? value)
        {
            if (!TypeUtilities.CanBeHeldWeakly(value))
            {
                throw new TypeError("Invalid value used in weak set");
            }

            _table.AddOrUpdate(value!, _dummyValue);
            return this;
        }

        public bool has(object? value)
        {
            if (!TypeUtilities.CanBeHeldWeakly(value))
            {
                return false;
            }

            return _table.TryGetValue(value!, out _);
        }

        public bool delete(object? value)
        {
            if (!TypeUtilities.CanBeHeldWeakly(value))
            {
                return false;
            }

            return _table.Remove(value!);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            DefinePrototypeMethod(prototype, "add", (BuiltinFunction1)PrototypeAdd);
            DefinePrototypeMethod(prototype, "delete", (BuiltinFunction1)PrototypeDelete);
            DefinePrototypeMethod(prototype, "has", (BuiltinFunction1)PrototypeHas);
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "WeakSet"
            });
        }

        private static void DefinePrototypeMethod(object prototype, string name, Delegate method)
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

        private static WeakSet GetThisWeakSet(object? thisArgument, string memberName)
        {
            if (thisArgument is not WeakSet weakSet)
            {
                throw new TypeError($"WeakSet.prototype.{memberName} called on non-WeakSet");
            }

            return weakSet;
        }

        private static object? PrototypeAdd(object? thisArgument, object? value)
        {
            var weakSet = GetThisWeakSet(thisArgument, "add");
            return weakSet.add(value);
        }

        private static object? PrototypeDelete(object? thisArgument, object? value)
        {
            var weakSet = GetThisWeakSet(thisArgument, "delete");
            return weakSet.delete(value);
        }

        private static object? PrototypeHas(object? thisArgument, object? value)
        {
            var weakSet = GetThisWeakSet(thisArgument, "has");
            return weakSet.has(value);
        }
    }
}
