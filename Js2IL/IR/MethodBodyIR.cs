namespace Js2IL.IR;

public sealed class MethodBodyIR
{
    public List<string> Parameters { get; } = new();

    public List<LocalVariable> Locals { get; } = new();

    public List<TempVariable> Temps { get; } = new();

    public List<LIRInstruction> Instructions { get; } = new();
}