using System.Diagnostics;
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

        private  VariableBindings.VariableRegistry? _variableRegistry;

        private TypeReferenceRegistry _typeReferenceRegistry;

        private IServiceProvider _serviceProvider;

        public AssemblyGenerator(IServiceProvider serviceProvider, MetadataBuilder metadataBuilder, TypeReferenceRegistry typeReferenceRegistry)
        {
            this._metadataBuilder = metadataBuilder;
            this._typeReferenceRegistry = typeReferenceRegistry;
            this._bclReferences = serviceProvider.GetRequiredService<BaseClassLibraryReferences>();
            this._serviceProvider = serviceProvider;
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

            // Generate .NET types from the scope tree
            this._variableRegistry = this.GenerateScopeTypes(modules, methodBodyStream);

            // Compile the main script method
            this.GenerateModules(modules, methodBodyStream);

            // create the entry point for spining up the execution engine
            createEntryPoint(methodBodyStream);

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
        private VariableBindings.VariableRegistry GenerateScopeTypes(Modules modules, MethodBodyStreamEncoder methodBodyStream)
        {
            var typeGenerator = new TypeGenerator(_metadataBuilder, _bclReferences, methodBodyStream);

            // Multi-module compilation: every module gets its own global scope type and registry entries.
            // This avoids missing bindings for non-root modules and prevents collisions when different modules
            // declare the same function/class names.
            foreach (var module in modules._modules.Values)
            {
                typeGenerator.GenerateTypes(module.SymbolTable!);
            }

            return typeGenerator.GetVariableRegistry();
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
            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
            // new path which does proper IR/SSA lowering and IL generation
            var methodDefinitionHandle = methodCompiler.TryCompileMainMethod(module.Name, module.Ast, module.SymbolTable!.Root!, methodBodyStream);
            IR.IRPipelineMetrics.RecordMainMethodAttempt(!methodDefinitionHandle.IsNil);
            if (!methodDefinitionHandle.IsNil)
            {
                return methodDefinitionHandle;
            }

            // fallback to the old path.. eventually we will delete this code
            
            // Get parameter info from shared ModuleParameters
            var paramCount = JavaScriptRuntime.CommonJS.ModuleParameters.Count;
            var parameterNames = JavaScriptRuntime.CommonJS.ModuleParameters.ParameterNames;

            // create the tools we need to generate the module type and method
            var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Scripts", moduleName);
            var variables = new Variables(_variableRegistry!, moduleName, parameterNames);
            var mainGenerator = new MainGenerator(_serviceProvider, variables, _bclReferences, _metadataBuilder, methodBodyStream, module.SymbolTable!);

            // Create the method signature for the Main method with parameters
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Void(), parameters =>
                {
                    for (int i = 0; i < paramCount; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                parameters.AddParameter().Type().Object();
                                break;
                            case 1:
                                var requireDelegateReference = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.CommonJS.RequireDelegate)); 
                                parameters.AddParameter().Type().Type(requireDelegateReference, false);
                                break;
                            case 2:
                                parameters.AddParameter().Type().Object();
                                break;
                            case 3:
                            case 4:
                                parameters.AddParameter().Type().String();
                                break;
                        }
                    }
                });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);
            var bodyOffset = mainGenerator.GenerateMethod(module.Ast);

            methodDefinitionHandle = programTypeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                "Main",
                methodSig,
                bodyOffset);

            // Add parameter names to metadata (sequence starts at 1 for first parameter)
            int sequence = 1;
            foreach (var paramName in parameterNames)
            {
                _metadataBuilder.AddParameter(
                    ParameterAttributes.None,
                    _metadataBuilder.GetOrAddString(paramName),
                    sequence++);
            }

            // Define the Script main type via TypeBuilder
            var programTypeDef = programTypeBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);

            return methodDefinitionHandle;
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

            // create a method generator to emit IL for the entry point
            // variables is unused
            var variables = new Variables(_variableRegistry!, "EntryPoint");
            var entryPointGenerator = new ILMethodGenerator(_serviceProvider, variables, _bclReferences, _metadataBuilder, methodBodyStream, new ClassRegistry(), new FunctionRegistry());
            var ilEncoder = entryPointGenerator.IL;
            var runtime = entryPointGenerator.Runtime;


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
            var engineExecuteRef = entryPointGenerator.Runtime.GetInstanceMethodRef(
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
            var publicKey = _metadataBuilder.GetOrAddBlob(StandardPublicKey);
            this._metadataBuilder.AddAssembly(
                name: assemblyName,
                version: new Version(1, 0, 0, 0),
                culture: culture,
                publicKey: publicKey,
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
            // only if its not already there
            var jsRuntimeDll = typeof(JavaScriptRuntime.Object).Assembly.Location!;
            var jsRuntimeAssemblyFileName = Path.GetFileName(jsRuntimeDll);
            var jsRuntimeDllDest = Path.Combine(outputPath, jsRuntimeAssemblyFileName);
            if (File.Exists(jsRuntimeDll))
            {
                var sourceVersion = FileVersionInfo.GetVersionInfo(jsRuntimeDll).FileVersion;

                if (File.Exists(jsRuntimeDllDest))
                {
                    var targetVersion = FileVersionInfo.GetVersionInfo(jsRuntimeDllDest).FileVersion;
                    if (sourceVersion == targetVersion)
                    {
                        // same version, no need to copy
                        return;
                    }
                }

                File.Copy(jsRuntimeDll, jsRuntimeDllDest, true);
                var jsRuntimePdb = Path.ChangeExtension(jsRuntimeDll, ".pdb");
                var jsRuntimePdbDest = Path.ChangeExtension(jsRuntimeDllDest, ".pdb");
                if (File.Exists(jsRuntimePdb))
                {
                    File.Copy(jsRuntimePdb, jsRuntimePdbDest, true);
                }
            }
        }
    }
}
