using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakRef")]
    public sealed class WeakRef
    {
        internal static readonly object Prototype = CreatePrototype();
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

        private static object CreatePrototype()
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            var prototype = new JsObject();
            Func<object[], object?[]?, object?> deref = PrototypeDeref;
            Function.InitializeFunctionInstance(deref, 0d, "deref");
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
            return prototype;
        }

        private static object? PrototypeDeref(object[] scopes, object?[]? args)
        {
            if (RuntimeServices.GetCurrentThis() is not WeakRef weakRef)
            {
                throw new TypeError("WeakRef.prototype.deref called on incompatible receiver");
            }

            return weakRef.deref();
        }
    }
}
