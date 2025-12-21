namespace JavaScriptRuntime.CommonJS
{
    /// <summary>
    /// Delegate type for compiled module main methods.
    /// Parameters: (exports, require, module, __filename, __dirname)
    /// </summary>
    public delegate void ModuleMainDelegate(object? exports, object? require, object? module, object? __filename, object? __dirname);

    /// <summary>
    /// Defines a parameter for a module main method.
    /// </summary>
    public sealed class ModuleParameterInfo
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsConst { get; }

        public ModuleParameterInfo(string name, Type type, bool isConst = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsConst = isConst;
        }
    }

    /// <summary>
    /// Defines the standard parameters passed to compiled JavaScript module main methods.
    /// This list is shared between compilation (Js2IL) and runtime (JavaScriptRuntime).
    /// 
    /// The parameters mirror Node.js CommonJS module wrapper:
    /// (exports, require, module, __filename, __dirname) => { ... }
    /// </summary>
    public static class ModuleParameters
    {
        /// <summary>
        /// The delegate type for module main methods.
        /// </summary>
        public static readonly Type DelegateType = typeof(ModuleMainDelegate);

        /// <summary>
        /// The ordered list of parameters for module main methods.
        /// Order matters: it determines IL argument indices.
        /// </summary>
        public static readonly IReadOnlyList<ModuleParameterInfo> Parameters = new[]
        {
            new ModuleParameterInfo("exports", typeof(object), isConst: false),
            new ModuleParameterInfo("require", typeof(object), isConst: false),
            new ModuleParameterInfo("module", typeof(object), isConst: false),
            new ModuleParameterInfo("__filename", typeof(object), isConst: true),
            new ModuleParameterInfo("__dirname", typeof(object), isConst: true),
        };

        /// <summary>
        /// Gets the parameter names in order.
        /// </summary>
        public static IEnumerable<string> ParameterNames => Parameters.Select(p => p.Name);

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public static int Count => Parameters.Count;
    }
}
