using Acornima.Ast;

namespace Js2IL.Services;

public interface IILGenerator
{
    void GenerateIL(Acornima.Ast.Program ast, string outputPath);
} 