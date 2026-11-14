namespace Jroc.Tests;

public sealed class TemporalReviewRegressionTests
{
    [Theory]
    [InlineData("PlainTime", "({ valueOf() { calls.push('coerce'); return 1; } })", "hour", "1")]
    [InlineData("Duration", "({ valueOf() { calls.push('coerce'); return 1; } })", "years", "1")]
    [InlineData("Instant", "({ valueOf() { calls.push('coerce'); return 1n; } })", "epochNanoseconds", "1n")]
    public void ConstructReadsNewTargetPrototypeOnceAfterArgumentCoercion(
        string constructor, string argument, string field, string expected)
    {
        Run(constructor + "_prototype_once", $$"""
            const calls = [], prototype = Object.create(Temporal.{{constructor}}.prototype);
            const alternate = {};
            let reads = 0;
            const newTarget = new Proxy(function() {}, {
              get(target, key, receiver) {
                if (key === "prototype") {
                  calls.push("prototype");
                  return ++reads === 1 ? prototype : alternate;
                }
                return Reflect.get(target, key, receiver);
              }
            });
            const value = Reflect.construct(Temporal.{{constructor}}, [{{argument}}], newTarget);
            assert.sameValue(Object.getPrototypeOf(value), prototype);
            assert.sameValue(value.{{field}}, {{expected}});
            assert.compareArray(calls, ["coerce", "prototype"]);
            """);
    }

    [Theory]
    [InlineData("PlainTime", "[]")]
    [InlineData("Duration", "[]")]
    [InlineData("Instant", "[0n]")]
    public void ConstructPrimitivePrototypeFallsBackWithoutASecondGet(string constructor, string arguments)
    {
        Run(constructor + "_prototype_fallback", $$"""
            const calls = [];
            const newTarget = new Proxy(function() {}, {
              get(target, key, receiver) {
                if (key === "prototype") { calls.push("prototype"); return 42; }
                return Reflect.get(target, key, receiver);
              }
            });
            const value = Reflect.construct(Temporal.{{constructor}}, {{arguments}}, newTarget);
            assert.sameValue(Object.getPrototypeOf(value), Temporal.{{constructor}}.prototype);
            assert.compareArray(calls, ["prototype"]);
            """);
    }

    [Theory]
    [InlineData("PlainTime", "[({ valueOf() { calls.push('coerce'); return -1; } })]", "RangeError")]
    [InlineData("Duration", "[({ valueOf() { calls.push('coerce'); return 1; } }), -1]", "RangeError")]
    [InlineData("Duration", "[({ valueOf() { calls.push('coerce'); return 1.5; } })]", "RangeError")]
    [InlineData("Instant", "[({ valueOf() { calls.push('coerce'); return 1; } })]", "TypeError")]
    [InlineData("Instant", "[({ valueOf() { calls.push('coerce'); return 8640000000000000000001n; } })]", "RangeError")]
    public void ConstructRejectsInvalidArgumentsBeforeReadingNewTargetPrototype(
        string constructor, string arguments, string error)
    {
        Run(constructor + "_invalid_before_prototype", $$"""
            const calls = [];
            const newTarget = new Proxy(function() {}, {
              get(target, key, receiver) {
                if (key === "prototype") {
                  calls.push("prototype");
                  throw new Test262Error("prototype must not be read");
                }
                return Reflect.get(target, key, receiver);
              }
            });
            assert.throws({{error}}, () => Reflect.construct(Temporal.{{constructor}}, {{arguments}}, newTarget));
            assert.compareArray(calls, ["coerce"]);
            """);
    }

    [Theory]
    [InlineData("PlainTime", "[]")]
    [InlineData("Duration", "[]")]
    [InlineData("Instant", "[0n]")]
    public void ConstructPropagatesPrototypeGetterAbruptCompletionOnce(string constructor, string arguments)
    {
        Run(constructor + "_prototype_abrupt", $$"""
            const calls = [], reason = {};
            const newTarget = new Proxy(function() {}, {
              get(target, key, receiver) {
                if (key === "prototype") { calls.push("prototype"); throw reason; }
                return Reflect.get(target, key, receiver);
              }
            });
            let caught;
            try { Reflect.construct(Temporal.{{constructor}}, {{arguments}}, newTarget); }
            catch (error) { caught = error; }
            assert.sameValue(caught, reason);
            assert.compareArray(calls, ["prototype"]);
            """);
    }

    [Theory]
    [InlineData("PlainTime")]
    [InlineData("Duration")]
    [InlineData("Instant")]
    public void ConstructDoesNotReadPrototypeWhenArgumentCoercionThrows(string constructor)
    {
        Run(constructor + "_coercion_abrupt", $$"""
            const calls = [], reason = {};
            const argument = { valueOf() { calls.push("coerce"); throw reason; } };
            const newTarget = new Proxy(function() {}, {
              get(target, key, receiver) {
                if (key === "prototype") { calls.push("prototype"); return {}; }
                return Reflect.get(target, key, receiver);
              }
            });
            let caught;
            try { Reflect.construct(Temporal.{{constructor}}, [argument], newTarget); }
            catch (error) { caught = error; }
            assert.sameValue(caught, reason);
            assert.compareArray(calls, ["coerce"]);
            """);
    }

    [Theory]
    [InlineData("zero", "new Temporal.Duration()", "PT0S")]
    [InlineData("whole-hours", "Temporal.Duration.from('PT1H')", "PT1H")]
    [InlineData("whole-time", "new Temporal.Duration(0, 0, 0, 0, 5, 6, 7)", "PT5H6M7S")]
    [InlineData("fractional", "new Temporal.Duration(0, 0, 0, 0, 0, 0, 7, 8, 9, 10)", "PT7.00800901S")]
    [InlineData("subsecond", "new Temporal.Duration(0, 0, 0, 0, 0, 0, 0, 1, 2, 3)", "PT0.001002003S")]
    [InlineData("fraction-balance", "new Temporal.Duration(0, 0, 0, 0, 0, 0, 0, 1500)", "PT1.5S")]
    [InlineData("negative", "Temporal.Duration.from('-PT1.250S')", "-PT1.25S")]
    [InlineData("calendar", "new Temporal.Duration(1, 2, 3, 4)", "P1Y2M3W4D")]
    [InlineData("mixed", "new Temporal.Duration(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)", "P1Y2M3W4DT5H6M7.00800901S")]
    [InlineData("negative-mixed", "new Temporal.Duration(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10)", "-P1Y2M3W4DT5H6M7.00800901S")]
    public void DurationFormatsIsoForStringAndJsonAndRejectsNumericCoercion(
        string name, string expression, string expected)
    {
        Run("duration_" + name, $$"""
            const duration = {{expression}};
            assert.sameValue(duration.toString(), "{{expected}}");
            assert.sameValue(String(duration), "{{expected}}");
            assert.sameValue(duration.toJSON(), "{{expected}}");
            assert.sameValue(duration.toJSON(Symbol("ignored key")), "{{expected}}");
            assert.sameValue(JSON.stringify(duration), '"{{expected}}"');
            assert.throws(TypeError, () => duration.valueOf());
            assert.throws(TypeError, () => +duration);
            """);
    }

    [Fact]
    public void DurationPrototypeMethodsRejectIncompatibleReceivers()
    {
        Run("duration_incompatible_receivers", """
            for (const method of ["toString", "toJSON", "valueOf"]) {
              assert.throws(TypeError, () => Temporal.Duration.prototype[method].call({}));
              assert.throws(TypeError, () => Temporal.Duration.prototype[method].call(new Temporal.PlainTime()));
              assert.throws(TypeError, () => Temporal.Duration.prototype[method].call(null));
            }
            """);
    }

    [Theory]
    [InlineData("date-carry", "'P1DT23H59M59.999999999S'", "{ smallestUnit: 'second', roundingMode: 'halfExpand' }", "P2DT0S")]
    [InlineData("negative-date-carry", "'-P1DT23H59M59.999999999S'", "{ smallestUnit: 'second', roundingMode: 'halfExpand' }", "-P2DT0S")]
    [InlineData("negative-tie-expand", "'-PT1.5S'", "{ smallestUnit: 'second', roundingMode: 'halfExpand' }", "-PT2S")]
    [InlineData("negative-tie-trunc", "'-PT1.5S'", "{ smallestUnit: 'second', roundingMode: 'halfTrunc' }", "-PT1S")]
    [InlineData("negative-tie-even", "'-PT2.5S'", "{ smallestUnit: 'second', roundingMode: 'halfEven' }", "-PT2S")]
    [InlineData("preserve-hours-auto", "'PT25H'", "{ fractionalSecondDigits: 'auto' }", "PT25H")]
    [InlineData("preserve-minutes-auto", "'PT90M'", "{ fractionalSecondDigits: 'auto' }", "PT90M")]
    [InlineData("preserve-hours-nanosecond", "'PT25H'", "{ smallestUnit: 'nanosecond' }", "PT25H0.000000000S")]
    public void DurationStringPrecisionPreservesUnitsAndRoundsSignedDateCarries(
        string name, string input, string options, string expected)
    {
        Run("duration_precision_" + name, $$"""
            const duration = Temporal.Duration.from({{input}});
            assert.sameValue(duration.toString({{options}}), "{{expected}}");
            """);
    }

    [Theory]
    [InlineData("Europe/+bad")]
    [InlineData("Europe/..")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("./Europe")]
    [InlineData("../Europe")]
    [InlineData("Europe/./City")]
    [InlineData("Europe/../City")]
    [InlineData("Europe/.")]
    [InlineData("Europe/1bad")]
    [InlineData("Europe/-bad")]
    [InlineData("Europe//Paris")]
    public void PlainTimeRejectsInvalidIanaAnnotationComponents(string zone)
    {
        Run("invalid_iana", $$"""
            assert.throws(RangeError, () => Temporal.PlainTime.from("12:34:56.001002003[{{zone}}]"));
            """);
    }

    [Theory]
    [InlineData("Europe/Paris")]
    [InlineData("America/Argentina/Buenos_Aires")]
    [InlineData("Etc/GMT+1")]
    [InlineData("Etc/GMT-1")]
    [InlineData("_Region/_City")]
    [InlineData(".Region/.City")]
    [InlineData("Area/Name_1-2+3")]
    [InlineData("UTC")]
    public void PlainTimeAcceptsValidIanaAnnotationComponentVariants(string zone)
    {
        Run("valid_iana", $$"""
            const time = Temporal.PlainTime.from("12:34:56.001002003[{{zone}}]");
            assert.sameValue(time.hour, 12);
            assert.sameValue(time.minute, 34);
            assert.sameValue(time.second, 56);
            assert.sameValue(time.millisecond, 1);
            assert.sameValue(time.microsecond, 2);
            assert.sameValue(time.nanosecond, 3);
            """);
    }

    private static void Run(string name, string source)
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            name, "TemporalReview",
            _ => ("/*---\nincludes: [compareArray.js]\n---*/\n" + source,
                Path.Combine(Directory.GetCurrentDirectory(), name + ".js")));
        Assert.Null(result.UnhandledException);
        Assert.Empty(result.Output);
    }
}
