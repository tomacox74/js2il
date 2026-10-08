using Acornima.Ast;

namespace Jroc.Validation;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    internal Action<Jroc.DebugSymbols.SourceSpan, string>? UnsupportedSiteRecorder { get; init; }
}

public interface IAstValidator
{
    ValidationResult Validate(Acornima.Ast.Program ast);
} 