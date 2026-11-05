using Jroc.IR;
using Jroc.Services;
using Jroc.Tests.Utilities;

namespace Jroc.Tests;

public sealed class TaggedNumericConsumerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Normalize_FiltersIneligibleReadsAndRejectsUnsupportedUsesInEitherOrder(bool unsupportedUseFirst)
    {
        var body = new MethodBodyIR();
        var objectStorage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
        var receiver = AddTemp(body, objectStorage);
        var key = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var array = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
        var arrayIndex = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var numeric = AddTemp(body, objectStorage);
        var arrayElement = AddTemp(body, objectStorage);
        var mapped = AddTemp(body, objectStorage);
        var redefined = AddTemp(body, objectStorage);
        var aliased = AddTemp(body, objectStorage);
        var unused = AddTemp(body, objectStorage);
        var typed = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        body.TempVariableSlots[mapped.Index] = 0;
        foreach (var read in new[] { numeric, mapped, redefined, aliased, unused, typed })
        {
            body.Instructions.Add(new LIRGetItem(receiver, key, read));
        }
        body.Instructions.Add(new LIRGetItem(receiver, key, redefined));
        body.Instructions.Add(new LIRGetJsArrayElement(array, arrayIndex, arrayElement));
        if (unsupportedUseFirst)
        {
            body.Instructions.Add(new LIRReturn(aliased));
        }
        foreach (var read in new[] { numeric, arrayElement, mapped, redefined, aliased, typed })
        {
            var converted = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            body.Instructions.Add(new LIRConvertToNumber(read, converted));
        }
        if (!unsupportedUseFirst)
        {
            body.Instructions.Add(new LIRReturn(aliased));
        }
        var instructions = body.Instructions.ToArray();
        var storages = body.TempStorages.ToArray();

        LIRTaggedNumericNormalization.Normalize(body);

        foreach (var read in new[] { numeric, arrayElement })
        {
            Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[read.Index].Kind);
            Assert.Equal(typeof(JavaScriptRuntime.JsValue), body.TempStorages[read.Index].ClrType);
        }
        foreach (var read in new[] { mapped, redefined, aliased, unused, typed })
        {
            Assert.Same(storages[read.Index], body.TempStorages[read.Index]);
        }
        Assert.Equal(instructions, body.Instructions);
    }

    [Fact]
    public void Normalize_PropagatesComparisonRejectionWhileRetainingValidCyclesAndNumericOperands()
    {
        var body = new MethodBodyIR();
        var objectStorage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
        var receiver = AddTemp(body, objectStorage);
        var key = AddTemp(body, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        var number = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var first = AddTemp(body, objectStorage);
        var second = AddTemp(body, objectStorage);
        var third = AddTemp(body, objectStorage);
        var cycleLeft = AddTemp(body, objectStorage);
        var cycleRight = AddTemp(body, objectStorage);
        var numeric = AddTemp(body, objectStorage);
        foreach (var read in new[] { first, second, third, cycleLeft, cycleRight, numeric })
        {
            body.Instructions.Add(new LIRGetItem(receiver, key, read));
        }
        foreach (var (left, right) in new[]
        {
            (first, second), (second, third), (third, receiver),
            (cycleLeft, cycleRight), (cycleRight, cycleLeft), (number, numeric)
        })
        {
            var result = AddTemp(body, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            body.Instructions.Add(new LIRBinaryDynamicOperator(DynamicBinaryOperatorKind.LessThan, left, right, result));
        }
        var instructions = body.Instructions.ToArray();

        LIRTaggedNumericNormalization.Normalize(body);

        foreach (var read in new[] { first, second, third })
        {
            Assert.Same(objectStorage, body.TempStorages[read.Index]);
        }
        foreach (var read in new[] { cycleLeft, cycleRight, numeric })
        {
            Assert.Equal(ValueStorageKind.UnboxedValue, body.TempStorages[read.Index].Kind);
            Assert.Equal(typeof(JavaScriptRuntime.JsValue), body.TempStorages[read.Index].ClrType);
        }
        Assert.Equal(instructions, body.Instructions);
    }

    [Fact]
    public void CompiledComparison_UsesTaggedReadWithUnprovenProperty()
    {
        var artifact = Compile("""
            function below(i, obj) { return i < obj.limit; }
            function run(n) {
                let count = 0;
                const obj = { limit: n };
                for (let i = 0; i < obj.limit; i++) count++;
                return count;
            }
            console.log(run(3), below(2, { limit: 3 }), below(3, { limit: 3 }));
            """);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("ObjectRuntime::GetItemValue(object, string)", il);
        Assert.Contains("Operators::LessThan(float64, valuetype [JavaScriptRuntime]JavaScriptRuntime.JsValue)", il);
        Assert.DoesNotContain("box [JavaScriptRuntime]JavaScriptRuntime.JsValue", il);
        AssertOutput(artifact, "3 true false");
    }

    [Fact]
    public void CompiledMultiply_UsesTaggedReadsAndDeferredConversion()
    {
        var artifact = Compile("""
            function multiply(obj) { return obj.left * obj.right; }
            console.log(multiply({ left: 6, right: 7 }));
            console.log(multiply({ left: "3", right: "4" }));
            """);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        var instructions = il.Split('\n').Where(line => line.Contains("IL_", StringComparison.Ordinal)).ToArray();
        var reads = instructions.Select((line, index) => (line, index))
            .Where(item => item.line.Contains("ObjectRuntime::GetItemValue(object, string)", StringComparison.Ordinal))
            .Select(item => item.index).ToArray();
        var conversions = instructions.Select((line, index) => (line, index))
            .Where(item => item.line.Contains("JsValue::ToNumber(valuetype [JavaScriptRuntime]JavaScriptRuntime.JsValue)", StringComparison.Ordinal))
            .Select(item => item.index).ToArray();
        Assert.Equal(2, reads.Length);
        Assert.Equal(2, conversions.Length);
        Assert.True(reads[1] < conversions[0], "Both property values must be read before the first coercion.");
        Assert.DoesNotContain("box [JavaScriptRuntime]JavaScriptRuntime.JsValue", il);
        AssertOutput(artifact, "42\n12");
    }

    [Theory]
    [InlineData("none", "left-get,right-get,left-convert,right-convert", "42")]
    [InlineData("left-get", "left-get", "left-get")]
    [InlineData("right-get", "left-get,right-get", "right-get")]
    [InlineData("left-convert", "left-get,right-get,left-convert", "left-convert")]
    [InlineData("right-convert", "left-get,right-get,left-convert,right-convert", "right-convert")]
    public void CompiledMultiply_StopsAtExactGetterOrCoercionException(
        string stop, string events, string result)
    {
        var artifact = Compile("""
            function run(stop) {
                const events = [];
                function step(name, value) {
                    events.push(name);
                    if (stop === name) throw new Error(name);
                    return value;
                }
                const left = { valueOf() { return step("left-convert", 6); } };
                const right = { valueOf() { return step("right-convert", 7); } };
                const obj = {
                    get left() { return step("left-get", left); },
                    get right() { return step("right-get", right); }
                };
                let result;
                try { result = obj.left * obj.right; } catch (e) { result = e.message; }
                console.log(events.join(","), result);
            }
            """ + $"\nrun(\"{stop}\");");
        AssertOutput(artifact, $"{events} {result}");
    }

    [Fact]
    public void CompiledMultiply_PreservesProxyGetOnceAndNumericSnapshot()
    {
        var artifact = Compile("""
            function multiply(obj) { return obj.left * obj.right; }
            const events = [];
            const target = { left: 6 };
            Object.defineProperty(target, "right", {
                get() { events.push("right-get"); target.left = 100; return 7; }
            });
            const proxy = new Proxy(target, {
                get(t, key, receiver) { events.push(key); return Reflect.get(t, key, receiver); }
            });
            console.log(multiply(proxy), events.join(","), target.left);
            """);
        AssertOutput(artifact, "42 left,right,right-get 100");
    }

    [Fact]
    public void CompiledComparison_PreservesDynamicFallbacksAndCoercionOrder()
    {
        var artifact = Compile("""
            function compare(obj) {
                return [obj.left < obj.right, obj.left > obj.right,
                    obj.left <= obj.right, obj.left >= obj.right].join(",");
            }
            console.log(compare({ left: "10", right: "2" }));
            console.log(compare({ left: undefined, right: 0 }));
            console.log(compare({ left: null, right: 0 }));
            console.log(compare({ left: 9007199254740993n, right: 9007199254740992 }));
            const events = [];
            const obj = {
                get left() { events.push("left-get"); return {
                    valueOf() { events.push("left-convert"); return "10"; }
                }; },
                get right() { events.push("right-get"); return "2"; }
            };
            console.log(obj.left < obj.right, events.join(","));
            """);
        AssertOutput(artifact, """
            true,false,true,false
            false,false,false,false
            false,false,true,true
            false,true,false,true
            true left-get,right-get,left-convert
            """);
    }

    [Fact]
    public void CompiledAdjacentNumericRead_RetainsExistingGetItemAsNumberPath()
    {
        var artifact = Compile("""
            function number(obj) { return +obj.value; }
            console.log(number({ value: "8" }));
            """);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("ObjectRuntime::GetItemAsNumber(object, string)", il);
        AssertOutput(artifact, "8");
    }

    [Fact]
    public void CompiledAdditionCompoundAndAliasedValues_RetainObjectSemantics()
    {
        var artifact = Compile("""
            function add(obj) { return obj.left + obj.right; }
            function compound(obj) { obj.left += obj.right; return obj.left; }
            function alias(obj) {
                const value = obj.left;
                return [value * obj.right, value === obj.left, typeof value, value + "!"].join(",");
            }
            const events = [];
            const value = { valueOf() { events.push("convert"); return "6"; } };
            console.log(add({ left: "6", right: 7 }));
            console.log(compound({ left: "6", right: 7 }));
            console.log(alias({ left: value, right: 7 }), events.join(","));
            """);
        var il = AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName);
        Assert.Contains("ObjectRuntime::GetItem(object, string)", il);
        AssertOutput(artifact, """
            67
            67
            42,true,object,6! convert,convert
            """);
    }

    [Theory]
    [InlineData("original", "42,6!")]
    [InlineData("string", "63,9!")]
    [InlineData("number", "56,8!")]
    public void CompiledReassignedValue_RetainsObjectSemantics(string mode, string expected)
    {
        var artifact = Compile("""
            function reassign(obj, mode) {
                let value = obj.left;
                if (mode === "string") value = "9";
                else if (mode === "number") value = 8;
                return [value * obj.right, value + "!"].join(",");
            }
            """ + $"\nconsole.log(reassign({{ left: 6, right: 7 }}, \"{mode}\"));");
        AssertOutput(artifact, expected);
    }

    private static readonly string EntryPath = Path.Combine(Path.GetTempPath(), "TaggedNumericConsumers.js");

    private static TempVariable AddTemp(MethodBodyIR body, ValueStorage storage)
    {
        var temp = new TempVariable(body.Temps.Count);
        body.Temps.Add(temp);
        body.TempStorages.Add(storage);
        body.TempVariableSlots.Add(-1);
        return temp;
    }

    private static JrocCompiledAssemblyArtifact Compile(string source)
        => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(EntryPath)
        {
            SourceText = source,
            AssemblyName = "TaggedNumericConsumers"
        });

    private static void AssertOutput(JrocCompiledAssemblyArtifact artifact, string expected)
    {
        var result = InMemoryTestCompiler.ExecuteArtifact(artifact, EntryPath, "TaggedNumericConsumers",
            allowUnhandledException: true);
        Assert.True(result.UnhandledException is null,
            result.UnhandledException is null ? null
                : $"Unexpected exception: {result.UnhandledException}\n{AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName)}");
        Assert.Equal(expected.Replace("\r\n", "\n").Trim(), result.Output.Replace("\r\n", "\n").Trim());
    }
}
