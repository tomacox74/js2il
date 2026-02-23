using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Js2IL.IL;
using Js2IL.Services.ScopesAbi;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Services;
using Js2IL.DebugSymbols;
using Js2IL.Diagnostics;
 
namespace Js2IL;
 
public static class CompilerServices
{
    public static ServiceProvider BuildServiceProvider(CompilerOptions options, IFileSystem? fileSystem = null, ICompilerOutput? compilerOutput = null)
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

        // User-facing compiler output stream
        if (compilerOutput != null)
        {
            services.AddSingleton<ICompilerOutput>(compilerOutput);
        }
        else
        {
            services.AddSingleton<ICompilerOutput, Logger>();
        }

        // compiler and compiler options
        services.AddSingleton(options);

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            if (options.Verbose)
            {
                builder.AddProvider(new TextWriterLoggerProvider(Console.Out, ownsWriter: false));
            }

            if (!string.IsNullOrWhiteSpace(options.DiagnosticFilePath))
            {
                var diagnosticFilePath = Path.GetFullPath(options.DiagnosticFilePath);
                var diagnosticDirectory = Path.GetDirectoryName(diagnosticFilePath);
                if (!string.IsNullOrWhiteSpace(diagnosticDirectory))
                {
                    Directory.CreateDirectory(diagnosticDirectory);
                }

                var writer = new StreamWriter(diagnosticFilePath, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
                {
                    AutoFlush = true
                };
                builder.AddProvider(new TextWriterLoggerProvider(writer, ownsWriter: true));
            }

            builder.SetMinimumLevel(options.DiagnosticsEnabled ? LogLevel.Information : LogLevel.None);
        });

        services.AddSingleton<Compiler>();

        // Debug symbol collection (Portable PDB)
        services.AddSingleton<DebugSymbolRegistry>();


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
        services.AddSingleton<Services.NodeModuleResolver>();
        services.AddSingleton<Services.AssemblyGenerator>();
        services.AddSingleton<Services.BaseClassLibraryReferences>();

        // Collect nested type relationships and emit NestedClass rows in sorted order at the end.
        services.AddSingleton<Services.NestedTypeRelationshipRegistry>();
            services.AddSingleton<Services.ModuleTypeMetadataRegistry>();

        // Function type metadata (function declarations become nested types under Modules.<ModuleName>)
        services.AddSingleton<Services.FunctionTypeMetadataRegistry>();

        // Anonymous callable owner type metadata (arrows + function expressions)
        services.AddSingleton<Services.AnonymousCallableTypeMetadataRegistry>();

        // CallableMetadataRegistry will be populated and queried by the IL generators in a future phase.
        // It is registered here so that downstream components can start depending on it without breaking changes
        // when the callable metadata discovery and registration pipeline is fully integrated.
        services.AddSingleton<CallableMetadataRegistry>();

        // Shared ClassRegistry used by both legacy generators and IR pipeline (PL3.3b new Foo())
        services.AddSingleton<ClassRegistry>();

        // CallableRegistry is the canonical store for callable declarations (used by TwoPhaseCompilationCoordinator)
        services.AddSingleton<CallableRegistry>();
        services.AddSingleton<ICallableCatalog>(sp => sp.GetRequiredService<CallableRegistry>());
        services.AddSingleton<ICallableDeclarationWriter>(sp => sp.GetRequiredService<CallableRegistry>());
        services.AddSingleton<ICallableDeclarationReader>(sp => sp.GetRequiredService<CallableRegistry>());

        services.AddTransient<JsMethodCompiler>();
        
        // Two-phase compilation coordinator (singleton per compilation)
        // Generators resolve this from DI and must observe a single shared coordinator instance.
        services.AddSingleton<TwoPhaseCompilationCoordinator>();

        return services.BuildServiceProvider();
    }
}
