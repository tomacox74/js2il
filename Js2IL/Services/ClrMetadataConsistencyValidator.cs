using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;

namespace Js2IL.Services;

internal static class ClrMetadataConsistencyValidator
{
    public static void ValidateOrThrow(byte[] peBytes, string? label = null)
    {
        if (peBytes == null) throw new ArgumentNullException(nameof(peBytes));

        try
        {
            using var stream = new MemoryStream(peBytes, writable: false);
            using var peReader = new PEReader(stream);

            if (!peReader.HasMetadata)
            {
                throw new InvalidOperationException("PE does not contain metadata.");
            }

            var mdReader = peReader.GetMetadataReader();

            // Touch metadata so obvious corruption is caught early.
            _ = mdReader.TypeDefinitions.Count;

            // Strongest practical check: ensure CoreCLR can load the assembly.
            // This catches nested-type ordering issues (and other metadata invariants)
            // that would surface as BadImageFormatException later at runtime.
            ValidateLoadableByClrOrThrow(peBytes);
        }
        catch (Exception ex) when (ex is BadImageFormatException or InvalidOperationException or IOException)
        {
            var prefix = string.IsNullOrWhiteSpace(label) ? string.Empty : $"[{label}] ";
            throw new InvalidOperationException(prefix + "CLR metadata consistency validation failed: " + ex.Message, ex);
        }
    }

    private static void ValidateLoadableByClrOrThrow(byte[] peBytes)
    {
        var alc = new AssemblyLoadContext("Js2IL_MetadataValidation", isCollectible: true);
        try
        {
            using var ms = new MemoryStream(peBytes, writable: false);
            var asm = alc.LoadFromStream(ms);

            // Force type resolution. Some loader errors only surface when the runtime
            // walks the TypeDef table (e.g., invalid nesting layouts).
            _ = asm.GetTypes();
        }
        finally
        {
            alc.Unload();
        }
    }
}
