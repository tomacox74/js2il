using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace JavaScriptRuntime;

internal sealed class RegExpCompilationBudget
{
    private int _count;

    internal RegExpCompilationBudget(int capacity, int threshold = 3, bool? enabled = null)
    {
        Capacity = capacity;
        Threshold = threshold;
        Enabled = enabled ?? RuntimeFeature.IsDynamicCodeCompiled;
    }

    internal int Capacity { get; }
    internal int Threshold { get; }
    internal bool Enabled { get; }
    internal int Count => Volatile.Read(ref _count);

    internal bool TryReserve()
    {
        var count = Count;
        while (count < Capacity)
        {
            var observed = Interlocked.CompareExchange(ref _count, count + 1, count);
            if (observed == count)
            {
                return true;
            }
            count = observed;
        }
        return false;
    }

    internal void Release() => Interlocked.Decrement(ref _count);
}

internal sealed class RegExpAdaptiveCompilation
{
    private const int Interpreted = 0;
    private const int Promoting = 1;
    private const int Compiled = 2;
    private const int Disabled = 3;
    private const int Retired = 4;

    private readonly RegExpCompilationBudget _budget;
    private readonly Func<Regex, Regex>? _compile;
    private readonly object _gate = new();
    private int _constructions;
    private int _state;
    private bool _retired;
    private Regex? _compiled;
    private string? _failure;

    internal RegExpAdaptiveCompilation(
        RegExpCompilationBudget budget, Func<Regex, Regex>? compile = null, int initialConstructionCount = 0)
    {
        _budget = budget;
        _compile = compile;
        _constructions = initialConstructionCount;
        _state = budget.Enabled ? Interpreted : Disabled;
    }

    internal string? PromotionFailure => Volatile.Read(ref _failure);

    internal Regex GetRegexForConstruction(Regex interpreted)
    {
        var compiled = Volatile.Read(ref _compiled);
        if (compiled is not null)
        {
            return compiled;
        }
        if (Volatile.Read(ref _state) != Interpreted
            || Interlocked.Increment(ref _constructions) != _budget.Threshold)
        {
            return interpreted;
        }

        lock (_gate)
        {
            if (_state != Interpreted)
            {
                return interpreted;
            }
            if (!_budget.TryReserve())
            {
                Volatile.Write(ref _state, Disabled);
                return interpreted;
            }
            Volatile.Write(ref _state, Promoting);
        }

        Regex? promoted = null;
        var published = false;
        try
        {
            // Compilation is deliberately outside the entry lock. Other constructors
            // keep using the interpreted tier, and eviction can retire an in-flight entry.
            promoted = _compile is null
                ? new Regex(interpreted.ToString(), interpreted.Options | RegexOptions.Compiled, interpreted.MatchTimeout)
                : _compile(interpreted);
        }
        catch (Exception error) when (error is NotSupportedException or ArgumentException)
        {
            Volatile.Write(ref _failure, error.Message);
            Trace.TraceWarning("RegExp compiled-tier promotion failed; keeping the interpreted tier: {0}", error.Message);
        }
        finally
        {
            lock (_gate)
            {
                if (!_retired && promoted is not null)
                {
                    Volatile.Write(ref _compiled, promoted);
                    Volatile.Write(ref _state, Compiled);
                    published = true;
                }
                else
                {
                    Volatile.Write(ref _state, _retired ? Retired : Disabled);
                    _budget.Release();
                }
            }
        }
        return published ? promoted! : interpreted;
    }

    internal void Retire()
    {
        lock (_gate)
        {
            if (_retired)
            {
                return;
            }
            _retired = true;
            if (_state == Compiled)
            {
                Volatile.Write(ref _compiled, null);
                _budget.Release();
            }
            // In-flight compilation retains its reservation until it completes.
            // Releasing it here would allow an eviction flood to bypass the cap.
            if (_state != Promoting)
            {
                Volatile.Write(ref _state, Retired);
            }
        }
    }
}
