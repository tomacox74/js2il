namespace JavaScriptRuntime;

/// <summary>
/// Base class for compiled async generator leaf scopes.
///
/// This combines the async state machine fields (<see cref="AsyncScope"/>) with the generator
/// resume protocol fields (<see cref="GeneratorScope"/>) so the compiler can emit both await
/// and yield semantics within the same callable.
/// </summary>
public class AsyncGeneratorScope : AsyncScope
{
    // Generator state machine fields
    public int _genState;
    public bool _started;
    public bool _done;

    // yield* delegation state (mirrors GeneratorScope)
    // _yieldStarMode: 0 = not delegating, 1 = indexable (NormalizeForOfIterable), 2 = iterator (GeneratorObject)
    // Stored as double to match the compiler's numeric constant model.
    public double _yieldStarMode;
    public object? _yieldStarTarget;
    public double _yieldStarIndex;
    public double _yieldStarLength;

    // Resume protocol inputs
    public object? _resumeValue;
    public object? _resumeException;
    public bool _hasResumeException;

    public object? _returnValue;
    public bool _hasReturn;

    public int GenState
    {
        get => _genState;
        set => _genState = value;
    }

    public bool Started
    {
        get => _started;
        set => _started = value;
    }

    public bool Done
    {
        get => _done;
        set => _done = value;
    }

    public object? ResumeValue
    {
        get => _resumeValue;
        set => _resumeValue = value;
    }

    public object? ResumeException
    {
        get => _resumeException;
        set => _resumeException = value;
    }

    public bool HasResumeException
    {
        get => _hasResumeException;
        set => _hasResumeException = value;
    }

    public object? ReturnValue
    {
        get => _returnValue;
        set => _returnValue = value;
    }

    public bool HasReturn
    {
        get => _hasReturn;
        set => _hasReturn = value;
    }
}
