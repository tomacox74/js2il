using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is the entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private readonly MetadataBuilder _metadataBuilder;
        private JavaScriptFunctionGenerator _functionGenerator;
        private ClassesGenerator _classesGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private SymbolTable _symbolTable;

        private readonly Variables _rootVariables;

        private BaseClassLibraryReferences _bclReferences;

        private readonly ClassRegistry _classRegistry;
        
        private readonly TwoPhaseCompilationCoordinator _twoPhaseCoordinator;
        private readonly IServiceProvider _serviceProvider;

        public MainGenerator(IServiceProvider serviceProvider, Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, SymbolTable symbolTable)
        {
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            _rootVariables = variables ?? throw new ArgumentNullException(nameof(variables));

            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
            
            _serviceProvider = serviceProvider;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = serviceProvider.GetRequiredService<ClassRegistry>();
            _functionGenerator = new JavaScriptFunctionGenerator(serviceProvider, variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _classRegistry, symbolTable);
            _classesGenerator = new ClassesGenerator(serviceProvider,metadataBuilder, bclReferences, _classRegistry, variables);            
            _twoPhaseCoordinator = serviceProvider.GetRequiredService<TwoPhaseCompilationCoordinator>();
        }

        /// <summary>
        /// Determines if the global scope instance needs to be created.
        /// The instance is only needed when:
        /// 1. Any global variable is captured (referenced from nested functions/classes), OR
        /// 2. Any class or function needs parent scope access (ReferencesParentScopeVariables), OR
        /// 3. There are function declarations at global scope (stored as scope fields)
        /// </summary>
        private bool ShouldCreateGlobalScopeInstance()
        {
            var globalScope = _symbolTable.Root;
            
            // Check if any global binding is captured (accessed from nested scope)
            // OR if any binding is a function declaration (stored as scope field)
            foreach (var binding in globalScope.Bindings.Values)
            {
                if (binding.IsCaptured)
                {
                    return true;
                }
                // Function declarations are stored as scope fields
                if (binding.Kind == BindingKind.Function)
                {
                    return true;
                }
            }
            
            // Check if any child scope (function or class) references parent scope variables
            foreach (var child in globalScope.Children)
            {
                if (child.ReferencesParentScopeVariables)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Creates the global scope instance.
        /// The instance is stored in a local variable that can be accessed by variable operations.
        /// </summary>
        private void CreateGlobalScopeInstance(Variables variables) =>
            throw new NotSupportedException("Legacy main-method emission is no longer supported. Use JsMethodCompiler (IR pipeline) instead.");

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
                _rootVariables,
                _bclReferences,
                _methodBodyStreamEncoder,
                _classRegistry,
                _functionGenerator.FunctionRegistry,
                compileAnonymousCallablesPhase2: callables =>
                    _twoPhaseCoordinator.CompilePhase2AnonymousCallables(
                        callables,
                        _metadataBuilder,
                        _serviceProvider,
                        _rootVariables,
                        _bclReferences,
                        _methodBodyStreamEncoder,
                        _classRegistry,
                        _functionGenerator.FunctionRegistry,
                        _symbolTable),
                compileClassesAndFunctionsPhase2: () =>
                {
                    _classesGenerator.DeclareClasses(symbolTable);
                    // Function declarations are compiled in planned Phase 2.
                });
        }

        /// <summary>
        /// Generates the main method body using the legacy IL emitter.
        /// Call DeclareClassesAndFunctions first.
        /// </summary>
        public int GenerateMethodBody(Acornima.Ast.Program ast)
        {
            throw new NotSupportedException(
                "Legacy main-method emission is no longer supported. The module main method must be compiled via JsMethodCompiler (IR pipeline)." );
        }

        /// <summary>
        /// Generates the complete main method including class/function declarations and method body.
        /// This is the original combined method for backward compatibility.
        /// </summary>
        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            throw new NotSupportedException(
                "Legacy main-method emission is no longer supported. The module main method must be compiled via JsMethodCompiler (IR pipeline)." );
        }
    }
}
