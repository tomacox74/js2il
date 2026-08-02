using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class FsPromisesModuleContractTests
{
    [Fact]
    public void IFsPromisesModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IFsPromisesModule);

        Assert.Equal(
            "fs/promises",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        Assert.NotNull(contractType.GetCustomAttribute<GeneratedCodeAttribute>());
        Assert.Equal(
            "constants",
            contractType.GetProperty("constants")
                ?.GetCustomAttribute<NodeModuleMemberAttribute>()
                ?.MemberName);
    }

    [Fact]
    public void IFsPromisesModule_ProvidesRepresentativePromiseAndIteratorOverloads()
    {
        var accessMethods = GetMethods("access");

        Assert.Equal(2, accessMethods.Length);
        Assert.All(
            accessMethods,
            method => Assert.Equal(typeof(IJavaScriptPromise), method.ReturnType));
        Assert.Contains(accessMethods, method => method.GetParameters().Length == 1);
        Assert.Contains(accessMethods, method => method.GetParameters().Length == 2);

        var globMethods = GetMethods("glob");

        Assert.Equal(2, globMethods.Length);
        Assert.All(
            globMethods,
            method => Assert.Equal(typeof(IJavaScriptAsyncIterator), method.ReturnType));

        Assert.DoesNotContain(
            typeof(IFsPromisesModule).GetMethods(),
            method => method.ReturnType == typeof(Promise)
                || method.GetParameters().Any(parameter => parameter.ParameterType == typeof(Promise)));
        Assert.True(typeof(IJavaScriptPromise).IsAssignableFrom(typeof(Promise)));
    }

    [Fact]
    public void RuntimePromise_ImplementsPromiseContract()
    {
        Func<object[], object?, object?, object?> executor = (_, _, _) => null;
        IJavaScriptPromise promise = new Promise(executor);

        Assert.IsAssignableFrom<IJavaScriptPromise>(promise.then());
        Assert.IsAssignableFrom<IJavaScriptPromise>(promise.@catch(null));
        Assert.IsAssignableFrom<IJavaScriptPromise>(promise.@finally(null));
    }

    [Fact]
    public void IFsPromisesModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IFsPromisesModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            32,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Single(contractType.GetProperties());
    }

    [Fact]
    public void IntrinsicFsPromisesModule_DelegatesAvailableMembers()
    {
        WithRuntimeServices(module =>
        {
            Assert.IsType<Promise>(module.access(Environment.CurrentDirectory));
            Assert.IsType<Promise>(module.stat(Environment.CurrentDirectory));
        });
    }

    [Fact]
    public void IntrinsicFsPromisesModule_ThrowsForUnavailableMembers()
    {
        WithRuntimeServices(module =>
        {
            var methodException = Assert.Throws<NotImplementedException>(
                () => module.chmod("file.txt", 384d));
            Assert.Equal(
                "The intrinsic node:fs/promises module does not implement 'fsPromises.chmod'.",
                methodException.Message);

            var propertyException = Assert.Throws<TargetInvocationException>(
                () => typeof(IFsPromisesModule).GetProperty("constants")!.GetValue(module));
            Assert.IsType<NotImplementedException>(propertyException.InnerException);
        });
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IFsPromisesModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }

    private static void WithRuntimeServices(Action<IFsPromisesModule> assertion)
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();

        try
        {
            SetRuntimeServiceProvider(serviceProvider);
            assertion(new FSPromises());
        }
        finally
        {
            SetRuntimeServiceProvider(null);
        }
    }

    private static void SetRuntimeServiceProvider(object? serviceProvider)
    {
        typeof(GlobalThis)
            .GetProperty("ServiceProvider", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, serviceProvider);
    }
}
