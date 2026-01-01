namespace Js2IL.IR;

/// <summary>
/// Tracks metrics for IR pipeline compilation attempts.
/// Used to audit how many methods are compiled via the new IR pipeline vs falling back to legacy AST→IL.
/// </summary>
public static class IRPipelineMetrics
{
    private static int _mainMethodAttempts;
    private static int _mainMethodSuccesses;
    private static int _functionAttempts;
    private static int _functionSuccesses;
    private static int _arrowFunctionAttempts;
    private static int _arrowFunctionSuccesses;
    private static int _classMethodAttempts;
    private static int _classMethodSuccesses;
    private static int _constructorAttempts;
    private static int _constructorSuccesses;

    /// <summary>
    /// When true, metrics are collected. Default is false for production.
    /// </summary>
    public static bool Enabled { get; set; } = false;

    public static void Reset()
    {
        _mainMethodAttempts = 0;
        _mainMethodSuccesses = 0;
        _functionAttempts = 0;
        _functionSuccesses = 0;
        _arrowFunctionAttempts = 0;
        _arrowFunctionSuccesses = 0;
        _classMethodAttempts = 0;
        _classMethodSuccesses = 0;
        _constructorAttempts = 0;
        _constructorSuccesses = 0;
    }

    public static void RecordMainMethodAttempt(bool success)
    {
        if (!Enabled) return;
        Interlocked.Increment(ref _mainMethodAttempts);
        if (success) Interlocked.Increment(ref _mainMethodSuccesses);
    }

    public static void RecordFunctionAttempt(bool success)
    {
        if (!Enabled) return;
        Interlocked.Increment(ref _functionAttempts);
        if (success) Interlocked.Increment(ref _functionSuccesses);
    }

    public static void RecordArrowFunctionAttempt(bool success)
    {
        if (!Enabled) return;
        Interlocked.Increment(ref _arrowFunctionAttempts);
        if (success) Interlocked.Increment(ref _arrowFunctionSuccesses);
    }

    public static void RecordClassMethodAttempt(bool success)
    {
        if (!Enabled) return;
        Interlocked.Increment(ref _classMethodAttempts);
        if (success) Interlocked.Increment(ref _classMethodSuccesses);
    }

    public static void RecordConstructorAttempt(bool success)
    {
        if (!Enabled) return;
        Interlocked.Increment(ref _constructorAttempts);
        if (success) Interlocked.Increment(ref _constructorSuccesses);
    }

    public static IRPipelineStats GetStats() => new(
        MainMethodAttempts: _mainMethodAttempts,
        MainMethodSuccesses: _mainMethodSuccesses,
        FunctionAttempts: _functionAttempts,
        FunctionSuccesses: _functionSuccesses,
        ArrowFunctionAttempts: _arrowFunctionAttempts,
        ArrowFunctionSuccesses: _arrowFunctionSuccesses,
        ClassMethodAttempts: _classMethodAttempts,
        ClassMethodSuccesses: _classMethodSuccesses,
        ConstructorAttempts: _constructorAttempts,
        ConstructorSuccesses: _constructorSuccesses
    );
}

public readonly record struct IRPipelineStats(
    int MainMethodAttempts,
    int MainMethodSuccesses,
    int FunctionAttempts,
    int FunctionSuccesses,
    int ArrowFunctionAttempts,
    int ArrowFunctionSuccesses,
    int ClassMethodAttempts,
    int ClassMethodSuccesses,
    int ConstructorAttempts,
    int ConstructorSuccesses)
{
    public int TotalAttempts => MainMethodAttempts + FunctionAttempts + ArrowFunctionAttempts + ClassMethodAttempts + ConstructorAttempts;
    public int TotalSuccesses => MainMethodSuccesses + FunctionSuccesses + ArrowFunctionSuccesses + ClassMethodSuccesses + ConstructorSuccesses;
    public int TotalFallbacks => TotalAttempts - TotalSuccesses;
    public double SuccessRate => TotalAttempts == 0 ? 0 : (double)TotalSuccesses / TotalAttempts * 100;

    public override string ToString() => $"""
        IR Pipeline Metrics:
        ====================
        Main Methods:     {MainMethodSuccesses,4} / {MainMethodAttempts,4} ({(MainMethodAttempts == 0 ? 0 : MainMethodSuccesses * 100.0 / MainMethodAttempts):F1}%)
        Functions:        {FunctionSuccesses,4} / {FunctionAttempts,4} ({(FunctionAttempts == 0 ? 0 : FunctionSuccesses * 100.0 / FunctionAttempts):F1}%)
        Arrow Functions:  {ArrowFunctionSuccesses,4} / {ArrowFunctionAttempts,4} ({(ArrowFunctionAttempts == 0 ? 0 : ArrowFunctionSuccesses * 100.0 / ArrowFunctionAttempts):F1}%)
        Class Methods:    {ClassMethodSuccesses,4} / {ClassMethodAttempts,4} ({(ClassMethodAttempts == 0 ? 0 : ClassMethodSuccesses * 100.0 / ClassMethodAttempts):F1}%)
        Constructors:     {ConstructorSuccesses,4} / {ConstructorAttempts,4} ({(ConstructorAttempts == 0 ? 0 : ConstructorSuccesses * 100.0 / ConstructorAttempts):F1}%)
        -------------------
        TOTAL:            {TotalSuccesses,4} / {TotalAttempts,4} ({SuccessRate:F1}%)
        Legacy Fallbacks: {TotalFallbacks,4}
        """;
}
