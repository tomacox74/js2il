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
using static System.Net.Mime.MediaTypeNames;

namespace Js2IL.Services
{
    public class AssemblyGenerator : IGenerator
    {
        // Standard public key as defined in ECMA-335 for reference assemblies
        private static readonly byte[] StandardPublicKey = new byte[] {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public MetadataBuilder metadataBuilder = new MetadataBuilder();

        public void Generate(Acornima.Ast.Program ast, string name, string outputPath)
        {
            createAssemblyMetadata(name);
        }

        private void createAssemblyMetadata(string name)
        {
            var assemblyName = metadataBuilder.GetOrAddString(name);
            var culture = metadataBuilder.GetOrAddString("");
            var publicKey = metadataBuilder.GetOrAddBlob(StandardPublicKey);
            this.metadataBuilder.AddAssembly(
                name: assemblyName,
                version: new Version(1, 0, 0, 0),
                culture: culture,
                publicKey: publicKey,
                flags: AssemblyFlags.PublicKey,
                hashAlgorithm: AssemblyHashAlgorithm.None
            );

            metadataBuilder.AddModule(0, assemblyName, metadataBuilder.GetOrAddGuid(Guid.NewGuid()), default, default);

            var mscorlibAssemblyRef = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Runtime"),
                version: new Version(9, 0, 0, 0),
                culture: default,
                publicKeyOrToken: default,
                flags: 0,
                hashValue: default
            );

            // Program type definition
            var appNamespace = assemblyName;
            var programTypeDef = metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public,
                appNamespace,
                metadataBuilder.GetOrAddString("Program"),
                MetadataTokens.TypeDefinitionHandle(1),
                MetadataTokens.FieldDefinitionHandle(1),
                MetadataTokens.MethodDefinitionHandle(1)
            );

            // Method signature for Main method
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var methodSig = metadataBuilder.GetOrAddBlob(sigBuilder);

            // IL: return
            var ilBuilder = new BlobBuilder();
            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);
            var methodIl = new BlobBuilder();
            var il = new InstructionEncoder(methodIl);
            il.OpCode(ILOpCode.Ret);
            var bodyOffset = methodBodyStream.AddMethodBody(il);

            var methodDef = metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                metadataBuilder.GetOrAddString("Main"),
                methodSig,
                bodyOffset,
                parameterList: default);

            var pe = new ManagedPEBuilder(
                PEHeaderBuilder.CreateExecutableHeader(),
                new MetadataRootBuilder(metadataBuilder),
                ilBuilder,
                mappedFieldData: null,
                entryPoint: methodDef,
                flags: CorFlags.ILOnly);

            var peImage = new BlobBuilder();
            pe.Serialize(peImage);

            var dllPath = "c:\\git\\test.exe";
            File.WriteAllBytes(dllPath, peImage.ToArray());
        }
    }
}
