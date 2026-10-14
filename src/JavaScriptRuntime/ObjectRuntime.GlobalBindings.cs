namespace JavaScriptRuntime;

internal sealed class ScriptLexicalBinding(bool immutable)
{
    internal bool Immutable { get; } = immutable;
    internal bool Initialized { get; set; }
    internal object? Value { get; set; }
}

public static partial class ObjectRuntime
{
    private static bool TryGetGlobalLexicalBinding(string name, out ScriptLexicalBinding? binding)
    {
        binding = null;
        return RuntimeExecutionContext.CurrentOrOverride?.GlobalLexicalBindings.TryGetValue(name, out binding) == true;
    }

    public static void InstantiateGlobalDeclarations(
        string lexicalNames, string constantNames, string functionNames, string variableNames)
    {
        var global = GlobalThis.globalThis;
        var context = RuntimeExecutionContext.CurrentOrOverride
            ?? throw new InvalidOperationException("Script execution requires an active realm.");
        var lexicals = context.GlobalLexicalBindings;
        var lexical = Split(lexicalNames);
        var constants = Split(constantNames).ToHashSet(StringComparer.Ordinal);
        var functions = Split(functionNames);
        var variables = Split(variableNames);

        foreach (var name in lexical)
        {
            if (lexicals.ContainsKey(name) || context.GlobalVarDeclaredNames.Contains(name)
                || (TryGetOwnPropertyDescriptor(global, name, out var descriptor) && !descriptor.Configurable))
            {
                throw new SyntaxError($"Identifier '{name}' has already been declared");
            }
        }
        foreach (var name in functions.Concat(variables))
        {
            if (lexicals.ContainsKey(name))
            {
                throw new SyntaxError($"Identifier '{name}' has already been declared");
            }
        }
        foreach (var name in functions)
        {
            if (TryGetOwnPropertyDescriptor(global, name, out var descriptor)
                ? !descriptor.Configurable
                    && !(descriptor.Kind == JsPropertyDescriptorKind.Data && descriptor.Writable && descriptor.Enumerable)
                : !IsExtensibleInternal(global))
            {
                throw new TypeError($"Cannot declare global function '{name}'");
            }
        }
        foreach (var name in variables)
        {
            if (!TryGetOwnPropertyDescriptor(global, name, out _) && !IsExtensibleInternal(global))
            {
                throw new TypeError($"Cannot declare global variable '{name}'");
            }
        }

        foreach (var name in lexical)
        {
            lexicals.Add(name, new ScriptLexicalBinding(constants.Contains(name)));
        }
        foreach (var name in functions)
        {
            InitializeGlobalFunctionBinding(name, null);
        }
        foreach (var name in variables)
        {
            EnsureGlobalVarBinding(name);
        }
        context.GlobalVarDeclaredNames.UnionWith(variables);
        context.GlobalVarDeclaredNames.UnionWith(functions);

        static string[] Split(string names)
            => names.Length == 0 ? [] : names.Split('\0');
    }

    public static object? InitializeGlobalLexicalBinding(string name, object? value)
    {
        if (!TryGetGlobalLexicalBinding(name, out var binding) || binding is null || binding.Initialized)
        {
            throw new ReferenceError($"Cannot initialize global lexical binding '{name}'");
        }
        binding.Value = value;
        binding.Initialized = true;
        return value;
    }

    public static object? InitializeGlobalFunctionBinding(string name, object? value)
    {
        var global = GlobalThis.globalThis;
        if (!TryGetOwnPropertyDescriptor(global, name, out var descriptor) || descriptor.Configurable)
        {
            var attributes = new JsObject
            {
                ["value"] = value,
                ["writable"] = true,
                ["enumerable"] = true,
                ["configurable"] = false
            };
            defineProperty(global, name, attributes);
        }
        else
        {
            SetProperty(global, name, value, throwOnError: true);
        }
        return value;
    }
}
