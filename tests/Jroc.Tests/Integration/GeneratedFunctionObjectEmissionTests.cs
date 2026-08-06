using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Jroc.Services;
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

        console.log(answer());
        console.log(outer());
        console.log(new Echo().method(5));
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
                Assert.Equal(
                    methodNames.Contains("ConstructCore", StringComparer.Ordinal)
                        ? ["get_IsConstructor", "get_RequiresInvocationContext", "CallCore", "ConstructCore"]
                        : ["get_IsConstructor", "get_RequiresInvocationContext", "CallCore"],
                    methodNames);
            });

        var answerType = Assert.Single(
            GetFunctionObjectTypes(first.Assembly),
            type => type.Name.EndsWith("_answer", StringComparison.Ordinal));
        var innerType = Assert.Single(
            GetFunctionObjectTypes(first.Assembly),
            type => type.Name.EndsWith("_inner", StringComparison.Ordinal));

        Assert.Empty(answerType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));

        var captureField = Assert.Single(
            innerType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.NotEqual(typeof(object), captureField.FieldType);
        Assert.Contains("Scope", captureField.FieldType.Name, StringComparison.Ordinal);

        var captureConstructor = Assert.Single(
            innerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public));
        var captureParameter = Assert.Single(captureConstructor.GetParameters());
        Assert.Equal(captureField.FieldType, captureParameter.ParameterType);

        var scopeInstance = Activator.CreateInstance(captureField.FieldType)!;
        var firstInner = captureConstructor.Invoke([scopeInstance]);
        var secondInner = captureConstructor.Invoke([scopeInstance]);
        Assert.NotSame(firstInner, secondInner);
        Assert.Same(scopeInstance, captureField.GetValue(firstInner));
        Assert.Same(scopeInstance, captureField.GetValue(secondInner));
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
        var methodFunction = Assert.IsAssignableFrom<JsFunctionObject>(
            Activator.CreateInstance(methodFunctionType));
        var echoType = compiled.Assembly.GetTypes().Single(type => type.Name == "Echo");
        var echo = Activator.CreateInstance(echoType);
        Assert.Equal(5d, CallableOperations.Call1(methodFunction, echo, 5d));
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
            .Where(type =>
                type.Namespace?.StartsWith("FunctionObjects.", StringComparison.Ordinal) == true)
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
