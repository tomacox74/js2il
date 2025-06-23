using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    internal class BaseClassLibraryReferences
    {
        public BaseClassLibraryReferences(MetadataBuilder metadataBuilder, Version bclVersion, byte[] publicKeyToken)
        {
            // Assembly References
            var publicKeyTokenHandle = metadataBuilder.GetOrAddBlob(publicKeyToken);
            this.SystemRuntimeAssembly = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Runtime"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            this.SystemConsoleAssembly = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Console"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            // Common Runtime References
            this.BooleanType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Boolean")
            );

            this.DoubleType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Double")
            );

            this.ObjectType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Object")
            );

            this.StringType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("String")
            );

            // System.Math References
            this.SystemMathType = metadataBuilder.AddTypeReference(
                this.SystemRuntimeAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Math"));

            // System.Console References
            var systemConsoleTypeReference = metadataBuilder.AddTypeReference(
                this.SystemConsoleAssembly,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Console"));

            // Create method signature: void WriteLine(string)
            var consoleSig = new BlobBuilder();
            new BlobEncoder(consoleSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });
            var writeLineSig = metadataBuilder.GetOrAddBlob(consoleSig);

            this.ConsoleWriteLine_StringObject_Ref = metadataBuilder.AddMemberReference(
                systemConsoleTypeReference,
                metadataBuilder.GetOrAddString("WriteLine"),
                writeLineSig);
        }

        public AssemblyReferenceHandle SystemRuntimeAssembly { get; private set; }
        public AssemblyReferenceHandle SystemConsoleAssembly { get; private set; }
        public TypeReferenceHandle BooleanType { get; private set; }
        public TypeReferenceHandle DoubleType { get; private set; }
        public TypeReferenceHandle ObjectType { get; private set; }
        public TypeReferenceHandle StringType { get; private set; }
        public TypeReferenceHandle SystemMathType { get; private set; }

        public MemberReferenceHandle ConsoleWriteLine_StringObject_Ref { get; private set; }
    }
}
