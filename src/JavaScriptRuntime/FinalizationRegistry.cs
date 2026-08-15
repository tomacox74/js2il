using System.Collections.Generic;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime
{
    [IntrinsicObject("FinalizationRegistry")]
    public sealed class FinalizationRegistry
    {
        /// <summary>Realm-owned <c>FinalizationRegistry.prototype</c> intrinsic (issue #1824).</summary>
        internal static object Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.FinalizationRegistryPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));
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

        public FinalizationRegistry() : this(null)
        {
        }

        public FinalizationRegistry(object? cleanupCallback)
        {
            if (!CallableOperations.IsCallable(cleanupCallback))
            {
                throw new TypeError("FinalizationRegistry cleanupCallback must be a function");
            }

            _cleanupCallback = cleanupCallback!;
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
            CallableOperations.Call1(_cleanupCallback, null, heldValue);
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
            PrototypeChain.SetPrototype(this, Prototype);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            Func<object[], object?[]?, object?> register = PrototypeRegister;
            Func<object[], object?[]?, object?> unregister = PrototypeUnregister;
            Function.InitializeFunctionInstance(register, 2d, "register");
            Function.InitializeFunctionInstance(unregister, 1d, "unregister");
            PropertyDescriptorStore.DefineOrUpdate(register, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(unregister, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = null
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, "register", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = register
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, "unregister", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = unregister
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "FinalizationRegistry"
            });
        }

        private static object? PrototypeRegister(object[] _, object?[]? args)
        {
            if (RuntimeServices.GetCurrentThis() is not FinalizationRegistry finalizationRegistry)
            {
                throw new TypeError("FinalizationRegistry.prototype.register called on incompatible receiver");
            }

            args ??= [];
            return finalizationRegistry.register(
                args.Length > 0 ? args[0] : null,
                args.Length > 1 ? args[1] : null,
                args.Length > 2 ? args[2] : null);
        }

        private static object? PrototypeUnregister(object[] _, object?[]? args)
        {
            if (RuntimeServices.GetCurrentThis() is not FinalizationRegistry finalizationRegistry)
            {
                throw new TypeError("FinalizationRegistry.prototype.unregister called on incompatible receiver");
            }

            args ??= [];
            return finalizationRegistry.unregister(args.Length > 0 ? args[0] : null);
        }
    }
}
