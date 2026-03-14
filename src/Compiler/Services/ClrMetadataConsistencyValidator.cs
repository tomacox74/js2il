using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
        catch (Exception ex) when (ex is BadImageFormatException or InvalidOperationException or IOException or ReflectionTypeLoadException)
        {
            var prefix = string.IsNullOrWhiteSpace(label) ? string.Empty : $"[{label}] ";
            var message = ex is ReflectionTypeLoadException reflectionTypeLoadException
                ? FormatTypeLoadFailureMessage(reflectionTypeLoadException)
                : ex.Message;
            throw new InvalidOperationException(prefix + "CLR metadata consistency validation failed: " + message, ex);
        }
    }

    internal static string FormatTypeLoadFailureMessage(ReflectionTypeLoadException ex)
    {
        if (ex.LoaderExceptions is not { Length: > 0 })
        {
            return ex.Message;
        }

        var loaderMessages = ex.LoaderExceptions
            .Where(static loaderException => loaderException is not null)
            .Select(static loaderException => loaderException!.Message)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return loaderMessages.Length == 0
            ? ex.Message
            : ex.Message + " Loader exceptions: " + string.Join(" | ", loaderMessages);
    }

    private static void ValidateLoadableByClrOrThrow(byte[] peBytes)
    {
        var runtimeAssembly = typeof(JavaScriptRuntime.Object).Assembly;
        var runtimeAssemblyPath = runtimeAssembly.Location;
        var runtimeAssemblyName = runtimeAssembly.GetName().Name;
        var alc = new AssemblyLoadContext("Js2IL_MetadataValidation", isCollectible: true);

        Assembly? ResolveValidationDependency(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            if (!string.Equals(assemblyName.Name, runtimeAssemblyName, StringComparison.Ordinal)
                || string.IsNullOrWhiteSpace(runtimeAssemblyPath)
                || !File.Exists(runtimeAssemblyPath))
            {
                return null;
            }

            return context.LoadFromAssemblyPath(runtimeAssemblyPath);
        }

        alc.Resolving += ResolveValidationDependency;
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
            alc.Resolving -= ResolveValidationDependency;
            alc.Unload();
        }
    }
}
