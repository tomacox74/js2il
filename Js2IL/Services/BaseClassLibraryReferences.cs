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
            this.SystemRuntime = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Runtime"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            this.SystemConsole = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString("System.Console"),
                version: bclVersion,
                culture: default,
                publicKeyOrToken: publicKeyTokenHandle,
                flags: 0,
                hashValue: default
            );

            // Common Type References
            this.ObjectType = metadataBuilder.AddTypeReference(
                this.SystemRuntime,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Object")
            );

            this.Int32Type = metadataBuilder.AddTypeReference(
                this.SystemRuntime,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Int32")
            );

            this.DoubleType = metadataBuilder.AddTypeReference(
                this.SystemRuntime,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("Double")
            );

            this.StringType = metadataBuilder.AddTypeReference(
                this.SystemRuntime,
                metadataBuilder.GetOrAddString("System"),
                metadataBuilder.GetOrAddString("String")
            );
        }

        public AssemblyReferenceHandle SystemRuntime { get; private set; }
        public AssemblyReferenceHandle SystemConsole { get; private set; }
        public TypeReferenceHandle ObjectType { get; private set; }
        public TypeReferenceHandle Int32Type { get; private set; }
        public TypeReferenceHandle DoubleType { get; private set; }
        public TypeReferenceHandle StringType { get; private set; }
    }
}
