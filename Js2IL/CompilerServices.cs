using Microsoft.Extensions.DependencyInjection;
 
namespace Js2IL;
 
public static class CompilerServices
{
    public static ServiceProvider BuildServiceProvider(CompilerOptions options, IFileSystem? fileSystem = null)
    {
        var services = new ServiceCollection();

        if (fileSystem != null)
        {
            services.AddSingleton(fileSystem);
        }
        else
        {
            services.AddSingleton<IFileSystem, FileSystem>();
        }
        
        services.AddSingleton<ModuleLoader>();

        services.AddSingleton(options);
        services.AddSingleton<Compiler>();

        return services.BuildServiceProvider();
    }
}