using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakRef")]
    public sealed class WeakRef
    {
        private readonly WeakReference<object> _target;

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
            PropertyDescriptorStore.DefineOrUpdate(this, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "WeakRef"
            });
        }
    }
}
