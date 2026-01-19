using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services
{
    internal class AssemblyGenerator
    {
        // Standard public key as defined in ECMA-335 for reference assemblies
        private static readonly byte[] StandardPublicKey = new byte[] {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public MetadataBuilder _metadataBuilder;
        private BlobBuilder _ilBuilder = new BlobBuilder();
        private MethodDefinitionHandle _entryPoint;

        private MethodDefinitionHandle _mainScriptMethod;
        private BaseClassLibraryReferences _bclReferences;

        private readonly VariableBindings.VariableRegistry _variableRegistry;

        private TypeReferenceRegistry _typeReferenceRegistry;

        private IServiceProvider _serviceProvider;

        public AssemblyGenerator(IServiceProvider serviceProvider, MetadataBuilder metadataBuilder, TypeReferenceRegistry typeReferenceRegistry, VariableBindings.VariableRegistry variableRegistry)
        {
            this._metadataBuilder = metadataBuilder;
            this._typeReferenceRegistry = typeReferenceRegistry;
            this._bclReferences = serviceProvider.GetRequiredService<BaseClassLibraryReferences>();
            this._serviceProvider = serviceProvider;
            this._variableRegistry = variableRegistry;
        }

        /// <summary>
        /// Generates a new assembly from the provided AST and scope tree.
        /// </summary>
        /// <param name="modules">Then javascript modules to generate the assembly from.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="outputPath">The directory to output the generated assembly and related files to.</param>
        public void Generate(Modules modules, string assemblyName, string outputPath)
        {
            createAssemblyMetadata(assemblyName);

            // Add the <Module> type first (as required by .NET metadata) using TypeBuilder
            var moduleTypeBuilder = new TypeBuilder(_metadataBuilder, "", "<Module>");
            moduleTypeBuilder.AddTypeDefinition(
                TypeAttributes.NotPublic,
                baseType: default(EntityHandle));

            // the API for generating IL is a little confusing
            // there is 1 MethodBodyStreamEncoder for all methods in the assembly
            var methodBodyStream = new MethodBodyStreamEncoder(this._ilBuilder);

            // Phase 0: compute callable counts across all modules so we can assign stable
            // future ctor MethodDef tokens for ALL scope types (regardless of module processing order).
            var totalCallableMethods = modules._modules.Values.Sum(m => new CallableDiscovery(m.SymbolTable!).DiscoverAll().Count);
            var totalModuleInitMethods = modules._modules.Values.Count;

            // Scope types are generated before callables are compiled (so variable binding has FieldDef handles),
            // but scope constructors are emitted later. We create a single TypeGenerator instance so it can
            // remember deferred ctor plans across all modules.
            var typeGenerator = new TypeGenerator(
                _metadataBuilder,
                _bclReferences,
                methodBodyStream,
                _serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>(),
                _variableRegistry,
                deferredCtorStartRow: _metadataBuilder.GetRowCount(TableIndex.MethodDef) + totalCallableMethods + totalModuleInitMethods + 1);

            var moduleList = modules._modules.Values.ToList();

            // Multi-module assemblies must keep TypeDef.MethodList monotonic across the entire TypeDef table.
            // Additionally, nested types must have their enclosing TypeDef created earlier in the TypeDef table
            // (otherwise some CLR loaders throw BadImageFormatException).
            //
            // To support nesting function-declaration owner types under the module type (Modules.<ModuleName>+<FunctionName>),
            // we allocate/emits module init methods FIRST in the MethodDef table, then all callable MethodDefs.
            // This allows module root TypeDefs to be created before callable-owner TypeDefs while keeping MethodList monotonic.

            // Track expected MethodDef tokens for module init methods so we can validate emission order.
            var expectedModuleInitHandles = new Dictionary<string, MethodDefinitionHandle>(StringComparer.Ordinal);

            // Reserve MethodDef row ids for module init methods first.
            var methodDefBaseRow = _metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1;
            var moduleInitMethodRow = methodDefBaseRow;
            var moduleTypeRegistry = _serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>();
            foreach (var module in moduleList)
            {
                var expectedInitHandle = MetadataTokens.MethodDefinitionHandle(moduleInitMethodRow++);
                expectedModuleInitHandles[module.Name] = expectedInitHandle;

                var moduleRootTypeBuilder = new TypeBuilder(_metadataBuilder, "Modules", module.Name);
                var moduleTypeHandle = moduleRootTypeBuilder.AddTypeDefinition(
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                    _bclReferences.ObjectType,
                    firstFieldOverride: null,
                    firstMethodOverride: expectedInitHandle);

                moduleTypeRegistry.Add(module.Name, moduleTypeHandle);
            }

            // Phase 1: Predeclare callable owner TypeDefs + class TypeDefs for ALL modules.
            // Callable MethodDefs come AFTER module init MethodDefs.
            var callableMethodDefBaseRow = methodDefBaseRow + totalModuleInitMethods;
            foreach (var module in moduleList)
            {
                var symbolTable = module.SymbolTable!;
                var coordinator = _serviceProvider.GetRequiredService<TwoPhaseCompilationCoordinator>();

                // Count callables for this module (deterministic; used for global MethodDef row assignment).
                var callableCount = new CallableDiscovery(symbolTable).DiscoverAll().Count;

                // Phase 1 (per module): allocate future MethodDef row ids for all callables.
                // IMPORTANT: do this before declaring any owner TypeDefs so we can keep TypeDef.MethodList monotonic.
                coordinator.PreallocateCallableMethodDefsForNesting(
                    symbolTable,
                    _metadataBuilder,
                    methodDefBaseRowOverride: callableMethodDefBaseRow);

                // Predeclare class TypeDefs (requires ctor MethodDef token to already be allocated above).
                var classRegistry = _serviceProvider.GetRequiredService<ClassRegistry>();
                var nestedTypeRegistry = _serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>();
                var classesGenerator = new ClassesGenerator(
                    _serviceProvider,
                    _metadataBuilder,
                    _bclReferences,
                    classRegistry,
                    nestedTypeRegistry,
                    module.Name);
                classesGenerator.DeclareClasses(symbolTable);

                // Declare callable-owner TypeDefs that scopes may nest under.
                // IMPORTANT: function owner types must come AFTER classes (class ctors are emitted first).
                coordinator.DeclareFunctionAndAnonymousOwnerTypesForNesting(symbolTable, _metadataBuilder, _bclReferences);

                callableMethodDefBaseRow += callableCount;
            }

            // Phase 2: Emit scope TypeDefs for ALL modules (constructors deferred).
            foreach (var module in moduleList)
            {
                var symbolTable = module.SymbolTable!;
                typeGenerator.GenerateTypes(symbolTable);
            }

            // Phase 3: Compile module init methods now (after scope types, before callables).
            // Module types were declared earlier with MethodList pointing at these MethodDefs.
            foreach (var module in moduleList)
            {
                var methodDefinitionHandle = GenerateModule(module, methodBodyStream, module.Name);
                if (!expectedModuleInitHandles.TryGetValue(module.Name, out var expectedInitHandle))
                {
                    throw new InvalidOperationException($"Missing expected module init handle for module '{module.Name}'.");
                }
                if (methodDefinitionHandle != expectedInitHandle)
                {
                    throw new InvalidOperationException(
                        $"Module init MethodDef token mismatch for module '{module.Name}'. Expected 0x{MetadataTokens.GetToken(expectedInitHandle):X8}, got 0x{MetadataTokens.GetToken(methodDefinitionHandle):X8}.");
                }
                if (module == modules.rootModule)
                {
                    _mainScriptMethod = methodDefinitionHandle;
                }
            }

            // Phase 4: Compile and emit callable MethodDefs for ALL modules.
            foreach (var module in moduleList)
            {
                var symbolTable = module.SymbolTable!;
                var mainGenerator = new MainGenerator(_serviceProvider, module.Name, _bclReferences, _metadataBuilder, methodBodyStream, symbolTable);
                mainGenerator.DeclareClassesAndFunctions(symbolTable);
            }

            // Phase 5: Emit all deferred scope constructors in the exact TypeDef creation order.
            typeGenerator.EmitDeferredScopeConstructors();

            // create the entry point for spining up the execution engine
            createEntryPoint(methodBodyStream);

            // Emit NestedClass table rows in one globally sorted pass.
            // This must happen after all TypeDefs have been created.
            _serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>().EmitAllSorted(_metadataBuilder);

            this.CreateAssembly(assemblyName, outputPath);
        }

        /// <summary>
        /// Generates the scope types for the assembly.
        /// </summary>
        /// <param name="modules">The JavaScript modules (need the symbol tables)</param>
        /// <remarks>
        /// Scopes are for captured varables.  Fore example:
        /// const globalVar = 42;
        /// function logGlobal() { console.log(globalVar); }
        /// We create 
        /// class <ScopeName> {
        ///     public object globalVar;
        /// }
        /// </remarks>
        // Scope type generation is orchestrated in Generate() so we can defer scope constructors
        // until after callable method bodies have been emitted.

        private void GenerateModules(Modules modules, MethodBodyStreamEncoder methodBodyStream)
        {
            foreach (var module in modules._modules.Values)
            {
                var methodDefinitionHandle = GenerateModule(module, methodBodyStream, module.Name);
                if (module == modules.rootModule)
                {
                    _mainScriptMethod = methodDefinitionHandle;
                }
            }
        }

        private MethodDefinitionHandle GenerateModule(ModuleDefinition module, MethodBodyStreamEncoder methodBodyStream, string moduleName)
        {
            // Get parameter info from shared ModuleParameters
            var paramCount = JavaScriptRuntime.CommonJS.ModuleParameters.Count;
            var parameterNames = JavaScriptRuntime.CommonJS.ModuleParameters.ParameterNames;

            var symbolTable = module.SymbolTable!;

            // Now compile the module init method (IR pipeline).
            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
            var methodDefinitionHandle = methodCompiler.TryCompileMainMethod(module.Name, module.Ast, symbolTable.Root!, methodBodyStream);
            IR.IRPipelineMetrics.RecordMainMethodAttempt(!methodDefinitionHandle.IsNil);
            if (!methodDefinitionHandle.IsNil)
            {
                return methodDefinitionHandle;
            }

            throw new NotSupportedException(
                $"IR pipeline could not compile module '{moduleName}' main method.");
        }

        private void createEntryPoint(MethodBodyStreamEncoder methodBodyStream)
        {
            var entryPointTypeBuilder = new TypeBuilder(_metadataBuilder, "", "Program");

            // create the signature for the entry point method
            var methodSig = MethodBuilder.BuildMethodSignature(
                _metadataBuilder,
                isInstance: false,
                paramCount: 0, 
                hasScopesParam: false, 
                returnsVoid: true);

            // Emit IL for the entry point method directly (no legacy method generator dependency).
            var ilBuilder = new BlobBuilder();
            var ilEncoder = new InstructionEncoder(ilBuilder);
            var runtime = new Runtime(
                ilEncoder,
                _serviceProvider.GetRequiredService<TypeReferenceRegistry>(),
                _serviceProvider.GetRequiredService<MemberReferenceRegistry>());


            // emit IL for the entry point method

            // first create new instance of the engine
            runtime.InvokeEngineCtor();

            // Create a ModuleMainDelegate that wraps the main module method
            // ModuleMainDelegate takes (exports, require, module, __filename, __dirname)
            ilEncoder.OpCode(ILOpCode.Ldnull);
            ilEncoder.OpCode(ILOpCode.Ldftn);
            ilEncoder.Token(this._mainScriptMethod);
            ilEncoder.OpCode(ILOpCode.Newobj);
            ilEncoder.Token(_bclReferences.ModuleMainDelegate_Ctor_Ref);

            ilEncoder.OpCode(ILOpCode.Callvirt);
            var engineExecuteRef = runtime.GetInstanceMethodRef(
                typeof(JavaScriptRuntime.Engine),
                "Execute",
                0,
                typeof(JavaScriptRuntime.CommonJS.ModuleMainDelegate));
            ilEncoder.Token(engineExecuteRef);

            ilEncoder.OpCode(ILOpCode.Ret);

            var entryPointOffset = methodBodyStream.AddMethodBody(
                ilEncoder,
                maxStack: 3,
                localVariablesSignature: default,
                attributes: MethodBodyAttributes.None);

            _entryPoint = entryPointTypeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                "Main",
                methodSig,
                entryPointOffset);

            // Define the Program type that contains the entry point
            entryPointTypeBuilder.AddTypeDefinition(
                TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);
        }

        private void createAssemblyMetadata(string name)
        {
            // Create the assembly metadata.
            var assemblyName = _metadataBuilder.GetOrAddString(name);
            var culture = _metadataBuilder.GetOrAddString("");
            this._metadataBuilder.AddAssembly(
                name: assemblyName,
                version: new Version(1, 0, 0, 0),
                culture: culture,
                publicKey: default,
                flags: 0,
                hashAlgorithm: AssemblyHashAlgorithm.None
            );

            _metadataBuilder.AddModule(0, assemblyName, _metadataBuilder.GetOrAddGuid(Guid.NewGuid()), default, default);
        }

        private void CreateAssembly(string name, string outputPath)
        {
            var pe = new ManagedPEBuilder(
                PEHeaderBuilder.CreateLibraryHeader(),
                new MetadataRootBuilder(_metadataBuilder),
                _ilBuilder,
                mappedFieldData: null,
                entryPoint: this._entryPoint,
                flags: CorFlags.ILOnly);

            var peImage = new BlobBuilder();
            pe.Serialize(peImage);

            string assemblyDll = Path.Combine(outputPath, $"{name}.dll");
            File.WriteAllBytes(assemblyDll, peImage.ToArray());

            RuntimeConfigWriter.WriteRuntimeConfigJson(assemblyDll, typeof(object).Assembly.GetName());

            // Copy JavaScriptRuntime.dll to output directory
            // when it differs from the source.
            var jsRuntimeDll = typeof(JavaScriptRuntime.Object).Assembly.Location!;
            var jsRuntimeAssemblyFileName = Path.GetFileName(jsRuntimeDll);
            var jsRuntimeDllDest = Path.Combine(outputPath, jsRuntimeAssemblyFileName);
            if (File.Exists(jsRuntimeDll))
            {
                var sourceInfo = new FileInfo(jsRuntimeDll);

                // Assembly file version often remains constant during local development.
                // Use metadata on disk to detect changes and avoid leaving stale runtimes in output folders.
                if (File.Exists(jsRuntimeDllDest))
                {
                    var destInfo = new FileInfo(jsRuntimeDllDest);
                    if (sourceInfo.Length == destInfo.Length && sourceInfo.LastWriteTimeUtc == destInfo.LastWriteTimeUtc)
                    {
                        return;
                    }
                }

                try
                {
                    File.Copy(jsRuntimeDll, jsRuntimeDllDest, true);
                    File.SetLastWriteTimeUtc(jsRuntimeDllDest, sourceInfo.LastWriteTimeUtc);
                }
                catch (IOException)
                {
                    // In parallel test runs multiple compilations may attempt to write the runtime
                    // into the same output folder at the same time, or the runtime may already be
                    // loaded by another process. If the destination exists, treat it as good enough.
                    if (!File.Exists(jsRuntimeDllDest))
                    {
                        throw;
                    }
                }

                var jsRuntimePdb = Path.ChangeExtension(jsRuntimeDll, ".pdb");
                var jsRuntimePdbDest = Path.ChangeExtension(jsRuntimeDllDest, ".pdb");
                if (File.Exists(jsRuntimePdb))
                {
                    var sourcePdbInfo = new FileInfo(jsRuntimePdb);
                    try
                    {
                        File.Copy(jsRuntimePdb, jsRuntimePdbDest, true);
                        File.SetLastWriteTimeUtc(jsRuntimePdbDest, sourcePdbInfo.LastWriteTimeUtc);
                    }
                    catch (IOException)
                    {
                        if (!File.Exists(jsRuntimePdbDest))
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }
}
