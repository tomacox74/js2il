namespace JavaScriptRuntime;

[Flags]
public enum InvocationContextRequirements
{
    None = 0,
    This = 1 << 0,
    Arguments = 1 << 1,
    Callee = 1 << 2,
    NewTarget = 1 << 3,
    LexicalSuper = 1 << 4,
    All = This | Arguments | Callee | NewTarget | LexicalSuper
}

