using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakRef")]
    public sealed class WeakRef
    {
        /// <summary>Realm-owned <c>WeakRef.prototype</c> intrinsic (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.WeakRefPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));
        private readonly WeakReference<object> _target;

        public WeakRef() : this(null)
        {
        }

        public WeakRef(object? target)
        {
            if (!TypeUtilities.CanBeHeldWeakly(target))
            {
                throw new TypeError("WeakRef target must be an object");
            }

            _target = new WeakReference<object>((object)target!);
            InitializeIntrinsicSurface();
        }

        public object? deref()
        {
            if (!_target.TryGetTarget(out var target))
            {
                return null;
            }

            var serviceProvider = GlobalThis.ServiceProvider;
            if (serviceProvider != null && serviceProvider.IsRegistered<IFinalizationRegistryHost>())
            {
                serviceProvider.Resolve<IFinalizationRegistryHost>().AddToKeptObjects(target);
            }

            return target;
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            BuiltinFunction0 deref = PrototypeDeref;
            Function.InitializeFunctionInstance(
                deref,
                0d,
                "deref",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(deref));
            PropertyDescriptorStore.DefineOrUpdate(deref, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, "deref", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = deref
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "WeakRef"
            });
        }

        private static object? PrototypeDeref(object? thisArgument)
        {
            if (thisArgument is not WeakRef weakRef)
            {
                throw new TypeError("WeakRef.prototype.deref called on incompatible receiver");
            }

            return weakRef.deref();
        }
    }
}
