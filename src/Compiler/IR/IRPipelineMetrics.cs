using System.Threading;

namespace Js2IL.IR;

/// <summary>
/// Tracks metrics for IR pipeline compilation attempts.
/// Used to audit how many methods are compiled via the new IR pipeline vs falling back to legacy AST→IL.
/// </summary>
public static class IRPipelineMetrics
{
    private static readonly ThreadLocal<int> _mainMethodAttempts = new(() => 0);
    private static readonly ThreadLocal<int> _mainMethodSuccesses = new(() => 0);
    private static readonly ThreadLocal<int> _functionAttempts = new(() => 0);
    private static readonly ThreadLocal<int> _functionSuccesses = new(() => 0);
    private static readonly ThreadLocal<int> _arrowFunctionAttempts = new(() => 0);
    private static readonly ThreadLocal<int> _arrowFunctionSuccesses = new(() => 0);
    private static readonly ThreadLocal<int> _classMethodAttempts = new(() => 0);
    private static readonly ThreadLocal<int> _classMethodSuccesses = new(() => 0);
    private static readonly ThreadLocal<int> _constructorAttempts = new(() => 0);
    private static readonly ThreadLocal<int> _constructorSuccesses = new(() => 0);

    private static readonly ThreadLocal<string?> _lastFailure = new(() => null);

    private static readonly ThreadLocal<bool> _enabled = new(() => false);

    /// <summary>
    /// When true, metrics are collected. Default is false for production.
    /// </summary>
    public static bool Enabled
    {
        get => _enabled.Value;
        set => _enabled.Value = value;
    }

    /// <summary>
    /// Resets all metrics counters to zero.
    /// Note: This only resets metrics for the current thread.
    /// </summary>
    public static void Reset()
    {
        _mainMethodAttempts.Value = 0;
        _mainMethodSuccesses.Value = 0;
        _functionAttempts.Value = 0;
        _functionSuccesses.Value = 0;
        _arrowFunctionAttempts.Value = 0;
        _arrowFunctionSuccesses.Value = 0;
        _classMethodAttempts.Value = 0;
        _classMethodSuccesses.Value = 0;
        _constructorAttempts.Value = 0;
        _constructorSuccesses.Value = 0;

        _lastFailure.Value = null;
    }

    public static void RecordFailure(string message)
    {
        if (!Enabled) return;
        _lastFailure.Value = message;
    }

    /// <summary>
    /// Records a failure message only if no failure has been recorded yet for the current thread.
    /// Useful for preserving the first, most specific failure when outer layers also report failures.
    /// </summary>
    public static void RecordFailureIfUnset(string message)
    {
        if (!Enabled) return;
        _lastFailure.Value ??= message;
    }

    public static string? GetLastFailure() => _lastFailure.Value;

    public static void RecordMainMethodAttempt(bool success)
    {
        if (!Enabled) return;
        _mainMethodAttempts.Value++;
        if (success) _mainMethodSuccesses.Value++;
    }

    public static void RecordFunctionAttempt(bool success)
    {
        if (!Enabled) return;
        _functionAttempts.Value++;
        if (success) _functionSuccesses.Value++;
    }

    public static void RecordArrowFunctionAttempt(bool success)
    {
        if (!Enabled) return;
        _arrowFunctionAttempts.Value++;
        if (success) _arrowFunctionSuccesses.Value++;
    }

    public static void RecordClassMethodAttempt(bool success)
    {
        if (!Enabled) return;
        _classMethodAttempts.Value++;
        if (success) _classMethodSuccesses.Value++;
    }

    public static void RecordConstructorAttempt(bool success)
    {
        if (!Enabled) return;
        _constructorAttempts.Value++;
        if (success) _constructorSuccesses.Value++;
    }

    /// <summary>
    /// Gets a snapshot of current metrics for the current thread.
    /// </summary>
    public static IRPipelineStats GetStats() => new(
        MainMethodAttempts: _mainMethodAttempts.Value,
        MainMethodSuccesses: _mainMethodSuccesses.Value,
        FunctionAttempts: _functionAttempts.Value,
        FunctionSuccesses: _functionSuccesses.Value,
        ArrowFunctionAttempts: _arrowFunctionAttempts.Value,
        ArrowFunctionSuccesses: _arrowFunctionSuccesses.Value,
        ClassMethodAttempts: _classMethodAttempts.Value,
        ClassMethodSuccesses: _classMethodSuccesses.Value,
        ConstructorAttempts: _constructorAttempts.Value,
        ConstructorSuccesses: _constructorSuccesses.Value
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
