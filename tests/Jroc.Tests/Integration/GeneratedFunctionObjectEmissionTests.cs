using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Tests.Integration;

public sealed class GeneratedFunctionObjectEmissionTests
{
    private const string Source = """
        function answer() {
            return 42;
        }

        function outer() {
            let captured = 41;
            function inner() {
                return captured;
            }

            return inner();
        }

        class Echo {
            method(value) {
                return value;
            }
        }

        const increment = value => {
            {
                class FunctionObject {
                    read() {
                        return value;
                    }
                }

                new FunctionObject().read();
            }

            function FunctionObject() {
                return value + 1;
            }

            return FunctionObject();
        };

        console.log(answer());
        console.log(outer());
        console.log(new Echo().method(5));
        console.log(increment(5));
        """;

    private const string CallableFamilySource = """
        function ordinary(value) { return value; }
        const arrow = value => value;
        async function asyncValue(value) { return value; }
        function* generatorValue(value) { yield value; }
        async function* asyncGeneratorValue(value) { yield value; }

        class Example {
            method(value) { return value; }
            get accessor() { return 1; }
            set accessor(value) { this.value = value; }
            async asyncMethod(value) { return value; }
            *generatorMethod(value) { yield value; }
        }

        module.exports = {
            ordinary,
            arrow,
            asyncValue,
            generatorValue,
            asyncGeneratorValue,
            Example
        };
        """;

    [Fact]
    public void EmittedTypesAreDeterministicAndCalleeShaped()
    {
        using var first = CompileAndLoad(Source);
        using var second = CompileAndLoad(Source);

        var firstDescriptors = DescribeFunctionObjectTypes(first.Assembly);
        var secondDescriptors = DescribeFunctionObjectTypes(second.Assembly);

        Assert.NotEmpty(firstDescriptors);
        Assert.Equal(firstDescriptors, secondDescriptors);
        var emittedTypes = GetFunctionObjectTypes(first.Assembly);
        Assert.Equal(
            emittedTypes.Select(type => type.MetadataToken).Order(),
            emittedTypes.Select(type => type.MetadataToken));
        Assert.All(
            emittedTypes,
            type =>
            {
                var constructor = Assert.Single(
                    type.GetConstructors(
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.DeclaredOnly));
                var methods = type.GetMethods(
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.DeclaredOnly)
                    .OrderBy(method => method.MetadataToken)
                    .ToArray();
                Assert.True(constructor.MetadataToken < methods[0].MetadataToken);
                var methodNames = methods.Select(method => method.Name).ToArray();
                var expectedMethods = new List<string>
                {
                    "get_IsConstructor",
                    "get_RequiresInvocationContext"
                };
                if (methodNames.Contains("ResolveThisArgumentCore", StringComparer.Ordinal))
                {
                    expectedMethods.Add("ResolveThisArgumentCore");
                }
                expectedMethods.Add("CallCore");
                if (methodNames.Contains("ConstructBodyCore", StringComparer.Ordinal))
                {
                    expectedMethods.Add("ConstructBodyCore");
                }
                if (methodNames.Contains("ConstructCore", StringComparer.Ordinal))
                {
                    expectedMethods.Add("ConstructCore");
                }
                Assert.Equal(expectedMethods, methodNames);
            });

        var answerType = Assert.Single(
            GetFunctionObjectTypes(first.Assembly),
            type => type.Name.EndsWith("_answer", StringComparison.Ordinal));
        var innerType = Assert.Single(
            GetFunctionObjectTypes(first.Assembly),
            type => type.Name.EndsWith("_inner", StringComparison.Ordinal));
        var arrowTypes = GetFunctionObjectTypes(first.Assembly)
            .Where(type => type.DeclaringType?.Name.StartsWith(
                    "ArrowFunction_",
                    StringComparison.Ordinal) == true)
            .ToArray();

        Assert.Empty(answerType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.All(
            emittedTypes,
            type =>
            {
                Assert.NotNull(type.DeclaringType);
                Assert.DoesNotContain(
                    "FunctionObjects.",
                    type.Namespace,
                    StringComparison.Ordinal);
            });
        Assert.Equal("answer", answerType.DeclaringType!.Name);
        Assert.Equal("inner", innerType.DeclaringType!.Name);
        Assert.NotEmpty(arrowTypes);
        Assert.All(
            arrowTypes,
            arrowType =>
            {
                Assert.Equal(GeneratedFunctionObjectNaming.WrapperTypeName, arrowType.Name);
                Assert.NotNull(arrowType.DeclaringType);
            });
        Assert.True(
            first.Assembly.GetTypes().Count(type => type.Name.StartsWith(
                $"<User>{GeneratedFunctionObjectNaming.WrapperTypeName}_",
                StringComparison.Ordinal)) >= 2);

        var innerFields = innerType.GetFields(
            BindingFlags.Instance | BindingFlags.NonPublic);
        var captureField = Assert.Single(
            innerFields,
            field => field.FieldType != typeof(object[]));
        var transitionalScopesField = Assert.Single(
            innerFields,
            field => field.FieldType == typeof(object[]));
        Assert.NotEqual(typeof(object), captureField.FieldType);
        Assert.Contains("Scope", captureField.FieldType.Name, StringComparison.Ordinal);

        var captureConstructor = Assert.Single(
            innerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public));
        var captureParameters = captureConstructor.GetParameters();
        Assert.Equal(2, captureParameters.Length);
        var captureParameter = captureParameters[0];
        Assert.Equal(captureField.FieldType, captureParameter.ParameterType);
        Assert.Equal(typeof(object[]), captureParameters[1].ParameterType);

        var scopeInstance = Activator.CreateInstance(captureField.FieldType)!;
        object[] scopes = [scopeInstance];
        var firstInner = captureConstructor.Invoke([scopeInstance, scopes]);
        var secondInner = captureConstructor.Invoke([scopeInstance, scopes]);
        Assert.NotSame(firstInner, secondInner);
        Assert.Same(scopeInstance, captureField.GetValue(firstInner));
        Assert.Same(scopeInstance, captureField.GetValue(secondInner));
        Assert.Same(scopes, transitionalScopesField.GetValue(firstInner));
    }

    [Fact]
    public void TypedBodyAndDynamicAdapterKeepSeparateSignatures()
    {
        using var compiled = CompileAndLoad(Source);
        var answerType = Assert.Single(
            GetFunctionObjectTypes(compiled.Assembly),
            type => type.Name.EndsWith("_answer", StringComparison.Ordinal));

        var functionObject = Assert.IsAssignableFrom<JsFunctionObject>(
            Activator.CreateInstance(answerType));
        Assert.NotSame(
            functionObject,
            Activator.CreateInstance(answerType));
        Assert.Equal(42d, CallableOperations.Call0(functionObject, null));

        var callAdapter = answerType.GetMethod(
            "CallCore",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        Assert.Equal(typeof(object), callAdapter.ReturnType);
        Assert.Equal(
            typeof(JsCallArguments).MakeByRefType(),
            callAdapter.GetParameters()[1].ParameterType);

        var ownerType = compiled.Assembly
            .GetTypes()
            .Single(type => type.Name == "answer");
        var typedBody = ownerType.GetMethod(
            "__js_call__",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        Assert.Equal(typeof(double), typedBody.ReturnType);
        Assert.Equal(42d, typedBody.Invoke(null, [null]));
        Assert.DoesNotContain(
            unchecked((byte)OpCodes.Box.Value),
            typedBody.GetMethodBody()!.GetILAsByteArray()!);
        Assert.Contains(
            unchecked((byte)OpCodes.Box.Value),
            callAdapter.GetMethodBody()!.GetILAsByteArray()!);

        var methodFunctionType = Assert.Single(
            GetFunctionObjectTypes(compiled.Assembly),
            type => type.Name.Contains("ClassMethod", StringComparison.Ordinal)
                && type.Name.EndsWith("_Echo_method", StringComparison.Ordinal));
        Assert.Equal("Echo", methodFunctionType.DeclaringType!.Name);
        var methodFunction = Assert.IsAssignableFrom<JsFunctionObject>(
            Activator.CreateInstance(methodFunctionType));
        var echoType = compiled.Assembly.GetTypes().Single(type => type.Name == "Echo");
        var echo = Activator.CreateInstance(echoType);
        Assert.Equal(5d, CallableOperations.Call1(methodFunction, echo, 5d));
    }

    [Fact]
    public void MaterializedCallableFamiliesNeverUseRawDelegateStorage()
    {
        using var compiled = CompileAndLoad(CallableFamilySource);
        var functionObjectTypes = GetFunctionObjectTypes(compiled.Assembly);

        Assert.True(functionObjectTypes.Length >= 10);
        Assert.Contains(
            functionObjectTypes,
            type => typeof(JsAsyncFunctionObject).IsAssignableFrom(type));
        Assert.Contains(
            functionObjectTypes,
            type => type.Name.Contains("ClassMethod", StringComparison.Ordinal));
        Assert.Contains(
            functionObjectTypes,
            type => type.Name.Contains("ClassGetter", StringComparison.Ordinal));
        Assert.Contains(
            functionObjectTypes,
            type => type.Name.Contains("ClassSetter", StringComparison.Ordinal));

        Assert.DoesNotContain(
            compiled.Assembly.GetTypes()
                .SelectMany(type => type.GetFields(
                    BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic)),
            field => typeof(Delegate).IsAssignableFrom(field.FieldType));

        Assert.All(
            functionObjectTypes,
            type =>
            {
                Assert.True(typeof(JsFunctionObject).IsAssignableFrom(type));
                Assert.DoesNotContain(
                    type.GetFields(
                        BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.Public
                        | BindingFlags.NonPublic),
                    field => typeof(Delegate).IsAssignableFrom(field.FieldType));
                Assert.DoesNotContain(
                    type.GetMethods(
                        BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.DeclaredOnly),
                    method => typeof(Delegate).IsAssignableFrom(method.ReturnType)
                        || method.GetParameters().Any(parameter =>
                            typeof(Delegate).IsAssignableFrom(
                                parameter.ParameterType)));
            });
    }

    [Fact]
    public void CommonArityAdaptersDoNotAllocateArgumentArrays()
    {
        using var compiled = CompileAndLoad(
            "function identity(value) { return arguments[0]; } module.exports = identity;");
        var functionObjectType = Assert.Single(
            GetFunctionObjectTypes(compiled.Assembly));
        var callAdapter = functionObjectType.GetMethod(
            "CallCore",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        var arrayAdapter = functionObjectType.GetMethod(
            "__js_call_with_arguments__",
            BindingFlags.Static | BindingFlags.NonPublic)!;
        var canonicalBody = functionObjectType.DeclaringType!.GetMethod(
            "__js_call__",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;

        Assert.Equal(
            [typeof(object), typeof(JsCallArguments).MakeByRefType()],
            callAdapter.GetParameters().Select(parameter => parameter.ParameterType));
        Assert.DoesNotContain(
            unchecked((byte)OpCodes.Newarr.Value),
            callAdapter.GetMethodBody()!.GetILAsByteArray()!);
        Assert.Equal(
            [typeof(JsFunctionObject), typeof(object[]), typeof(object[])],
            arrayAdapter.GetParameters().Select(parameter => parameter.ParameterType));
        Assert.DoesNotContain(
            typeof(object[]),
            canonicalBody.GetParameters().Skip(1).Select(parameter => parameter.ParameterType));
    }

    [Fact]
    public void StaticArrayAdaptersAreReservedForArrayBoundaries()
    {
        using var simple = CompileAndLoad(
            "function identity(value) { return value; } module.exports = identity;");
        var simpleFunctionType = Assert.Single(
            GetFunctionObjectTypes(simple.Assembly));
        Assert.Null(simpleFunctionType.GetMethod(
            "__js_call_with_arguments__",
            BindingFlags.Static | BindingFlags.NonPublic));

        using var argumentSensitive = CompileAndLoad(
            "function identity(value) { return arguments[0]; } module.exports = identity;");
        var argumentSensitiveType = Assert.Single(
            GetFunctionObjectTypes(argumentSensitive.Assembly));
        Assert.NotNull(argumentSensitiveType.GetMethod(
            "__js_call_with_arguments__",
            BindingFlags.Static | BindingFlags.NonPublic));
    }

    private static string[] DescribeFunctionObjectTypes(Assembly assembly)
    {
        return GetFunctionObjectTypes(assembly)
            .Select(type =>
            {
                var fields = string.Join(
                    ",",
                    type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Select(field => $"{field.Name}:{field.FieldType.FullName}"));
                var methods = string.Join(
                    ",",
                    type.GetMethods(
                            BindingFlags.Instance
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.DeclaredOnly)
                        .OrderBy(method => method.MetadataToken)
                        .Select(method =>
                            $"{method.Name}:{method.ReturnType.FullName}({string.Join(",", method.GetParameters().Select(parameter => parameter.ParameterType.FullName))})"));
                return $"{type.FullName}|{fields}|{methods}";
            })
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static Type[] GetFunctionObjectTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type => type != typeof(JsFunctionObject)
                && typeof(JsFunctionObject).IsAssignableFrom(type))
            .OrderBy(type => type.MetadataToken)
            .ToArray();
    }

    private static CompiledAssembly CompileAndLoad(string source)
    {
        var outputDirectory = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "GeneratedFunctionObjects",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);
        var inputPath = Path.Combine(outputDirectory, "generated-function-objects.js");
        File.WriteAllText(inputPath, source);

        var options = new CompilerOptions
        {
            OutputDirectory = outputDirectory
        };
        var services = CompilerServices.BuildServiceProvider(
            options,
            fileSystem: null,
            new TestLogger());
        var compiler = services.GetRequiredService<Compiler>();
        Assert.True(compiler.Compile(inputPath));

        var outputPath = Path.Combine(
            outputDirectory,
            "generated-function-objects.dll");
        var loadContext = new AssemblyLoadContext(
            $"generated-function-objects-{Guid.NewGuid():N}",
            isCollectible: true);
        loadContext.Resolving += static (_, name) =>
            string.Equals(
                name.Name,
                typeof(JsObject).Assembly.GetName().Name,
                StringComparison.Ordinal)
                ? typeof(JsObject).Assembly
                : null;
        using var stream = File.OpenRead(outputPath);
        var assembly = loadContext.LoadFromStream(stream);
        return new CompiledAssembly(
            outputDirectory,
            assembly,
            loadContext,
            services);
    }

    private sealed class CompiledAssembly(
        string outputDirectory,
        Assembly assembly,
        AssemblyLoadContext loadContext,
        IServiceProvider services) : IDisposable
    {
        public Assembly Assembly { get; } = assembly;

        public void Dispose()
        {
            if (services is IDisposable disposable)
            {
                disposable.Dispose();
            }
            loadContext.Unload();
            try
            {
                Directory.Delete(outputDirectory, recursive: true);
            }
            catch
            {
            }
        }
    }
}
