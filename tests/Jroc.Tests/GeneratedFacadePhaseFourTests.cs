using System.Diagnostics;
using System.Reflection;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseFourTests
{
    [Fact]
    public void SyncGeneratorsAndIterables_PreserveProtocolSemanticsAndCleanup()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            SyncIterableJavaScript,
            "PhaseFourSync");

        var result = harness.Build(
            """
            var cleanup = new List<string>();
            Action<object?> notify = value => cleanup.Add(Convert.ToString(value)!);

            using var exports = PhaseFourSync.Import();

            Console.WriteLine(string.Join(",", exports.Sequence("a", notify)));

            using (var early = exports.Sequence("early", notify).GetEnumerator())
            {
                Console.WriteLine(early.MoveNext());
                Console.WriteLine(early.Current);
            }
            Console.WriteLine(string.Join(",", cleanup));

            using var left = exports.Sequence("left", notify).GetEnumerator();
            using var right = exports.Sequence("right", notify).GetEnumerator();
            left.MoveNext();
            right.MoveNext();
            Console.WriteLine($"{left.Current},{right.Current}");

            Console.WriteLine(string.Join(",", exports.Single));
            Console.WriteLine(string.Join(",", exports.Single));

            using (var custom = exports.CreateCustom(notify).GetEnumerator())
            {
                custom.MoveNext();
                Console.WriteLine(custom.Current);
            }
            Console.WriteLine(string.Join(",", cleanup));

            using var reusableLeft = exports.Reusable.GetEnumerator();
            using var reusableRight = exports.Reusable.GetEnumerator();
            reusableLeft.MoveNext();
            reusableRight.MoveNext();
            Console.WriteLine($"{reusableLeft.Current},{reusableRight.Current}");
            Console.WriteLine(string.Join(",", exports.Reusable));
            Console.WriteLine(string.Join(",", exports.CustomIterator));
            Console.WriteLine(exports.CustomIterator.Count());

            try
            {
                foreach (var value in exports.Failing())
                {
                    Console.WriteLine(value);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains("failing", StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(exception.InnerException?.Message.Contains("iteration boom", StringComparison.Ordinal) == true);
                var contextualException = exception.GetType().GetProperty("JsStack") != null
                    ? exception
                    : exception.InnerException;
                var jsStack = (string?)contextualException?.GetType()
                    .GetProperty("JsStack")?.GetValue(contextualException);
                Console.WriteLine(jsStack?.Contains("entry.js", StringComparison.Ordinal) == true);
            }

            var methodValues = new List<object?>();
            foreach (var value in exports.MethodGenerator("m"))
            {
                methodValues.Add(value);
            }
            Console.WriteLine(string.Join(",", methodValues));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "a1,a2",
                "True",
                "early1",
                "a,early",
                "left1,right1",
                "single1,single2",
                "custom1",
                "a,early,custom",
                "reuse1,reuse1",
                "reuse1,reuse2",
                "iterator1,iterator2",
                "0",
                "before-error",
                "JsInvocationException",
                "True",
                "True",
                "True",
                "m1,m2"
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void AsyncGeneratorsAndIterables_PreserveDelayFailureCancellationAndRootCleanup()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            AsyncIterableJavaScript,
            "PhaseFourAsync");

        var result = harness.Build(
            """
            var cleanup = new List<string>();
            Action<object?> notify = value => cleanup.Add(Convert.ToString(value)!);

            using var exports = PhaseFourAsync.Import();

            var full = new List<object?>();
            await foreach (var value in exports.Delayed("full", notify))
            {
                full.Add(value);
            }
            Console.WriteLine(string.Join(",", full));
            Console.WriteLine(string.Join(",", cleanup));

            await using (var early = exports.Delayed("early", notify).GetAsyncEnumerator())
            {
                Console.WriteLine(await early.MoveNextAsync());
                Console.WriteLine(early.Current);
            }
            Console.WriteLine(string.Join(",", cleanup));

            var customValues = new List<object?>();
            await foreach (var value in exports.CreateCustom(notify))
            {
                customValues.Add(value);
                break;
            }
            Console.WriteLine(string.Join(",", customValues));
            Console.WriteLine(string.Join(",", cleanup));

            var directCustomValues = new List<object?>();
            await foreach (var value in exports.DirectCustom)
            {
                directCustomValues.Add(value);
            }
            Console.WriteLine(string.Join(",", directCustomValues));

            var thenableValues = new List<object?>();
            await foreach (var value in exports.CreateThenable(notify))
            {
                thenableValues.Add(value);
                break;
            }
            Console.WriteLine(string.Join(",", thenableValues));
            Console.WriteLine(string.Join(",", cleanup));

            try
            {
                await foreach (var value in exports.CreatePrimitiveResult())
                {
                    Console.WriteLine(value);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains(
                    "createPrimitiveResult",
                    StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(exception.InnerException?.Message.Contains(
                    "did not return an object",
                    StringComparison.Ordinal) == true);
            }

            var directIteratorValues = new List<object?>();
            await foreach (var value in exports.DirectIterator)
            {
                directIteratorValues.Add(value);
            }
            Console.WriteLine(string.Join(",", directIteratorValues));
            var directIteratorCount = 0;
            await foreach (var value in exports.DirectIterator)
            {
                directIteratorCount++;
            }
            Console.WriteLine(directIteratorCount);

            var singleValues = new List<object?>();
            await foreach (var value in exports.Single)
            {
                singleValues.Add(value);
            }
            Console.WriteLine(string.Join(",", singleValues));
            var singleCount = 0;
            await foreach (var value in exports.Single)
            {
                singleCount++;
            }
            Console.WriteLine(singleCount);

            try
            {
                await foreach (var value in exports.Failing())
                {
                    Console.WriteLine(value);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains("failing", StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(exception.Message.Contains("async iteration boom", StringComparison.Ordinal)
                    || exception.InnerException?.Message.Contains("async iteration boom", StringComparison.Ordinal) == true);
                var contextualException = exception.GetType().GetProperty("JsStack") != null
                    ? exception
                    : exception.InnerException;
                var jsStack = (string?)contextualException?.GetType()
                    .GetProperty("JsStack")?.GetValue(contextualException);
                Console.WriteLine(jsStack?.Contains("entry.js", StringComparison.Ordinal) == true);
            }

            using var cancellation = new CancellationTokenSource();
            await using (var cancelled = exports.Delayed("cancel", notify)
                .GetAsyncEnumerator(cancellation.Token))
            {
                await cancelled.MoveNextAsync();
                cancellation.Cancel();
                try { await cancelled.MoveNextAsync(); }
                catch (OperationCanceledException) { Console.WriteLine("cancelled"); }
            }
            Console.WriteLine(string.Join(",", cleanup));

            var methodValues = new List<object?>();
            await foreach (var value in exports.MethodGenerator("method"))
            {
                methodValues.Add(value);
            }
            Console.WriteLine(string.Join(",", methodValues));

            var left = exports.Delayed("left", notify).GetAsyncEnumerator();
            var right = exports.Delayed("right", notify).GetAsyncEnumerator();
            await left.MoveNextAsync();
            await right.MoveNextAsync();
            Console.WriteLine($"{left.Current},{right.Current}");
            await left.DisposeAsync();
            await right.DisposeAsync();

            var root = exports.Delayed("root", notify).GetAsyncEnumerator();
            await root.MoveNextAsync();
            exports.Dispose();
            Console.WriteLine(cleanup.Contains("root"));
            try { await root.MoveNextAsync(); }
            catch (ObjectDisposedException) { Console.WriteLine("root-disposed"); }

            var runtimeCleanup = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            Action<object?> runtimeNotify = value =>
                runtimeCleanup.TrySetResult(Convert.ToString(value)!);
            using var runtimeThreadExports = PhaseFourAsync.Import();
            var runtimeThreadIterator = runtimeThreadExports
                .CreatePromiseCleanup(runtimeNotify)
                .GetAsyncEnumerator();
            await runtimeThreadIterator.MoveNextAsync();
            Console.WriteLine(runtimeThreadIterator.Current);
            runtimeThreadExports.DisposeFromRuntime(
                new Action(runtimeThreadExports.Dispose));
            Console.WriteLine(await runtimeCleanup.Task.WaitAsync(TimeSpan.FromSeconds(5)));
            try { await runtimeThreadIterator.MoveNextAsync(); }
            catch (ObjectDisposedException) { Console.WriteLine("runtime-root-disposed"); }

            var concurrentCleanup = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            Action<object?> concurrentNotify = value =>
                concurrentCleanup.TrySetResult(Convert.ToString(value)!);
            using var concurrentExports = PhaseFourAsync.Import();
            var concurrentIterator = concurrentExports
                .CreateSlowCleanup(concurrentNotify)
                .GetAsyncEnumerator();
            await concurrentIterator.MoveNextAsync();
            Console.WriteLine(concurrentIterator.Current);
            var inFlightDispose = concurrentIterator.DisposeAsync().AsTask();
            concurrentExports.Dispose();
            Console.WriteLine(await concurrentCleanup.Task.WaitAsync(TimeSpan.FromSeconds(5)));
            Console.WriteLine(inFlightDispose.IsCompletedSuccessfully);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "full1,full2",
                "full",
                "True",
                "early1",
                "full,early",
                "custom1",
                "full,early,custom",
                "direct1,direct2",
                "thenable1",
                "full,early,custom,thenable",
                "JsInvocationException",
                "True",
                "True",
                "async-iterator1,async-iterator2",
                "0",
                "single1,single2",
                "0",
                "before-async-error",
                "JsErrorException",
                "True",
                "True",
                "True",
                "cancelled",
                "full,early,custom,thenable,cancel",
                "method1,method2",
                "left1,right1",
                "True",
                "root-disposed",
                "runtime1",
                "runtime-thread",
                "runtime-root-disposed",
                "concurrent1",
                "concurrent",
                "True"
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void Builtins_ProjectDateRegExpErrorAndSymbolWithIdentityAndMutation()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            BuiltinJavaScript,
            "PhaseFourBuiltins");

        var result = harness.Build(
            """
            using var exports = PhaseFourBuiltins.Import();

            Console.WriteLine(ReferenceEquals(exports.Date, exports.DateAlias));
            Console.WriteLine(ReferenceEquals(exports.Date, exports.Nested.Date));
            Console.WriteLine(ReferenceEquals(exports.Pattern, exports.Nested.Pattern));
            Console.WriteLine(ReferenceEquals(exports.Error, exports.Nested.Error));
            Console.WriteLine(ReferenceEquals(exports.GlobalSymbol, exports.Nested.GlobalSymbol));
            Console.WriteLine(ReferenceEquals(exports.Pattern, exports.GetPattern()));
            Console.WriteLine(ReferenceEquals(exports.Error, exports.GetError()));
            Console.WriteLine(ReferenceEquals(exports.GlobalSymbol, exports.GetSymbol()));
            Console.WriteLine(exports.Date.GetTime());
            exports.Date.SetTime(2500);
            Console.WriteLine(exports.DateAlias.GetTime());
            Console.WriteLine(exports.MakeDate().GetTime());
            Console.WriteLine(double.IsNaN(exports.InvalidDate.GetTime()));
            Console.WriteLine(exports.InvalidDate.ToDisplayString());
            try { exports.InvalidDate.ToISOString(); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }

            Console.WriteLine(exports.Pattern.Source);
            Console.WriteLine(exports.Pattern.Flags);
            exports.Pattern.LastIndex = 0;
            var match = exports.Pattern.Exec("baaa");
            Console.WriteLine(match.Get(0));
            Console.WriteLine(exports.Pattern.LastIndex);
            Console.WriteLine(exports.Pattern.Test("zz"));
            Console.WriteLine(exports.Pattern.LastIndex);
            Console.WriteLine(exports.Pattern.Exec("zz") == null);

            Console.WriteLine(exports.Error.Name);
            Console.WriteLine(exports.Error.Message);
            Console.WriteLine(exports.Error.Cause != null);
            Console.WriteLine(ReferenceEquals(exports.Error.Cause, exports.Error.Cause));
            Console.WriteLine(exports.Error.Stack.Contains("TypeError", StringComparison.Ordinal));
            exports.Error.Name = "HostedError";
            exports.Error.Message = "changed";
            exports.Error.Cause = "host-cause";
            Console.WriteLine($"{exports.ErrorAlias.Name}:{exports.ErrorAlias.Message}:{exports.ErrorAlias.Cause}");
            Console.WriteLine(exports.RangeError.Name);
            Console.WriteLine(exports.SyntaxError.Name);
            Console.WriteLine(exports.EvalError.Name);
            Console.WriteLine(exports.ReferenceError.Name);
            Console.WriteLine(exports.UriError.Name);
            Console.WriteLine(exports.AggregateError.Name);
            Console.WriteLine(exports.SuppressedError.Name);

            Console.WriteLine(exports.LocalSymbol.Description);
            Console.WriteLine(exports.LocalSymbol.RegistryKey == null);
            Console.WriteLine(exports.GlobalSymbol.RegistryKey);
            Console.WriteLine(ReferenceEquals(exports.GlobalSymbol, exports.GlobalSymbolAlias));
            Console.WriteLine(exports.WellKnownSymbol.WellKnownName);
            Console.WriteLine(exports.WellKnownSymbol.ToDisplayString());
            Console.WriteLine(!ReferenceEquals(exports.LocalSymbol, exports.OtherLocalSymbol));

            using var constructedDate = exports.DateConstructor.Construct(3000);
            Console.WriteLine(constructedDate.GetTime());
            Console.WriteLine(exports.DateConstructor.Now());
            Console.WriteLine(exports.DateConstructor.Parse("ignored"));
            Console.WriteLine(exports.DateConstructor.Utc(1970, 0, 1));
            using var constructedPattern = exports.RegExpConstructor.Construct("b+", "g");
            Console.WriteLine(constructedPattern.Test("abb"));
            Console.WriteLine(exports.RegExpConstructor.Escape("a+b"));
            using var constructedError = exports.ErrorConstructor.Construct("host error");
            Console.WriteLine($"{constructedError.Name}:{constructedError.Message}");
            using var createdSymbol = exports.SymbolConstructor.Create("created");
            using var registeredSymbol = exports.SymbolConstructor.For("host-registry");
            Console.WriteLine(createdSymbol.Description);
            Console.WriteLine(exports.SymbolConstructor.KeyFor(registeredSymbol));
            Console.WriteLine(ReferenceEquals(
                exports.SymbolConstructor.Iterator,
                exports.WellKnownSymbol));

            try { exports.RegExpConstructor.Construct("x", "gg"); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "True",
                "True",
                "True",
                "True",
                "True",
                "True",
                "True",
                "True",
                "1000",
                "2500",
                "5000",
                "True",
                "Invalid Date",
                "JsInvocationException",
                "a+",
                "g",
                "aaa",
                "4",
                "False",
                "0",
                "True",
                "TypeError",
                "boom",
                "True",
                "True",
                "True",
                "HostedError:changed:host-cause",
                "RangeError",
                "SyntaxError",
                "EvalError",
                "ReferenceError",
                "URIError",
                "AggregateError",
                "SuppressedError",
                "local",
                "True",
                "registry",
                "True",
                "iterator",
                "Symbol(Symbol.iterator)",
                "True",
                "3000",
                "111",
                "222",
                "333",
                "True",
                "patched:a+b",
                "TypeError:host error",
                "created",
                "host-registry",
                "True",
                "JsInvocationException"
            ],
            OutputLines(result.RunStandardOutput));

        using var directHarness = new GeneratedAssemblyConsumerHarness(
            "module.exports = new Date(42);",
            "PhaseFourDirectDate");
        var directResult = directHarness.Build(
            """
            using var exports = PhaseFourDirectDate.Import();
            Console.WriteLine(exports.Value.GetTime());
            """,
            run: true);
        AssertConsumerSucceeded(directResult);
        Assert.Equal(["42"], OutputLines(directResult.RunStandardOutput));

        using var defaultHarness = new GeneratedAssemblyConsumerHarness(
            "export default new RegExp('z+', 'i');",
            "PhaseFourDefaultRegExp");
        var defaultResult = defaultHarness.Build(
            """
            using var exports = PhaseFourDefaultRegExp.Import();
            Console.WriteLine(exports.Default.Source);
            Console.WriteLine(exports.Default.Flags);
            """,
            run: true);
        AssertConsumerSucceeded(defaultResult);
        Assert.Equal(["z+", "i"], OutputLines(defaultResult.RunStandardOutput));
    }

    [Fact]
    public void Collections_PreserveJavaScriptOrderingIdentityWeakSemanticsAndLifetime()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            CollectionJavaScript,
            "PhaseFourCollections");

        var result = harness.Build(
            """
            using var exports = PhaseFourCollections.Import();
            var map = exports.Map;
            var keyFromMap = map.Keys().Single(item => item is not string);
            var valueFromMap = map.Get(keyFromMap);
            var key = exports.GetKey();
            var value = exports.GetValue();

            Console.WriteLine(ReferenceEquals(map, exports.MapAlias));
            Console.WriteLine(ReferenceEquals(map, exports.Nested.Map));
            Console.WriteLine(ReferenceEquals(map, exports.GetMap()));
            Console.WriteLine(ReferenceEquals(key, exports.GetKey()));
            Console.WriteLine(ReferenceEquals(value, exports.GetValue()));
            Console.WriteLine(map.Count);
            Console.WriteLine(map.Has(key));
            Console.WriteLine(ReferenceEquals(map.Get(key), map.Get(key)));
            Console.WriteLine(ReferenceEquals(value, valueFromMap));
            Console.WriteLine(ReferenceEquals(
                key,
                keyFromMap));
            Console.WriteLine(ReferenceEquals(
                value,
                map.Values().Single(item => item is not double)));

            foreach (var entry in map)
            {
                var keyText = entry.Key is string text ? text : "object-key";
                var valueText = entry.Value is double number
                    ? number.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : "object-value";
                Console.WriteLine($"{keyText}:{valueText}");
            }
            Console.WriteLine(ReferenceEquals(
                value,
                map.Single(entry => entry.Key is not string).Value));

            Console.WriteLine(string.Join(",", map.Keys().Select(item => item is string text ? text : "object-key")));
            Console.WriteLine(string.Join(",", map.Values().Select(item => item is double number
                ? number.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : "object-value")));

            map.Set("added", 4);
            Console.WriteLine(map.Get("added"));
            Console.WriteLine(map.Delete("first"));
            Console.WriteLine(map.Count);

            var set = exports.Set;
            Console.WriteLine(ReferenceEquals(set, exports.Nested.Set));
            Console.WriteLine(string.Join(",", set.Select(item => item is string text ? text : "object-value")));
            Console.WriteLine(set.Has(value));
            Console.WriteLine(ReferenceEquals(
                value,
                set.Single(item => item is not string)));
            set.Add("added");
            Console.WriteLine(set.Count);
            Console.WriteLine(set.Delete("first"));

            Console.WriteLine(exports.WeakMap.Has(key));
            Console.WriteLine(ReferenceEquals(exports.WeakMap.Get(key), exports.WeakMap.Get(key)));
            Console.WriteLine(ReferenceEquals(value, exports.WeakMap.Get(key)));
            exports.WeakMap.Set(value, "second");
            Console.WriteLine(exports.WeakMap.Get(value));
            Console.WriteLine(exports.WeakMap.Delete(value));
            try { exports.WeakMap.Set("primitive", 1); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }

            Console.WriteLine(exports.WeakSet.Has(key));
            exports.WeakSet.Add(value);
            Console.WriteLine(exports.WeakSet.Has(value));
            Console.WriteLine(exports.WeakSet.Delete(value));
            try { exports.WeakSet.Add("primitive"); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }

            Console.WriteLine(!exports.WeakMap.GetType().GetInterfaces()
                .Any(type => type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
            Console.WriteLine(!exports.WeakSet.GetType().GetInterfaces()
                .Any(type => type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)));

            var callableFromMap = exports.CallableMap.Keys().Single();
            var callable = exports.GetCallable();
            Console.WriteLine(ReferenceEquals(callable, callableFromMap));
            Console.WriteLine(ReferenceEquals(
                callable,
                exports.CallableMap.Get(callable)));
            Console.WriteLine(ReferenceEquals(
                callable,
                exports.CallableWeakMap.Get(callable)));
            Console.WriteLine(ReferenceEquals(
                callable,
                exports.CallableMap.Single().Value));

            exports.Dispose();
            try { Console.WriteLine(map.Count); }
            catch (ObjectDisposedException) { Console.WriteLine("map-disposed"); }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "True",
                "True",
                "True",
                "True",
                "True",
                "3",
                "True",
                "True",
                "True",
                "True",
                "True",
                "first:1",
                "object-key:object-value",
                "last:3",
                "True",
                "first,object-key,last",
                "1,object-value,3",
                "4",
                "True",
                "3",
                "True",
                "first,object-value,last",
                "True",
                "True",
                "4",
                "True",
                "True",
                "True",
                "True",
                "second",
                "True",
                "JsInvocationException",
                "True",
                "True",
                "True",
                "JsInvocationException",
                "True",
                "True",
                "True",
                "True",
                "True",
                "True",
                "map-disposed"
            ],
            OutputLines(result.RunStandardOutput));

        using var directHarness = new GeneratedAssemblyConsumerHarness(
            "module.exports = new Set(['a', 'b']);",
            "PhaseFourDirectSet");
        var directResult = directHarness.Build(
            """
            using var exports = PhaseFourDirectSet.Import();
            Console.WriteLine(exports.Value.Count);
            Console.WriteLine(string.Join(",", exports.Value));
            """,
            run: true);
        AssertConsumerSucceeded(directResult);
        Assert.Equal(["2", "a,b"], OutputLines(directResult.RunStandardOutput));
    }

    [Fact]
    public void BinaryValues_PreserveKindsOffsetsBackingStoresMutationAndLifetime()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            BinaryJavaScript,
            "PhaseFourBinary");

        var result = harness.Build(
            """
            using var exports = PhaseFourBinary.Import();

            Console.WriteLine(exports.Buffer.ByteLength);
            Console.WriteLine(!exports.Buffer.IsShared);
            Console.WriteLine(ReferenceEquals(exports.Buffer, exports.Bytes.Buffer));
            Console.WriteLine(ReferenceEquals(exports.Buffer, exports.View.Buffer));
            Console.WriteLine(ReferenceEquals(exports.Bytes, exports.Nested.Bytes));
            Console.WriteLine(ReferenceEquals(exports.Bytes, exports.GetBytes()));
            Console.WriteLine(exports.Bytes.ByteOffset);
            Console.WriteLine(exports.Bytes.ByteLength);
            Console.WriteLine(exports.Bytes.Length);
            Console.WriteLine(exports.Bytes.BytesPerElement);
            Console.WriteLine(exports.View.ByteOffset);
            Console.WriteLine(exports.View.ByteLength);

            exports.View.SetUint16(0, 0x1234, true);
            Console.WriteLine(exports.Bytes.Get(2));
            Console.WriteLine(exports.Bytes.Get(3));
            exports.Bytes.Set(4, 77);
            Console.WriteLine(exports.View.GetUint8(2));

            using var slice = exports.Buffer.Slice(2, 6);
            using var sliceView = exports.MakeView(slice);
            Console.WriteLine(slice.ByteLength);
            Console.WriteLine(sliceView.Get(0));
            sliceView.Set(0, 99);
            Console.WriteLine(exports.MakeView(exports.Buffer).Get(2));

            Console.WriteLine(exports.Shared.IsShared);
            Console.WriteLine(ReferenceEquals(exports.Shared, exports.SharedBytes.Buffer));
            exports.SharedView.SetUint8(1, 44);
            Console.WriteLine(exports.SharedBytes.Get(1));
            using var sharedSlice = exports.Shared.Slice(0, 4);
            using var sharedSliceBytes = exports.MakeView(sharedSlice);
            Console.WriteLine(sharedSlice.IsShared);
            Console.WriteLine(sharedSlice.ByteLength);
            Console.WriteLine(sharedSliceBytes.Get(1));
            sharedSliceBytes.Set(1, 55);
            Console.WriteLine(exports.SharedBytes.Get(1));

            var arrays = new[]
            {
                exports.Int8,
                exports.Uint8,
                exports.Uint8Clamped,
                exports.Int16,
                exports.Uint16,
                exports.Int32,
                exports.Uint32,
                exports.Float32,
                exports.Float64
            };
            foreach (var array in arrays)
            {
                var before = array.Get(0);
                array.Set(0, before + 1);
                Console.WriteLine($"{array.Kind}:{array.Get(0)}:{array.Length}:{array.BytesPerElement}");
            }

            try { exports.Bytes.Get(100); }
            catch (ArgumentOutOfRangeException) { Console.WriteLine("typed-index"); }
            try { exports.View.GetInt32(99, false); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }
            try { exports.Buffer.Resize(4); }
            catch (Exception exception) { Console.WriteLine(exception.GetType().Name); }
            Console.WriteLine(exports.Buffer.GetType().GetMethod("Detach") == null);

            var bytes = exports.Bytes;
            exports.Dispose();
            try { bytes.Get(0); }
            catch (ObjectDisposedException) { Console.WriteLine("bytes-disposed"); }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "16",
                "True",
                "True",
                "True",
                "True",
                "True",
                "2",
                "8",
                "8",
                "1",
                "4",
                "8",
                "52",
                "18",
                "77",
                "4",
                "9",
                "9",
                "True",
                "True",
                "44",
                "True",
                "4",
                "44",
                "44",
                "Int8Array:0:2:1",
                "Uint8Array:2:2:1",
                "Uint8ClampedArray:255:2:1",
                "Int16Array:0:2:2",
                "Uint16Array:2:2:2",
                "Int32Array:0:2:4",
                "Uint32Array:2:2:4",
                "Float32Array:2.5:2:4",
                "Float64Array:2.5:2:8",
                "typed-index",
                "JsInvocationException",
                "JsInvocationException",
                "True",
                "bytes-disposed"
            ],
            OutputLines(result.RunStandardOutput));

        using var directHarness = new GeneratedAssemblyConsumerHarness(
            "module.exports = new Uint16Array([4, 5]);",
            "PhaseFourDirectBinary");
        var directResult = directHarness.Build(
            """
            using var exports = PhaseFourDirectBinary.Import();
            Console.WriteLine(exports.Value.Kind);
            Console.WriteLine(exports.Value.Get(1));
            """,
            run: true);
        AssertConsumerSucceeded(directResult);
        Assert.Equal(["Uint16Array", "5"], OutputLines(directResult.RunStandardOutput));
    }

    [Fact]
    public void NestedControlFlowReturns_PreservePhaseFourProjectionKinds()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            const date = new Date(321);
            const map = new Map([["nested", 7]]);
            const buffer = new ArrayBuffer(4);
            const iterable = {
              [Symbol.iterator]() {
                let index = 0;
                return {
                  next() {
                    index++;
                    return index <= 2
                      ? { value: "nested" + index, done: false }
                      : { value: undefined, done: true };
                  }
                };
              }
            };

            function dateFromBranch(flag) {
              if (flag) {
                return date;
              }
              return date;
            }

            function mapFromTry(flag) {
              try {
                if (flag) {
                  return map;
                }
              } catch {
                return map;
              }
              return map;
            }

            function bufferFromSwitch(value) {
              switch (value) {
                case 1:
                  return buffer;
                default:
                  return buffer;
              }
            }

            function iterableFromLoop(flag) {
              while (flag) {
                return iterable;
              }
              return iterable;
            }

            module.exports = {
              dateFromBranch,
              mapFromTry,
              bufferFromSwitch,
              iterableFromLoop
            };
            """,
            "PhaseFourNestedReturns");

        var result = harness.Build(
            """
            using var exports = PhaseFourNestedReturns.Import();
            Console.WriteLine(exports.DateFromBranch(true).GetTime());
            Console.WriteLine(exports.MapFromTry(true).Get("nested"));
            Console.WriteLine(exports.BufferFromSwitch(1).ByteLength);
            Console.WriteLine(string.Join(",", exports.IterableFromLoop(false)));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["321", "7", "4", "nested1,nested2"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ClassInstances_ProjectInheritedSyncAndAsyncIterableContracts()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            class SyncBase {
              [Symbol.iterator]() {
                let index = 0;
                return {
                  next() {
                    index++;
                    return index <= 2
                      ? { value: "class-sync" + index, done: false }
                      : { value: undefined, done: true };
                  }
                };
              }
            }

            class SyncDerived extends SyncBase {}

            class AsyncBase {
              [Symbol.asyncIterator]() {
                let index = 0;
                return {
                  next() {
                    index++;
                    return Promise.resolve(index <= 2
                      ? { value: "class-async" + index, done: false }
                      : { value: undefined, done: true });
                  }
                };
              }
            }

            class AsyncDerived extends AsyncBase {}

            module.exports = {
              sync: new SyncDerived(),
              async: new AsyncDerived(),
              SyncDerived,
              AsyncDerived
            };
            """,
            "PhaseFourClassIterables");

        var result = harness.Build(
            """
            using var exports = PhaseFourClassIterables.Import();
            Console.WriteLine(string.Join(",", exports.Sync));

            var asyncValues = new List<object?>();
            await foreach (var value in exports.Async)
            {
                asyncValues.Add(value);
            }
            Console.WriteLine(string.Join(",", asyncValues));

            using var constructedSync = exports.SyncDerived.Construct();
            Console.WriteLine(string.Join(",", constructedSync));

            using var constructedAsync = exports.AsyncDerived.Construct();
            var constructedAsyncValues = new List<object?>();
            await foreach (var value in constructedAsync)
            {
                constructedAsyncValues.Add(value);
            }
            Console.WriteLine(string.Join(",", constructedAsyncValues));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "class-sync1,class-sync2",
                "class-async1,class-async2",
                "class-sync1,class-sync2",
                "class-async1,class-async2"
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void PublicContracts_RecursivelyUseOnlyGeneratedOrBclTypes()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            LeakAuditJavaScript,
            "PhaseFourLeakAudit");
        using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);

        var publicTypes = loaded.Assembly
            .GetTypes()
            .Where(type => type.IsVisible)
            .ToArray();

        Assert.NotEmpty(publicTypes);
        foreach (var type in publicTypes)
        {
            AssertAllowedPublicType(type, loaded.Assembly, $"{type.FullName} type");
            if (type.BaseType != null)
            {
                AssertAllowedPublicType(type.BaseType, loaded.Assembly, $"{type.FullName} base");
            }
            foreach (var iface in type.GetInterfaces())
            {
                AssertAllowedPublicType(iface, loaded.Assembly, $"{type.FullName} interface");
            }

            foreach (var method in type.GetMethods(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly))
            {
                AssertAllowedPublicType(method.ReturnType, loaded.Assembly, method.ToString()!);
                foreach (var parameter in method.GetParameters())
                {
                    AssertAllowedPublicType(
                        parameter.ParameterType,
                        loaded.Assembly,
                        parameter.ToString());
                }
            }

            foreach (var property in type.GetProperties(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly))
            {
                AssertAllowedPublicType(
                    property.PropertyType,
                    loaded.Assembly,
                    property.ToString()!);
            }
        }
    }

    [Fact]
    public void NodeObservableOrderingCleanupAndMutationMatch()
    {
        var nodeOutput = RunNode(
            NodeParityJavaScript
            + """

            const exported = module.exports;
            console.log(Array.from(exported.values()).join(","));
            const iterator = exported.values();
            console.log(iterator.next().value);
            iterator.return();
            console.log(exported.cleanup.join(","));
            console.log(Array.from(exported.map, entry =>
              (typeof entry[0] === "string" ? entry[0] : "object") + ":" + entry[1]).join(","));
            console.log(Array.from(exported.set).join(","));
            exported.view.setUint16(0, 4660, true);
            console.log(exported.bytes[0] + "," + exported.bytes[1]);
            console.log(exported.pattern.exec("baaa")[0] + ":" + exported.pattern.lastIndex);
            console.log(exported.date.getTime());
            """);

        using var harness = new GeneratedAssemblyConsumerHarness(
            NodeParityJavaScript,
            "PhaseFourNodeParity");
        var result = harness.Build(
            """
            using var exports = PhaseFourNodeParity.Import();
            Console.WriteLine(string.Join(",", exports.Values()));
            using (var iterator = exports.Values().GetEnumerator())
            {
                iterator.MoveNext();
                Console.WriteLine(iterator.Current);
            }
            Console.WriteLine(string.Join(",", Enumerable.Range(0, (int)exports.Cleanup.Length)
                .Select(index => exports.Cleanup.Get(index))));
            Console.WriteLine(string.Join(",", exports.Map.Select(entry =>
                $"{(entry.Key is string text ? text : "object")}:{entry.Value}")));
            Console.WriteLine(string.Join(",", exports.Set));
            exports.View.SetUint16(0, 4660, true);
            Console.WriteLine($"{exports.Bytes.Get(0)},{exports.Bytes.Get(1)}");
            Console.WriteLine($"{exports.Pattern.Exec("baaa").Get(0)}:{exports.Pattern.LastIndex}");
            Console.WriteLine(exports.Date.GetTime());
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            OutputLines(nodeOutput),
            OutputLines(result.RunStandardOutput));
    }

    private const string SyncIterableJavaScript =
        """
        function* sequence(prefix, notify) {
          try {
            yield prefix + "1";
            yield prefix + "2";
          } finally {
            notify(prefix);
          }
        }

        function* failing() {
          yield "before-error";
          throw new Error("iteration boom");
        }

        const single = sequence("single", () => {});
        function createCustom(notify) {
          return {
            [Symbol.iterator]() {
              let index = 0;
              return {
                next() {
                  index++;
                  return index <= 2
                    ? { value: "custom" + index, done: false }
                    : { value: undefined, done: true };
                },
                return() {
                  notify("custom");
                  return { value: undefined, done: true };
                },
                [Symbol.iterator]() { return this; }
              };
            }
          };
        }

        const reusable = {
          [Symbol.iterator]() {
            let index = 0;
            return {
              next() {
                index++;
                return index <= 2
                  ? { value: "reuse" + index, done: false }
                  : { value: undefined, done: true };
              },
              [Symbol.iterator]() { return this; }
            };
          }
        };

        const customIterator = {
          index: 0,
          next() {
            this.index++;
            return this.index <= 2
              ? { value: "iterator" + this.index, done: false }
              : { value: undefined, done: true };
          },
          [Symbol.iterator]() { return this; }
        };

        module.exports = {
          sequence,
          failing,
          single,
          reusable,
          customIterator,
          createCustom,
          *methodGenerator(prefix) {
            yield prefix + "1";
            yield prefix + "2";
          }
        };
        """;

    private const string AsyncIterableJavaScript =
        """
        async function* delayed(prefix, notify) {
          try {
            yield prefix + "1";
            await new Promise(resolve => setTimeout(resolve, 5));
            yield prefix + "2";
          } finally {
            notify(prefix);
          }
        }

        async function* failing() {
          yield "before-async-error";
          throw new Error("async iteration boom");
        }

        async function* singleSource() {
          yield "single1";
          yield "single2";
        }
        const single = singleSource();

        function createCustom(notify) {
          return {
            [Symbol.asyncIterator]() {
              let index = 0;
              return {
                next() {
                  index++;
                  return Promise.resolve(index <= 2
                    ? { value: "custom" + index, done: false }
                    : { value: undefined, done: true });
                },
                return() {
                  notify("custom");
                  return Promise.resolve({ value: undefined, done: true });
                }
              };
            }
          };
        }

        function createThenable(notify) {
          let index = 0;
          return {
            [Symbol.asyncIterator]() {
              return {
                next() {
                  index++;
                  return {
                    then(resolve) {
                      Promise.resolve().then(() => resolve(index <= 2
                        ? { value: "thenable" + index, done: false }
                        : { value: undefined, done: true }));
                    }
                  };
                },
                return() {
                  return {
                    then(resolve) {
                      Promise.resolve().then(() => {
                        notify("thenable");
                        resolve({ value: undefined, done: true });
                      });
                    }
                  };
                }
              };
            }
          };
        }

        function createPrimitiveResult() {
          return {
            [Symbol.asyncIterator]() {
              return {
                next() {
                  return {
                    then(resolve) {
                      resolve(42);
                    }
                  };
                }
              };
            }
          };
        }

        function createPromiseCleanup(notify) {
          let done = false;
          return {
            [Symbol.asyncIterator]() {
              return {
                next() {
                  if (done) {
                    return Promise.resolve({ value: undefined, done: true });
                  }
                  done = true;
                  return Promise.resolve({ value: "runtime1", done: false });
                },
                return() {
                  return new Promise(resolve => setTimeout(() => {
                    notify("runtime-thread");
                    resolve({ value: undefined, done: true });
                  }, 5));
                }
              };
            }
          };
        }

        function createSlowCleanup(notify) {
          let done = false;
          return {
            [Symbol.asyncIterator]() {
              return {
                next() {
                  if (done) {
                    return Promise.resolve({ value: undefined, done: true });
                  }
                  done = true;
                  return Promise.resolve({ value: "concurrent1", done: false });
                },
                return() {
                  return new Promise(resolve => setTimeout(() => {
                    notify("concurrent");
                    resolve({ value: undefined, done: true });
                  }, 20));
                }
              };
            }
          };
        }

        function disposeFromRuntime(callback) {
          callback();
        }

        const directCustom = {
          [Symbol.asyncIterator]() {
            let index = 0;
            return {
              next() {
                index++;
                return Promise.resolve(index <= 2
                  ? { value: "direct" + index, done: false }
                  : { value: undefined, done: true });
              }
            };
          }
        };

        const directIterator = {
          index: 0,
          next() {
            this.index++;
            return Promise.resolve(this.index <= 2
              ? { value: "async-iterator" + this.index, done: false }
              : { value: undefined, done: true });
          },
          [Symbol.asyncIterator]() { return this; }
        };

        module.exports = {
          delayed,
          failing,
          createCustom,
          createThenable,
          createPrimitiveResult,
          createPromiseCleanup,
          createSlowCleanup,
          disposeFromRuntime,
          directCustom,
          directIterator,
          single,
          async *methodGenerator(prefix) {
            yield prefix + "1";
            await Promise.resolve();
            yield prefix + "2";
          }
        };
        """;

    private const string BuiltinJavaScript =
        """
        const cause = { code: 7 };
        const date = new Date(1000);
        const invalidDate = new Date("not-a-date");
        const pattern = new RegExp("a+", "g");
        const error = new TypeError("boom", { cause });
        const rangeError = new RangeError("range");
        const syntaxError = new SyntaxError("syntax");
        const evalError = new EvalError("eval");
        const referenceError = new ReferenceError("reference");
        const uriError = new URIError("uri");
        const aggregateError = new AggregateError([], "aggregate");
        const suppressedError = new SuppressedError(
          new Error("primary"),
          new Error("suppressed"),
          "combined");
        const localSymbol = Symbol("local");
        const otherLocalSymbol = Symbol("local");
        const globalSymbol = Symbol.for("registry");
        const wellKnownSymbol = Symbol.iterator;
        Date.now = () => 111;
        Date.parse = () => 222;
        Date.UTC = () => 333;
        RegExp.escape = value => "patched:" + value;

        module.exports = {
          date,
          dateAlias: date,
          invalidDate,
          pattern,
          error,
          errorAlias: error,
          rangeError,
          syntaxError,
          evalError,
          referenceError,
          uriError,
          aggregateError,
          suppressedError,
          localSymbol,
          otherLocalSymbol,
          globalSymbol,
          globalSymbolAlias: globalSymbol,
          wellKnownSymbol,
          nested: { date, pattern, error, globalSymbol },
          makeDate() { return new Date(5000); },
          getPattern() { return pattern; },
          getError() { return error; },
          getSymbol() { return globalSymbol; },
          DateConstructor: Date,
          RegExpConstructor: RegExp,
          ErrorConstructor: TypeError,
          SymbolConstructor: Symbol
        };
        """;

    private const string CollectionJavaScript =
        """
        const key = { id: "key" };
        const value = { id: "value" };
        const map = new Map([
          ["first", 1],
          [key, value],
          ["last", 3]
        ]);
        const set = new Set(["first", value, "last"]);
        const weakMap = new WeakMap([[key, value]]);
        const weakSet = new WeakSet([key]);
        const callable = value => value;
        const callableMap = new Map([[callable, callable]]);
        const callableWeakMap = new WeakMap([[callable, callable]]);

        module.exports = {
          map,
          mapAlias: map,
          set,
          weakMap,
          weakSet,
          callableMap,
          callableWeakMap,
          nested: { map, set },
          getMap() { return map; },
          getKey() { return key; },
          getValue() { return value; },
          getCallable() { return callable; }
        };
        """;

    private const string BinaryJavaScript =
        """
        const buffer = new ArrayBuffer(16);
        const allBytes = new Uint8Array(buffer);
        allBytes[2] = 9;
        allBytes[3] = 8;
        allBytes[4] = 7;
        allBytes[5] = 6;
        const bytes = new Uint8Array(buffer, 2, 8);
        const view = new DataView(buffer, 4, 8);

        const shared = new SharedArrayBuffer(8);
        const sharedBytes = new Uint8Array(shared);
        const sharedView = new DataView(shared);

        const int8 = new Int8Array([-1, 2]);
        const uint8 = new Uint8Array([1, 2]);
        const uint8Clamped = new Uint8ClampedArray([255, 2]);
        const int16 = new Int16Array([-1, 2]);
        const uint16 = new Uint16Array([1, 2]);
        const int32 = new Int32Array([-1, 2]);
        const uint32 = new Uint32Array([1, 2]);
        const float32 = new Float32Array([1.5, 2.5]);
        const float64 = new Float64Array([1.5, 2.5]);

        module.exports = {
          buffer,
          bytes,
          view,
          shared,
          sharedBytes,
          sharedView,
          int8,
          uint8,
          uint8Clamped,
          int16,
          uint16,
          int32,
          uint32,
          float32,
          float64,
          nested: { bytes, view },
          getBytes() { return bytes; },
          makeView(target) { return new Uint8Array(target); }
        };
        """;

    private const string LeakAuditJavaScript =
        """
        function* syncValues() { yield 1; }
        async function* asyncValues() { yield 2; }
        const buffer = new ArrayBuffer(8);
        const key = {};

        module.exports = {
          syncValues,
          asyncValues,
          date: new Date(0),
          pattern: new RegExp("x", "g"),
          error: new Error("boom"),
          symbol: Symbol.for("leak-audit"),
          map: new Map([[key, 1]]),
          set: new Set([key]),
          weakMap: new WeakMap([[key, 1]]),
          weakSet: new WeakSet([key]),
          buffer,
          shared: new SharedArrayBuffer(8),
          view: new DataView(buffer),
          typed: new Float64Array(buffer),
          nested: {
            date: new Date(1),
            map: new Map(),
            typed: new Uint8Array(buffer)
          }
        };
        """;

    private const string NodeParityJavaScript =
        """
        const cleanup = [];
        function* values() {
          try {
            yield "g1";
            yield "g2";
          } finally {
            cleanup.push("closed");
          }
        }
        const key = {};
        const map = new Map([["first", 1], [key, 2], ["last", 3]]);
        const set = new Set(["a", "b"]);
        const buffer = new ArrayBuffer(4);
        const bytes = new Uint8Array(buffer);
        const view = new DataView(buffer);
        const pattern = new RegExp("a+", "g");
        const date = new Date(1234);
        module.exports = { cleanup, values, map, set, bytes, view, pattern, date };
        """;

    private static void AssertAllowedPublicType(
        Type type,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var inspected in FlattenType(type))
        {
            Assert.False(IsRuntimeType(inspected), $"{context} leaks {inspected.FullName}");
            Assert.True(
                inspected.Assembly == generatedAssembly
                || inspected.Namespace?.StartsWith("System", StringComparison.Ordinal) == true,
                $"{context} uses non-generated/non-BCL type {inspected.FullName}");
        }
    }

    private static IEnumerable<Type> FlattenType(Type type)
    {
        if (type.IsByRef || type.IsPointer || type.IsArray)
        {
            foreach (var nested in FlattenType(type.GetElementType()!))
            {
                yield return nested;
            }
            yield break;
        }

        if (type.IsGenericType)
        {
            yield return type.GetGenericTypeDefinition();
            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var nested in FlattenType(argument))
                {
                    yield return nested;
                }
            }
            yield break;
        }

        if (!type.IsGenericParameter)
        {
            yield return type;
        }
    }

    private static bool IsRuntimeType(Type type)
        => string.Equals(
               type.Assembly.GetName().Name,
               "JavaScriptRuntime",
               StringComparison.Ordinal)
           || type.Namespace?.StartsWith("Jroc.Runtime", StringComparison.Ordinal) == true
           || type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true;

    private static string RunNode(string source)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-e");
        startInfo.ArgumentList.Add(source);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start Node.js.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        Assert.True(process.WaitForExit(30_000), "Node.js parity process timed out.");
        Assert.True(
            process.ExitCode == 0,
            $"Node.js parity process failed.{Environment.NewLine}{standardError.GetAwaiter().GetResult()}");
        return standardOutput.GetAwaiter().GetResult();
    }

    private static void AssertConsumerSucceeded(GeneratedAssemblyConsumerResult result)
    {
        Assert.True(
            result.BuildExitCode == 0,
            $"Consumer build failed.{Environment.NewLine}{result.BuildDiagnostics}");
        Assert.True(
            result.RunExitCode == 0,
            $"Consumer run failed.{Environment.NewLine}" +
            $"{result.RunStandardOutput}{Environment.NewLine}{result.RunStandardError}");
    }

    private static string[] OutputLines(string output)
        => output.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
}
