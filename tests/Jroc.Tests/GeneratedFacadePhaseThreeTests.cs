using System.Reflection;
using System.Text;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseThreeTests
{
    [Fact]
    public void PackageTypesMetadata_GeneratesTypedFacadeForRuntimeEntrypoint()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            module.exports = {
              format(value) { return String(value); },
              enabled: true
            };
            """,
            "DeclarationPackage",
            new Dictionary<string, string>
            {
                ["node_modules/typed-package/package.json"] =
                    """{"name":"typed-package","main":"index.js","types":"index.d.ts"}""",
                ["node_modules/typed-package/index.d.ts"] =
                    """
                    import { Formatter } from "./types"
                    interface Api {
                      enabled: boolean
                      format: Formatter
                    }
                    declare const api: Api
                    export = api
                    """,
                ["node_modules/typed-package/types.d.ts"] =
                    """export type Formatter = (input: string | number) => string"""
            },
            entryFileName: "node_modules/typed-package/index.js",
            rootModuleId: "typed-package");

        var result = harness.Build(
            """
            using var api = DeclarationPackage.Import();
            Console.WriteLine(api.Enabled);
            Console.WriteLine(api.Format(42));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["True", "42"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void PackageDeclarationReturnTypes_GenerateNestedStandardObjectContracts()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            exports.createWindow = function(html, address) {
              return {
                document: {
                  title: "fixture",
                  getElementsByTagName: function(name) { return { length: name === "*" ? 3 : 1 }; }
                }
              };
            };
            """,
            "DeclaredGraphPackage",
            new Dictionary<string, string>
            {
                ["node_modules/@scope/graph-package/package.json"] =
                    """{"name":"@scope/graph-package","main":"index.js","typings":"index.d.ts"}""",
                ["node_modules/@scope/graph-package/index.d.ts"] =
                    """
                    declare module "@scope/graph-package" {
                      function createWindow(html?: string, address?: string): Window;
                    }
                    """
            },
            entryFileName: "node_modules/@scope/graph-package/index.js",
            rootModuleId: "@scope/graph-package");

        var result = harness.Build(
            """
            using var exports = DeclaredGraphPackage.Import();
            using var window = exports.CreateWindow("<html></html>", "about:blank");
            var document = window.Document;
            Console.WriteLine(document.Title);
            Console.WriteLine(document.GetElementsByTagName("*").Length);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["fixture", "3"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void PackageDeclarationParameters_PreserveOptionalAndRestCallShapes()
    {
        using var harness = CreatePackageDeclarationHarness(
            """
            module.exports = {
              optional(value, suffix) { return suffix === undefined ? value : value + suffix; },
              join(separator, ...values) { return values.join(separator); }
            };
            """,
            """
            interface Api {
              optional(value: string, suffix?: string): string
              join(separator: string, ...values: string[]): string
            }
            declare const api: Api
            export = api
            """,
            "ParameterPackage");

        var result = harness.Build(
            """
            using var api = ParameterPackage.Import();
            Console.WriteLine(api.Optional("value"));
            Console.WriteLine(api.Join(",", "a", "b"));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["value", "a,b"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void PackageDeclarationOverloads_FallBackInsteadOfEmittingDuplicateMembers()
    {
        AssertRejectedDeclarationFallsBack(
            """
            declare module "fallback-package" {
              function parse(value: string): string;
              function parse(value: number): string;
            }
            """,
            "OverloadFallbackPackage");
    }

    [Fact]
    public void CallableIntersection_FallsBackInsteadOfDroppingCallableConstituent()
    {
        AssertRejectedDeclarationFallsBack(
            """
            type Callable = (value: string) => string
            interface Properties { enabled: boolean }
            declare const api: Callable & Properties
            export = api
            """,
            "IntersectionFallbackPackage");
    }

    [Fact]
    public void InterfaceInheritance_FallsBackInsteadOfDroppingBaseMembers()
    {
        AssertRejectedDeclarationFallsBack(
            """
            interface Base { inherited(): string }
            interface Api extends Base { own(): string }
            declare const api: Api
            export = api
            """,
            "InheritedInterfaceFallbackPackage");
    }

    [Theory]
    [InlineData("method?(): string")]
    [InlineData("property?: string")]
    public void OptionalInterfaceMembers_FallBackInsteadOfBecomingMandatory(string member)
    {
        AssertRejectedDeclarationFallsBack(
            $$"""
            interface Api { {{member}} }
            declare const api: Api
            export = api
            """,
            "OptionalMemberFallbackPackage");
    }

    [Fact]
    public void NullableDeclarationReturns_RemainConservativeObjects()
    {
        using var harness = CreatePackageDeclarationHarness(
            "module.exports = { maybeNumber() { return null; }, maybeObject() { return undefined; } };",
            """
            interface Result { value: string; }
            interface Api {
              maybeNumber(): number | null;
              maybeObject(): Result | undefined;
            }
            declare const api: Api
            export = api
            """,
            "NullablePackage");

        using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);
        var maybeMethods = loaded.Assembly
            .GetTypes()
            .Where(type => type.IsInterface)
            .SelectMany(type => type.GetMethods())
            .Where(method => method.Name is "MaybeNumber" or "MaybeObject")
            .ToArray();

        Assert.Contains(maybeMethods, method => method.Name == "MaybeNumber");
        Assert.Contains(maybeMethods, method => method.Name == "MaybeObject");
        Assert.All(maybeMethods, method => Assert.Equal(typeof(object), method.ReturnType));
    }

    [Fact]
    public void CompactPackageInterfaces_ParseEveryTopLevelMember()
    {
        using var harness = CreatePackageDeclarationHarness(
            "module.exports = { enabled: true, format(value) { return String(value); } };",
            """
            interface Api { enabled: boolean; format(value: string): string; }
            declare const api: Api
            export = api
            """,
            "CompactInterfacePackage");

        var result = harness.Build(
            """
            using var api = CompactInterfacePackage.Import();
            Console.WriteLine(api.Enabled);
            Console.WriteLine(api.Format("ok"));
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["True", "ok"], OutputLines(result.RunStandardOutput));
    }

    [Theory]
    [InlineData("interface Api { call($: string, _: string): string } declare const api: Api\nexport = api")]
    [InlineData("interface Api { call(default: string): string } declare const api: Api\nexport = api")]
    [InlineData("interface Api { value: class } declare const api: Api\nexport = api")]
    public void InvalidSynthesizedIdentifiers_FallBackWithoutCompilerFailure(string declaration)
    {
        AssertRejectedDeclarationFallsBack(declaration, "IdentifierFallbackPackage");
    }

    [Fact]
    public void PublicContracts_RecursivelyUseOnlyGeneratedOrBclTypes()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            RichExportsJavaScript,
            "RichExportsAssembly");
        using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);

        var publicTypes = loaded.Assembly
            .GetTypes()
            .Where(type => type.IsPublic || type.IsNestedPublic)
            .Where(type => type.FullName?.StartsWith("RichExportsAssembly", StringComparison.Ordinal) == true
                        || type.FullName?.StartsWith("Jroc.RichExportsAssembly", StringComparison.Ordinal) == true)
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

            AssertPublicAttributesDoNotLeakRuntime(
                type,
                loaded.Assembly,
                $"{type.FullName} attributes");

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                AssertAllowedPublicType(method.ReturnType, loaded.Assembly, method.ToString()!);
                AssertPublicAttributesDoNotLeakRuntime(
                    method,
                    loaded.Assembly,
                    $"{method} attributes");
                AssertModifiersDoNotLeakRuntime(method.ReturnParameter, $"{method} return modifiers");

                foreach (var parameter in method.GetParameters())
                {
                    AssertAllowedPublicType(parameter.ParameterType, loaded.Assembly, parameter.ToString());
                    AssertPublicAttributesDoNotLeakRuntime(
                        parameter,
                        loaded.Assembly,
                        $"{method} parameter attributes");
                    AssertModifiersDoNotLeakRuntime(parameter, $"{method} parameter modifiers");
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                AssertAllowedPublicType(property.PropertyType, loaded.Assembly, property.ToString()!);
                AssertPublicAttributesDoNotLeakRuntime(
                    property,
                    loaded.Assembly,
                    $"{property} attributes");
            }
        }
    }

    [Fact]
    public void CSharpConsumer_UsesFunctionsClassesObjectsArraysAndAsyncWithoutRuntimeTypes()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            RichExportsJavaScript,
            "RichExportsAssembly");

        var result = harness.Build(
            """
            using System.Threading.Tasks;

            using var exports = RichExportsAssembly.Import();

            Console.WriteLine(exports.Add(1, 2));
            Console.WriteLine(exports.Optional());
            Console.WriteLine(exports.Optional(6));
            Console.WriteLine(exports.Rest(1, 2, 3, 4));

            Func<object, object?> twice = value => Convert.ToDouble(value) * 2;
            Console.WriteLine(exports.UseCallback(twice, 5));
            Console.WriteLine(exports.MethodThis());
            Console.WriteLine(exports.RegularReceiver());
            Console.WriteLine(exports.ArrowLexicalThis());
            Console.WriteLine(exports.ArrowLexicalArguments());
            Console.WriteLine(exports.SameFunctionAlias());

            using var returnedCallable = exports.GetIdentity();
            Console.WriteLine(returnedCallable.Invoke());
            Console.WriteLine(ReferenceEquals(returnedCallable, exports.GetIdentity()));

            try { exports.ThrowSync(); }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains("throwSync") || exception.Message.Contains("sync boom"));
            }

            var graph = exports.Graph;
            Console.WriteLine(ReferenceEquals(graph, exports.Graph));
            Console.WriteLine(graph.Read());
            graph.Value = 10;
            Console.WriteLine(graph.Doubled);
            graph.Doubled = 50;
            Console.WriteLine(graph.Value);
            Console.WriteLine(graph.Child.Label);
            graph.Child.Label = "changed";
            Console.WriteLine(graph.Child.Read());
            Console.WriteLine(graph.ComputedName);
            Console.WriteLine(ReferenceEquals(
                graph.GetDynamicProperty("self"),
                graph.GetDynamicProperty("self")));
            Console.WriteLine(ReferenceEquals(
                graph.GetDynamicProperty("alias"),
                graph.GetDynamicProperty("child")));
            Console.WriteLine(exports.ProtoObject.GetDynamicProperty("own"));
            Console.WriteLine(exports.ProtoObject.GetDynamicProperty("inherited"));
            Console.WriteLine(exports.NullPrototypeObject.GetDynamicProperty("value"));

            var values = graph.Values;
            Console.WriteLine(values.Length);
            Console.WriteLine(values.HasIndex(1));
            Console.WriteLine(values.Get(1) == null);
            values.Set(1, 2);
            Console.WriteLine(values.HasIndex(1));
            Console.WriteLine(values.Get(1));
            Console.WriteLine(values.Push(4));
            Console.WriteLine(values.Length);

            using var returnedValues = exports.GetValues();
            Console.WriteLine(returnedValues.Length);
            Console.WriteLine(returnedValues.HasIndex(1));

            using var counter = exports.Counter.Construct(10);
            Console.WriteLine(counter.Add(5));
            Console.WriteLine(counter.GetValue());
            Console.WriteLine(counter.GetSecret());
            Console.WriteLine(exports.Counter.Description);
            Console.WriteLine(counter.BaseOnly());
            Console.WriteLine(exports.Counter.BaseLabel());
            Console.WriteLine(exports.AcceptCounter(counter));

            using var created = exports.CreateCounter(3);
            Console.WriteLine(created.Add(1));

            using var returnedCounterClass = exports.GetCounterClass();
            Console.WriteLine(ReferenceEquals(returnedCounterClass, exports.Counter));
            using var returnedCounter = returnedCounterClass.Construct(8);
            Console.WriteLine(returnedCounter.Add(1));
            using var sharedCounter = exports.GetSharedCounter();
            Console.WriteLine(ReferenceEquals(sharedCounter, exports.GetSharedCounter()));

            using var anonymousClass = exports.MakeAnonymousClass();
            using var anonymous = anonymousClass.Construct(9);
            Console.WriteLine(anonymous.GetDynamicProperty("value"));

            Console.WriteLine(await exports.ImmediateAsync(4));
            Console.WriteLine(await exports.DelayedAsync(5));
            using var asyncCounter = await exports.CreateCounterAsync(6);
            Console.WriteLine(asyncCounter.Add(1));
            var asyncObject = await exports.ObjectAsync();
            Console.WriteLine(asyncObject.GetDynamicProperty("value"));
            Console.WriteLine(ReferenceEquals(asyncObject, await exports.ObjectAsync()));
            var concurrent = await Task.WhenAll(exports.DelayedAsync(1), exports.DelayedAsync(2));
            Console.WriteLine(string.Join(",", concurrent));

            try { await exports.RejectError(); }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains("reject error"));
            }

            try { await exports.RejectValue(); }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetType().Name);
                Console.WriteLine(exception.Message.Contains("bad-value"));
            }

            var pending = exports.NeverAsync();
            exports.Dispose();
            try { await pending; }
            catch (ObjectDisposedException) { Console.WriteLine("pending-disposed"); }

            try { graph.Read(); }
            catch (ObjectDisposedException) { Console.WriteLine("graph-disposed"); }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            [
                "3",
                "5",
                "7",
                "10",
                "11",
                "root",
                "receiver",
                "lexical",
                "outer",
                "True",
                "1",
                "True",
                "JsInvocationException",
                "True",
                "True",
                "1",
                "20",
                "25",
                "child",
                "changed",
                "computed",
                "True",
                "True",
                "own",
                "proto",
                "null-prototype",
                "3",
                "False",
                "True",
                "True",
                "2",
                "4",
                "4",
                "3",
                "False",
                "15",
                "15",
                "2",
                "counter",
                "base",
                "base-static",
                "True",
                "4",
                "True",
                "9",
                "True",
                "9",
                "5",
                "7",
                "7",
                "25",
                "True",
                "3,4",
                "JsErrorException",
                "True",
                "JsErrorException",
                "True",
                "pending-disposed",
                "graph-disposed"
            ],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void CSharpConsumers_RepresentSampleExportShapesWithoutDynamicOrRuntimeApis()
    {
        using var basic = new GeneratedAssemblyConsumerHarness(
            "module.exports = { version: '1.0.0', add: (a, b) => a + b };",
            "HostedMathModule");
        var basicResult = basic.Build(
            """
            using var exports = HostedMathModule.Import();
            Console.WriteLine(exports.Version);
            Console.WriteLine(exports.Add(1, 2));
            """,
            run: true);
        AssertConsumerSucceeded(basicResult);
        Assert.Equal(["1.0.0", "3"], OutputLines(basicResult.RunStandardOutput));

        using var picocolors = new GeneratedAssemblyConsumerHarness(
            """
            module.exports = {
              red: value => "red=" + value,
              green: value => "green=" + value,
              yellow: value => "yellow=" + value,
              cyan: value => "cyan=" + value,
              bold: value => "bold=" + value
            };
            """,
            "picocolors");
        var picocolorsResult = picocolors.Build(
            """
            using var pc = global::picocolors.Import();
            Console.WriteLine(pc.Red("error"));
            Console.WriteLine(pc.Green("ok"));
            Console.WriteLine(pc.Yellow("warn"));
            Console.WriteLine(pc.Cyan("info"));
            Console.WriteLine(pc.Bold("bold"));
            """,
            run: true);
        AssertConsumerSucceeded(picocolorsResult);
        Assert.Equal(
            ["red=error", "green=ok", "yellow=warn", "cyan=info", "bold=bold"],
            OutputLines(picocolorsResult.RunStandardOutput));

        using var npmRunAll2 = new GeneratedAssemblyConsumerHarness(
            """
            function taskHeader(nameAndArgs) { return "> " + nameAndArgs; }
            function filterTasks(taskListCsv, pattern) {
              return taskListCsv.split(",").filter(task => pattern === "test:*" ? task.startsWith("test:") : task === pattern).join(",");
            }
            module.exports = { taskHeader, filterTasks };
            """,
            "NpmRunAll2Module",
            entryFileName: "index.js");
        var npmRunAll2Result = npmRunAll2.Build(
            """
            using var exports = NpmRunAll2Module.Import();
            Console.WriteLine(exports.TaskHeader("build"));
            Console.WriteLine(exports.FilterTasks("build,test:unit,lint", "test:*"));
            """,
            run: true);
        AssertConsumerSucceeded(npmRunAll2Result);
        Assert.Equal(["> build", "test:unit"], OutputLines(npmRunAll2Result.RunStandardOutput));
    }

    private const string RichExportsJavaScript =
        """
        "use strict";

        class BaseCounter {
          constructor(start) {
            this.value = start;
          }

          add(delta) {
            this.value = this.value + delta;
            return this.value;
          }

          baseOnly() {
            return "base";
          }

          static baseLabel() {
            return "base-static";
          }
        }

        class Counter extends BaseCounter {
          #bonus = 2;

          constructor(start) {
            super(start);
          }

          add(delta) {
            return super.add(delta);
          }

          getValue() {
            return this.value;
          }

          getSecret() {
            return this.#bonus;
          }

          static get description() {
            return "counter";
          }
        }

        function add(x, y) {
          return x + y;
        }

        function optional(value = 4) {
          return value + 1;
        }

        function rest(...values) {
          return values.reduce((total, value) => total + value, 0);
        }

        function useCallback(callback, value) {
          return callback(value) + 1;
        }

        function identity() {
          return 1;
        }

        function createCounter(start) {
          return new Counter(start);
        }

        function getIdentity() {
          return identity;
        }

        function getCounterClass() {
          return Counter;
        }

        const sharedCounter = new Counter(12);
        function getSharedCounter() {
          return sharedCounter;
        }

        function makeAnonymousClass() {
          return class {
            constructor(value) {
              this.value = value;
            }
          };
        }

        function getValues() {
          return [5, , 7];
        }

        async function immediateAsync(value) {
          return value + 1;
        }

        async function delayedAsync(value) {
          await Promise.resolve();
          return value + 2;
        }

        async function createCounterAsync(start) {
          return new Counter(start);
        }

        async function objectAsync() {
          return graph;
        }

        async function rejectError() {
          throw new Error("reject error");
        }

        async function rejectValue() {
          throw "bad-value";
        }

        async function neverAsync() {
          return await new Promise(resolve => setTimeout(() => resolve(1), 1000));
        }

        function makeReceiver(arg) {
          const lexical = { marker: "lexical" };
          return {
            marker: "receiver",
            regular() { return this.marker; },
            arrow: () => lexical.marker,
            args: () => arg
          };
        }

        const receiver = makeReceiver("outer");
        const graph = {
          value: 1,
          child: {
            label: "child",
            read() { return this.label; }
          },
          values: [1, , 3],
          get doubled() { return this.value * 2; },
          set doubled(value) { this.value = value / 2; },
          read() { return this.value; },
          ["computed-name"]: "computed"
        };
        graph.self = graph;
        graph.alias = graph.child;
        const prototype = { inherited: "proto" };
        const protoObject = Object.create(prototype);
        protoObject.own = "own";
        const nullPrototypeObject = Object.create(null);
        nullPrototypeObject.value = "null-prototype";

        module.exports = {
          add,
          optional,
          rest,
          useCallback,
          arrowLexicalThis: () => receiver.arrow(),
          arrowLexicalArguments: () => receiver.args(),
          graph,
          protoObject,
          nullPrototypeObject,
          Counter,
          acceptCounter(counter) { return counter instanceof Counter; },
          createCounter,
          getIdentity,
          getCounterClass,
          getSharedCounter,
          makeAnonymousClass,
          getValues,
          marker: "root",
          methodThis() { return this.marker; },
          regularReceiver() { return receiver.regular(); },
          aliasA: identity,
          aliasB: identity,
          sameFunctionAlias() { return this.aliasA === this.aliasB; },
          throwSync() { throw new Error("sync boom"); },
          immediateAsync,
          delayedAsync,
          createCounterAsync,
          objectAsync,
          rejectError,
          rejectValue,
          neverAsync
        };
        """;

    private static void AssertAllowedPublicType(Type type, Assembly generatedAssembly, string context)
    {
        foreach (var inspected in FlattenType(type))
        {
            Assert.False(IsRuntimeType(inspected), $"{context} leaks {inspected.FullName}");
            Assert.True(
                inspected.Assembly == generatedAssembly
                || inspected.Namespace?.StartsWith("System", StringComparison.Ordinal) == true,
                $"{context} uses non-generated/non-BCL type {inspected.FullName}");

            if (typeof(Delegate).IsAssignableFrom(inspected))
            {
                var invoke = inspected.GetMethod("Invoke");
                if (invoke != null)
                {
                    AssertAllowedPublicType(
                        invoke.ReturnType,
                        generatedAssembly,
                        $"{context} delegate return");
                    foreach (var parameter in invoke.GetParameters())
                    {
                        AssertAllowedPublicType(
                            parameter.ParameterType,
                            generatedAssembly,
                            $"{context} delegate parameter");
                    }
                }
            }
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

    private static void AssertPublicAttributesDoNotLeakRuntime(
        MemberInfo member,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var attribute in member.GetCustomAttributesData())
        {
            AssertAttributeDoesNotLeakRuntime(attribute, generatedAssembly, context);
        }
    }

    private static void AssertPublicAttributesDoNotLeakRuntime(
        ParameterInfo parameter,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var attribute in parameter.GetCustomAttributesData())
        {
            AssertAttributeDoesNotLeakRuntime(attribute, generatedAssembly, context);
        }
    }

    private static void AssertAttributeDoesNotLeakRuntime(
        CustomAttributeData attribute,
        Assembly generatedAssembly,
        string context)
    {
        AssertAllowedPublicType(attribute.AttributeType, generatedAssembly, context);
        if (attribute.Constructor.DeclaringType != null)
        {
            AssertAllowedPublicType(
                attribute.Constructor.DeclaringType,
                generatedAssembly,
                $"{context} constructor");
        }

        foreach (var argument in attribute.ConstructorArguments)
        {
            AssertAllowedPublicType(
                argument.ArgumentType,
                generatedAssembly,
                $"{context} constructor argument");
        }

        foreach (var argument in attribute.NamedArguments)
        {
            AssertAllowedPublicType(
                argument.TypedValue.ArgumentType,
                generatedAssembly,
                $"{context} named argument");
        }
    }

    private static void AssertModifiersDoNotLeakRuntime(ParameterInfo parameter, string context)
    {
        foreach (var modifier in parameter.GetRequiredCustomModifiers().Concat(parameter.GetOptionalCustomModifiers()))
        {
            Assert.False(IsRuntimeType(modifier), $"{context} leaks {modifier.FullName}");
        }
    }

    private static bool IsRuntimeType(Type type)
        => string.Equals(type.Assembly.GetName().Name, "JavaScriptRuntime", StringComparison.Ordinal)
           || type.Namespace?.StartsWith("Jroc.Runtime", StringComparison.Ordinal) == true
           || type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true;

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

    private static GeneratedAssemblyConsumerHarness CreatePackageDeclarationHarness(
        string javaScript,
        string declaration,
        string assemblyName)
        => new(
            javaScript,
            assemblyName,
            new Dictionary<string, string>
            {
                ["node_modules/test-package/package.json"] =
                    """{"name":"test-package","main":"index.js","types":"index.d.ts"}""",
                ["node_modules/test-package/index.d.ts"] = declaration
            },
            entryFileName: "node_modules/test-package/index.js",
            rootModuleId: "test-package");

    private static void AssertRejectedDeclarationFallsBack(string declaration, string assemblyName)
    {
        using var harness = CreatePackageDeclarationHarness(
            "const key = 'late'; exports[key] = 9;",
            declaration,
            assemblyName);

        var result = harness.Build(
            $$"""
            using var exports = {{assemblyName}}.Import();
            Console.WriteLine(exports.Value != null);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["True"], OutputLines(result.RunStandardOutput));
    }

    private static string[] OutputLines(string output) =>
        output.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
}
