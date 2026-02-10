using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using Js2IL.DebugSymbols;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.Contracts;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Js2IL.Validation;
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

        private AssemblyDefinitionHandle _assemblyDefinition;

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

            EmitDebuggableAttributeIfEnabled();

            EmitCompiledModuleManifest(modules);

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
            var compileOptions = _serviceProvider.GetRequiredService<CompilerOptions>();
            var typeGenerator = new TypeGenerator(
                _metadataBuilder,
                _bclReferences,
                methodBodyStream,
                _variableRegistry,
                deferredCtorStartRow: _metadataBuilder.GetRowCount(TableIndex.MethodDef) + totalCallableMethods + totalModuleInitMethods + 1,
                emitDebuggerDisplay: compileOptions.EmitPdb);

            var moduleList = modules._modules.Values.ToList();

            // Prototype-chain behavior is opt-in: only enable it when explicitly requested by options
            // or when the script clearly uses prototype-related features.
            compileOptions.PrototypeChainEnabled = compileOptions.PrototypeChain switch
            {
                PrototypeChainMode.On => true,
                PrototypeChainMode.Off => false,
                _ => moduleList.Any(m => PrototypeFeatureDetector.UsesPrototypeFeatures(m.Ast))
            };

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

                var effectiveNamespace = !string.IsNullOrWhiteSpace(module.ClrNamespace)
                    ? module.ClrNamespace!
                    : (module.IsPackageModule ? "Packages" : "Modules");
                var effectiveTypeName = !string.IsNullOrWhiteSpace(module.ClrTypeName)
                    ? module.ClrTypeName!
                    : module.Name;

                var moduleRootTypeBuilder = new TypeBuilder(_metadataBuilder, effectiveNamespace, effectiveTypeName);
                var moduleTypeHandle = moduleRootTypeBuilder.AddTypeDefinition(
                    TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
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

                if (!moduleTypeRegistry.TryGet(module.Name, out var moduleTypeHandle) || moduleTypeHandle.IsNil)
                {
                    throw new InvalidOperationException($"Missing module type handle for module '{module.Name}' during callable-owner predeclaration.");
                }

                // Shared registries for nesting and class lookup
                var classRegistry = _serviceProvider.GetRequiredService<ClassRegistry>();
                var nestedTypeRegistry = _serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>();

                // Declare callable-owner TypeDefs that scope TypeDefs may nest under.
                // NOTE: Do not declare class TypeDefs here; classes are declared by the planned two-phase
                // compilation path (MainGenerator -> TwoPhaseCompilationCoordinator). Duplicating class
                // declarations here corrupts TypeDef/MethodDef ordering and can cause CLR TypeLoadException.
                coordinator.DeclareFunctionAndAnonymousOwnerTypesForNesting(
                    symbolTable,
                    _metadataBuilder,
                    _bclReferences,
                    moduleTypeHandleForNesting: moduleTypeHandle,
                    nestedTypeRelationshipRegistry: nestedTypeRegistry,
                    declareFunctionDeclarationOwnerTypes: true,
                    declareAnonymousOwnerTypes: true);

                // Predeclare class TypeDefs now (idempotent) so scope TypeDefs emitted in Phase 2 can nest
                // under their correct owning class type. This is required for function-local classes when
                // scopes are NestedPrivate.
                var classesGenerator = new ClassesGenerator(
                    _serviceProvider,
                    _metadataBuilder,
                    _bclReferences,
                    classRegistry,
                    nestedTypeRegistry,
                    module.Name);
                classesGenerator.DeclareClasses(symbolTable);

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

            // Emit strongly-typed hosting contracts for module.exports (interfaces) if enabled.
            var options = _serviceProvider.GetRequiredService<CompilerOptions>();
            if (options.GenerateModuleExportContracts)
            {
                new ModuleExportsContractEmitter(_metadataBuilder, _bclReferences)
                    .Emit(modules, assemblyName);
            }

            // Emit NestedClass table rows in one globally sorted pass.
            // This must happen after all TypeDefs have been created.
            _serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>().EmitAllSorted(_metadataBuilder);

            this.CreateAssembly(assemblyName, outputPath);
        }

        private void EmitDebuggableAttributeIfEnabled()
        {
            var options = _serviceProvider.GetRequiredService<CompilerOptions>();
            if (!options.EmitPdb)
            {
                return;
            }

            // This attribute improves debugging/stepping fidelity (notably in VS Code) by
            // reducing JIT optimizations/inlining when debugging generated assemblies.
            if (_assemblyDefinition.IsNil)
            {
                throw new InvalidOperationException("Assembly definition handle not initialized.");
            }

            var ctorRef = _bclReferences.DebuggableAttribute_Ctor_Ref;
            // ECMA-335 CustomAttribute blob format:
            // - prolog: 0x0001 (UInt16)
            // - fixed args: bool, bool
            // - named args count: UInt16 (0)
            var blob = new BlobBuilder();
            blob.WriteUInt16(0x0001);
            // DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
            blob.WriteByte(1); // true
            blob.WriteByte(1); // true
            blob.WriteUInt16(0);

            _metadataBuilder.AddCustomAttribute(
                parent: _assemblyDefinition,
                constructor: ctorRef,
                value: _metadataBuilder.GetOrAddBlob(blob));
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

            var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
            if (!string.IsNullOrWhiteSpace(lastFailure))
            {
                throw new NotSupportedException(
                    $"IR pipeline could not compile module '{moduleName}' main method.\nIR failure: {lastFailure}");
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
            _assemblyDefinition = this._metadataBuilder.AddAssembly(
                name: assemblyName,
                version: new Version(1, 0, 0, 0),
                culture: culture,
                publicKey: default,
                flags: 0,
                hashAlgorithm: AssemblyHashAlgorithm.None
            );

            _metadataBuilder.AddModule(0, assemblyName, _metadataBuilder.GetOrAddGuid(Guid.NewGuid()), default, default);
        }

        private void EmitCompiledModuleManifest(Modules modules)
        {
            // Emit a simple assembly-level manifest describing module ids.
            // This preserves human-friendly ids like "calculator/index" which cannot be recovered from
            // the sanitized CLR type name (e.g. Modules.calculator_index).
            if (_assemblyDefinition.IsNil)
            {
                throw new InvalidOperationException("Assembly definition handle not initialized.");
            }

            var ctorRef = _bclReferences.JsCompiledModuleAttribute_Ctor_Ref;
            var typeMapCtorRef = _bclReferences.JsCompiledModuleTypeAttribute_Ctor_Ref;

            // Root module path is used as the base for stable relative module ids.
            var rootModulePath = modules.rootModule.Path;

            foreach (var module in modules._modules.Values)
            {
                var canonicalModuleId = module.ModuleId;

                // Host-facing module-id discovery should include both the canonical id and any aliases.
                var idsToPublish = new List<string> { canonicalModuleId };
                foreach (var alias in module.AliasModuleIds)
                {
                    if (!idsToPublish.Contains(alias, StringComparer.OrdinalIgnoreCase))
                    {
                        idsToPublish.Add(alias);
                    }
                }

                var effectiveNamespace = !string.IsNullOrWhiteSpace(module.ClrNamespace)
                    ? module.ClrNamespace!
                    : (module.IsPackageModule ? "Packages" : "Modules");
                var effectiveTypeName = !string.IsNullOrWhiteSpace(module.ClrTypeName)
                    ? module.ClrTypeName!
                    : module.Name;
                var typeName = $"{effectiveNamespace}.{effectiveTypeName}";

                foreach (var publishedId in idsToPublish)
                {
                    var valueBlob = CreateSingleStringCustomAttributeValue(publishedId);

                    _metadataBuilder.AddCustomAttribute(
                        parent: _assemblyDefinition,
                        constructor: ctorRef,
                        value: valueBlob);

                    // Emit moduleId -> (canonicalModuleId, typeName)
                    var mapBlob = CreateThreeStringCustomAttributeValue(publishedId, canonicalModuleId, typeName);

                    _metadataBuilder.AddCustomAttribute(
                        parent: _assemblyDefinition,
                        constructor: typeMapCtorRef,
                        value: mapBlob);
                }
            }
        }

        private BlobHandle CreateThreeStringCustomAttributeValue(string value1, string value2, string value3)
        {
            // ECMA-335 CustomAttribute blob format:
            // - prolog: 0x0001 (UInt16)
            // - fixed args: SerString, SerString, SerString
            // - named args count: UInt16 (0)
            var blob = new BlobBuilder();
            blob.WriteUInt16(0x0001);
            WriteSerString(blob, value1);
            WriteSerString(blob, value2);
            WriteSerString(blob, value3);
            blob.WriteUInt16(0);
            return _metadataBuilder.GetOrAddBlob(blob);
        }

        private BlobHandle CreateSingleStringCustomAttributeValue(string value)
        {
            // ECMA-335 CustomAttribute blob format:
            // - prolog: 0x0001 (UInt16)
            // - fixed args: SerString
            // - named args count: UInt16 (0)
            var blob = new BlobBuilder();
            blob.WriteUInt16(0x0001);
            WriteSerString(blob, value);
            blob.WriteUInt16(0);
            return _metadataBuilder.GetOrAddBlob(blob);
        }

        private static void WriteSerString(BlobBuilder blob, string value)
        {
            // SerString: (null -> 0xFF) else: compressed length + UTF8 bytes
            // For our manifest, value is always non-null.
            var utf8 = Encoding.UTF8.GetBytes(value);
            WriteCompressedUInt32(blob, (uint)utf8.Length);
            blob.WriteBytes(utf8);
        }

        private static void WriteCompressedUInt32(BlobBuilder blob, uint value)
        {
            // ECMA-335 II.23.2 Blobs and signatures (compressed unsigned integer)
            if (value <= 0x7Fu)
            {
                blob.WriteByte((byte)value);
                return;
            }

            if (value <= 0x3FFFu)
            {
                blob.WriteByte((byte)((value >> 8) | 0x80u));
                blob.WriteByte((byte)(value & 0xFFu));
                return;
            }

            if (value <= 0x1FFFFFFFu)
            {
                blob.WriteByte((byte)((value >> 24) | 0xC0u));
                blob.WriteByte((byte)((value >> 16) & 0xFFu));
                blob.WriteByte((byte)((value >> 8) & 0xFFu));
                blob.WriteByte((byte)(value & 0xFFu));
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), "Value too large for compressed integer encoding.");
        }

        private void CreateAssembly(string name, string outputPath)
        {
            var options = _serviceProvider.GetRequiredService<CompilerOptions>();

            DebugDirectoryBuilder? debugDirectoryBuilder = null;
            if (options.EmitPdb)
            {
                var pdbPath = Path.Combine(outputPath, $"{name}.pdb");

                var debugRegistry = _serviceProvider.GetRequiredService<DebugSymbolRegistry>();
                var (pdbContentId, portablePdbVersion) = PortablePdbEmitter.Emit(_metadataBuilder, debugRegistry, pdbPath, this._entryPoint);

                debugDirectoryBuilder = new DebugDirectoryBuilder();
                debugDirectoryBuilder.AddCodeViewEntry(Path.GetFileName(pdbPath), pdbContentId, portablePdbVersion);
            }

            var pe = new ManagedPEBuilder(
                PEHeaderBuilder.CreateLibraryHeader(),
                new MetadataRootBuilder(_metadataBuilder),
                _ilBuilder,
                mappedFieldData: null,
                entryPoint: this._entryPoint,
                flags: CorFlags.ILOnly,
                debugDirectoryBuilder: debugDirectoryBuilder);

            var peImage = new BlobBuilder();
            pe.Serialize(peImage);

            string assemblyDll = Path.Combine(outputPath, $"{name}.dll");

            // In test runs (and on some machines with aggressive file scanning), the just-produced
            // assembly can briefly be locked by another process. Retry a few times to avoid flaky
            // failures while still surfacing a persistent lock.
            var peBytes = peImage.ToArray();

            // Fail-fast: validate metadata invariants that would otherwise surface later as
            // BadImageFormatException during Assembly.Load.
            ClrMetadataConsistencyValidator.ValidateOrThrow(peBytes, label: name);
            // Windows can keep the output DLL briefly locked (AV/indexing/build hosts). Writing the final
            // file repeatedly can prolong the lock (each write can re-trigger scanning), so we write to a
            // unique temp file first and then retry only the final replace.
            string tempDll = assemblyDll + ".tmp_" + Guid.NewGuid().ToString("N");
            File.WriteAllBytes(tempDll, peBytes);

            try
            {
                const int maxReplaceWaitMs = 60_000;
                long startTick = Environment.TickCount64;
                int attempt = 0;
                while (true)
                {
                    attempt++;
                    try
                    {
                        File.Move(tempDll, assemblyDll, overwrite: true);
                        break;
                    }
                    catch (IOException) when ((Environment.TickCount64 - startTick) < maxReplaceWaitMs)
                    {
                        int delayMs = Math.Min(1000, 50 * attempt);
                        Thread.Sleep(delayMs);
                    }
                    catch (UnauthorizedAccessException) when ((Environment.TickCount64 - startTick) < maxReplaceWaitMs)
                    {
                        int delayMs = Math.Min(1000, 50 * attempt);
                        Thread.Sleep(delayMs);
                    }
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(tempDll)) File.Delete(tempDll);
                }
                catch (IOException)
                {
                    // Best-effort cleanup; temp files are safe to leave behind.
                }
            }

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
