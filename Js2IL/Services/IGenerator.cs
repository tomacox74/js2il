namespace Js2IL.Services;

public interface IGenerator
{
    void Generate(ModuleDefinition module, string name, string outputPath);
} 