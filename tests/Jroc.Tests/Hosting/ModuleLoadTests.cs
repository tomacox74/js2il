using System.Reflection;
using System.Runtime.Loader;
using JavaScriptRuntime;
using ChildProcessLaunchRequest = JavaScriptRuntime.Node.ChildProcessLaunchRequest;
using IChildProcessLauncher = JavaScriptRuntime.Node.IChildProcessLauncher;
using Jroc.Runtime;
using Jroc.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Jroc.Tests.Hosting;

public class ModuleLoadTests
{
    private const string HostingJavaScriptResourcePrefix = "Jroc.Tests.Hosting.JavaScript.";
    private delegate object? HostedSingleScopeDelegate(
        PackedArgumentsHost scope,
        object? newTarget,
        object? addend);

    public interface IMathExports : IDisposable
    {
        string Version { get; }
        double Add(double x, double y);
    }

    public interface IObjectReturnExports : IDisposable
    {
        object GetWindow();
        string GetTitle(object win);
        double GetHostValue(object win);
    }

    public interface IGeneratedCallableExports : IDisposable
    {
        JsCallable Ordinary { get; }
        JsCallable Describe { get; }
        JsCallable SumSeven { get; }
        JsCallable DoubleAsync { get; }
        JsCallable RejectAsync { get; }
        JsCallable Sequence { get; }
        JsCallable Thrower { get; }
        JsCallable Person { get; }
        JsCallable NewTargetProbe { get; }
        JsCallable Arrow { get; }
        object Nested { get; }
        object Echo(object value);
        bool Same(object left, object right);
        object InspectCallback(object callback);
        double InvokeCallback(object callback, double left, double right);
        string InvokeCallbackVariadic(object callback);
        Task<object?> AwaitCallback(
            object callback,
            object? first,
            object? second);
        string InvokeCallbackWithReceiver(
            object callback,
            string value,
            string prefix,
            string suffix);
        object ConstructCallback(object callback, double value);
        double ReadValue(object value);
    }

    private sealed class CompiledModuleAssembly : IDisposable
    {
        private readonly string _outputDir;
        private readonly string _uniqueAssemblyPath;
        private readonly string _launchableAssemblyPath;
        private readonly AssemblyLoadContext _alc;

        public Assembly Assembly { get; }
        public string AssemblyPath => _launchableAssemblyPath;

        public CompiledModuleAssembly(string outputDir, string uniqueAssemblyPath, string launchableAssemblyPath, AssemblyLoadContext alc, Assembly assembly)
        {
            _outputDir = outputDir;
            _uniqueAssemblyPath = uniqueAssemblyPath;
            _launchableAssemblyPath = launchableAssemblyPath;
            _alc = alc;
            Assembly = assembly;
        }

        public void Dispose()
        {
            _alc.Unload();
            try { File.Delete(_uniqueAssemblyPath); } catch { }
            try { Directory.Delete(_outputDir, recursive: true); } catch { }
        }
    }

    private sealed class PackedArgumentsHost
    {
        public double BaseValue { get; init; }

        public string Pack(object[] arguments)
            => $"{arguments.Length}:{arguments[0]}:{arguments[1]}:{arguments[2]}";

        [JsCallableScopeAbi(
            CallableScopeAbiKind.SingleScope,
            SingleScopeType = typeof(PackedArgumentsHost))]
        public object? AddWithScope(
            PackedArgumentsHost scope,
            object? newTarget,
            object? addend)
            => scope.BaseValue + Convert.ToDouble(addend);
    }

    [Fact]
    public void JsEngine_LoadModule_NonGenericSignaturesRemainBinaryCompatible()
    {
        var loadModule = typeof(JsEngine)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(JsEngine.LoadModule)
                && !method.IsGenericMethod
                && method.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .SequenceEqual(
                        new[] { typeof(Assembly), typeof(string) }));
        var loadModuleWithOptions = typeof(JsEngine)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(JsEngine.LoadModule)
                && !method.IsGenericMethod
                && method.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .SequenceEqual(
                        new[]
                        {
                            typeof(Assembly),
                            typeof(string),
                            typeof(JsModuleLoadOptions)
                        }));
        var loadDynamicModule = typeof(JsEngine).GetMethod(
            nameof(JsEngine.LoadDynamicModule),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(Assembly), typeof(string) },
            modifiers: null);

        Assert.NotNull(loadDynamicModule);
        Assert.Equal(typeof(IDisposable), loadModule.ReturnType);
        Assert.Equal(typeof(IDisposable), loadModuleWithOptions.ReturnType);
        Assert.Equal(typeof(JsDynamicExports), loadDynamicModule.ReturnType);
    }

    [Fact]
    public void JsEngine_LoadModule_AllowsCallingExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IMathExports>(module.Assembly, "math");

        Assert.Equal("1.0.0", exports.Version);
        Assert.Equal(3.0, exports.Add(1, 2));
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsCallingExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "math");
        dynamic exports = exportsObj;

        Assert.Equal("1.0.0", (string)exports.version);
        Assert.Equal(3.0, (double)exports.add(1, 2));
    }

    [Fact]
    public void JsEngine_LoadModule_HostedExportResolution_UsesJsObjectExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");

        using var exportsProxy = Assert.IsType<Jroc.Runtime.JsDynamicExports>(
            Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "math"));
        var exports = Assert.IsType<JsObject>(exportsProxy.UnwrapExports());

        Assert.Equal("1.0.0", ExportMemberResolver.GetExportMember(exports, "version"));
        Assert.IsAssignableFrom<JsFunctionObject>(ExportMemberResolver.GetExportMember(exports, "add"));
    }

    [Fact]
    public void JsEngine_RepeatedModuleLoadsKeepCompiledFunctionObjectsIsolated()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var first = JsEngine.LoadDynamicModule(module.Assembly, "math");
        using var second = JsEngine.LoadDynamicModule(module.Assembly, "math");
        var firstExports = Assert.IsType<JsObject>(first.UnwrapExports());
        var secondExports = Assert.IsType<JsObject>(second.UnwrapExports());
        var firstAdd = Assert.IsAssignableFrom<JsFunctionObject>(
            ExportMemberResolver.GetExportMember(firstExports, "add"));
        var secondAdd = Assert.IsAssignableFrom<JsFunctionObject>(
            ExportMemberResolver.GetExportMember(secondExports, "add"));

        Assert.NotSame(firstAdd, secondAdd);
        Assert.Equal(3d, CallableOperations.Call2(firstAdd, null, 1d, 2d));
        Assert.Equal(7d, CallableOperations.Call2(secondAdd, null, 3d, 4d));
    }

    [Fact]
    public void JsEngine_LoadModule_Typed_AllowsCallingEsmExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("mathEsm", "mathEsm.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IMathExports>(module.Assembly, "mathEsm");

        Assert.Equal("2.0.0", exports.Version);
        Assert.Equal(3.0, exports.Add(1, 2));
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsCallingEsmExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("mathEsm", "mathEsm.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "mathEsm");
        dynamic exports = exportsObj;

        Assert.Equal("2.0.0", (string)exports.version);
        Assert.Equal(3.0, (double)exports.add(1, 2));
    }

    [Fact]
    public void JsEngine_RepeatedEsmModuleLoadsKeepLiveBindingsIsolated()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "esmRealmIsolation",
            "esmRealmIsolation.js");
        using var first = JsEngine.LoadDynamicModule(module.Assembly, "esmRealmIsolation");
        using var second = JsEngine.LoadDynamicModule(module.Assembly, "esmRealmIsolation");
        dynamic firstExports = first;
        dynamic secondExports = second;

        Assert.Equal(0d, (double)firstExports.read());
        Assert.Equal(0d, (double)secondExports.read());

        Assert.Equal(1d, (double)firstExports.increment());
        Assert.Equal(1d, (double)firstExports.read());
        Assert.Equal(0d, (double)secondExports.read());

        Assert.Equal(1d, (double)secondExports.increment());
        Assert.Equal(1d, (double)firstExports.read());
        Assert.Equal(1d, (double)secondExports.read());
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsMutatingExportsObject()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("hostingMutable", "Hosting_TypedExports.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "hostingMutable");
        dynamic exports = exportsObj;

        Assert.Equal(0.0, (double)exports.mutableValue);

        exports.mutableValue = 12;

        Assert.Equal(12.0, (double)exports.mutableValue);
        Assert.Equal(12.0, (double)exports.readMutableValue());

        exports.hostValue = 27;

        Assert.Equal(27.0, (double)exports.hostValue);
        Assert.Equal(27.0, (double)exports.readExport("hostValue"));
    }

    [Fact]
    public void JsEngine_LoadModule_WhenHostedForkNotConfigured_ThrowsExplicitConfigurationError()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("hostingForkUnsupported", "Hosting_ForkUnsupported.js");
        AssertHostedForkConfigurationError(module);
    }

    [Fact]
    public void JsEngine_LoadModule_WhenHostedForkAssemblyLoadedFromPathWithoutExplicitConfig_ThrowsExplicitConfigurationError()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "hostingForkUnsupported",
            "Hosting_ForkUnsupported.js",
            loadAssemblyFromPath: true);
        AssertHostedForkConfigurationError(module);
    }

    public interface IHostedForkExports : IDisposable
    {
        Task<string> StartFork();
    }

    [Fact]
    public async Task JsEngine_LoadModule_WhenHostedForkConfigured_AllowsChildProcessFork()
    {
        using var module = CompileAndLoadModuleAssemblyFromResources(
            rootModuleName: "hostingForkSupported",
            rootScriptResourcePath: "Hosting_ForkSupported.js",
            additionalFiles: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Hosting_ForkSupported_Child.js"] = "Hosting_ForkSupported_Child.js"
            });

        var launcher = new RecordingChildProcessLauncher();
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IHostedForkExports>(
            module.Assembly,
            "hostingForkSupported",
            new JsModuleLoadOptions
            {
                CompiledAssemblyPath = module.AssemblyPath,
                ChildProcessLauncher = launcher
            });

        var result = await exports.StartFork();

        Assert.NotNull(launcher.LastRequest);
        Assert.Equal(module.AssemblyPath, launcher.LastRequest!.CompiledAssemblyPath);
        Assert.Equal("./Hosting_ForkSupported_Child", launcher.LastRequest.EntryModule);
        Assert.True(launcher.LastRequest.HostedParent);
        Assert.Equal(new[] { "from-host" }, launcher.LastRequest.ModuleArguments);

        Assert.Contains("ready:from-host:env-ok", result, StringComparison.Ordinal);
        Assert.Contains("reply:42", result, StringComparison.Ordinal);
        Assert.Contains("disconnect:true", result, StringComparison.Ordinal);
        Assert.Contains("close:0:", result, StringComparison.Ordinal);
    }

    public interface IImmutableExports : IDisposable
    {
        double LockedValue { get; set; }
        double ReadLockedValue();
    }

    [Fact]
    public void JsEngine_LoadModule_Typed_WhenHostMutatesImmutableExport_ThrowsJsInvocationException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("immutableExports", "immutableExports.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IImmutableExports>(module.Assembly, "immutableExports");

        var ex = Assert.Throws<JsInvocationException>(() => exports.LockedValue = 2);
        Assert.Equal("immutableExports", ex.ModuleId);
        Assert.Equal("LockedValue", ex.MemberName);

        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        Assert.Equal("TypeError", jsError.JsName);
        Assert.Contains("read only property", jsError.JsMessage ?? jsError.Message, StringComparison.OrdinalIgnoreCase);

        Assert.Equal(1.0, exports.LockedValue);
        Assert.Equal(1.0, exports.ReadLockedValue());
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_WhenHostMutatesImmutableExport_ThrowsJsInvocationException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("immutableExports", "immutableExports.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "immutableExports");
        dynamic exports = exportsObj;

        var ex = Assert.Throws<JsInvocationException>(() =>
        {
            exports.lockedValue = 2;
        });

        Assert.Equal("immutableExports", ex.ModuleId);
        Assert.Equal("lockedValue", ex.MemberName);

        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        Assert.Equal("TypeError", jsError.JsName);
        Assert.Contains("read only property", jsError.JsMessage ?? jsError.Message, StringComparison.OrdinalIgnoreCase);

        Assert.Equal(1.0, (double)exports.lockedValue);
        Assert.Equal(1.0, (double)exports.readLockedValue());
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsNestedMemberAccess_OnReturnedObjects()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("nestedReturn", "nestedReturn.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "nestedReturn");
        dynamic exports = exportsObj;

        dynamic win = exports.getWindow();
        Assert.Equal("Hello", (string)win.document.title);
        Assert.Equal("Hello", (string)win.title);
        Assert.Equal("Hello", (string)exports.getTitle(win));
        Assert.Equal("Hello", (string)exports.getTitleViaHost());

        win.hostValue = 17;
        Assert.Equal(17.0, (double)exports.getHostValue(win));

        Assert.Equal("Updated", (string)win.setTitle("Updated"));
        Assert.Equal("Updated", (string)win.document.title);
        Assert.Equal("Updated", (string)win.title);
    }

    [Fact]
    public void JsEngine_LoadModule_Typed_ObjectReturn_UsesDynamicBoundaryProxy()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("nestedReturn", "nestedReturn.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IObjectReturnExports>(module.Assembly, "nestedReturn");

        var returnedValue = exports.GetWindow();
        Assert.IsNotType<JsObject>(returnedValue);

        dynamic win = returnedValue;
        Assert.Equal("Hello", (string)win.title);

        win.hostValue = 23;
        Assert.Equal(23.0, exports.GetHostValue(win));
        Assert.Equal("Hello", exports.GetTitle(win));
    }

    [Fact]
    public async Task JsEngine_LoadModule_Dynamic_ReturnedObject_MarshalsAndTranslatesCalls()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("nestedReturn", "nestedReturn.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "nestedReturn");
        dynamic exports = exportsObj;
        dynamic win = exports.getWindow();

        var title = await Task.Run(() => (string)win.title);
        Assert.Equal("Hello", title);

        var ex = Assert.Throws<JsInvocationException>(() => win.fail());
        Assert.Equal("nestedReturn", ex.ModuleId);
        Assert.Equal("fail", ex.MemberName);

        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        Assert.Equal("Error", jsError.JsName);
        Assert.Contains("nested boom", jsError.JsMessage ?? jsError.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsInvokingReturnedFunctionValues()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("functionReturn", "functionReturn.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "functionReturn");
        dynamic exports = exportsObj;

        dynamic inc = exports.getIncrementer();
        Assert.Equal(2.0, (double)inc(1));
    }

    [Fact]
    public void JsEngine_ProjectsGeneratedFunctions_WithStableIdentityAndProperties()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "generatedFunctionInterop",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "generatedFunctionInterop");

        var ordinary = exports.Ordinary;

        Assert.Same(ordinary, exports.Ordinary);
        Assert.Same(ordinary, exports.Echo(ordinary));

        dynamic nested = exports.Nested;
        Assert.Same(ordinary, (object)nested.ordinary);

        Assert.Equal("ordinary", ordinary.Name);
        Assert.Equal(2.0, ordinary.Length);
        Assert.Equal("ordinary-property", ordinary.GetProperty("extra"));

        ordinary.SetProperty("hostProperty", 42);
        Assert.Equal(42.0, ordinary.GetProperty("hostProperty"));
        Assert.Equal(3.0, ordinary.Call(1, 2));
        Assert.Equal(28.0, exports.SumSeven.Call(1, 2, 3, 4, 5, 6, 7));
    }

    [Fact]
    public void JsCallable_PreservesReceiverErrorsAsyncGeneratorsAndConstruction()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "generatedFunctionBehavior",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "generatedFunctionBehavior");

        dynamic receiver = new Dictionary<string, object?>
        {
            ["value"] = "middle"
        };
        Assert.Equal(
            "<middle>",
            exports.Describe.CallWithReceiver(receiver, "<", ">"));

        var error = Assert.Throws<JsInvocationException>(
            () => exports.Thrower.Call());
        var jsError = Assert.IsType<JsErrorException>(error.InnerException);
        Assert.Contains(
            "hosted callable boom",
            jsError.JsMessage ?? jsError.Message,
            StringComparison.Ordinal);

        dynamic iterator = exports.Sequence.Call(5)!;
        dynamic first = iterator.next();
        dynamic second = iterator.next();
        dynamic completed = iterator.next();
        Assert.Equal(5.0, (double)first.value);
        Assert.False((bool)first.done);
        Assert.Equal(6.0, (double)second.value);
        Assert.Equal(7.0, (double)completed.value);
        Assert.True((bool)completed.done);

        Assert.True(exports.Person.IsConstructor);
        dynamic person = exports.Person.Construct("Ada")!;
        Assert.Equal("Ada", (string)person.name);
        dynamic newTargetProbe = exports.NewTargetProbe.ConstructWithNewTarget(
            exports.Ordinary)!;
        Assert.Equal("ordinary", (string)newTargetProbe.targetName);

        var arrowNewTargetError = Assert.Throws<JsInvocationException>(
            () => exports.Person.ConstructWithNewTarget(
                exports.Arrow,
                "Ada"));
        Assert.Equal(
            "TypeError",
            Assert.IsType<JsErrorException>(
                arrowNewTargetError.InnerException).JsName);

        var primitiveNewTargetError = Assert.Throws<JsInvocationException>(
            () => exports.Person.ConstructWithNewTarget(42, "Ada"));
        Assert.Equal(
            "TypeError",
            Assert.IsType<JsErrorException>(
                primitiveNewTargetError.InnerException).JsName);

        Assert.False(exports.Arrow.IsConstructor);
        _ = Assert.Throws<JsInvocationException>(
            () => exports.Arrow.Construct());
    }

    [Fact]
    public async Task JsCallable_BridgesPromiseResultsToTask()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "generatedFunctionAsync",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "generatedFunctionAsync");

        Assert.Equal(42.0, await exports.DoubleAsync.CallAsync<double>(21));

        var error = await Assert.ThrowsAsync<JavaScriptRuntime.Error>(
            () => exports.RejectAsync.CallAsync<object?>());
        Assert.Contains(
            "async hosted boom",
            error.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void JsEngine_AdaptsHostDelegates_WithStableRoundTripIdentity()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "hostDelegateInterop",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "hostDelegateInterop");

        Func<object, object, object?> add = (left, right) =>
            Convert.ToDouble(left) + Convert.ToDouble(right);

        Assert.True(exports.Same(add, add));
        Assert.Equal(7.0, exports.InvokeCallback(add, 3, 4));

        var returned = Assert.IsType<JsCallable>(exports.Echo(add));
        Assert.Same(returned, exports.Echo(add));
        Assert.True(exports.Same(returned, add));
        Assert.Equal(11.0, returned.Call(5, 6));
        Assert.False(returned.IsConstructor);

        exports.Dispose();
        _ = Assert.Throws<ObjectDisposedException>(
            () => returned.Call(1, 2));
    }

    [Fact]
    public void JsEngine_HostObjectArrayParametersReceivePackedJavaScriptArguments()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "hostPackedArguments",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "hostPackedArguments");

        Func<object[], object?> packedDelegate = arguments =>
            $"{arguments.Length}:{arguments[0]}:{arguments[1]}:{arguments[2]}";
        Assert.Equal(
            "3:first:2:True",
            exports.InvokeCallbackVariadic(packedDelegate));

        var hostFunction = new JsHostFunction(
            (_, arguments) =>
                $"{arguments.Length}:{arguments[0]}:{arguments[1]}:{arguments[2]}");
        Assert.Equal(
            "3:first:2:True",
            exports.InvokeCallbackVariadic(hostFunction));

        using var runtime = new JsRuntimeInstance(
            module.Assembly,
            "hostPackedArguments");
        var host = new PackedArgumentsHost { BaseValue = 5 };
        var method = typeof(PackedArgumentsHost).GetMethod(
            nameof(PackedArgumentsHost.Pack))
            ?? throw new InvalidOperationException("Expected Pack method.");
        var adapter = runtime.GetOrCreateHostMethodAdapter(host, method);
        var methodResult = runtime.Invoke(
            () => CallableOperations.Call(
                adapter,
                null,
                new object?[] { "first", 2d, true }));

        Assert.Equal("3:first:2:True", methodResult);

        HostedSingleScopeDelegate generatedDelegate = host.AddWithScope;
        var generatedDelegateAdapter = Assert.IsAssignableFrom<JsFunctionObject>(
            runtime.NormalizeHostValue(generatedDelegate));
        var generatedDelegateResult = runtime.Invoke(
            () => CallableOperations.Call(
                generatedDelegateAdapter,
                null,
                new object?[] { 2d }));
        Assert.Equal(7d, generatedDelegateResult);

        var generatedMethod = typeof(PackedArgumentsHost).GetMethod(
            nameof(PackedArgumentsHost.AddWithScope))
            ?? throw new InvalidOperationException("Expected AddWithScope method.");
        var generatedMethodAdapter = runtime.GetOrCreateHostMethodAdapter(
            host,
            generatedMethod);
        var generatedMethodResult = runtime.Invoke(
            () => CallableOperations.Call(
                generatedMethodAdapter,
                null,
                new object?[] { 3d }));
        Assert.Equal(8d, generatedMethodResult);
    }

    [Fact]
    public async Task JsEngine_HostTasksBecomeJavaScriptPromises()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "hostTaskInterop",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "hostTaskInterop");

        var delegateSource = new TaskCompletionSource<object?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        Func<object[], Task<object?>> delegateSuccess = arguments =>
        {
            Assert.Equal(3d, Convert.ToDouble(arguments[0]));
            Assert.Equal(4d, Convert.ToDouble(arguments[1]));
            return delegateSource.Task;
        };
        var delegatePending = exports.AwaitCallback(
            delegateSuccess,
            3,
            4);
        delegateSource.SetResult(7d);
        Assert.Equal(
            7d,
            await delegatePending
                .WaitAsync(TimeSpan.FromSeconds(2)));

        var hostFunctionSuccess = new JsHostFunction(
            (_, arguments) => Task.FromResult<object?>(
                $"{arguments[0]}:{arguments[1]}"));
        Assert.Equal(
            "left:right",
            await exports.AwaitCallback(
                    hostFunctionSuccess,
                    "left",
                    "right")
                .WaitAsync(TimeSpan.FromSeconds(2)));

        Func<object[], Task<object?>> delegateFailure = _ =>
            Task.FromException<object?>(
                new InvalidOperationException("host task boom"));
        var failure = await Assert.ThrowsAsync<InvalidOperationException>(
            () => exports.AwaitCallback(
                    delegateFailure,
                    null,
                    null)
                .WaitAsync(TimeSpan.FromSeconds(2)));
        Assert.Equal("host task boom", failure.Message);

        var hostFunctionCancellation = new JsHostFunction(
            (_, _) => Task.FromCanceled<object?>(
                new CancellationToken(canceled: true)));
        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => exports.AwaitCallback(
                    hostFunctionCancellation,
                    null,
                    null)
                .WaitAsync(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public async Task JsEngine_DisposeFaultsPendingHostTaskPromiseBridge()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "hostTaskDisposal",
            "Hosting_GeneratedFunctionInterop.js");
        var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "hostTaskDisposal");
        var source = new TaskCompletionSource<object?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var hostFunction = new JsHostFunction(
            (_, _) => source.Task);

        var pending = exports.AwaitCallback(
            hostFunction,
            null,
            null);
        exports.Dispose();

        _ = await Assert.ThrowsAsync<ObjectDisposedException>(
            () => pending.WaitAsync(TimeSpan.FromSeconds(2)));
        source.TrySetResult("late");
    }

    [Fact]
    public void JsEngine_AdaptsExplicitHostFunctions_WithMetadataReceiverAndConstruction()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "explicitHostFunctionInterop",
            "Hosting_GeneratedFunctionInterop.js");
        using var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "explicitHostFunctionInterop");

        var receiverAware = new JsHostFunction(
            (receiver, arguments) =>
            {
                dynamic projectedReceiver = receiver
                    ?? throw new InvalidOperationException("Expected a receiver.");
                return $"{arguments[0]}{projectedReceiver.value}{arguments[1]}";
            },
            name: "receiverAware",
            length: 2);

        dynamic info = exports.InspectCallback(receiverAware);
        Assert.Equal("receiverAware", (string)info.name);
        Assert.Equal(2.0, (double)info.length);
        Assert.True((bool)info.functionPrototype);
        Assert.Equal("set-by-js", (string)info.customProperty);
        Assert.False((bool)info.constructable);
        var projected = Assert.IsType<JsCallable>(
            exports.Echo(receiverAware));
        Assert.Equal("receiverAware", projected.Name);
        Assert.Equal(2.0, projected.Length);
        Assert.Equal("set-by-js", projected.GetProperty("customProperty"));
        Assert.Equal(
            "<middle>",
            exports.InvokeCallbackWithReceiver(
                receiverAware,
                "middle",
                "<",
                ">"));

        var constructor = new JsHostFunction(
            (_, _) => null,
            name: "HostBox",
            length: 1,
            constructor: (arguments, _) =>
                new Dictionary<string, object?>
                {
                    ["value"] = arguments.Length > 0 ? arguments[0] : null
                });

        dynamic constructorInfo = exports.InspectCallback(constructor);
        Assert.True((bool)constructorInfo.constructable);
        var instance = exports.ConstructCallback(constructor, 9);
        Assert.Equal(9.0, exports.ReadValue(instance));
    }

    [Fact]
    public void FixedArityHostedDelegateCallsDoNotAllocateArgumentArrays()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "fixedArityHostAllocation",
            "Hosting_GeneratedFunctionInterop.js");
        using var runtime = new JsRuntimeInstance(
            module.Assembly,
            "fixedArityHostAllocation");
        Func<object, object?> callback = static value => value;
        var adapter = Assert.IsAssignableFrom<JsFunctionObject>(
            runtime.NormalizeHostValue(callback));
        const string argument = "argument";

        var allocated = runtime.Invoke(() =>
        {
            // Cross both tiered-compilation thresholds before measuring. Otherwise
            // Tier 1 JIT bookkeeping can be charged to the runtime thread.
            for (var index = 0; index < 100_000; index++)
            {
                _ = CallableOperations.Call1(adapter, null, argument);
            }

            var before = GC.GetAllocatedBytesForCurrentThread();
            object? result = null;
            for (var index = 0; index < 10_000; index++)
            {
                result = CallableOperations.Call1(
                    adapter,
                    null,
                    argument);
            }
            var measured = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Same(argument, result);
            return measured;
        });

        Assert.Equal(0, allocated);
    }

    [Fact]
    public void JsCallable_ThrowsAfterOwningRuntimeIsDisposed()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "disposedCallableInterop",
            "Hosting_GeneratedFunctionInterop.js");
        var exports = JsEngine.LoadModule<IGeneratedCallableExports>(
            module.Assembly,
            "disposedCallableInterop");
        var callable = exports.Ordinary;

        exports.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(
            () => callable.Call(1, 2));
        _ = Assert.Throws<ObjectDisposedException>(
            () => _ = callable.Name);
    }

    [Fact]
    public void JsEngine_DynamicRootFunctionExport_UsesPublicCallableProjection()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "rootFunctionExport",
            "Hosting_RootFunctionExport.js");
        using var exports = JsEngine.LoadDynamicModule(
            module.Assembly,
            "rootFunctionExport");

        var callable = Assert.IsType<JsCallable>(exports.Value);
        Assert.Same(callable, exports.Value);
        Assert.Equal("rootIncrement", callable.Name);
        Assert.Equal("root-function", callable.GetProperty("kind"));
        Assert.Equal(3.0, callable.Call(2));

        dynamic dynamicExports = exports;
        Assert.Equal(4.0, (double)dynamicExports(3));
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_NewOnValue_PadsMissingArgsWithUndefined()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("ctorPadding", "ctorPadding.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "ctorPadding");
        dynamic exports = exportsObj;

        Assert.True((bool)exports.undefinedWhenMissingArgs());
    }

    [Fact]
    public async Task JsEngine_LoadModule_AllowsCallingExports_FromAnotherThread()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IMathExports>(module.Assembly, "math");

        // Validate cross-thread marshalling: calls from any host thread should execute on the script thread.
        var result = await Task.Run(() => exports.Add(1, 2));
        Assert.Equal(3.0, result);
    }

    [Fact]
    public async Task JsEngine_LoadModule_Dynamic_AllowsMutatingExports_FromAnotherThread()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("hostingMutable", "Hosting_TypedExports.js");

        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "hostingMutable");
        dynamic exports = exportsObj;

        var result = await Task.Run(() =>
        {
            exports.mutableValue = 19;
            return (double)exports.readMutableValue();
        });

        Assert.Equal(19.0, result);
        Assert.Equal(19.0, (double)exports.mutableValue);
    }

    [Fact]
    public void JsEngine_LoadModule_WhenDisposed_ThrowsObjectDisposedExceptionOnFurtherCalls()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IMathExports>(module.Assembly, "math");

        exports.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => exports.Add(1, 2));
        _ = Assert.Throws<ObjectDisposedException>(() => _ = exports.Version);
    }

    [Fact]
    public void JsEngine_LoadModule_WhenDisposed_ShutsDownScriptThread()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");

        var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "math");
        var exports = Assert.IsType<Jroc.Runtime.JsDynamicExports>(exportsObj);

        exports.Dispose();

        Assert.True(exports.WaitForShutdown(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public async Task JsRuntimeInstance_DisposeFaultsQueuedInvokeWithoutBlocking()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            "concurrentDispose",
            "math.js");
        using var runtime = new JsRuntimeInstance(
            module.Assembly,
            "concurrentDispose");
        using var started = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();

        var running = Task.Run(() => runtime.Invoke(() =>
        {
            started.Set();
            if (!release.Wait(TimeSpan.FromSeconds(2)))
            {
                throw new TimeoutException("The test did not release the running invocation.");
            }
        }));

        Assert.True(started.Wait(TimeSpan.FromSeconds(2)));
        var queued = Task.Run(() => runtime.Invoke(() => 42));
        Assert.True(SpinWait.SpinUntil(
            () => runtime.PendingWorkItemCount >= 2,
            TimeSpan.FromSeconds(2)));

        var dispose = Task.Run(runtime.Dispose);
        try
        {
            _ = await Assert.ThrowsAsync<ObjectDisposedException>(
                () => queued.WaitAsync(TimeSpan.FromSeconds(2)));
        }
        finally
        {
            release.Set();
        }

        await running.WaitAsync(TimeSpan.FromSeconds(2));
        await dispose.WaitAsync(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void JsEngine_GetModuleIds_ReturnsExpectedModuleIds()
    {
        using var module = CompileAndLoadModuleAssemblyFromResources(
            rootModuleName: "main",
            rootScriptResourcePath: "main.js",
            additionalFiles: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["calculator/index.js"] = "calculator/index.js"
            });

        var moduleIds = Jroc.Runtime.JsEngine.GetModuleIds(module.Assembly);

        Assert.Equal(new[] { "calculator/index", "main" }, moduleIds);
    }

    [Fact]
    public void JsEngine_LoadModule_WhenModuleThrowsDuringInitialization_PropagatesException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("boom", "boom.js");

        var ex = Assert.Throws<Jroc.Runtime.JsModuleLoadException>(
            () => Jroc.Runtime.JsEngine.LoadDynamicModule(module.Assembly, "boom"));
        Assert.Equal("boom", ex.ModuleId);

        var jsError = Assert.IsType<Jroc.Runtime.JsErrorException>(ex.InnerException);
        Assert.Equal("Error", jsError.JsName);
        Assert.Contains("boom", jsError.JsMessage ?? jsError.Message, StringComparison.OrdinalIgnoreCase);
    }

    public interface IMissingMemberExports : IDisposable
    {
        string DoesNotExist { get; }
    }

    [Fact]
    public void JsEngine_LoadModule_WhenExportMemberMissing_ThrowsJsContractProjectionException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IMissingMemberExports>(module.Assembly, "math");

        var ex = Assert.Throws<Jroc.Runtime.JsContractProjectionException>(() => _ = exports.DoesNotExist);
        Assert.Equal("math", ex.ModuleId);
        Assert.Equal("DoesNotExist", ex.MemberName);
    }

    public interface IWrongShapeExports : IDisposable
    {
        double Version();
    }

    [Fact]
    public void JsEngine_LoadModule_WhenExportExpectedFunctionButWasNot_ThrowsJsContractProjectionException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IWrongShapeExports>(module.Assembly, "math");

        var ex = Assert.Throws<Jroc.Runtime.JsContractProjectionException>(() => _ = exports.Version());
        Assert.Equal("math", ex.ModuleId);
        Assert.Equal("Version", ex.MemberName);
    }

    public interface IThrowingExports : IDisposable
    {
        void Boom();
    }

    [Fact]
    public void JsEngine_LoadModule_WhenInvocationThrowsJsError_ThrowsJsInvocationExceptionWithInnerJsError()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("throws", "throws.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IThrowingExports>(module.Assembly, "throws");

        var ex = Assert.Throws<Jroc.Runtime.JsInvocationException>(() => exports.Boom());
        Assert.Equal("throws", ex.ModuleId);
        Assert.Equal("Boom", ex.MemberName);

        var jsError = Assert.IsType<Jroc.Runtime.JsErrorException>(ex.InnerException);
        Assert.Equal("Error", jsError.JsName);
        Assert.Contains("boom", jsError.JsMessage ?? jsError.Message, StringComparison.OrdinalIgnoreCase);
    }

    public interface IThrowValueExports : IDisposable
    {
        void ThrowValue();
    }

    [Fact]
    public void JsEngine_LoadModule_WhenInvocationThrowsNonErrorValue_ThrowsJsInvocationExceptionWithThrownValue()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("throwValue", "throwValue.js");
        using var exports = Jroc.Runtime.JsEngine.LoadModule<IThrowValueExports>(module.Assembly, "throwValue");

        var ex = Assert.Throws<Jroc.Runtime.JsInvocationException>(() => exports.ThrowValue());
        var jsError = Assert.IsType<Jroc.Runtime.JsErrorException>(ex.InnerException);

        Assert.NotNull(jsError.ThrownValue);
        Assert.Equal(123.0, Convert.ToDouble(jsError.ThrownValue));
    }

    private static string LoadHostingJavaScript(string resourcePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourcePath);

        var normalized = resourcePath.Trim().Replace('\\', '/');
        var resourceName = HostingJavaScriptResourcePrefix + normalized.Replace("/", ".");

        var assembly = typeof(ModuleLoadTests).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var candidates = assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith(HostingJavaScriptResourcePrefix, StringComparison.Ordinal))
                .Order(StringComparer.Ordinal)
                .ToArray();

            throw new InvalidOperationException(
                $"Could not find embedded resource '{resourceName}' for script '{resourcePath}'. " +
                $"Available Hosting JavaScript resources: {string.Join(", ", candidates)}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static void AssertHostedForkConfigurationError(CompiledModuleAssembly module)
    {
        using var exportsObj = Jroc.Runtime.JsEngine.LoadDynamicModule(
            module.Assembly,
            "hostingForkUnsupported");
        dynamic exports = exportsObj;

        var ex = Assert.Throws<JsInvocationException>(() => exports.attemptFork());
        Assert.Equal("hostingForkUnsupported", ex.ModuleId);
        Assert.Equal("attemptFork", ex.MemberName);

        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        Assert.Equal("Error", jsError.JsName);
        Assert.Contains(
            "child_process.fork requires a compiled assembly path when running under JsEngine hosting",
            jsError.JsMessage ?? jsError.Message,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "Pass JsModuleLoadOptions.CompiledAssemblyPath",
            jsError.JsMessage ?? jsError.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    private static CompiledModuleAssembly CompileAndLoadModuleAssemblyFromResource(
        string moduleName,
        string scriptResourcePath,
        bool loadAssemblyFromPath = false)
    {
        return CompileAndLoadModuleAssemblyFromResources(
            rootModuleName: moduleName,
            rootScriptResourcePath: scriptResourcePath,
            additionalFiles: new Dictionary<string, string>(StringComparer.Ordinal),
            loadAssemblyFromPath: loadAssemblyFromPath);
    }

    private static CompiledModuleAssembly CompileAndLoadModuleAssemblyFromResources(
        string rootModuleName,
        string rootScriptResourcePath,
        IReadOnlyDictionary<string, string> additionalFiles,
        bool loadAssemblyFromPath = false)
    {
        var outputDir = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "ModuleLoad", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var rootJs = LoadHostingJavaScript(rootScriptResourcePath);

        var filePath = Path.Combine(outputDir, rootModuleName + ".js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(filePath, rootJs);

        foreach (var kvp in additionalFiles)
        {
            var fullPath = Path.Combine(outputDir, kvp.Key.Replace('/', Path.DirectorySeparatorChar));
            var content = LoadHostingJavaScript(kvp.Value);
            mockFs.AddFile(fullPath, content);
        }

        var options = new CompilerOptions { OutputDirectory = outputDir };
        var logger = new TestLogger();
        var sp = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = sp.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(filePath), logger.Errors);

        var compiledPath = Path.Combine(outputDir, rootModuleName + ".dll");
        Assert.True(File.Exists(compiledPath), $"Expected compiled output at '{compiledPath}'");

        var jsRuntimeAsm = typeof(EnvironmentProvider).Assembly;
        var uniquePath = Path.Combine(outputDir, rootModuleName + ".run-" + Guid.NewGuid().ToString("N") + ".dll");
        File.Copy(compiledPath, uniquePath, overwrite: true);

        var alc = new HostingTestAssemblyLoadContext(jsRuntimeAsm, outputDir);
        Assembly compiledAssembly;
        string launchableAssemblyPath;
        if (loadAssemblyFromPath)
        {
            compiledAssembly = alc.LoadFromAssemblyPath(uniquePath);
            launchableAssemblyPath = uniquePath;
        }
        else
        {
            using var stream = File.OpenRead(uniquePath);
            compiledAssembly = alc.LoadFromStream(stream);
            launchableAssemblyPath = compiledPath;
        }

        return new CompiledModuleAssembly(outputDir, uniquePath, launchableAssemblyPath, alc, compiledAssembly);
    }

    private sealed class HostingTestAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly Assembly _jsRuntimeAssembly;
        private readonly string _baseDirectory;

        public HostingTestAssemblyLoadContext(Assembly jsRuntimeAssembly, string baseDirectory)
            : base(isCollectible: true)
        {
            _jsRuntimeAssembly = jsRuntimeAssembly;
            _baseDirectory = baseDirectory;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (string.Equals(assemblyName.Name, _jsRuntimeAssembly.GetName().Name, StringComparison.Ordinal))
            {
                return _jsRuntimeAssembly;
            }

            var candidatePath = Path.Combine(_baseDirectory, (assemblyName.Name ?? string.Empty) + ".dll");
            if (File.Exists(candidatePath))
            {
                return LoadFromAssemblyPath(candidatePath);
            }

            return null;
        }
    }

    private sealed class RecordingChildProcessLauncher : IChildProcessLauncher
    {
        public ChildProcessLaunchRequest? LastRequest { get; private set; }

        public System.Diagnostics.Process Start(ChildProcessLaunchRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.StartInfo);
            LastRequest = request;
            return System.Diagnostics.Process.Start(request.StartInfo)
                ?? throw new InvalidOperationException("Failed to start hosted child process.");
        }
    }
}
