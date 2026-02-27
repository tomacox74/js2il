using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    [NodeModule("fs/promises")]
    public sealed class FSPromises
    {
        private readonly IIOScheduler _ioScheduler;

        public FSPromises()
        {
            _ioScheduler = GlobalThis.ServiceProvider?.Resolve<IIOScheduler>()
                ?? throw new InvalidOperationException("IIOScheduler is not available for fs/promises.");
        }

        public object? access(object path, object? mode = null)
        {
            _ = mode;
            try
            {
                var p = path?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(p) || (!File.Exists(p) && !Directory.Exists(p)))
                {
                    return Promise.reject(new Error($"ENOENT: no such file or directory, access '{p}'"));
                }

                return Promise.resolve(null);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        public object? readdir(object dir, object? options = null)
        {
            var path = dir?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                path = Environment.CurrentDirectory;
            }

            bool withFileTypes = false;
            try
            {
                if (options is ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue("withFileTypes", out var val))
                    {
                        withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                }
                else if (options != null)
                {
                    var val = JavaScriptRuntime.ObjectRuntime.GetProperty(options, "withFileTypes");
                    withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                }
            }
            catch { }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                // Fire-and-forget is intentional: exceptions are handled inside
                // CompleteReaddirAsync and forwarded via _ioScheduler.EndIo(...).
                _ = CompleteReaddirAsync(path, withFileTypes, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateReaddirError(path, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        private async Task CompleteReaddirAsync(string path, bool withFileTypes, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var list = await Task.Run(() =>
                {
                    var result = new JavaScriptRuntime.Array();
                    foreach (var entry in Directory.EnumerateFileSystemEntries(path))
                    {
                        var name = System.IO.Path.GetFileName(entry);
                        if (!withFileTypes)
                        {
                            result.Add(name);
                            continue;
                        }
                        bool isDir = false;
                        try { isDir = Directory.Exists(entry); } catch { }
                        result.Add(new FS.DirEnt(name, isDir));
                    }
                    return result;
                }).ConfigureAwait(false);

                _ioScheduler.EndIo(promiseWithResolvers, list, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateReaddirError(path, ex), isError: true);
            }
        }

        public object? mkdir(object dir, object? options = null)
        {
            try
            {
                var path = dir?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    return Promise.reject(new Error("Path must be a non-empty string"));
                }

                bool recursive = false;
                try
                {
                    if (options is ExpandoObject exp)
                    {
                        var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                        if (dict.TryGetValue("recursive", out var val))
                        {
                            recursive = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                        }
                    }
                    else if (options != null)
                    {
                        var val = JavaScriptRuntime.ObjectRuntime.GetProperty(options, "recursive");
                        recursive = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                }
                catch { }

                _ = recursive; // Directory.CreateDirectory is already recursive.
                Directory.CreateDirectory(path);
                return Promise.resolve(null);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        public object? copyFile(object src, object dest)
        {
            var s = src?.ToString() ?? string.Empty;
            var d = dest?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(d))
            {
                return Promise.reject(new Error("src and dest must be non-empty strings"));
            }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                // Fire-and-forget is intentional: exceptions are handled inside
                // CompleteCopyFileAsync and forwarded via _ioScheduler.EndIo(...).
                _ = CompleteCopyFileAsync(s, d, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, new Error(ex.Message, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        public object? readFile(object file, object? options = null)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return Promise.reject(new Error("Path must be a non-empty string"));
            }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                if (FsEncodingOptions.TryGetTextEncoding(options, out var textEncoding))
                {
                    // Fire-and-forget is intentional: exceptions are handled inside
                    // CompleteReadFileTextAsync and forwarded via _ioScheduler.EndIo(...).
                    _ = CompleteReadFileTextAsync(path, textEncoding!, promiseWithResolvers);
                    return promiseWithResolvers.promise;
                }

                // Fire-and-forget is intentional: exceptions are handled inside
                // CompleteReadFileBytesAsync and forwarded via _ioScheduler.EndIo(...).
                _ = CompleteReadFileBytesAsync(path, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateReadFileError(path, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        private async Task CompleteReadFileTextAsync(string path, System.Text.Encoding textEncoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var content = await File.ReadAllTextAsync(path, textEncoding).ConfigureAwait(false);
                _ioScheduler.EndIo(promiseWithResolvers, content, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateReadFileError(path, ex), isError: true);
            }
        }

        private async Task CompleteReadFileBytesAsync(string path, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                var buffer = Buffer.FromBytes(bytes);
                _ioScheduler.EndIo(promiseWithResolvers, buffer, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateReadFileError(path, ex), isError: true);
            }
        }

        public object? writeFile(object file, object? content, object? options = null)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return Promise.reject(new Error("Path must be a non-empty string"));
            }

            if (content == null || content is JsNull)
            {
                return Promise.reject(new TypeError("The \"data\" argument must be of type string or Buffer or TypedArray or DataView. Received null"));
            }

            var promiseWithResolvers = Promise.withResolvers();
            _ioScheduler.BeginIo();

            try
            {
                if (content is Buffer buffer)
                {
                    // Fire-and-forget is intentional: exceptions are handled inside
                    // CompleteWriteFileBytesAsync and forwarded via _ioScheduler.EndIo(...).
                    _ = CompleteWriteFileBytesAsync(path, buffer.ToByteArray(), promiseWithResolvers);
                    return promiseWithResolvers.promise;
                }

                if (content is byte[] bytes)
                {
                    // Fire-and-forget is intentional: exceptions are handled inside
                    // CompleteWriteFileBytesAsync and forwarded via _ioScheduler.EndIo(...).
                    _ = CompleteWriteFileBytesAsync(path, bytes, promiseWithResolvers);
                    return promiseWithResolvers.promise;
                }

                var text = content?.ToString() ?? string.Empty;
                if (FsEncodingOptions.TryGetTextEncoding(options, out var textEncoding))
                {
                    // Fire-and-forget is intentional: exceptions are handled inside
                    // CompleteWriteFileTextAsync and forwarded via _ioScheduler.EndIo(...).
                    _ = CompleteWriteFileTextAsync(path, text, textEncoding!, promiseWithResolvers);
                    return promiseWithResolvers.promise;
                }

                // Fire-and-forget is intentional: exceptions are handled inside
                // CompleteWriteFileTextAsync and forwarded via _ioScheduler.EndIo(...).
                _ = CompleteWriteFileTextAsync(path, text, FsEncodingOptions.Utf8NoBom, promiseWithResolvers);
                return promiseWithResolvers.promise;
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
                return promiseWithResolvers.promise;
            }
        }

        private async Task CompleteCopyFileAsync(string sourcePath, string destinationPath, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                {
                    await using var source = new FileStream(
                        sourcePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 81920,
                        options: FileOptions.Asynchronous);

                    await using var destination = new FileStream(
                        destinationPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        options: FileOptions.Asynchronous);

                    await source.CopyToAsync(destination).ConfigureAwait(false);
                    await destination.FlushAsync().ConfigureAwait(false);
                }

                _ioScheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, new Error(ex.Message, ex), isError: true);
            }
        }

        private async Task CompleteWriteFileBytesAsync(string path, byte[] bytes, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
                _ioScheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        private async Task CompleteWriteFileTextAsync(string path, string text, System.Text.Encoding encoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await File.WriteAllTextAsync(path, text, encoding).ConfigureAwait(false);
                _ioScheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                _ioScheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        public object? stat(object file)
        {
            try
            {
                var path = file?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    return Promise.reject(new Error("Path must be a non-empty string"));
                }

                if (File.Exists(path))
                {
                    var fi = new FileInfo(path);
                    return Promise.resolve(new FS.Stats(fi.Length));
                }

                if (Directory.Exists(path))
                {
                    return Promise.resolve(new FS.Stats(0));
                }

                return Promise.reject(new Error($"ENOENT: no such file or directory, stat '{path}'"));
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        public object? lstat(object file)
        {
            // In .NET, we don't have direct symlink support in the same way as Node
            // For now, lstat behaves the same as stat
            return stat(file);
        }

        public object? realpath(object file)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return Promise.reject(new Error("Path must be a non-empty string"));
            }

            try
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    return Promise.reject(new Error($"ENOENT: no such file or directory, realpath '{path}'"));
                }

                var fullPath = System.IO.Path.GetFullPath(path);
                return Promise.resolve(fullPath);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error($"EIO: i/o error, realpath '{path}'", ex));
            }
        }

        private static Error TranslateReadFileError(string path, Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, open '{path}'", ex);
            }

            if (Directory.Exists(path))
            {
                return new Error($"EISDIR: illegal operation on a directory, read '{path}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, open '{path}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, read '{path}'", ex);
            }

            return new Error($"EIO: i/o error, read '{path}'", ex);
        }

        private static Error TranslateWriteFileError(string path, Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, open '{path}'", ex);
            }

            if (Directory.Exists(path))
            {
                return new Error($"EISDIR: illegal operation on a directory, open '{path}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, open '{path}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, write '{path}'", ex);
            }

            return new Error($"EIO: i/o error, write '{path}'", ex);
        }

        private static Error TranslateReaddirError(string path, Exception ex)
        {
            if (ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, scandir '{path}'", ex);
            }

            if (File.Exists(path))
            {
                return new Error($"ENOTDIR: not a directory, scandir '{path}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, scandir '{path}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, scandir '{path}'", ex);
            }

            return new Error($"EIO: i/o error, scandir '{path}'", ex);
        }
    }
}
