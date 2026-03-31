using System;
using System.Linq;
using System.Reflection;

namespace JavaScriptRuntime;

public static class Iterator
{
    internal static readonly object Prototype = CreatePrototype();
    internal static readonly object HelperPrototype = CreateHelperPrototype();

    internal static void ConfigureIntrinsicSurface(object iteratorConstructorValue)
    {
        DefineDataProperty(iteratorConstructorValue, "prototype", Prototype);
        DefineDataProperty(iteratorConstructorValue, "from", (Func<object[], object?[]?, object?>)ConstructorFrom);

        DefineDataProperty(Prototype, "constructor", iteratorConstructorValue);
        DefineDataProperty(Prototype, "next", (Func<object[], object?[]?, object?>)PrototypeNext);
        DefineDataProperty(Prototype, "return", (Func<object[], object?[]?, object?>)PrototypeReturn);
        DefineDataProperty(Prototype, "drop", (Func<object[], object?[]?, object?>)PrototypeDrop);
        DefineDataProperty(Prototype, "every", (Func<object[], object?[]?, object?>)PrototypeEvery);
        DefineDataProperty(Prototype, "filter", (Func<object[], object?[]?, object?>)PrototypeFilter);
        DefineDataProperty(Prototype, "find", (Func<object[], object?[]?, object?>)PrototypeFind);
        DefineDataProperty(Prototype, "flatMap", (Func<object[], object?[]?, object?>)PrototypeFlatMap);
        DefineDataProperty(Prototype, "forEach", (Func<object[], object?[]?, object?>)PrototypeForEach);
        DefineDataProperty(Prototype, "map", (Func<object[], object?[]?, object?>)PrototypeMap);
        DefineDataProperty(Prototype, "reduce", (Func<object[], object?[]?, object?>)PrototypeReduce);
        DefineDataProperty(Prototype, "some", (Func<object[], object?[]?, object?>)PrototypeSome);
        DefineDataProperty(Prototype, "take", (Func<object[], object?[]?, object?>)PrototypeTake);
        DefineDataProperty(Prototype, "toArray", (Func<object[], object?[]?, object?>)PrototypeToArray);
        DefineDataProperty(Prototype, Symbol.iterator.DebugId, (Func<object[], object?[]?, object?>)PrototypeSymbolIterator);
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "Iterator");

        DefineDataProperty(HelperPrototype, "next", (Func<object[], object?[]?, object?>)PrototypeNext);
        DefineDataProperty(HelperPrototype, "return", (Func<object[], object?[]?, object?>)PrototypeReturn);
        DefineDataProperty(HelperPrototype, Symbol.iterator.DebugId, (Func<object[], object?[]?, object?>)PrototypeSymbolIterator);
        DefineDataProperty(HelperPrototype, Symbol.toStringTag.DebugId, "Iterator Helper");
    }

    internal static void InitializeIteratorSurface(object iterator)
    {
        if (PrototypeChain.GetPrototypeOrNull(iterator) == null)
        {
            PrototypeChain.SetPrototype(iterator, Prototype);
        }
    }

    internal static void InitializeHelperSurface(object iterator)
    {
        PrototypeChain.SetPrototype(iterator, HelperPrototype);
    }

    public static IJavaScriptIterator From(object? value)
    {
        if (value is IJavaScriptIterator iterator)
        {
            InitializeIteratorSurface(iterator);
            return iterator;
        }

        if (TryCreateIteratorLikeWrapper(value, out var iteratorLike))
        {
            return iteratorLike;
        }

        var wrapped = Object.GetIterator(value);
        InitializeIteratorSurface(wrapped);
        return wrapped;
    }

    private static object CreatePrototype()
    {
        return new JsObject();
    }

    private static object CreateHelperPrototype()
    {
        var prototype = new JsObject();
        PrototypeChain.SetPrototype(prototype, Prototype);
        return prototype;
    }

    private static void DefineDataProperty(object target, string key, object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    private static object? ConstructorFrom(object[] scopes, object?[]? args)
    {
        return From(args != null && args.Length > 0 ? args[0] : null);
    }

    private static object? PrototypeNext(object[] scopes, object?[]? args)
    {
        return GetReceiverIterator("next").Next();
    }

    private static object? PrototypeReturn(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("return");
        if (iterator.HasReturn)
        {
            iterator.Return();
        }

        return IteratorResult.Create(null, done: true);
    }

    private static object? PrototypeDrop(object[] scopes, object?[]? args)
    {
        return new DropIteratorHelper(GetReceiverIterator("drop"), GetNonNegativeInteger(args, "drop"));
    }

    private static object? PrototypeEvery(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("every");
        var predicate = GetRequiredCallback(args, "every");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return true;
                }

                if (!Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return false;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeFilter(object[] scopes, object?[]? args)
    {
        return new FilterIteratorHelper(GetReceiverIterator("filter"), GetRequiredCallback(args, "filter"));
    }

    private static object? PrototypeFind(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("find");
        var predicate = GetRequiredCallback(args, "find");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return null;
                }

                if (Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return step.value;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeFlatMap(object[] scopes, object?[]? args)
    {
        return new FlatMapIteratorHelper(GetReceiverIterator("flatMap"), GetRequiredCallback(args, "flatMap"));
    }

    private static object? PrototypeForEach(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("forEach");
        var procedure = GetRequiredCallback(args, "forEach");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return null;
                }

                _ = InvokeCallback(procedure, step.value, (double)index++);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeMap(object[] scopes, object?[]? args)
    {
        return new MapIteratorHelper(GetReceiverIterator("map"), GetRequiredCallback(args, "map"));
    }

    private static object? PrototypeReduce(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("reduce");
        var reducer = GetRequiredCallback(args, "reduce");
        bool hasInitialValue = args != null && args.Length > 1;
        object? accumulator = null;
        long index = 0;

        try
        {
            if (hasInitialValue)
            {
                accumulator = args![1];
            }
            else
            {
                var first = iterator.Next();
                if (first.done)
                {
                    throw new TypeError("Reduce of empty iterator with no initial value");
                }

                accumulator = first.value;
                index = 1;
            }

            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return accumulator;
                }

                accumulator = InvokeCallback(reducer, accumulator, step.value, (double)index++);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeSome(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("some");
        var predicate = GetRequiredCallback(args, "some");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return false;
                }

                if (Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return true;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeTake(object[] scopes, object?[]? args)
    {
        return new TakeIteratorHelper(GetReceiverIterator("take"), GetNonNegativeInteger(args, "take"));
    }

    private static object? PrototypeToArray(object[] scopes, object?[]? args)
    {
        var iterator = GetReceiverIterator("toArray");
        var result = new JavaScriptRuntime.Array();

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return result;
                }

                result.Add(step.value);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeSymbolIterator(object[] scopes, object?[]? args)
    {
        return RuntimeServices.GetCurrentThis();
    }

    private static IJavaScriptIterator GetReceiverIterator(string methodName)
    {
        var thisValue = RuntimeServices.GetCurrentThis();
        if (thisValue is IJavaScriptIterator iterator)
        {
            return iterator;
        }

        if (thisValue is GeneratorObject generator)
        {
            return new GeneratorIteratorAdapter(generator);
        }

        if (TryCreateIteratorLikeWrapper(thisValue, out var iteratorLike))
        {
            return iteratorLike;
        }

        throw new TypeError($"Iterator.prototype.{methodName} called on incompatible receiver");
    }

    private static bool TryCreateIteratorLikeWrapper(object? value, out IJavaScriptIterator iterator)
    {
        if (value is not null && value is not JsNull && CallableMember.TryCreate(value, "next", out var next))
        {
            iterator = new IteratorLikeWrapper(value, next);
            return true;
        }

        iterator = default!;
        return false;
    }

    private static Delegate GetRequiredCallback(object?[]? args, string methodName)
    {
        var callback = args != null && args.Length > 0 ? args[0] : null;
        if (callback is Delegate del)
        {
            return del;
        }

        throw new TypeError($"Iterator.prototype.{methodName} requires a callback function");
    }

    private static double GetNonNegativeInteger(object?[]? args, string methodName)
    {
        if (args == null || args.Length == 0)
        {
            throw new TypeError($"Iterator.prototype.{methodName} requires a limit");
        }

        var value = TypeUtilities.ToNumber(args[0]);
        if (double.IsNaN(value))
        {
            return 0;
        }

        if (double.IsNegativeInfinity(value) || value < 0)
        {
            throw new RangeError($"Iterator.prototype.{methodName} requires a non-negative limit");
        }

        if (double.IsPositiveInfinity(value))
        {
            return double.PositiveInfinity;
        }

        return System.Math.Truncate(value);
    }

    private static object? InvokeCallback(Delegate callback, params object?[] args)
    {
        var previousThis = RuntimeServices.SetCurrentThis(null);
        try
        {
            return Closure.InvokeWithArgs(callback, System.Array.Empty<object>(), args);
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    private static void CloseIterator(IJavaScriptIterator iterator)
    {
        if (iterator.HasReturn)
        {
            iterator.Return();
        }
    }

    private abstract class IteratorHelperBase : IJavaScriptIterator
    {
        private bool _closed;

        protected IteratorHelperBase(IJavaScriptIterator source)
        {
            Source = source;
            InitializeHelperSurface(this);
        }

        protected IJavaScriptIterator Source { get; }

        protected bool Done { get; set; }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            if (Done)
            {
                return IteratorResult.Create(null, true);
            }

            try
            {
                return NextCore();
            }
            catch
            {
                Abort();
                throw;
            }
        }

        public void Return()
        {
            Done = true;
            CloseEarly();
        }

        protected abstract IteratorResultObject NextCore();

        protected IteratorResultObject Finish(object? value = null)
        {
            Done = true;
            return IteratorResult.Create(value, true);
        }

        protected IteratorResultObject FinishAndClose(object? value = null)
        {
            Done = true;
            CloseEarly();
            return IteratorResult.Create(value, true);
        }

        protected virtual void Abort()
        {
            Done = true;
            CloseSource();
        }

        protected virtual void CloseEarly()
        {
            CloseSource();
        }

        protected void CloseSource()
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            OnClose();
            if (Source.HasReturn)
            {
                Source.Return();
            }
        }

        protected virtual void OnClose()
        {
        }
    }

    private sealed class MapIteratorHelper : IteratorHelperBase
    {
        private readonly Delegate _mapper;
        private long _index;

        public MapIteratorHelper(IJavaScriptIterator source, Delegate mapper)
            : base(source)
        {
            _mapper = mapper;
        }

        protected override IteratorResultObject NextCore()
        {
            var step = Source.Next();
            if (step.done)
            {
                return Finish();
            }

            return IteratorResult.Create(InvokeCallback(_mapper, step.value, (double)_index++), false);
        }
    }

    private sealed class FilterIteratorHelper : IteratorHelperBase
    {
        private readonly Delegate _predicate;
        private long _index;

        public FilterIteratorHelper(IJavaScriptIterator source, Delegate predicate)
            : base(source)
        {
            _predicate = predicate;
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                var step = Source.Next();
                if (step.done)
                {
                    return Finish();
                }

                if (Operators.IsTruthy(InvokeCallback(_predicate, step.value, (double)_index++)))
                {
                    return IteratorResult.Create(step.value, false);
                }
            }
        }
    }

    private sealed class DropIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _dropped;

        public DropIteratorHelper(IJavaScriptIterator source, double limit)
            : base(source)
        {
            _limit = limit;
        }

        protected override IteratorResultObject NextCore()
        {
            while (_dropped < _limit)
            {
                var skipped = Source.Next();
                if (skipped.done)
                {
                    return Finish();
                }

                _dropped++;
            }

            var step = Source.Next();
            return step.done ? Finish() : step;
        }
    }

    private sealed class TakeIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _taken;

        public TakeIteratorHelper(IJavaScriptIterator source, double limit)
            : base(source)
        {
            _limit = limit;
        }

        protected override IteratorResultObject NextCore()
        {
            if (_taken >= _limit)
            {
                return FinishAndClose();
            }

            var step = Source.Next();
            if (step.done)
            {
                return Finish();
            }

            _taken++;
            return step;
        }
    }

    private sealed class FlatMapIteratorHelper : IteratorHelperBase
    {
        private readonly Delegate _mapper;
        private long _index;
        private IJavaScriptIterator? _inner;

        public FlatMapIteratorHelper(IJavaScriptIterator source, Delegate mapper)
            : base(source)
        {
            _mapper = mapper;
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                if (_inner != null)
                {
                    var innerStep = _inner.Next();
                    if (!innerStep.done)
                    {
                        return innerStep;
                    }

                    ReleaseInner();
                }

                var step = Source.Next();
                if (step.done)
                {
                    return Finish();
                }

                var mapped = InvokeCallback(_mapper, step.value, (double)_index++);
                _inner = From(mapped);
            }
        }

        protected override void OnClose()
        {
            CloseInner();
        }

        protected override void Abort()
        {
            ReleaseInner();
            base.Abort();
        }

        protected override void CloseEarly()
        {
            CloseInner();
            base.CloseEarly();
        }

        private void CloseInner()
        {
            if (_inner == null)
            {
                return;
            }

            if (_inner.HasReturn)
            {
                _inner.Return();
            }

            _inner = null;
        }

        private void ReleaseInner()
        {
            _inner = null;
        }
    }

    private readonly record struct CallableMember(Delegate? Delegate, MethodInfo? Method)
    {
        public static bool TryCreate(object target, string name, out CallableMember member)
        {
            var propertyValue = Object.GetProperty(target, name);
            if (propertyValue is Delegate del)
            {
                member = new CallableMember(del, null);
                return true;
            }

            var method = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(candidate =>
                    string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase)
                    && candidate.GetParameters().Length <= 1);

            if (method != null)
            {
                member = new CallableMember(null, method);
                return true;
            }

            member = default;
            return false;
        }

        public bool HasValue => Delegate != null || Method != null;

        public object? Invoke(object target, params object?[] args)
        {
            var previousThis = RuntimeServices.SetCurrentThis(target);
            try
            {
                if (Delegate != null)
                {
                    return Closure.InvokeWithArgs(Delegate, System.Array.Empty<object>(), args);
                }

                if (Method == null)
                {
                    return null;
                }

                var parameters = Method.GetParameters();
                return Method.Invoke(target, parameters.Length == 0 ? System.Array.Empty<object?>() : args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }
    }

    private sealed class GeneratorIteratorAdapter : IJavaScriptIterator
    {
        private readonly GeneratorObject _generator;

        public GeneratorIteratorAdapter(GeneratorObject generator)
        {
            _generator = generator;
        }

        public bool HasReturn => true;

        public IteratorResultObject Next()
            => (IteratorResultObject)_generator.next();

        public void Return()
        {
            _ = _generator.@return(null);
        }
    }

    private sealed class IteratorLikeWrapper : IJavaScriptIterator
    {
        private readonly object _target;
        private readonly CallableMember _next;
        private readonly CallableMember _return;

        public IteratorLikeWrapper(object target, CallableMember next)
        {
            _target = target;
            _next = next;
            _ = CallableMember.TryCreate(target, "return", out _return);
            InitializeIteratorSurface(this);
        }

        public bool HasReturn => _return.HasValue;

        public IteratorResultObject Next()
        {
            var result = _next.Invoke(_target);
            if (result is IteratorResultObject iteratorResult)
            {
                return iteratorResult;
            }

            if (result is IIteratorResult typedResult)
            {
                return IteratorResult.Create(typedResult.value, typedResult.done);
            }

            if (result == null)
            {
                throw new TypeError("Iterator.next() returned null or undefined");
            }

            var done = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(result, "done"));
            var value = ObjectRuntime.GetItem(result, "value");
            return IteratorResult.Create(value, done);
        }

        public void Return()
        {
            if (HasReturn)
            {
                _ = _return.Invoke(_target);
            }
        }
    }
}
