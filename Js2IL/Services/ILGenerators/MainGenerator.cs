using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is teh entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private ILMethodGenerator _ilGenerator;
        private JavaScriptFunctionGenerator _functionGenerator;
        private ClassesGenerator _classesGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private SymbolTable _symbolTable;

        private BaseClassLibraryReferences _bclReferences;

        private readonly ClassRegistry _classRegistry = new();
        
        private readonly CompilerOptions _compilerOptions;
        private readonly TwoPhaseCompilationCoordinator? _twoPhaseCoordinator;

        public MainGenerator(IServiceProvider serviceProvider, Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, SymbolTable symbolTable, CompilerOptions? compilerOptions = null)
        {
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            if (variables == null) throw new ArgumentNullException(nameof(variables));
            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
            
            _bclReferences = bclReferences;
            _compilerOptions = compilerOptions ?? new CompilerOptions();
            _functionGenerator = new JavaScriptFunctionGenerator(serviceProvider, variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _classRegistry, symbolTable);
            _ilGenerator = new ILMethodGenerator(serviceProvider, variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _classRegistry, _functionGenerator.FunctionRegistry, symbolTable: symbolTable);
            _classesGenerator = new ClassesGenerator(serviceProvider,metadataBuilder, bclReferences, methodBodyStreamEncoder, _classRegistry, variables);
            this._methodBodyStreamEncoder = methodBodyStreamEncoder;
            
            // Initialize two-phase coordinator if enabled
            if (_compilerOptions.TwoPhaseCompilation)
            {
                var logger = serviceProvider.GetRequiredService<ILogger>();
                _twoPhaseCoordinator = new TwoPhaseCompilationCoordinator(logger, _compilerOptions.Verbose);
            }
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
        private void CreateGlobalScopeInstance(Variables variables)
        {
            // Delegate to shared helper; safe no-op if no registry or scope type is available
            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(variables, _ilGenerator.IL, _ilGenerator.MetadataBuilder);
        }

        /// <summary>
        /// Declares classes and functions, populating the CompiledMethodCache.
        /// This must be called before attempting IR compilation of the main method,
        /// because the IR pipeline relies on CompiledMethodCache to obtain compiled function
        /// method handles when emitting function call instructions.
        /// </summary>
        public void DeclareClassesAndFunctions(SymbolTable symbolTable)
        {
            if (_twoPhaseCoordinator != null)
            {
                // Two-phase path: discover callables, then delegate to legacy declaration
                _twoPhaseCoordinator.RunPhase1Discovery(symbolTable);
                _twoPhaseCoordinator.RunPhase1Declaration(() =>
                {
                    // Legacy declaration path (will be replaced in Milestone 2+)
                    _classesGenerator.DeclareClasses(symbolTable);
                    _functionGenerator.DeclareFunctions(symbolTable);
                });
            }
            else
            {
                // Legacy path: declare classes first so their types exist under the Classes namespace
                _classesGenerator.DeclareClasses(symbolTable);

                // Declare functions (emits static method definitions and populates CompiledMethodCache)
                _functionGenerator.DeclareFunctions(symbolTable);
            }
        }

        /// <summary>
        /// Generates the main method body using the legacy IL emitter.
        /// Call DeclareClassesAndFunctions first.
        /// </summary>
        public int GenerateMethodBody(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;

            // Step 1: Only create the global scope instance if it's actually needed
            // (i.e., variables are captured or classes/functions need parent scope access)
            if (ShouldCreateGlobalScopeInstance())
            {
                CreateGlobalScopeInstance(variables);
            }

            // Initialize top-level function variables directly (no dispatch table indirection)
            _ilGenerator.InitializeLocalFunctionVariables(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());

            _ilGenerator.GenerateStatementsForBody(variables.GetLeafScopeName(), false, ast.Body);

            _ilGenerator.IL.OpCode(ILOpCode.Ret);

            // local variables
            var (localSignature, methodBodyAttributes) = MethodBuilder.CreateLocalVariableSignature(metadataBuilder, variables, this._bclReferences);

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL,
                maxStack: 32,
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }

        /// <summary>
        /// Generates the complete main method including class/function declarations and method body.
        /// This is the original combined method for backward compatibility.
        /// </summary>
        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            DeclareClassesAndFunctions(_symbolTable);
            return GenerateMethodBody(ast);
        }
    }
}
