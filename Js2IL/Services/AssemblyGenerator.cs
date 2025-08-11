using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;

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

            // Add the <Module> type first (as required by .NET metadata)
            _metadataBuilder.AddTypeDefinition(
                TypeAttributes.NotPublic,                          // Access flags
                default(StringHandle),                             // No namespace
                _metadataBuilder.GetOrAddString("<Module>"),       // Name
                baseType: default(EntityHandle),                   // No base type
                fieldList: MetadataTokens.FieldDefinitionHandle(1), // First field
                methodList: MetadataTokens.MethodDefinitionHandle(1) // First method
            );

            // the API for generating IL is a little confusing
            // there is 1 MethodBodyStreamEncoder for all methods in the assembly
            var methodBodyStream = new MethodBodyStreamEncoder(this._ilBuilder);

            // Step 1: Generate .NET types from the scope tree
            var typeGenerator = new TypeGenerator(_metadataBuilder, _bclReferences, methodBodyStream);
            var rootTypeHandle = typeGenerator.GenerateTypes(symbolTable);

            // Step 2: Get the variable registry from the type generator and update Variables
            var variableRegistry = typeGenerator.GetVariableRegistry();
            _variables = new Variables(variableRegistry, symbolTable.Root.Name);

            // Create the dispatch table.
            // The dispatch table exists for two reasons:
            // 1. It is necessary because JavaScript allows for circular references.
            // 2. It allows you to dynamically change function implementations at runtime.
            var dispatchTableGenerator = new Dispatch.DispatchTableGenerator(_metadataBuilder, _bclReferences, methodBodyStream);
            dispatchTableGenerator.GenerateDispatchTable(symbolTable);

            // Create the method signature for the Main method.
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);


            // Emit IL: return.
            var mainGenerator = new MainGenerator(_variables!, _bclReferences, _metadataBuilder, methodBodyStream, dispatchTableGenerator, symbolTable);
            var bodyOffset = mainGenerator.GenerateMethod(ast);
            var parameterList = MetadataTokens.ParameterHandle(_metadataBuilder.GetRowCount(TableIndex.Param) + 1);
            this._entryPoint = _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString("Main"),
                methodSig,
                bodyOffset,
                parameterList: parameterList);

            var nextField = MetadataTokens.FieldDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.Field) + 1);

            // Program type should own only Main; other methods belong to their respective owner types
            var firstMethod = this._entryPoint;
        
            // Define the Program type.
            // var appNamespace = assemblyName;
            var programTypeDef = _metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                _metadataBuilder.GetOrAddString(""),
                _metadataBuilder.GetOrAddString("Program"),
                _bclReferences.ObjectType,
                nextField,
                firstMethod
            );

            this.CreateAssembly(name, outputPath);
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
