namespace JavaScriptRuntime;

public class AsyncScope : IAsyncScope
{
    // State machine fields
    public int _asyncState;
    public PromiseWithResolvers? _deferred;
    public object? _moveNext;

    // Async try/finally completion tracking
    public object? _pendingException;
    public bool _hasPendingException;

    public object? _pendingReturnValue;
    public bool _hasPendingReturn;

    public int AsyncState
    {
        get => _asyncState;
        set => _asyncState = value;
    }

    public PromiseWithResolvers? Deferred
    {
        get => _deferred;
        set => _deferred = value;
    }

    public object? MoveNext
    {
        get => _moveNext;
        set => _moveNext = value;
    }
}
