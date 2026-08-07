namespace Jroc;

public sealed record JrocCompiledAssemblyArtifact(
    string AssemblyName,
    byte[] PeBytes,
    byte[]? PdbBytes,
    IReadOnlyList<string> ModuleIds)
{
    /// <summary>
    /// Writes this artifact and its runtime dependencies to <paramref name="outputDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Artifact compilation itself is always in-memory. Call this method only when a launchable
    /// on-disk assembly is required, such as for a child process or external inspection tool.
    /// </remarks>
    public JrocMaterializedAssembly Materialize(string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        var fullOutputDirectory = Path.GetFullPath(outputDirectory);
        var assemblyPath = Path.Combine(fullOutputDirectory, $"{AssemblyName}.dll");
        WriteBytesAtomicallyWithRetry(assemblyPath, PeBytes);

        string? pdbPath = null;
        if (PdbBytes is { Length: > 0 } pdbBytes)
        {
            pdbPath = Path.Combine(fullOutputDirectory, $"{AssemblyName}.pdb");
            WriteBytesAtomicallyWithRetry(pdbPath, pdbBytes);
        }

        RuntimeConfigWriter.WriteRuntimeConfigJson(assemblyPath, typeof(object).Assembly.GetName());
        CopyRuntimeAssembly(fullOutputDirectory);

        return new JrocMaterializedAssembly(
            assemblyPath,
            pdbPath,
            Path.ChangeExtension(assemblyPath, ".runtimeconfig.json"));
    }

    private static void CopyRuntimeAssembly(string outputDirectory)
    {
        var runtimeAssemblyPath = typeof(JavaScriptRuntime.ObjectRuntime).Assembly.Location;
        if (string.IsNullOrWhiteSpace(runtimeAssemblyPath) || !File.Exists(runtimeAssemblyPath))
        {
            return;
        }

        var runtimeAssemblyDestination = Path.Combine(outputDirectory, Path.GetFileName(runtimeAssemblyPath));
        CopyFileIfChanged(runtimeAssemblyPath, runtimeAssemblyDestination);

        var runtimePdbPath = Path.ChangeExtension(runtimeAssemblyPath, ".pdb");
        if (File.Exists(runtimePdbPath))
        {
            CopyFileIfChanged(runtimePdbPath, Path.ChangeExtension(runtimeAssemblyDestination, ".pdb"));
        }
    }

    private static void CopyFileIfChanged(string sourcePath, string destinationPath)
    {
        var sourceInfo = new FileInfo(sourcePath);
        if (File.Exists(destinationPath))
        {
            var destinationInfo = new FileInfo(destinationPath);
            if (sourceInfo.Length == destinationInfo.Length
                && sourceInfo.LastWriteTimeUtc == destinationInfo.LastWriteTimeUtc)
            {
                return;
            }
        }

        try
        {
            File.Copy(sourcePath, destinationPath, overwrite: true);
            File.SetLastWriteTimeUtc(destinationPath, sourceInfo.LastWriteTimeUtc);
        }
        catch (IOException) when (File.Exists(destinationPath))
        {
            // Parallel materializations can race while copying the shared runtime. An existing
            // destination is usable because the assembly artifact itself was already written.
        }
    }

    private static void WriteBytesAtomicallyWithRetry(string destinationPath, byte[] bytes)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? ".");

        var temporaryPath = destinationPath + ".tmp_" + Guid.NewGuid().ToString("N");
        File.WriteAllBytes(temporaryPath, bytes);

        try
        {
            const int maxReplaceWaitMs = 60_000;
            var startTick = Environment.TickCount64;
            var attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    File.Move(temporaryPath, destinationPath, overwrite: true);
                    return;
                }
                catch (IOException) when (Environment.TickCount64 - startTick < maxReplaceWaitMs)
                {
                    Thread.Sleep(Math.Min(1000, 50 * attempt));
                }
                catch (UnauthorizedAccessException) when (Environment.TickCount64 - startTick < maxReplaceWaitMs)
                {
                    Thread.Sleep(Math.Min(1000, 50 * attempt));
                }
            }
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
            catch (IOException)
            {
                // Best-effort cleanup; a later materialization uses a distinct temporary path.
            }
        }
    }
}

public sealed record JrocMaterializedAssembly(
    string AssemblyPath,
    string? PdbPath,
    string RuntimeConfigPath);
