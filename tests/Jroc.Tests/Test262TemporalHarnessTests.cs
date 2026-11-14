namespace Jroc.Tests;

public sealed class Test262TemporalHarnessTests
{
    [Theory]
    [InlineData("number", "1")]
    [InlineData("string", "'true'")]
    [InlineData("object", "({})")]
    [InlineData("boxed-boolean", "new Boolean(true)")]
    [InlineData("false", "false")]
    public void PlainTimesEqualRejectsNonTrueEqualsResults(string name, string expression)
    {
        var source = $$"""
            const actual = new Temporal.PlainTime(1, 2, 3);
            const expected = new Temporal.PlainTime(1, 2, 3);
            let calls = 0;
            actual.equals = function(value) {
              assert.sameValue(this, actual);
              assert.sameValue(value, expected);
              calls++;
              return {{expression}};
            };
            let caught;
            try { TemporalHelpers.assertPlainTimesEqual(actual, expected); }
            catch (error) { caught = error; }
            assert.sameValue(caught.name, "AssertionError");
            assert.sameValue(caught.operator, "strictEqual");
            assert.sameValue(caught.expected, true);
            assert.sameValue(calls, 1);
            actual.equals = () => true;
            TemporalHelpers.assertPlainTimesEqual(actual, expected);
            """;
        AssertHelperScript(name, source);
    }

    [Fact]
    public void PlainTimeAssertionsRejectNonInstancesBeforeReadingFieldsOrEquals()
    {
        AssertHelperScript("noninstances", """
            let calls = 0;
            const fake = {
              get hour() { calls++; return 1; },
              equals() { calls++; return true; }
            };
            const valid = new Temporal.PlainTime(1, 2, 3, 4, 5, 6);
            function reject(callback) {
              let caught;
              try { callback(); } catch (error) { caught = error; }
              assert.sameValue(caught.name, "AssertionError");
              assert.sameValue(caught.operator, "strictEqual");
            }
            reject(() => TemporalHelpers.assertPlainTime(fake, 1, 2, 3, 4, 5, 6));
            reject(() => TemporalHelpers.assertPlainTimesEqual(fake, valid));
            reject(() => TemporalHelpers.assertPlainTimesEqual(valid, fake));
            assert.sameValue(calls, 0);
            TemporalHelpers.assertPlainTime(valid, 1, 2, 3, 4, 5, 6);
            TemporalHelpers.assertPlainTimesEqual(valid, new Temporal.PlainTime(1, 2, 3, 4, 5, 6));
            """);
    }

    private static void AssertHelperScript(string name, string source)
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "strict_temporal_" + name, "Test262TemporalHarness",
            _ => ("/*---\nincludes: [temporalHelpers.js]\n---*/\n" + source,
                Path.Combine(Directory.GetCurrentDirectory(), "strict_temporal_" + name + ".js")));
        Assert.Null(result.UnhandledException);
        Assert.Empty(result.Output);
    }

    [Fact]
    public void ObserveMethodPreservesReceiverArgumentsAndAbruptCompletion()
    {
        const string source = """
            /*---
            includes: [temporalHelpers.js, compareArray.js]
            ---*/
            assert.sameValue(TemporalHelpers.observeMethod.length, 3);
            const calls = [], expected = {}, target = {
              run(first, second) {
                assert.sameValue(this, target);
                assert.sameValue(arguments.length, 2);
                assert.sameValue(first, 1);
                assert.sameValue(second, 2);
                return expected;
              }
            };
            const original = target.run;
            TemporalHelpers.observeMethod(calls, target, "run", "target");
            assert.notSameValue(target.run, original);
            assert.sameValue(target.run.length, 0);
            assert.sameValue(target.run.call({}, 1, 2), expected);
            assert.sameValue(new target.run(1, 2), expected);
            assert.compareArray(calls, ["call target.run", "call target.run"]);
            const absent = {};
            TemporalHelpers.observeMethod(calls, absent, "missing");
            assert.throws(TypeError, () => absent.missing());
            assert.sameValue(calls[2], "call missing");
            const reason = {}, throwing = { run() { throw reason; } };
            TemporalHelpers.observeMethod(calls, throwing, "run");
            let caught;
            try { throwing.run(); } catch (error) { caught = error; }
            assert.sameValue(caught, reason);
            assert.sameValue(calls[3], "call run");
            let invoked = false;
            const blocked = { run() { invoked = true; } };
            TemporalHelpers.observeMethod({ push() { throw new RangeError(); } }, blocked, "run");
            assert.throws(RangeError, () => blocked.run());
            assert.sameValue(invoked, false);
            """;
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "observe_method", "Test262TemporalHarness",
            _ => (source, Path.Combine(Directory.GetCurrentDirectory(), "observe_method.js")));
        Assert.Null(result.UnhandledException);
        Assert.Empty(result.Output);
    }

    [Theory]
    [InlineData("""
        assert.sameValue(TemporalHelpers.observeProperty.length, 4);
        assert.sameValue(TemporalHelpers.propertyBagObserver.length, 4);
        assert.sameValue(TemporalHelpers.toPrimitiveObserver.length, 3);
        assert.sameValue(TemporalHelpers.observeProperty.name, "observeProperty");
        const calls = [], target = {};
        TemporalHelpers.observeProperty(calls, target, "value", 42, "target");
        assert.sameValue(target.value, 42);
        target.value = 100;
        assert.sameValue(target.value, 42);
        const descriptor = Object.getOwnPropertyDescriptor(target, "value");
        assert.sameValue(descriptor.enumerable, false);
        assert.sameValue(descriptor.configurable, false);
        assert.sameValue(descriptor.set.length, 0);
        assert.compareArray(calls, ["get target.value", "set target.value", "get target.value"]);
        """)]
    [InlineData("""
        const calls = [], target = {};
        for (const key of ["word", "0", "01", "a'b", "word\n", 2, Symbol.iterator, Symbol.for("shared"), Symbol("local")]) {
          TemporalHelpers.observeProperty(calls, target, key, undefined, "obj");
          assert.sameValue(target[key], undefined);
        }
        assert.compareArray(calls, ["get obj.word", "get obj[0]", "get obj['01']", "get obj['a\\'b']", "get obj['word\n']",
          "get obj[2]", "get obj[Symbol.iterator]", "get obj[Symbol.for('shared')]", "get obj[Symbol('local')]"]);
        const unnamed = {};
        TemporalHelpers.observeProperty(calls, unnamed, "name", 1);
        assert.sameValue(unnamed.name, 1);
        assert.sameValue(calls[calls.length - 1], "get name");
        """)]
    [InlineData("""
        const calls = [], target = {}, key = Symbol();
        TemporalHelpers.observeProperty(calls, target, key, 1);
        assert.throws(TypeError, () => target[key]);
        assert.compareArray(calls, []);
        """)]
    [InlineData("""
        const calls = [];
        const wrapped = TemporalHelpers.toPrimitiveObserver(calls, 12, "value");
        assert.sameValue(+wrapped, 12);
        assert.sameValue(String(wrapped), "12");
        assert.compareArray(calls, ["get value.valueOf", "call value.valueOf",
          "get value.toString", "call value.toString"]);
        const descriptor = Object.getOwnPropertyDescriptor(wrapped, "valueOf");
        assert.sameValue(descriptor.enumerable, true);
        assert.sameValue(descriptor.configurable, true);
        assert.sameValue(descriptor.set, undefined);
        assert.notSameValue(wrapped.valueOf, wrapped.valueOf);
        """)]
    [InlineData("""
        const calls = [];
        const absent = TemporalHelpers.toPrimitiveObserver(calls, undefined, "absent");
        assert.sameValue(absent.valueOf(), undefined);
        assert.sameValue(absent.toString(), undefined);
        const nil = TemporalHelpers.toPrimitiveObserver(calls, null, "nil");
        assert.sameValue(nil.valueOf(), null);
        assert.throws(TypeError, () => nil.toString());
        assert.compareArray(calls, ["get absent.valueOf", "call absent.valueOf",
          "get absent.toString", "call absent.toString", "get nil.valueOf", "call nil.valueOf",
          "get nil.toString", "call nil.toString"]);
        """)]
    [InlineData("""
        const calls = [], symbol = Symbol("key"), object = {}, fn = () => {};
        const bag = TemporalHelpers.propertyBagObserver(calls,
          { missing: undefined, nil: null, number: 3, object, fn, [symbol]: 9 }, "bag", ["number", symbol]);
        assert.sameValue(bag.missing, undefined);
        assert.sameValue(bag.number, 3);
        assert.sameValue(bag[symbol], 9);
        assert.sameValue(bag.object, object);
        assert.sameValue(bag.fn, fn);
        assert.sameValue(bag.nil.valueOf(), null);
        assert.compareArray(calls, ["get bag.missing", "get bag.number", "get bag[Symbol('key')]",
          "get bag.object", "get bag.fn", "get bag.nil", "get bag.nil.valueOf", "call bag.nil.valueOf"]);
        """)]
    [InlineData("""
        const calls = [], target = { value: 7 };
        const bag = TemporalHelpers.propertyBagObserver(calls, target, "bag");
        assert.compareArray(Reflect.ownKeys(bag), ["value"]);
        assert.sameValue(Reflect.getOwnPropertyDescriptor(bag, "value").value, 7);
        assert.sameValue("value" in bag, true);
        assert.sameValue("absent" in bag, false);
        assert.compareArray(calls, ["ownKeys bag", "getOwnPropertyDescriptor bag.value",
          "has bag.value", "has bag.absent"]);
        """)]
    [InlineData("""
        const calls = [], target = { get value() { return this; } };
        const bag = TemporalHelpers.propertyBagObserver(calls, target, "bag");
        assert.sameValue(bag.value, bag);
        const receiver = {};
        assert.sameValue(Reflect.get(bag, "value", receiver), receiver);
        assert.compareArray(calls, ["get bag.value", "get bag.value"]);
        """)]
    [InlineData("""
        const calls = [], target = { value: 7 };
        const bag = TemporalHelpers.propertyBagObserver(calls, target, "bag");
        TemporalHelpers.toPrimitiveObserver = (actual, value, name) => {
          assert.sameValue(actual, calls);
          assert.sameValue(value, 7);
          assert.sameValue(name, "bag.value");
          return 99;
        };
        assert.sameValue(bag.value, 99);
        """)]
    [InlineData("""
        const calls = { push() { throw new RangeError("push failed"); } };
        const bag = TemporalHelpers.propertyBagObserver(calls, { value: 1 }, "bag");
        assert.throws(RangeError, () => bag.value);
        const target = {};
        TemporalHelpers.observeProperty(calls, target, "value", 1);
        assert.throws(RangeError, () => target.value);
        """)]
    public void ObservationHelpersPreserveUpstreamSemantics(string source)
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "temporal_helpers", "Test262TemporalHarness",
            _ => ("/*---\nincludes: [temporalHelpers.js]\n---*/\n" + source,
                Path.Combine(Directory.GetCurrentDirectory(), "temporal_helpers.js")));
        Assert.Null(result.UnhandledException);
        Assert.Empty(result.Output);
    }

    [Fact]
    public void HelpersAreOnlyRegisteredWhenIncluded()
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "no_temporal_helpers", "Test262TemporalHarness",
            _ => ("assert.sameValue(typeof TemporalHelpers, 'undefined');",
                Path.Combine(Directory.GetCurrentDirectory(), "no_temporal_helpers.js")));
        Assert.Empty(result.Output);
    }
}
