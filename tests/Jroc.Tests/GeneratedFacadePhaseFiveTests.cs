using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseFiveTests
{
    [Fact]
    public async Task ReviewedPublicSurface_IsDeterministicAndContainsNoRuntimeImplementationTypes()
    {
        using var first = CompileSurface();
        using var second = CompileSurface();

        Assert.Equal(first.Surface, second.Surface);
        AssertPublicSurfaceDoesNotLeakRuntime(first.Assembly);
        AssertPublicSurfaceDoesNotLeakRuntime(second.Assembly);

        var settings = new VerifySettings();
        settings.UseDirectory(Path.Combine(
            Path.GetDirectoryName(GetSourceFilePath())!,
            "Snapshots"));
        await Verify(first.Surface, settings);
    }

    [Fact]
    public void SeparateConsumer_UsesMultiplePackageStyleAssembliesWithIsolationConcurrencyAndFailures()
    {
        using var alpha = new GeneratedAssemblyConsumerHarness(
            """
            require("./deep/tools/math.js");
            if (false) { require("./deep/failure.js"); }
            globalThis.loadCount = (globalThis.loadCount || 0) + 1;
            module.exports = {
              loadCount() { return loadCount; },
              label: "alpha"
            };
            """,
            "alpha.tools",
            new Dictionary<string, string>
            {
                ["node_modules/@scope/alpha/deep/tools/math.js"] =
                    "module.exports = { add(left, right) { return left + right; } };",
                ["node_modules/@scope/alpha/deep/failure.js"] =
                    "module.exports = { value: 1 }; throw new Error('alpha load failure');"
            },
            entryFileName: "node_modules/@scope/alpha/index.js",
            rootModuleId: "@scope/alpha");
        using var beta = new GeneratedAssemblyConsumerHarness(
            """
            require("./features/deep/label.js");
            globalThis.loadCount = (globalThis.loadCount || 0) + 1;
            module.exports = {
              loadCount() { return loadCount; },
              label: "beta"
            };
            """,
            "beta.package",
            new Dictionary<string, string>
            {
                ["node_modules/@scope/beta/features/deep/label.js"] =
                    "module.exports = { value: 'deep-beta' };"
            },
            entryFileName: "node_modules/@scope/beta/index.js",
            rootModuleId: "@scope/beta");

        var result = BuildMultiAssemblyConsumer(
            alpha,
            beta,
            """
            alpha_tools.Run([]);
            beta_package.Run([]);
            Console.WriteLine("runs");

            for (var index = 0; index < 3; index++)
            {
                using var alphaExports = alpha_tools.Import();
                using var betaExports = beta_package.Import();
                Console.WriteLine($"{alphaExports.Label}:{alphaExports.LoadCount()}");
                Console.WriteLine($"{betaExports.Label}:{betaExports.LoadCount()}");
            }

            using (var math = alpha_tools.Scripts.deep.tools.math.Import())
            using (var label = beta_package.Scripts.features.deep.label.Import())
            {
                Console.WriteLine(math.Add(20, 22));
                Console.WriteLine(label.Value);
            }

            var concurrent = await Task.WhenAll(
                Enumerable.Range(0, 8).Select(async index =>
                {
                    await Task.Yield();
                    using var exports = index % 2 == 0
                        ? alpha_tools.Import()
                        : null;
                    using var other = index % 2 == 0
                        ? null
                        : beta_package.Import();
                    return exports?.Label ?? other!.Label;
                }));
            Console.WriteLine(string.Join(",", concurrent));

            for (var index = 0; index < 2; index++)
            {
                try
                {
                    using var failure = alpha_tools.Scripts.deep.failure.Import();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.GetType().Name);
                    Console.WriteLine(
                        exception.InnerException?.Message.Contains(
                            "alpha load failure",
                            StringComparison.Ordinal) == true);
                }
            }
            """);

        Assert.True(
            result.BuildExitCode == 0,
            $"Consumer build failed.{Environment.NewLine}{result.BuildDiagnostics}");
        Assert.True(
            result.RunExitCode == 0,
            $"Consumer run failed.{Environment.NewLine}" +
            $"{result.RunStandardOutput}{Environment.NewLine}{result.RunStandardError}");
        Assert.Equal(
            [
                "runs",
                "alpha:1", "beta:1",
                "alpha:1", "beta:1",
                "alpha:1", "beta:1",
                "42",
                "deep-beta",
                "alpha,beta,alpha,beta,alpha,beta,alpha,beta",
                "JsModuleLoadException", "True",
                "JsModuleLoadException", "True"
            ],
            OutputLines(result.RunStandardOutput));
    }

    private static SurfaceFixture CompileSurface()
    {
        var harness = new GeneratedAssemblyConsumerHarness(
            RepresentativeJavaScript,
            "PhaseFive.Surface",
            new Dictionary<string, string>
            {
                ["deep/package/contracts/toolkit.js"] =
                    """
                    class DeepCounter {
                      constructor(value) { this.value = value; }
                      increment() { return ++this.value; }
                    }
                    module.exports = {
                      multiply(left, right) { return left * right; },
                      DeepCounter
                    };
                    """
            });
        var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);
        return new SurfaceFixture(
            harness,
            loaded,
            DescribePublicSurface(loaded.Assembly));
    }

    private static string DescribePublicSurface(Assembly assembly)
    {
        var lines = new List<string>();
        foreach (var type in assembly.GetTypes()
                     .Where(type => type.IsVisible)
                     .OrderBy(type => FormatType(type), StringComparer.Ordinal))
        {
            lines.Add($"type {DescribeType(type)}");

            foreach (var constructor in type.GetConstructors(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly)
                     .OrderBy(FormatMethodBase, StringComparer.Ordinal))
            {
                lines.Add($"  ctor {FormatMethodBase(constructor)}");
            }

            foreach (var field in type.GetFields(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly)
                     .OrderBy(field => field.Name, StringComparer.Ordinal))
            {
                lines.Add(
                    $"  field {(field.IsStatic ? "static " : string.Empty)}" +
                    $"{FormatType(field.FieldType)} {field.Name}");
            }

            foreach (var property in type.GetProperties(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly)
                     .OrderBy(property => property.Name, StringComparer.Ordinal))
            {
                var accessor = property.GetMethod ?? property.SetMethod;
                lines.Add(
                    $"  property {(accessor?.IsStatic == true ? "static " : string.Empty)}" +
                    $"{FormatType(property.PropertyType)} {property.Name} " +
                    $"{{ {(property.CanRead ? "get; " : string.Empty)}" +
                    $"{(property.CanWrite ? "set; " : string.Empty)}}}");
            }

            foreach (var @event in type.GetEvents(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly)
                     .OrderBy(@event => @event.Name, StringComparer.Ordinal))
            {
                lines.Add($"  event {FormatType(@event.EventHandlerType!)} {@event.Name}");
            }

            foreach (var method in type.GetMethods(
                         BindingFlags.Public
                         | BindingFlags.Instance
                         | BindingFlags.Static
                         | BindingFlags.DeclaredOnly)
                     .Where(method => !method.IsSpecialName)
                     .OrderBy(FormatMethodBase, StringComparer.Ordinal))
            {
                lines.Add(
                    $"  method {(method.IsStatic ? "static " : string.Empty)}" +
                    $"{FormatType(method.ReturnType)} {FormatMethodBase(method)}");
            }
        }

        return string.Join('\n', lines);
    }

    private static string DescribeType(Type type)
    {
        var kind = type.IsInterface
            ? "interface"
            : type.IsEnum
                ? "enum"
                : typeof(MulticastDelegate).IsAssignableFrom(type.BaseType)
                    ? "delegate"
                    : type.IsValueType
                        ? "struct"
                        : type.IsAbstract && type.IsSealed
                            ? "static class"
                            : "class";
        var inheritance = new List<string>();
        if (type.BaseType != null && type.BaseType != typeof(object))
        {
            inheritance.Add(FormatType(type.BaseType));
        }
        inheritance.AddRange(type.GetInterfaces().Select(FormatType));
        return $"{kind} {FormatType(type)}" +
               (inheritance.Count == 0
                   ? string.Empty
                   : $" : {string.Join(", ", inheritance.Order(StringComparer.Ordinal))}");
    }

    private static string FormatMethodBase(MethodBase method)
    {
        var genericArguments = method.IsGenericMethod
            ? $"<{string.Join(", ", method.GetGenericArguments().Select(FormatType))}>"
            : string.Empty;
        return $"{method.Name}{genericArguments}(" +
               string.Join(
                   ", ",
                   method.GetParameters().Select(parameter =>
                       $"{FormatType(parameter.ParameterType)} {parameter.Name}")) +
               ")";
    }

    private static string FormatType(Type type)
    {
        if (type.IsByRef)
        {
            return $"{FormatType(type.GetElementType()!)}&";
        }
        if (type.IsPointer)
        {
            return $"{FormatType(type.GetElementType()!)}*";
        }
        if (type.IsArray)
        {
            return $"{FormatType(type.GetElementType()!)}[{new string(',', type.GetArrayRank() - 1)}]";
        }
        if (type.IsGenericParameter)
        {
            return type.Name;
        }

        var name = (type.FullName ?? type.Name).Replace('+', '.');
        var tick = name.IndexOf('`');
        if (tick >= 0)
        {
            name = name[..tick];
        }
        if (!type.IsGenericType)
        {
            return name;
        }
        return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(FormatType))}>";
    }

    private static void AssertPublicSurfaceDoesNotLeakRuntime(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(type => type.IsVisible))
        {
            AssertTypeGraph(type, assembly, $"{type.FullName} type");
            if (type.BaseType != null)
            {
                AssertTypeGraph(type.BaseType, assembly, $"{type.FullName} base");
            }
            foreach (var iface in type.GetInterfaces())
            {
                AssertTypeGraph(iface, assembly, $"{type.FullName} interface");
            }
            AssertGenericConstraints(type.GetGenericArguments(), assembly, $"{type.FullName} generic");
            AssertAttributesDoNotLeakRuntime(
                type.GetCustomAttributesData(),
                assembly,
                $"{type.FullName} attributes");

            foreach (var constructor in type.GetConstructors(PublicDeclaredMembers))
            {
                AssertMethodBaseDoesNotLeakRuntime(constructor, assembly);
            }
            foreach (var method in type.GetMethods(PublicDeclaredMembers))
            {
                AssertMethodBaseDoesNotLeakRuntime(method, assembly);
                AssertParameterDoesNotLeakRuntime(
                    method.ReturnParameter,
                    assembly,
                    $"{method} return");
                AssertGenericConstraints(
                    method.GetGenericArguments(),
                    assembly,
                    $"{method} generic");
            }
            foreach (var field in type.GetFields(PublicDeclaredMembers))
            {
                AssertTypeGraph(field.FieldType, assembly, field.ToString()!);
                AssertModifiersDoNotLeakRuntime(
                    field.GetRequiredCustomModifiers()
                        .Concat(field.GetOptionalCustomModifiers()),
                    assembly,
                    $"{field} modifiers");
                AssertAttributesDoNotLeakRuntime(
                    field.GetCustomAttributesData(),
                    assembly,
                    $"{field} attributes");
            }
            foreach (var property in type.GetProperties(PublicDeclaredMembers))
            {
                AssertTypeGraph(property.PropertyType, assembly, property.ToString()!);
                AssertModifiersDoNotLeakRuntime(
                    property.GetRequiredCustomModifiers()
                        .Concat(property.GetOptionalCustomModifiers()),
                    assembly,
                    $"{property} modifiers");
                AssertAttributesDoNotLeakRuntime(
                    property.GetCustomAttributesData(),
                    assembly,
                    $"{property} attributes");
                foreach (var parameter in property.GetIndexParameters())
                {
                    AssertParameterDoesNotLeakRuntime(parameter, assembly, property.ToString()!);
                }
            }
            foreach (var @event in type.GetEvents(PublicDeclaredMembers))
            {
                AssertTypeGraph(@event.EventHandlerType!, assembly, @event.ToString()!);
                AssertAttributesDoNotLeakRuntime(
                    @event.GetCustomAttributesData(),
                    assembly,
                    $"{@event} attributes");
            }
        }
    }

    private static void AssertMethodBaseDoesNotLeakRuntime(
        MethodBase method,
        Assembly generatedAssembly)
    {
        AssertAttributesDoNotLeakRuntime(
            method.GetCustomAttributesData(),
            generatedAssembly,
            $"{method} attributes");
        foreach (var parameter in method.GetParameters())
        {
            AssertParameterDoesNotLeakRuntime(parameter, generatedAssembly, method.ToString()!);
        }
    }

    private static void AssertParameterDoesNotLeakRuntime(
        ParameterInfo parameter,
        Assembly generatedAssembly,
        string context)
    {
        AssertTypeGraph(parameter.ParameterType, generatedAssembly, context);
        AssertModifiersDoNotLeakRuntime(
            parameter.GetRequiredCustomModifiers()
                .Concat(parameter.GetOptionalCustomModifiers()),
            generatedAssembly,
            $"{context} modifiers");
        AssertAttributesDoNotLeakRuntime(
            parameter.GetCustomAttributesData(),
            generatedAssembly,
            $"{context} attributes");
    }

    private static void AssertTypeGraph(
        Type type,
        Assembly generatedAssembly,
        string context,
        HashSet<Type>? visited = null)
    {
        visited ??= [];
        if (!visited.Add(type))
        {
            return;
        }

        if (type.HasElementType)
        {
            AssertTypeGraph(type.GetElementType()!, generatedAssembly, context, visited);
            return;
        }
        if (type.IsGenericParameter)
        {
            AssertGenericConstraints([type], generatedAssembly, context);
            return;
        }

        Assert.False(IsRuntimeType(type), $"{context} leaks {type.AssemblyQualifiedName}");
        Assert.True(
            type.Assembly == generatedAssembly
            || type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true,
            $"{context} uses non-generated/non-BCL type {type.AssemblyQualifiedName}");

        if (type.IsGenericType)
        {
            AssertTypeGraph(type.GetGenericTypeDefinition(), generatedAssembly, context, visited);
            foreach (var argument in type.GetGenericArguments())
            {
                AssertTypeGraph(argument, generatedAssembly, context, visited);
            }
        }

        if (typeof(Delegate).IsAssignableFrom(type))
        {
            var invoke = type.GetMethod("Invoke");
            if (invoke != null)
            {
                AssertTypeGraph(
                    invoke.ReturnType,
                    generatedAssembly,
                    $"{context} delegate return",
                    visited);
                foreach (var parameter in invoke.GetParameters())
                {
                    AssertTypeGraph(
                        parameter.ParameterType,
                        generatedAssembly,
                        $"{context} delegate parameter",
                        visited);
                    AssertModifiersDoNotLeakRuntime(
                        parameter.GetRequiredCustomModifiers()
                            .Concat(parameter.GetOptionalCustomModifiers()),
                        generatedAssembly,
                        $"{context} delegate modifiers");
                }
            }
        }
    }

    private static void AssertGenericConstraints(
        IEnumerable<Type> genericParameters,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var parameter in genericParameters.Where(type => type.IsGenericParameter))
        {
            foreach (var constraint in parameter.GetGenericParameterConstraints())
            {
                AssertTypeGraph(
                    constraint,
                    generatedAssembly,
                    $"{context} constraint");
            }
            AssertAttributesDoNotLeakRuntime(
                parameter.GetCustomAttributesData(),
                generatedAssembly,
                $"{context} parameter attributes");
        }
    }

    private static void AssertAttributesDoNotLeakRuntime(
        IEnumerable<CustomAttributeData> attributes,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var attribute in attributes)
        {
            AssertTypeGraph(attribute.AttributeType, generatedAssembly, context);
            if (attribute.Constructor.DeclaringType != null)
            {
                AssertTypeGraph(
                    attribute.Constructor.DeclaringType,
                    generatedAssembly,
                    $"{context} constructor");
            }
            foreach (var argument in attribute.ConstructorArguments)
            {
                AssertAttributeArgumentDoesNotLeakRuntime(
                    argument,
                    generatedAssembly,
                    $"{context} constructor argument");
            }
            foreach (var argument in attribute.NamedArguments)
            {
                AssertAttributeArgumentDoesNotLeakRuntime(
                    argument.TypedValue,
                    generatedAssembly,
                    $"{context} named argument");
            }
        }
    }

    private static void AssertAttributeArgumentDoesNotLeakRuntime(
        CustomAttributeTypedArgument argument,
        Assembly generatedAssembly,
        string context)
    {
        AssertTypeGraph(argument.ArgumentType, generatedAssembly, context);
        if (argument.Value is Type type)
        {
            AssertTypeGraph(type, generatedAssembly, $"{context} value");
        }
        else if (argument.Value is IEnumerable<CustomAttributeTypedArgument> values)
        {
            foreach (var value in values)
            {
                AssertAttributeArgumentDoesNotLeakRuntime(value, generatedAssembly, context);
            }
        }
    }

    private static void AssertModifiersDoNotLeakRuntime(
        IEnumerable<Type> modifiers,
        Assembly generatedAssembly,
        string context)
    {
        foreach (var modifier in modifiers)
        {
            AssertTypeGraph(modifier, generatedAssembly, context);
        }
    }

    private static bool IsRuntimeType(Type type)
        => string.Equals(type.Assembly.GetName().Name, "JavaScriptRuntime", StringComparison.Ordinal)
           || type.Namespace?.StartsWith("Jroc.Runtime", StringComparison.Ordinal) == true
           || type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true;

    private static GeneratedAssemblyConsumerResult BuildMultiAssemblyConsumer(
        GeneratedAssemblyConsumerHarness first,
        GeneratedAssemblyConsumerHarness second,
        string consumerSource)
    {
        var projectDirectory = Path.Combine(first.WorkingDirectory, "multi-consumer");
        var firstAssembly = first.Artifact.Materialize(
            Path.Combine(projectDirectory, "generated", "first"));
        var secondAssembly = second.Artifact.Materialize(
            Path.Combine(projectDirectory, "generated", "second"));
        Directory.CreateDirectory(projectDirectory);

        var projectText = CreateMultiAssemblyProject(
            firstAssembly.AssemblyPath,
            secondAssembly.AssemblyPath);
        GeneratedAssemblyConsumerHarness.AssertNoDirectRuntimeReference(
            consumerSource,
            projectText);
        File.WriteAllText(Path.Combine(projectDirectory, "Program.cs"), consumerSource);
        File.WriteAllText(Path.Combine(projectDirectory, "Consumer.csproj"), projectText);

        var build = RunProcess(
            "dotnet",
            ["build", "Consumer.csproj", "--nologo", "--verbosity:minimal"],
            projectDirectory);
        if (build.ExitCode != 0)
        {
            return new GeneratedAssemblyConsumerResult(
                build.ExitCode,
                build.StandardOutput,
                build.StandardError,
                null,
                string.Empty,
                string.Empty);
        }

        var outputDirectory = Path.Combine(projectDirectory, "bin", "Debug", "net10.0");
        var runtimePath = typeof(JavaScriptRuntime.ObjectRuntime).Assembly.Location;
        File.Copy(
            runtimePath,
            Path.Combine(outputDirectory, Path.GetFileName(runtimePath)),
            overwrite: true);
        var run = RunProcess(
            "dotnet",
            [Path.Combine(outputDirectory, "Consumer.dll")],
            projectDirectory);
        return new GeneratedAssemblyConsumerResult(
            build.ExitCode,
            build.StandardOutput,
            build.StandardError,
            run.ExitCode,
            run.StandardOutput,
            run.StandardError);
    }

    private static string CreateMultiAssemblyProject(
        string firstAssemblyPath,
        string secondAssemblyPath)
    {
        var first = SecurityElement.Escape(firstAssemblyPath);
        var second = SecurityElement.Escape(secondAssemblyPath);
        return $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
              </PropertyGroup>
              <ItemGroup>
                <Reference Include="AlphaTools">
                  <HintPath>{{first}}</HintPath>
                  <Private>true</Private>
                </Reference>
                <Reference Include="BetaPackage">
                  <HintPath>{{second}}</HintPath>
                  <Private>true</Private>
                </Reference>
              </ItemGroup>
            </Project>
            """;
    }

    private static ProcessResult RunProcess(
        string fileName,
        IReadOnlyList<string> arguments,
        string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not start '{fileName}'.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(120_000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"Process '{fileName}' timed out.");
        }
        return new ProcessResult(
            process.ExitCode,
            standardOutput.GetAwaiter().GetResult(),
            standardError.GetAwaiter().GetResult());
    }

    private static string[] OutputLines(string output)
        => output.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

    private static string GetSourceFilePath([CallerFilePath] string path = "") => path;

    private const BindingFlags PublicDeclaredMembers =
        BindingFlags.Public
        | BindingFlags.Instance
        | BindingFlags.Static
        | BindingFlags.DeclaredOnly;

    private const string RepresentativeJavaScript =
        """
        require("./deep/package/contracts/toolkit.js");

        class Counter {
          constructor(value) { this.value = value; }
          increment() { return ++this.value; }
          values() { return [this.value, this.value + 1]; }
        }

        function add(left, right) { return left + right; }
        async function delayed(value) {
          await Promise.resolve();
          return value + 1;
        }
        function* sequence(prefix) {
          yield prefix + "1";
          yield prefix + "2";
        }
        async function* asyncSequence(prefix) {
          yield prefix + "1";
          await Promise.resolve();
          yield prefix + "2";
        }

        const buffer = new ArrayBuffer(8);
        const key = {};
        module.exports = {
          add,
          delayed,
          sequence,
          asyncSequence,
          Counter,
          createCounter(value) { return new Counter(value); },
          date: new Date(1234),
          pattern: new RegExp("a+", "g"),
          error: new TypeError("surface"),
          symbol: Symbol.for("surface"),
          map: new Map([["first", 1], [key, 2]]),
          set: new Set(["a", "b"]),
          buffer,
          view: new DataView(buffer),
          bytes: new Uint8Array(buffer),
          nested: {
            callback(value) { return value; },
            values: [1, 2, 3]
          }
        };
        """;

    private sealed record ProcessResult(
        int ExitCode,
        string StandardOutput,
        string StandardError);

    private sealed class SurfaceFixture : IDisposable
    {
        private readonly JrocLoadedAssembly _loaded;
        private readonly GeneratedAssemblyConsumerHarness _harness;

        public SurfaceFixture(
            GeneratedAssemblyConsumerHarness harness,
            JrocLoadedAssembly loaded,
            string surface)
        {
            _loaded = loaded;
            _harness = harness;
            Surface = surface;
        }

        public Assembly Assembly => _loaded.Assembly;

        public string Surface { get; }

        public void Dispose()
        {
            _loaded.Dispose();
            _harness.Dispose();
        }
    }
}
