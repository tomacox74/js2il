namespace Js2IL.Services;

public interface IGenerator
{
    void Generate(Acornima.Ast.Program ast, string name, string outputPath);
} 