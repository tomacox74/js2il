using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    public class AssemblyGenerator : IGenerator
    {
        // Standard public key as defined in ECMA-335 for reference assemblies
        private static readonly byte[] StandardPublicKey = new byte[] {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public MetadataBuilder _metadataBuilder = new MetadataBuilder();
        private AssemblyName _systemRuntimeAssembly;
        private BlobBuilder _ilBuilder = new BlobBuilder();
        private MethodDefinitionHandle _entryPoint;

        private MethodDefinitionHandle _mainScriptMethod;
        private BaseClassLibraryReferences _bclReferences;

        private Variables? _variables;

        public AssemblyGenerator()
        {
            // Get the version and public key token for the System.Runtime assembly reference.
            // We use the same one that this assembly is compiled against for consistency.
            if (!ReferenceAssemblyResolver.TryFindSystemRuntime(out this._systemRuntimeAssembly))
            {
                throw new InvalidOperationException("Could not find System.Runtime assembly reference.");
            }

            this._bclReferences = new BaseClassLibraryReferences(_metadataBuilder, _systemRuntimeAssembly.Version!, _systemRuntimeAssembly.GetPublicKeyToken()!);
        }

        /// <summary>
        /// Generates a new assembly from the provided AST.
        /// </summary>
        /// <param name="ast">The JavaScript AST.</param>
        /// <param name="name">The assembly name.</param>
        /// <param name="outputPath">The directory to output the generated assembly and related files to.</param>
        public void Generate(Acornima.Ast.Program ast, string name, string outputPath)
        {
            // Build scope tree first
            var symbolTableBuilder = new SymbolTableBuilder();
            var symbolTable = symbolTableBuilder.Build(ast, $"{name}.js");
            
            // Call the new overload
            Generate(ast, symbolTable, name, outputPath);
        }

        /// <summary>
        /// Generates a new assembly from the provided AST and scope tree.
        /// </summary>
        /// <param name="ast">The JavaScript AST.</param>
        /// <param name="scopeTree">The pre-built scope tree.</param>
        /// <param name="name">The assembly name.</param>
        /// <param name="outputPath">The directory to output the generated assembly and related files to.</param>
        public void Generate(Acornima.Ast.Program ast, SymbolTable symbolTable, string name, string outputPath)
        {
            createAssemblyMetadata(name);

            // Add the <Module> type first (as required by .NET metadata) using TypeBuilder
            var moduleTypeBuilder = new TypeBuilder(_metadataBuilder, "", "<Module>");
            moduleTypeBuilder.AddTypeDefinition(
                TypeAttributes.NotPublic,
                baseType: default(EntityHandle));

            // the API for generating IL is a little confusing
            // there is 1 MethodBodyStreamEncoder for all methods in the assembly
            var methodBodyStream = new MethodBodyStreamEncoder(this._ilBuilder);

            // Step 1: Generate .NET types from the scope tree
            var typeGenerator = new TypeGenerator(_metadataBuilder, _bclReferences, methodBodyStream);
            var rootTypeHandle = typeGenerator.GenerateTypes(symbolTable);

            // Step 2: Get the variable registry from the type generator and update Variables
            var variableRegistry = typeGenerator.GetVariableRegistry();
            _variables = new Variables(variableRegistry, symbolTable.Root.Name);

            // Previous design created a dispatch table for indirection and circular refs.
            // New design: functions are referenced directly by static method handles stored in scope fields.
            // Circular references are handled naturally because we assign delegates after all top-level
            // methods are defined (during Main method body) or within function bodies for nested cases.

            // Create the method signature for the Main method.
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);


            // Emit IL: return.
            // Prepare a TypeBuilder for the main script and pass it to MainGenerator
            var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Scripts", name);
            var mainGenerator = new MainGenerator(_variables!, _bclReferences, _metadataBuilder, methodBodyStream, symbolTable, programTypeBuilder);
            var bodyOffset = mainGenerator.GenerateMethod(ast);
            this._mainScriptMethod = programTypeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                "Main",
                methodSig,
                bodyOffset);

            // Define the Script main type via TypeBuilder
            var programTypeDef = programTypeBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);

            // create the entry point for spining up the execution engine
            createEntryPoint(methodBodyStream);

            this.CreateAssembly(name, outputPath);
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

            // create a method generator to emit IL
            // method generator is specifically for translating JavaScript constructs to IL so its a little overkill for the entry point
            var entryPointGenerator = new ILMethodGenerator(_variables!, _bclReferences, _metadataBuilder, methodBodyStream, new ClassRegistry(), new FunctionRegistry());
            var ilEncoder = entryPointGenerator.IL;
            var runtime = entryPointGenerator.Runtime;


            // emit IL for the entry point method

            // first create new instance of the engine
            runtime.InvokeEngineCtor();

            // first create a new action delegate, no return value, no parameters
            ilEncoder.OpCode(ILOpCode.Ldnull);
            ilEncoder.OpCode(ILOpCode.Ldftn);
            ilEncoder.Token(this._mainScriptMethod);
            ilEncoder.OpCode(ILOpCode.Newobj);
            ilEncoder.Token(_bclReferences.Action_Ctor_Ref);

            ilEncoder.OpCode(ILOpCode.Callvirt);
            var engineExecuteRef = entryPointGenerator.Runtime.GetInstanceMethodRef(
                typeof(JavaScriptRuntime.Engine),
                "Execute",
                typeof(void),
                typeof(Action));
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

            RuntimeConfigWriter.WriteRuntimeConfigJson(assemblyDll, _systemRuntimeAssembly);

            // Copy JavaScriptRuntime.dll instead of js2il.dll.
            var jsRuntimeDll = typeof(JavaScriptRuntime.Object).Assembly.Location!;
            var jsRuntimeAssemblyFileName = Path.GetFileName(jsRuntimeDll);
            var jsRuntimeDllDest = Path.Combine(outputPath, jsRuntimeAssemblyFileName);
            if (File.Exists(jsRuntimeDll))
            {
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
