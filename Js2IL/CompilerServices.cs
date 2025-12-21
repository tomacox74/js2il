using System.Reflection.Metadata.Ecma335;
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

        // compiler and compiler options
        services.AddSingleton(options);
        services.AddSingleton<Compiler>();


        // if this compiler is extended to generate multiple assemblies in a single run
        // this will need to be changed to a factory that can produce multiple builders
        services.AddSingleton<MetadataBuilder>();


        // these classes cache references to external assembies.
        // prior to having these caches the generated assembly had multiple references to the same types
        // these classes also have methods that generate references from provided dotnet types using reflection
        services.AddSingleton<Utilities.Ecma335.TypeReferenceRegistry>();
        services.AddSingleton<Utilities.Ecma335.MemberReferenceRegistry>();

        services.AddSingleton<ModuleLoader>();
        services.AddSingleton<Services.AssemblyGenerator>();
        services.AddSingleton<Services.BaseClassLibraryReferences>();

        return services.BuildServiceProvider();
    }
}