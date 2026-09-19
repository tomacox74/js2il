using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;
using Jroc.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is the entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private readonly MetadataBuilder _metadataBuilder;
        private ClassesGenerator _classesGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private SymbolTable _symbolTable;

        private readonly string _moduleName;

        private BaseClassLibraryReferences _bclReferences;

        private readonly ClassRegistry _classRegistry;
        
        private readonly TwoPhaseCompilationCoordinator _twoPhaseCoordinator;
        private readonly IServiceProvider _serviceProvider;

        public MainGenerator(IServiceProvider serviceProvider, string moduleName, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, SymbolTable symbolTable)
        {
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            _moduleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));

            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
            
            _serviceProvider = serviceProvider;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = serviceProvider.GetRequiredService<ClassRegistry>();
            _classesGenerator = new ClassesGenerator(
                serviceProvider,
                metadataBuilder,
                bclReferences,
                _classRegistry,
                serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>(),
                _moduleName);
            _twoPhaseCoordinator = serviceProvider.GetRequiredService<TwoPhaseCompilationCoordinator>();
        }

        /// <summary>
        /// Declares classes and functions and runs the two-phase compilation coordinator.
        /// This must be called before attempting IR compilation of the main method,
        /// because the IR pipeline relies on CallableRegistry to obtain declared function
        /// method tokens when emitting function call instructions (ldftn).
        /// </summary>
        public void DeclareClassesAndFunctions(SymbolTable symbolTable)
        {
            // Two-phase pipeline is always enabled: coordinator owns ordering and compilation.
            _twoPhaseCoordinator.RunPlannedTwoPhaseCompilation(
                symbolTable,
                _metadataBuilder,
                _serviceProvider,
                _bclReferences,
                _methodBodyStreamEncoder,
                _classRegistry,
                compileAnonymousCallablesPhase2: callables =>
                    _twoPhaseCoordinator.CompilePhase2AnonymousCallables(
                        callables,
                        _metadataBuilder,
                        _serviceProvider,
                        _bclReferences,
                        _methodBodyStreamEncoder,
                        _symbolTable),
                compileClassesAndFunctionsPhase2: () =>
                {
                    _classesGenerator.DeclareClasses(symbolTable);
                    // Function declarations are compiled in planned Phase 2.
                });
        }
    }
}
