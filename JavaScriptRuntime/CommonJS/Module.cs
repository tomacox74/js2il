using System.Dynamic;

namespace JavaScriptRuntime.CommonJS;

/// <summary>
/// Represents a CommonJS module object per Node.js specification.
/// Each module gets its own Module instance with properties like exports, id, filename, etc.
/// https://nodejs.org/api/modules.html#the-module-object
/// </summary>
public class Module : DynamicObject
{
    private readonly RequireDelegate _requireDelegate;
    private readonly Func<object[], object?, object?> _requireFunc;
    private Array? _cachedChildren;
    private bool _childrenDirty = true;

    /// <summary>
    /// Creates a new Module instance.
    /// </summary>
    /// <param name="id">The identifier for the module. Typically the fully resolved filename.</param>
    /// <param name="filename">The fully resolved filename of the module.</param>
    /// <param name="parent">The module that required this one, or null for the main module.</param>
    /// <param name="requireDelegate">The require function bound to this module's context.</param>
    public Module(string id, string filename, Module? parent, RequireDelegate requireDelegate)
    {
        this.id = id;
        this.filename = filename;
        this.path = GetDirectoryName(filename);
        this.parent = parent;
        this._requireDelegate = requireDelegate;
        // Cache the Func instance to avoid creating a new one on every access
        this._requireFunc = (scopes, moduleId) => _requireDelegate(moduleId);
        this._childrenList = new List<Module>();
        this.paths = ComputeModulePaths(this.path);
        
        // exports starts as an empty ExpandoObject, same as Node.js
        this.exports = new ExpandoObject();
        
        // Module is not loaded until execution completes
        this.loaded = false;
    }

    /// <summary>
    /// The module.exports object. This is the value that will be returned when this module is required.
    /// </summary>
    public object? exports { get; set; }

    /// <summary>
    /// The identifier for the module. Typically this is the fully resolved filename.
    /// For the main module, this may be '.'.
    /// </summary>
    public string id { get; }

    /// <summary>
    /// The fully-resolved filename of the module.
    /// </summary>
    public string filename { get; }

    /// <summary>
    /// The directory name of the module.
    /// </summary>
    public string path { get; }

    /// <summary>
    /// Whether the module has finished loading.
    /// </summary>
    public bool loaded { get; set; }

    /// <summary>
    /// The module that first required this one, or null if this is the main module.
    /// </summary>
    public Module? parent { get; }

    /// <summary>
    /// The module objects required by this module as a JavaScript-compatible array.
    /// </summary>
    /// <remarks>
    /// This property caches the Array instance and only recreates it when the children
    /// list is modified (via AddChild). The cache is invalidated when new children are added.
    /// </remarks>
    public object children
    {
        get
        {
            if (_childrenDirty || _cachedChildren == null)
            {
                _cachedChildren = new Array(_childrenList.Cast<object>().ToArray());
                _childrenDirty = false;
            }
            return _cachedChildren;
        }
    }
    
    private readonly List<Module> _childrenList;

    /// <summary>
    /// The search paths for the module as a JavaScript-compatible array.
    /// </summary>
    public object paths { get; }

    /// <summary>
    /// The require function bound to this module's context.
    /// This property exposes the require delegate for JavaScript access.
    /// </summary>
    public object require => _requireFunc;

    /// <summary>
    /// Use the internal require function to import a module.
    /// This method is for direct C# calls.
    /// </summary>
    /// <param name="moduleId">The module specifier to require.</param>
    /// <returns>The exports of the required module.</returns>
    public object? Require(object? moduleId)
    {
        return _requireDelegate(moduleId);
    }

    /// <summary>
    /// Adds a child module to this module's children list.
    /// </summary>
    /// <param name="child">The child module to add.</param>
    internal void AddChild(Module child)
    {
        if (!_childrenList.Contains(child))
        {
            _childrenList.Add(child);
            _childrenDirty = true;
        }
    }

    /// <summary>
    /// Marks the module as loaded.
    /// </summary>
    internal void MarkLoaded()
    {
        loaded = true;
    }

    /// <summary>
    /// Gets the directory name from a path, using forward slashes.
    /// </summary>
    private static string GetDirectoryName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        // Normalize to forward slashes
        var normalized = filePath.Replace('\\', '/');
        var lastSlash = normalized.LastIndexOf('/');
        
        if (lastSlash < 0)
            return string.Empty;
            
        return normalized.Substring(0, lastSlash);
    }

    /// <summary>
    /// Computes the module search paths similar to Node.js algorithm.
    /// For a module at /home/user/project/src/module.js, this returns:
    /// - /home/user/project/src/node_modules
    /// - /home/user/project/node_modules
    /// - /home/user/node_modules
    /// - /home/node_modules
    /// - /node_modules
    /// Returns a JavaScript Array for compatibility with Array.isArray().
    /// </summary>
    private static object ComputeModulePaths(string modulePath)
    {
        var pathList = new List<object>();
        
        if (string.IsNullOrEmpty(modulePath))
            return new Array(pathList.ToArray());

        var normalized = modulePath.Replace('\\', '/');
        
        // Split into parts and build paths from specific to general
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = parts.Length; i >= 0; i--)
        {
            var basePath = string.Join("/", parts.Take(i));
            if (!string.IsNullOrEmpty(basePath))
            {
                // Handle both Unix and Windows paths
                if (normalized.StartsWith("/"))
                {
                    basePath = "/" + basePath;
                }
                pathList.Add(basePath + "/node_modules");
            }
        }

        // Add global node_modules as last resort
        if (!pathList.Any(p => p as string == "/node_modules"))
        {
            pathList.Add("/node_modules");
        }

        return new Array(pathList.ToArray());
    }

    // DynamicObject overrides for JavaScript interop
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        switch (binder.Name)
        {
            case "exports":
                result = exports;
                return true;
            case "id":
                result = id;
                return true;
            case "filename":
                result = filename;
                return true;
            case "path":
                result = path;
                return true;
            case "loaded":
                result = loaded;
                return true;
            case "parent":
                result = parent;
                return true;
            case "children":
                result = children;
                return true;
            case "paths":
                result = paths;
                return true;
            case "require":
                result = require;
                return true;
            default:
                result = null;
                return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        switch (binder.Name)
        {
            case "exports":
                exports = value;
                return true;
            case "loaded":
                loaded = value is bool b ? b : TypeUtilities.ToBoolean(value);
                return true;
            default:
                // Other properties are read-only
                return false;
        }
    }
}
