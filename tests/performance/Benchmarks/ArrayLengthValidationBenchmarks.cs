#if SOURCE_JROC_PROJECTS
using BenchmarkDotNet.Attributes;
using JavaScriptRuntime;
using JsArray = JavaScriptRuntime.Array;

namespace Benchmarks;

// Baselines retain the validators from ab85c3d26 so later runs remain comparable.
internal static class ArrayLengthValidationCandidates
{
    internal static double Original(object? value)
    {
        var newLength = TypeUtilities.ToUint32(value);
        var numberLength = TypeUtilities.ToNumber(value);
        if (newLength != numberLength)
        {
            throw new RangeError("Invalid array length");
        }

        return newLength;
    }

    internal static double DoubleFastPath(object? value)
    {
        if (value is double number)
        {
            var length = TypeUtilities.ToUint32(number);
            if (length != number)
            {
                throw new RangeError("Invalid array length");
            }

            return length;
        }

        return Original(value);
    }

    internal static double StringReuse(object? value)
    {
        var firstNumber = TypeUtilities.ToNumber(value);
        var newLength = TypeUtilities.ToUint32(firstNumber);
        var numberLength = value is string ? firstNumber : TypeUtilities.ToNumber(value);
        if (newLength != numberLength)
        {
            throw new RangeError("Invalid array length");
        }

        return newLength;
    }

    internal static double PrimitiveReuse(object? value)
    {
        var firstNumber = TypeUtilities.ToNumber(value);
        var newLength = TypeUtilities.ToUint32(firstNumber);
        var numberLength = TypeUtilities.IsPrimitive(value)
            ? firstNumber
            : TypeUtilities.ToNumber(value);
        if (newLength != numberLength)
        {
            throw new RangeError("Invalid array length");
        }

        return newLength;
    }

    internal static double Combined(object? value)
    {
        if (value is double number)
        {
            var length = TypeUtilities.ToUint32(number);
            if (length != number)
            {
                throw new RangeError("Invalid array length");
            }

            return length;
        }

        return PrimitiveReuse(value);
    }

    internal static double OriginalNumeric(double value)
    {
        if (double.IsNaN(value)
            || double.IsInfinity(value)
            || value < 0
            || value >= 4294967296d
            || global::System.Math.Truncate(value) != value)
        {
            throw new RangeError("Invalid array length");
        }

        return value;
    }

    internal static double RangeCast(double value)
    {
        if (value >= 0d && value <= uint.MaxValue)
        {
            var length = (uint)value;
            if (length == value)
            {
                return length;
            }
        }

        throw new RangeError("Invalid array length");
    }

    internal static double RangeTruncate(double value)
    {
        if (!(value >= 0d && value <= uint.MaxValue)
            || global::System.Math.Truncate(value) != value)
        {
            throw new RangeError("Invalid array length");
        }

        return value == 0d ? 0d : value;
    }
}

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3)]
public class ArrayLengthObjectValidationBenchmarks
{
    private object[] _values = null!;
    private IDisposable _scope = null!;
    private Action _disposeRealm = null!;

    [Params("Double", "Int32", "String", "LongString", "Object", "Mixed")]
    public string Input { get; set; } = "";

    [GlobalSetup]
    public void Setup()
    {
        var services = RuntimeServices.BuildServiceProvider();
        _scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        _disposeRealm = () => services.OwningRealm!.Agent.Cluster.Dispose();
        double[] numbers = [0d, 1d, 16d, 1024d, 65536d, 2147483647d, 4294967295d, -0d];
        _values = numbers.Select((number, index) => Input switch
        {
            "Double" => (object)number,
            "Int32" => (int)(number % int.MaxValue),
            "String" => number.ToString(global::System.Globalization.CultureInfo.InvariantCulture),
            "LongString" => new string('0', 128) + global::System.Math.Abs(number).ToString(global::System.Globalization.CultureInfo.InvariantCulture),
            "Object" => CreateCoercible(number),
            "Mixed" => (index % 4) switch
            {
                0 => (object)number,
                1 => (int)(number % int.MaxValue),
                2 => number.ToString(global::System.Globalization.CultureInfo.InvariantCulture),
                _ => CreateCoercible(number)
            },
            _ => throw new InvalidOperationException(Input)
        }).ToArray();
        foreach (var value in _values)
        {
            var expected = ArrayLengthValidationCandidates.Original(value);
            if (ArrayLengthValidationCandidates.DoubleFastPath(value) != expected
                || ArrayLengthValidationCandidates.StringReuse(value) != expected
                || ArrayLengthValidationCandidates.PrimitiveReuse(value) != expected
                || ArrayLengthValidationCandidates.Combined(value) != expected
                || JsArray.ValidateLengthValue(value) != expected)
            {
                throw new InvalidOperationException("Validator results differ.");
            }
        }
    }

    private static object CreateCoercible(double number)
    {
        var result = new JsObject();
        object boxed = number;
        ObjectRuntime.SetProperty(result, "valueOf", (Func<object[], object>)(_ => boxed));
        return result;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _scope.Dispose();
        _disposeRealm();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = 8)]
    public double Original()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.Original(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double DoubleFastPath()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.DoubleFastPath(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double StringReuse()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.StringReuse(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double PrimitiveReuse()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.PrimitiveReuse(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double Combined()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.Combined(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double Product()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += JsArray.ValidateLengthValue(value);
        return sum;
    }
}

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3)]
public class ArrayLengthNumericValidationBenchmarks
{
    private readonly double[] _values = [0d, 1d, 16d, 1024d, 65536d, 2147483647d, 4294967295d, -0d];

    [Benchmark(Baseline = true, OperationsPerInvoke = 8)]
    public double Original()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.OriginalNumeric(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double RangeCast()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.RangeCast(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = 8)]
    public double RangeTruncate()
    {
        var sum = 0d;
        foreach (var value in _values)
            sum += ArrayLengthValidationCandidates.RangeTruncate(value);
        return sum;
    }
}
#endif
