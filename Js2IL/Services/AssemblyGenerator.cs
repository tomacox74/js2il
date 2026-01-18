using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Js2IL.Services.ILGenerators;
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

            // Generate .NET types from the scope tree (populates the injected VariableRegistry)
            this.GenerateScopeTypes(modules, methodBodyStream);

            // Compile the main script method
            this.GenerateModules(modules, methodBodyStream);

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
        private void GenerateScopeTypes(Modules modules, MethodBodyStreamEncoder methodBodyStream)
        {
            var typeGenerator = new TypeGenerator(_metadataBuilder, _bclReferences, methodBodyStream, _variableRegistry);

            // Multi-module compilation: every module gets its own global scope type and registry entries.
            // This avoids missing bindings for non-root modules and prevents collisions when different modules
            // declare the same function/class names.
            foreach (var module in modules._modules.Values)
            {
                typeGenerator.GenerateTypes(module.SymbolTable!);
            }
        }

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

            // Declare classes/functions before compiling the module body so call sites can do token lookup (ldftn).
            var mainGenerator = new MainGenerator(_serviceProvider, moduleName, _bclReferences, _metadataBuilder, methodBodyStream, module.SymbolTable!);
            
            // Declare functions and classes first - this populates CallableRegistry with tokens
            // which is needed for IR pipeline to emit function calls (ldftn)
            mainGenerator.DeclareClassesAndFunctions(module.SymbolTable!);

            // Now try IR pipeline for the main method body
            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
            var methodDefinitionHandle = methodCompiler.TryCompileMainMethod(module.Name, module.Ast, module.SymbolTable!.Root!, methodBodyStream);
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
