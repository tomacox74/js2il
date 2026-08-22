namespace Jroc;

public static class JrocAssemblyIdentity
{
    public static string Resolve(string entryFilePath, string? configuredAssemblyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryFilePath);

        var assemblyName = configuredAssemblyName is null
            ? Path.GetFileNameWithoutExtension(entryFilePath)
            : configuredAssemblyName;

        Validate(assemblyName);
        return assemblyName;
    }

    public static void Validate(string assemblyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);

        var invalidPortableFileNameCharacters = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        if (assemblyName is "." or ".."
            || assemblyName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || assemblyName.IndexOfAny(invalidPortableFileNameCharacters) >= 0
            || assemblyName.Any(char.IsControl)
            || assemblyName.StartsWith(' ')
            || assemblyName.EndsWith(' ')
            || assemblyName.EndsWith('.'))
        {
            throw new ArgumentException(
                $"Assembly name '{assemblyName}' is not a valid portable assembly and artifact name.",
                nameof(assemblyName));
        }

        try
        {
            var parsedName = new System.Reflection.AssemblyName(assemblyName).Name;
            if (!string.Equals(parsedName, assemblyName, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"Assembly name '{assemblyName}' must be a simple assembly name without version, culture, or public-key components.",
                    nameof(assemblyName));
            }
        }
        catch (FileLoadException ex)
        {
            throw new ArgumentException(
                $"Assembly name '{assemblyName}' is not a valid CLR assembly identity.",
                nameof(assemblyName),
                ex);
        }
    }
}
