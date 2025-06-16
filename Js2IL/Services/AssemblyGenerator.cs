using PowerArgs.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Js2IL.Services.ILGenerators;

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

        private Variables _variables = new Variables();

        public AssemblyGenerator()
        {
            // get the version and public key toketn for the System.Runtime assembly reference
            // we use the same that this assembly is compiled against for consistency
            if (!ReferenceAssemblyResolver.TryFindSystemRuntime(out this._systemRuntimeAssembly))
            {
                throw new InvalidOperationException("Could not find System.Runtime assembly reference.");
            }

            this._bclReferences = new BaseClassLibraryReferences(_metadataBuilder, _systemRuntimeAssembly.Version!, _systemRuntimeAssembly.GetPublicKeyToken()!);
        }

        public void Generate(Acornima.Ast.Program ast, string name, string outputPath)
        {
            createAssemblyMetadata(name);

            // Method signature for Main method
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);

            // IL: return
            var methodBodyStream = new MethodBodyStreamEncoder(this._ilBuilder);
            var bodyOffset = MainGenerator.GenerateMethod(ast, _metadataBuilder, methodBodyStream, _bclReferences, _variables);

            this._entryPoint = _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString("Main"),
                methodSig,
                bodyOffset,
                parameterList: default);

            this.CreateAssembly();
        }

        private void createAssemblyMetadata(string name)
        {
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

            // Program type definition
            //var appNamespace = assemblyName;
            var programTypeDef = _metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public,
                _metadataBuilder.GetOrAddString(""),
                _metadataBuilder.GetOrAddString("Program"),
                _bclReferences.ObjectType,
                MetadataTokens.FieldDefinitionHandle(1),
                MetadataTokens.MethodDefinitionHandle(1)
            );
        }

        private void CreateAssembly()
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

            var dllPath = "c:\\git\\test.dll";
            File.WriteAllBytes(dllPath, peImage.ToArray());

            RuntimeConfigWriter.WriteRuntimeConfigJson(dllPath, _systemRuntimeAssembly);
        }
    }
}
