using System.Collections;
using System.Reflection;
using JavaScriptRuntime;

namespace Jroc.Runtime;

internal static class JsIterableAdapterFactory
{
    private static readonly MethodInfo CreateEnumerableOpenGeneric = typeof(JsIterableAdapterFactory)
        .GetMethod(nameof(CreateEnumerableCore), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly MethodInfo CreateAsyncEnumerableOpenGeneric = typeof(JsIterableAdapterFactory)
        .GetMethod(nameof(CreateAsyncEnumerableCore), BindingFlags.Static | BindingFlags.NonPublic)!;

    internal static object CreateEnumerable(
        JsRuntimeInstance runtime,
        object target,
        Type elementType,
        string? memberName,
        Type? contractType)
        => CreateEnumerableOpenGeneric
            .MakeGenericMethod(elementType)
            .Invoke(null, [runtime, target, memberName, contractType])!;

    internal static object CreateAsyncEnumerable(
        JsRuntimeInstance runtime,
        object target,
        Type elementType,
        string? memberName,
        Type? contractType)
        => CreateAsyncEnumerableOpenGeneric
            .MakeGenericMethod(elementType)
            .Invoke(null, [runtime, target, memberName, contractType])!;

    private static IEnumerable<T> CreateEnumerableCore<T>(
        JsRuntimeInstance runtime,
        object target,
        string? memberName,
        Type? contractType)
        => new JsEnumerable<T>(runtime, target, memberName, contractType);

    private static IAsyncEnumerable<T> CreateAsyncEnumerableCore<T>(
        JsRuntimeInstance runtime,
        object target,
        string? memberName,
        Type? contractType)
        => new JsAsyncEnumerable<T>(runtime, target, memberName, contractType);
}

internal sealed class JsEnumerable<T> : IEnumerable<T>
{
    private readonly JsRuntimeInstance _runtime;
    private readonly object _target;
    private readonly string? _memberName;
    private readonly Type? _contractType;

    internal JsEnumerable(
        JsRuntimeInstance runtime,
        object target,
        string? memberName,
        Type? contractType)
    {
        _runtime = runtime;
        _target = target;
        _memberName = memberName;
        _contractType = contractType;
    }

    public IEnumerator<T> GetEnumerator()
    {
        try
        {
            var iterator = _runtime.Invoke(() => ObjectRuntime.GetIterator(_target));
            return new JsEnumerator<T>(
                _runtime,
                iterator,
                _memberName,
                _contractType);
        }
        catch (Exception exception)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "GetEnumerator",
                _contractType);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class JsEnumerator<T> : IEnumerator<T>, IRuntimeDisposalParticipant
{
    private readonly JsRuntimeInstance _runtime;
    private readonly IJavaScriptIterator _iterator;
    private readonly string? _memberName;
    private readonly Type? _contractType;
    private readonly object _disposeGate = new();
    private Task? _disposeTask;
    private bool _completed;
    private volatile bool _disposed;
    private bool _hasCurrent;
    private T? _current;

    internal JsEnumerator(
        JsRuntimeInstance runtime,
        IJavaScriptIterator iterator,
        string? memberName,
        Type? contractType)
    {
        _runtime = runtime;
        _iterator = iterator;
        _memberName = memberName;
        _contractType = contractType;
        _runtime.RegisterRuntimeDisposalParticipant(this);
    }

    public T Current
        => _hasCurrent
            ? _current!
            : throw new InvalidOperationException("The JavaScript iterator is not positioned on a value.");

    object? IEnumerator.Current => Current;

    public bool MoveNext()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            var result = _runtime.Invoke(() =>
            {
                var step = _iterator.Next();
                if (step.done)
                {
                    return (HasValue: false, Value: default(T));
                }

                var converted = JsReturnConverter.ConvertReturn(
                    _runtime,
                    step.value,
                    typeof(T),
                    _memberName,
                    _contractType);
                return (HasValue: true, Value: (T?)converted);
            });

            _hasCurrent = result.HasValue;
            _current = result.Value;
            if (!result.HasValue)
            {
                Complete();
            }

            return result.HasValue;
        }
        catch (Exception exception)
        {
            Complete();
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "MoveNext",
                _contractType);
        }
    }

    public void Reset()
        => throw new NotSupportedException("JavaScript iterators cannot be reset.");

    public void Dispose()
        => GetOrStartDisposeTask(forRuntimeShutdown: false).GetAwaiter().GetResult();

    Task IRuntimeDisposalParticipant.DisposeForRuntimeShutdownAsync()
        => GetOrStartDisposeTask(forRuntimeShutdown: true);

    // Host disposal and runtime-shutdown disposal can race from different threads;
    // both paths share one cleanup so iterator return() runs exactly once.
    private Task GetOrStartDisposeTask(bool forRuntimeShutdown)
    {
        lock (_disposeGate)
        {
            return _disposeTask ??= RunDisposeCore(forRuntimeShutdown);
        }
    }

    private Task RunDisposeCore(bool forRuntimeShutdown)
    {
        try
        {
            DisposeCore(forRuntimeShutdown);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    private void DisposeCore(bool forRuntimeShutdown)
    {
        _disposed = true;
        _hasCurrent = false;
        try
        {
            if (!_completed && _iterator.HasReturn)
            {
                if (forRuntimeShutdown)
                {
                    _runtime.InvokeDuringDisposal(_iterator.Return);
                }
                else
                {
                    try
                    {
                        _runtime.Invoke(_iterator.Return);
                    }
                    catch (ObjectDisposedException)
                        when (!_runtime.IsShutdown)
                    {
                        // Runtime shutdown was signaled after this enumerator won the
                        // cleanup race; the script thread is still draining disposal
                        // work, so route return() through the disposal queue.
                        _runtime.InvokeDuringDisposal(_iterator.Return);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "Dispose",
                _contractType);
        }
        finally
        {
            Complete();
        }
    }

    private void Complete()
    {
        if (_completed)
        {
            return;
        }

        _completed = true;
        _runtime.UnregisterRuntimeDisposalParticipant(this);
    }
}

internal sealed class JsAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly JsRuntimeInstance _runtime;
    private readonly object _target;
    private readonly string? _memberName;
    private readonly Type? _contractType;

    internal JsAsyncEnumerable(
        JsRuntimeInstance runtime,
        object target,
        string? memberName,
        Type? contractType)
    {
        _runtime = runtime;
        _target = target;
        _memberName = memberName;
        _contractType = contractType;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var iterator = _runtime.Invoke(() => ObjectRuntime.GetAsyncIterator(_target));
            return new JsAsyncEnumerator<T>(
                _runtime,
                iterator,
                cancellationToken,
                _memberName,
                _contractType);
        }
        catch (Exception exception)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "GetAsyncEnumerator",
                _contractType);
        }
    }
}

internal sealed class JsAsyncEnumerator<T> :
    IAsyncEnumerator<T>,
    IRuntimeDisposalParticipant
{
    private readonly JsRuntimeInstance _runtime;
    private readonly IJavaScriptAsyncIterator _iterator;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _memberName;
    private readonly Type? _contractType;
    private readonly object _disposeGate = new();
    private Task? _disposeTask;
    private bool _completed;
    private bool _disposed;
    private T? _current;

    internal JsAsyncEnumerator(
        JsRuntimeInstance runtime,
        IJavaScriptAsyncIterator iterator,
        CancellationToken cancellationToken,
        string? memberName,
        Type? contractType)
    {
        _runtime = runtime;
        _iterator = iterator;
        _cancellationToken = cancellationToken;
        _memberName = memberName;
        _contractType = contractType;
        _runtime.RegisterRuntimeDisposalParticipant(this);
    }

    public T Current => _current!;

    public async ValueTask<bool> MoveNextAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            _cancellationToken.ThrowIfCancellationRequested();
            var nextPromise = _runtime.Invoke(
                () => (Promise)Promise.resolve(_iterator.Next())!);
            var result = await JsPromiseTaskInterop.ToRawTask(
                    _runtime,
                    nextPromise,
                    _memberName,
                    _contractType)
                .WaitAsync(_cancellationToken)
                .ConfigureAwait(false);

            _cancellationToken.ThrowIfCancellationRequested();
            var step = _runtime.Invoke(() => ReadIteratorResult(result));
            if (step.Done)
            {
                _current = default;
                Complete();
                return false;
            }

            _current = (T?)_runtime.Invoke(() => JsReturnConverter.ConvertReturn(
                _runtime,
                step.Value,
                typeof(T),
                _memberName,
                _contractType));
            return true;
        }
        catch (OperationCanceledException)
        {
            await GetOrStartDisposeTask(forRuntimeShutdown: false).ConfigureAwait(false);
            throw;
        }
        catch (Exception exception)
        {
            Complete();
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "MoveNextAsync",
                _contractType);
        }
    }

    public ValueTask DisposeAsync()
        => new(GetOrStartDisposeTask(forRuntimeShutdown: false));

    Task IRuntimeDisposalParticipant.DisposeForRuntimeShutdownAsync()
        => GetOrStartDisposeTask(forRuntimeShutdown: true);

    private Task GetOrStartDisposeTask(bool forRuntimeShutdown)
    {
        lock (_disposeGate)
        {
            return _disposeTask ??= DisposeCoreAsync(forRuntimeShutdown);
        }
    }

    private async Task DisposeCoreAsync(bool forRuntimeShutdown)
    {
        _disposed = true;
        try
        {
            if (!_completed && _iterator.HasReturn)
            {
                object? InvokeReturn()
                    => _iterator is AsyncGeneratorObject generator
                        ? ExportMemberResolver.InvokeInstanceMethod(
                            _runtime,
                            generator,
                            "return",
                            [null])
                        : _iterator.Return();

                var returnPromise = forRuntimeShutdown
                    ? _runtime.InvokeDuringDisposal(
                        () => (Promise)Promise.resolve(InvokeReturn())!)
                    : _runtime.Invoke(
                        () => (Promise)Promise.resolve(InvokeReturn())!);
                _ = await JsPromiseTaskInterop.ToRawTask(
                        _runtime,
                        returnPromise,
                        _memberName,
                        _contractType,
                        forRuntimeShutdown)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            throw JsHostingExceptionTranslator.TranslateProxyCall(
                exception,
                _runtime,
                _memberName ?? "DisposeAsync",
                _contractType);
        }
        finally
        {
            Complete();
        }
    }

    private (bool Done, object? Value) ReadIteratorResult(object? result)
    {
        if (result is IIteratorResult iteratorResult)
        {
            return (iteratorResult.done, iteratorResult.value);
        }

        if (result == null
            || result is JsNull
            || TypeUtilities.IsPrimitive(result))
        {
            throw new TypeError("Async iterator next() did not return an object.");
        }

        return (
            TypeUtilities.ToBoolean(ObjectRuntime.GetProperty(result, "done")),
            ObjectRuntime.GetProperty(result, "value"));
    }

    private void Complete()
    {
        if (_completed)
        {
            return;
        }

        _completed = true;
        _runtime.UnregisterRuntimeDisposalParticipant(this);
    }
}
