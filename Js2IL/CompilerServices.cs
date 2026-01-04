using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using Js2IL.IL;
using Js2IL.Services.ScopesAbi;
using Js2IL.Services.VariableBindings;
 
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


        // these classes cache references to external assemblies.
        // prior to having these caches the generated assembly had multiple references to the same types
        // these classes also have methods that generate references from provided dotnet types using reflection
        services.AddSingleton<Utilities.Ecma335.TypeReferenceRegistry>();
        services.AddSingleton<Utilities.Ecma335.MemberReferenceRegistry>();

        // Scope metadata and variable registries - singleton per compilation
        // ScopeMetadataRegistry holds scope type handles and field handles
        // VariableRegistry wraps ScopeMetadataRegistry and adds variable metadata
        services.AddSingleton<ScopeMetadataRegistry>();
        services.AddSingleton<VariableRegistry>();

        services.AddSingleton<ModuleLoader>();
        services.AddSingleton<Services.AssemblyGenerator>();
        services.AddSingleton<Services.BaseClassLibraryReferences>();
        services.AddSingleton<CompiledMethodCache>();
        services.AddSingleton<CallableMetadataRegistry>();

        services.AddTransient<JsMethodCompiler>();

        return services.BuildServiceProvider();
    }
}