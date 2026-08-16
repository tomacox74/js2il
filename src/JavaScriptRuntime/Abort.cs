using System;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    public sealed class AbortController : JsObject
    {
        private static readonly Func<object[], object?[]?, object?> PrototypeAbortValue =
            PrototypeAbort;
        private static readonly Func<object[], object?[]?, object?> PrototypeSignalGetterValue =
            PrototypeSignalGetter;

        /// <summary>Realm-owned <c>AbortController.prototype</c> intrinsic.</summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.AbortControllerPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));

        public AbortController()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
            signal = new AbortSignal();
        }

        public AbortSignal signal { get; }

        public object? abort(object? reason = null)
        {
            signal.Abort(reason);
            return null;
        }

        internal static void InitializeIntrinsicSurface(object objectPrototype)
        {
            GlobalThis.ConfigureBuiltinFunctionObject(typeof(AbortController));
            PrototypeChain.SetPrototype(Prototype, objectPrototype);

            AbortIntrinsicSurface.DefineConstructorPrototypeSurface(
                typeof(AbortController),
                Prototype);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            Function.InitializeFunctionInstance(PrototypeAbortValue, 1d, "abort");
            Function.MarkUndefinedPrototype(PrototypeAbortValue);
            Function.InitializeFunctionInstance(
                PrototypeSignalGetterValue,
                0d,
                "get signal");
            Function.MarkUndefinedPrototype(PrototypeSignalGetterValue);
            AbortIntrinsicSurface.DefinePrototypeMethod(
                prototype,
                "abort",
                PrototypeAbortValue);
            AbortIntrinsicSurface.DefinePrototypeAccessor(
                prototype,
                "signal",
                PrototypeSignalGetterValue);
        }

        private static object? PrototypeAbort(object[] scopes, object?[]? args)
        {
            GetAbortControllerReceiver("abort").abort(
                args is { Length: > 0 } ? args[0] : null);
            return null;
        }

        private static object? PrototypeSignalGetter(object[] scopes, object?[]? args)
            => GetAbortControllerReceiver("signal").signal;

        private static AbortController GetAbortControllerReceiver(string memberName)
        {
            if (RuntimeServices.GetCurrentThis() is not AbortController controller)
            {
                throw new TypeError(
                    $"AbortController.prototype.{memberName} called on incompatible receiver");
            }

            return controller;
        }
    }

    public sealed class AbortSignal : JsObject
    {
        private static readonly Func<object[], object?[]?, object?> PrototypeAddEventListenerValue =
            PrototypeAddEventListener;
        private static readonly Func<object[], object?[]?, object?> PrototypeRemoveEventListenerValue =
            PrototypeRemoveEventListener;
        private static readonly Func<object[], object?[]?, object?> PrototypeAbortedGetterValue =
            PrototypeAbortedGetter;
        private static readonly Func<object[], object?[]?, object?> PrototypeReasonGetterValue =
            PrototypeReasonGetter;

        /// <summary>Realm-owned <c>AbortSignal.prototype</c> intrinsic.</summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.AbortSignalPrototype,
                static () => new JsObject(),
                static prototype => InitializePrototype(prototype));

        private readonly object _syncRoot = new();
        private readonly List<object> _eventListeners = new();
        private readonly List<Action<object?>> _internalListeners = new();

        public AbortSignal()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
        }

        public bool aborted { get; private set; }

        public object? reason { get; private set; }

        public object? addEventListener(object? eventName, object? listener, object? options = null)
        {
            _ = options;

            if (!IsAbortEventName(eventName) || !CallableOperations.IsCallable(listener))
            {
                return null;
            }

            lock (_syncRoot)
            {
                if (!aborted && !_eventListeners.Contains(listener!))
                {
                    _eventListeners.Add(listener!);
                }
            }

            return null;
        }

        public object? removeEventListener(object? eventName, object? listener, object? options = null)
        {
            _ = options;

            if (!IsAbortEventName(eventName) || !CallableOperations.IsCallable(listener))
            {
                return null;
            }

            lock (_syncRoot)
            {
                _eventListeners.Remove(listener!);
            }

            return null;
        }

        internal bool TryRegisterInternalListener(Action<object?> listener, out Action unregister)
        {
            lock (_syncRoot)
            {
                if (aborted)
                {
                    unregister = static () => { };
                    return false;
                }

                _internalListeners.Add(listener);
            }

            unregister = () =>
            {
                lock (_syncRoot)
                {
                    _internalListeners.Remove(listener);
                }
            };

            return true;
        }

        internal void Abort(object? abortReason = null)
        {
            object[] listeners;
            Action<object?>[] internalListeners;
            object? resolvedReason;

            lock (_syncRoot)
            {
                if (aborted)
                {
                    return;
                }

                aborted = true;
                reason = abortReason ?? new AbortError("This operation was aborted");
                resolvedReason = reason;
                listeners = _eventListeners.ToArray();
                internalListeners = _internalListeners.ToArray();
                _eventListeners.Clear();
                _internalListeners.Clear();
            }

            foreach (var listener in listeners)
            {
                CallableOperations.Call0(listener, null);
            }

            foreach (var listener in internalListeners)
            {
                listener(resolvedReason);
            }
        }

        private static bool IsAbortEventName(object? eventName)
        {
            return string.Equals(DotNet2JSConversions.ToString(eventName), "abort", StringComparison.Ordinal);
        }

        internal static void InitializeIntrinsicSurface(object objectPrototype)
        {
            GlobalThis.ConfigureBuiltinFunctionObject(typeof(AbortSignal));
            PrototypeChain.SetPrototype(Prototype, objectPrototype);

            AbortIntrinsicSurface.DefineConstructorPrototypeSurface(
                typeof(AbortSignal),
                Prototype);
        }

        private static void InitializePrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            Function.InitializeFunctionInstance(
                PrototypeAddEventListenerValue,
                2d,
                "addEventListener");
            Function.MarkUndefinedPrototype(PrototypeAddEventListenerValue);
            Function.InitializeFunctionInstance(
                PrototypeRemoveEventListenerValue,
                2d,
                "removeEventListener");
            Function.MarkUndefinedPrototype(PrototypeRemoveEventListenerValue);
            Function.InitializeFunctionInstance(
                PrototypeAbortedGetterValue,
                0d,
                "get aborted");
            Function.MarkUndefinedPrototype(PrototypeAbortedGetterValue);
            Function.InitializeFunctionInstance(
                PrototypeReasonGetterValue,
                0d,
                "get reason");
            Function.MarkUndefinedPrototype(PrototypeReasonGetterValue);

            AbortIntrinsicSurface.DefinePrototypeAccessor(
                prototype,
                "aborted",
                PrototypeAbortedGetterValue);
            AbortIntrinsicSurface.DefinePrototypeAccessor(
                prototype,
                "reason",
                PrototypeReasonGetterValue);
            AbortIntrinsicSurface.DefinePrototypeMethod(
                prototype,
                "addEventListener",
                PrototypeAddEventListenerValue);
            AbortIntrinsicSurface.DefinePrototypeMethod(
                prototype,
                "removeEventListener",
                PrototypeRemoveEventListenerValue);
        }

        private static object? PrototypeAbortedGetter(object[] scopes, object?[]? args)
            => GetAbortSignalReceiver("aborted").aborted;

        private static object? PrototypeReasonGetter(object[] scopes, object?[]? args)
            => GetAbortSignalReceiver("reason").reason;

        private static object? PrototypeAddEventListener(
            object[] scopes,
            object?[]? args)
        {
            return GetAbortSignalReceiver("addEventListener").addEventListener(
                args is { Length: > 0 } ? args[0] : null,
                args is { Length: > 1 } ? args[1] : null,
                args is { Length: > 2 } ? args[2] : null);
        }

        private static object? PrototypeRemoveEventListener(
            object[] scopes,
            object?[]? args)
        {
            return GetAbortSignalReceiver("removeEventListener").removeEventListener(
                args is { Length: > 0 } ? args[0] : null,
                args is { Length: > 1 } ? args[1] : null,
                args is { Length: > 2 } ? args[2] : null);
        }

        private static AbortSignal GetAbortSignalReceiver(string memberName)
        {
            if (RuntimeServices.GetCurrentThis() is not AbortSignal signal)
            {
                throw new TypeError(
                    $"AbortSignal.prototype.{memberName} called on incompatible receiver");
            }

            return signal;
        }
    }

    internal static class AbortIntrinsicSurface
    {
        internal static void DefineConstructorPrototypeSurface(
            Type constructor,
            JsObject prototype)
        {
            PropertyDescriptorStore.DefineOrUpdate(constructor, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototype
            });
            PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructor
            });
        }

        internal static void DefinePrototypeMethod(
            JsObject prototype,
            string name,
            object method)
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

        internal static void DefinePrototypeAccessor(
            JsObject prototype,
            string name,
            object getter)
        {
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = getter
            });
        }
    }

    public sealed class AbortError : Error
    {
        public AbortError() : this("The operation was aborted")
        {
        }

        public AbortError(string? message) : base(message)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, object? cause) : base(message, cause)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, Exception? innerException) : base(message, innerException)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, Exception? innerException, object? cause) : base(message, innerException, cause)
        {
            Name = "AbortError";
        }

        public string code => "ABORT_ERR";
    }
}
