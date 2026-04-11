using JavaScriptRuntime.DependencyInjection;
using Xunit;

namespace Js2IL.Tests;

public class ServiceContainerTests
{
    [Fact]
    public void RegisterInstance_StoresAndReturnsInstance()
    {
        var container = new ServiceContainer();
        var service = new TestService();

        container.RegisterInstance(service);

        var resolved = container.Resolve<TestService>();
        Assert.Same(service, resolved);
    }

    [Fact]
    public void Resolve_CreatesSingletonWhenNotRegistered()
    {
        var container = new ServiceContainer();

        var first = container.Resolve<TestService>();
        var second = container.Resolve<TestService>();

        Assert.Same(first, second);
    }

    [Fact]
    public void Resolve_InjectsConstructorDependencies()
    {
        var container = new ServiceContainer();

        var service = container.Resolve<ServiceWithDependency>();

        Assert.NotNull(service.Dependency);
    }

    [Fact]
    public void Resolve_InjectsSameSingletonToDifferentConsumers()
    {
        var container = new ServiceContainer();

        var service1 = container.Resolve<ServiceWithDependency>();
        var service2 = container.Resolve<AnotherServiceWithDependency>();

        Assert.Same(service1.Dependency, service2.Dependency);
    }

    [Fact]
    public void Register_InterfaceToImplementation_ResolvesCorrectly()
    {
        var container = new ServiceContainer();
        container.Register<ITestInterface, TestInterfaceImplementation>();

        var resolved = container.Resolve<ITestInterface>();

        Assert.IsType<TestInterfaceImplementation>(resolved);
    }

    [Fact]
    public void Replace_SwapsInstance()
    {
        var container = new ServiceContainer();
        var original = new TestService { Value = "original" };
        var mock = new TestService { Value = "mock" };

        container.RegisterInstance(original);
        container.Replace(mock);

        var resolved = container.Resolve<TestService>();
        Assert.Equal("mock", resolved.Value);
    }

    [Fact]
    public void Replace_AllowsMockingForTests()
    {
        var container = new ServiceContainer();
        container.Register<ITestInterface, TestInterfaceImplementation>();
        
        // First resolve creates the real implementation
        var real = container.Resolve<ITestInterface>();
        Assert.Equal("Real", real.GetName());

        // Replace with mock
        var mock = new MockTestInterface();
        container.Replace<ITestInterface>(mock);

        // Now resolves to mock
        var resolved = container.Resolve<ITestInterface>();
        Assert.Equal("Mock", resolved.GetName());
    }

    [Fact]
    public void TryResolve_ReturnsFalseWhenNotRegistered()
    {
        var container = new ServiceContainer();

        var found = container.TryResolve<TestService>(out var instance);

        Assert.False(found);
        Assert.Null(instance);
    }

    [Fact]
    public void TryResolve_ReturnsTrueWhenRegistered()
    {
        var container = new ServiceContainer();
        var service = new TestService();
        container.RegisterInstance(service);

        var found = container.TryResolve<TestService>(out var instance);

        Assert.True(found);
        Assert.Same(service, instance);
    }

    [Fact]
    public void IsRegistered_ReturnsTrueForRegisteredInstance()
    {
        var container = new ServiceContainer();
        container.RegisterInstance(new TestService());

        Assert.True(container.IsRegistered<TestService>());
    }

    [Fact]
    public void IsRegistered_ReturnsTrueForRegisteredType()
    {
        var container = new ServiceContainer();
        container.Register<ITestInterface, TestInterfaceImplementation>();

        Assert.True(container.IsRegistered<ITestInterface>());
    }

    [Fact]
    public void IsRegistered_ReturnsFalseForUnregistered()
    {
        var container = new ServiceContainer();

        Assert.False(container.IsRegistered<TestService>());
    }

    [Fact]
    public void Remove_RemovesInstance()
    {
        var container = new ServiceContainer();
        var service = new TestService();
        container.RegisterInstance(service);

        var removed = container.Remove<TestService>();

        Assert.True(removed);
        Assert.False(container.TryResolve<TestService>(out _));
    }

    [Fact]
    public void Clear_RemovesAllRegistrations()
    {
        var container = new ServiceContainer();
        container.RegisterInstance(new TestService());
        container.Register<ITestInterface, TestInterfaceImplementation>();

        container.Clear();

        Assert.False(container.IsRegistered<TestService>());
        Assert.False(container.IsRegistered<ITestInterface>());
    }

    [Fact]
    public void CreateScope_CreatesChildContainerWithInheritedRegistrations()
    {
        var container = new ServiceContainer();
        var service = new TestService { Value = "parent" };
        container.RegisterInstance(service);

        var child = container.CreateScope();

        var resolved = child.Resolve<TestService>();
        Assert.Same(service, resolved);
    }

    [Fact]
    public void CreateScope_AllowsOverridingInChild()
    {
        var container = new ServiceContainer();
        var parentService = new TestService { Value = "parent" };
        container.RegisterInstance(parentService);

        var child = container.CreateScope();
        var childService = new TestService { Value = "child" };
        child.Replace(childService);

        // Parent unchanged
        Assert.Equal("parent", container.Resolve<TestService>().Value);
        // Child has override
        Assert.Equal("child", child.Resolve<TestService>().Value);
    }

    [Fact]
    public void Resolve_ThrowsForUnregisteredInterface()
    {
        var container = new ServiceContainer();

        Assert.Throws<InvalidOperationException>(() => 
            container.Resolve<ITestInterface>());
    }

    [Fact]
    public void Resolve_HandlesDeepDependencyChain()
    {
        var container = new ServiceContainer();

        var service = container.Resolve<Level3Service>();

        Assert.NotNull(service.Level2);
        Assert.NotNull(service.Level2.Level1);
        Assert.NotNull(service.Level2.Level1.Dependency);
    }

    [Fact]
    public void RegisterInstance_FluentApi()
    {
        var container = new ServiceContainer()
            .RegisterInstance(new TestService())
            .Register<ITestInterface, TestInterfaceImplementation>();

        Assert.True(container.IsRegistered<TestService>());
        Assert.True(container.IsRegistered<ITestInterface>());
    }

    [Fact]
    public void Resolve_UsesGreediestConstructor()
    {
        var container = new ServiceContainer();
        container.RegisterInstance(new TestService { Value = "injected" });

        var service = container.Resolve<ServiceWithMultipleConstructors>();

        // Should use constructor with TestService parameter
        Assert.Equal("injected", service.Value);
    }

    // Test classes

    public class TestService
    {
        public string Value { get; set; } = "default";
    }

    public class ServiceWithDependency
    {
        public TestService Dependency { get; }

        public ServiceWithDependency(TestService dependency)
        {
            Dependency = dependency;
        }
    }

    public class AnotherServiceWithDependency
    {
        public TestService Dependency { get; }

        public AnotherServiceWithDependency(TestService dependency)
        {
            Dependency = dependency;
        }
    }

    public interface ITestInterface
    {
        string GetName();
    }

    public class TestInterfaceImplementation : ITestInterface
    {
        public string GetName() => "Real";
    }

    public class MockTestInterface : ITestInterface
    {
        public string GetName() => "Mock";
    }

    public class Level1Service
    {
        public TestService Dependency { get; }

        public Level1Service(TestService dependency)
        {
            Dependency = dependency;
        }
    }

    public class Level2Service
    {
        public Level1Service Level1 { get; }

        public Level2Service(Level1Service level1)
        {
            Level1 = level1;
        }
    }

    public class Level3Service
    {
        public Level2Service Level2 { get; }

        public Level3Service(Level2Service level2)
        {
            Level2 = level2;
        }
    }

    public class ServiceWithMultipleConstructors
    {
        public string Value { get; }

        public ServiceWithMultipleConstructors()
        {
            Value = "default";
        }

        public ServiceWithMultipleConstructors(TestService service)
        {
            Value = service.Value;
        }
    }
}
