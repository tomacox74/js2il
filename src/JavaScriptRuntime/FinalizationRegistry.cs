using System.Collections.Generic;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime
{
    [IntrinsicObject("FinalizationRegistry")]
    public sealed class FinalizationRegistry
    {
        private sealed class Registration
        {
            public Registration(object target, object? heldValue, object? unregisterToken)
            {
                Target = new WeakReference<object>(target);
                HeldValue = heldValue;
                UnregisterToken = unregisterToken == null ? null : new WeakReference<object>((object)unregisterToken);
            }

            public WeakReference<object> Target { get; }
            public object? HeldValue { get; }
            public WeakReference<object>? UnregisterToken { get; }

            public bool MatchesUnregisterToken(object token)
            {
                return UnregisterToken != null
                    && UnregisterToken.TryGetTarget(out var existing)
                    && ReferenceEquals(existing, token);
            }
        }

        private readonly object _sync = new();
        private readonly object _cleanupCallback;
        private readonly List<Registration> _registrations = new();
        private bool _trackedWithHost;

        public FinalizationRegistry(object? cleanupCallback)
        {
            if (cleanupCallback is not Delegate)
            {
                throw new TypeError("FinalizationRegistry cleanupCallback must be a function");
            }

            _cleanupCallback = cleanupCallback;
            InitializeIntrinsicSurface();
        }

        public object? register(object? target, object? heldValue)
        {
            return register(target, heldValue, null);
        }

        public object? register(object? target, object? heldValue, object? unregisterToken)
        {
            if (!TypeUtilities.CanBeHeldWeakly(target))
            {
                throw new TypeError("FinalizationRegistry target must be an object");
            }

            if (ReferenceEquals(target, heldValue))
            {
                throw new TypeError("FinalizationRegistry target and holdings must not be the same");
            }

            if (unregisterToken != null && !TypeUtilities.CanBeHeldWeakly(unregisterToken))
            {
                throw new TypeError("FinalizationRegistry unregisterToken must be an object");
            }

            lock (_sync)
            {
                _registrations.Add(new Registration((object)target!, heldValue, unregisterToken));
            }

            EnsureTrackedWithHost();
            return null;
        }

        public bool unregister(object? unregisterToken)
        {
            if (!TypeUtilities.CanBeHeldWeakly(unregisterToken))
            {
                throw new TypeError("FinalizationRegistry unregisterToken must be an object");
            }

            var removed = false;

            lock (_sync)
            {
                for (int i = _registrations.Count - 1; i >= 0; i--)
                {
                    if (_registrations[i].MatchesUnregisterToken((object)unregisterToken!))
                    {
                        _registrations.RemoveAt(i);
                        removed = true;
                    }
                }
            }

            return removed;
        }

        internal void CollectCleanupJobs(List<(FinalizationRegistry Registry, object? HeldValue)> jobs)
        {
            ArgumentNullException.ThrowIfNull(jobs);

            lock (_sync)
            {
                for (int i = _registrations.Count - 1; i >= 0; i--)
                {
                    if (_registrations[i].Target.TryGetTarget(out _))
                    {
                        continue;
                    }

                    jobs.Add((this, _registrations[i].HeldValue));
                    _registrations.RemoveAt(i);
                }
            }
        }

        internal void InvokeCleanupCallback(object? heldValue)
        {
            Closure.InvokeWithArgs(_cleanupCallback, RuntimeServices.EmptyScopes, heldValue);
        }

        private void EnsureTrackedWithHost()
        {
            if (_trackedWithHost)
            {
                return;
            }

            lock (_sync)
            {
                if (_trackedWithHost)
                {
                    return;
                }

                var serviceProvider = GlobalThis.ServiceProvider
                    ?? throw new InvalidOperationException("No runtime service provider is configured for FinalizationRegistry.");
                if (!serviceProvider.IsRegistered<IFinalizationRegistryHost>())
                {
                    throw new InvalidOperationException("FinalizationRegistry requires finalization services to be registered with the runtime.");
                }

                serviceProvider.Resolve<IFinalizationRegistryHost>().TrackRegistry(this);
                _trackedWithHost = true;
            }
        }

        private void InitializeIntrinsicSurface()
        {
            PropertyDescriptorStore.DefineOrUpdate(this, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "FinalizationRegistry"
            });
        }
    }
}
