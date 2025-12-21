using System.Reflection;

namespace JavaScriptRuntime.DependencyInjection;

/// <summary>
/// A simple dependency injection container that manages singleton instances.
/// Supports constructor injection and automatic dependency resolution.
/// </summary>
public class ServiceContainer
{
    private readonly Dictionary<Type, object> _singletons = new();
    private readonly Dictionary<Type, Type> _typeRegistrations = new();
    private readonly object _lock = new();

    /// <summary>
    /// Registers a singleton instance for a specific type.
    /// </summary>
    /// <typeparam name="T">The type to register.</typeparam>
    /// <param name="instance">The singleton instance.</param>
    /// <returns>The container for fluent configuration.</returns>
    public ServiceContainer RegisterInstance<T>(T instance) where T : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        
        lock (_lock)
        {
            _singletons[typeof(T)] = instance;
        }
        return this;
    }

    /// <summary>
    /// Registers a singleton instance for a specific type (non-generic version).
    /// </summary>
    /// <param name="serviceType">The type to register.</param>
    /// <param name="instance">The singleton instance.</param>
    /// <returns>The container for fluent configuration.</returns>
    public ServiceContainer RegisterInstance(Type serviceType, object instance)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(instance);
        
        if (!serviceType.IsAssignableFrom(instance.GetType()))
        {
            throw new ArgumentException($"Instance of type {instance.GetType().Name} is not assignable to {serviceType.Name}");
        }
        
        lock (_lock)
        {
            _singletons[serviceType] = instance;
        }
        return this;
    }

    /// <summary>
    /// Registers a type mapping. When the service type is requested, 
    /// an instance of the implementation type will be created.
    /// </summary>
    /// <typeparam name="TService">The service type (typically an interface).</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The container for fluent configuration.</returns>
    public ServiceContainer Register<TService, TImplementation>() 
        where TService : class 
        where TImplementation : class, TService
    {
        lock (_lock)
        {
            _typeRegistrations[typeof(TService)] = typeof(TImplementation);
        }
        return this;
    }

    /// <summary>
    /// Registers a type to be instantiated as a singleton when first requested.
    /// </summary>
    /// <typeparam name="T">The type to register.</typeparam>
    /// <returns>The container for fluent configuration.</returns>
    public ServiceContainer Register<T>() where T : class
    {
        lock (_lock)
        {
            _typeRegistrations[typeof(T)] = typeof(T);
        }
        return this;
    }

    /// <summary>
    /// Gets or creates a singleton instance of the specified type.
    /// If the instance doesn't exist, it will be created using constructor injection.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <returns>The singleton instance.</returns>
    public T Resolve<T>() where T : class
    {
        return (T)Resolve(typeof(T));
    }

    /// <summary>
    /// Gets or creates a singleton instance of the specified type.
    /// If the instance doesn't exist, it will be created using constructor injection.
    /// </summary>
    /// <param name="serviceType">The type to resolve.</param>
    /// <returns>The singleton instance.</returns>
    public object Resolve(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        
        lock (_lock)
        {
            // Check if we already have an instance
            if (_singletons.TryGetValue(serviceType, out var existing))
            {
                return existing;
            }

            // Determine the actual type to instantiate
            var implementationType = ResolveImplementationType(serviceType);
            
            // Create the instance with constructor injection
            var instance = CreateInstance(implementationType);
            
            // Store as singleton
            _singletons[serviceType] = instance;
            
            return instance;
        }
    }

    /// <summary>
    /// Tries to get an existing singleton instance without creating one.
    /// </summary>
    /// <typeparam name="T">The type to look up.</typeparam>
    /// <param name="instance">The instance if found.</param>
    /// <returns>True if an instance exists, false otherwise.</returns>
    public bool TryResolve<T>(out T? instance) where T : class
    {
        lock (_lock)
        {
            if (_singletons.TryGetValue(typeof(T), out var obj))
            {
                instance = (T)obj;
                return true;
            }
            instance = null;
            return false;
        }
    }

    /// <summary>
    /// Checks if a singleton instance exists for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>True if an instance exists.</returns>
    public bool IsRegistered<T>() where T : class
    {
        lock (_lock)
        {
            return _singletons.ContainsKey(typeof(T)) || _typeRegistrations.ContainsKey(typeof(T));
        }
    }

    /// <summary>
    /// Replaces an existing singleton with a new instance.
    /// Useful for replacing implementations with mocks during testing.
    /// </summary>
    /// <typeparam name="T">The type to replace.</typeparam>
    /// <param name="instance">The new instance (e.g., a mock).</param>
    /// <returns>The container for fluent configuration.</returns>
    public ServiceContainer Replace<T>(T instance) where T : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        
        lock (_lock)
        {
            _singletons[typeof(T)] = instance;
        }
        return this;
    }

    /// <summary>
    /// Removes a singleton instance, allowing it to be recreated on next resolve.
    /// </summary>
    /// <typeparam name="T">The type to remove.</typeparam>
    /// <returns>True if the instance was removed.</returns>
    public bool Remove<T>() where T : class
    {
        lock (_lock)
        {
            return _singletons.Remove(typeof(T));
        }
    }

    /// <summary>
    /// Clears all singleton instances and type registrations.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _singletons.Clear();
            _typeRegistrations.Clear();
        }
    }

    /// <summary>
    /// Creates a child container that inherits registrations from this container
    /// but can have its own overrides.
    /// </summary>
    /// <returns>A new child container.</returns>
    public ServiceContainer CreateScope()
    {
        var child = new ServiceContainer();
        lock (_lock)
        {
            foreach (var kvp in _singletons)
            {
                child._singletons[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in _typeRegistrations)
            {
                child._typeRegistrations[kvp.Key] = kvp.Value;
            }
        }
        return child;
    }

    private Type ResolveImplementationType(Type serviceType)
    {
        // Check if there's a type registration
        if (_typeRegistrations.TryGetValue(serviceType, out var implementationType))
        {
            return implementationType;
        }

        // If it's a concrete type, use it directly
        if (!serviceType.IsInterface && !serviceType.IsAbstract)
        {
            return serviceType;
        }

        throw new InvalidOperationException(
            $"Cannot resolve type '{serviceType.Name}'. No implementation has been registered for this interface/abstract type.");
    }

    private object CreateInstance(Type type)
    {
        // Get the constructor with the most parameters (greedy resolution)
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        
        if (constructors.Length == 0)
        {
            throw new InvalidOperationException(
                $"Type '{type.Name}' has no public constructors.");
        }

        // Sort by parameter count descending and take the first one
        var constructor = constructors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var parameters = constructor.GetParameters();
        var arguments = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            
            try
            {
                // Recursively resolve dependencies
                arguments[i] = Resolve(paramType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve parameter '{parameters[i].Name}' of type '{paramType.Name}' " +
                    $"for constructor of '{type.Name}'.", ex);
            }
        }

        try
        {
            return constructor.Invoke(arguments);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create instance of type '{type.Name}'.", ex);
        }
    }
}
