namespace JavaScriptRuntime.Modules.CommonJS;

internal sealed class ModuleNotFoundError : Error
{
    public ModuleNotFoundError(string code, string message)
        : base(message)
    {
        this.code = code;
    }

    public string code { get; }
}
