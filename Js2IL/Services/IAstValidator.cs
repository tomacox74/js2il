using Acornima.Ast;

namespace Js2IL.Services;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public interface IAstValidator
{
    ValidationResult Validate(Acornima.Ast.Program ast);
} 