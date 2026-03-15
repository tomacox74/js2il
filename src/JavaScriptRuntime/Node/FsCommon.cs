using System;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    internal static class FsCommon
    {
        private static int _nextFileDescriptor = 17;

        internal static bool GetBooleanOption(object? options, string propertyName)
        {
            if (options == null || options is JsNull)
            {
                return false;
            }

            try
            {
                if (options is ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue(propertyName, out var val))
                    {
                        return TypeUtilities.ToBoolean(val);
                    }

                    return false;
                }

                var value = ObjectRuntime.GetProperty(options, propertyName);
                return TypeUtilities.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        internal static object? GetOption(object? options, string propertyName)
        {
            if (options == null || options is JsNull)
            {
                return null;
            }

            try
            {
                if (options is ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue(propertyName, out var value))
                    {
                        return value;
                    }
                }

                return ObjectRuntime.GetProperty(options, propertyName);
            }
            catch
            {
                return null;
            }
        }

        internal static int GetIntOption(object? options, string propertyName, int defaultValue)
        {
            var value = GetOption(options, propertyName);
            return value == null || value is JsNull
                ? defaultValue
                : CoerceNonNegativeInt(value, defaultValue);
        }

        internal static long? GetNullableLongOption(object? options, string propertyName)
        {
            var value = GetOption(options, propertyName);
            if (value == null || value is JsNull)
            {
                return null;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                throw new RangeError($"The \"{propertyName}\" option must be a finite number.");
            }

            if (number < 0)
            {
                throw new RangeError($"The \"{propertyName}\" option must be greater than or equal to 0.");
            }

            return (long)number;
        }

        internal static int CoerceNonNegativeInt(object? value, int defaultValue)
        {
            if (value == null || value is JsNull)
            {
                return defaultValue;
            }

            var number = TypeUtilities.ToNumber(value);
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                return defaultValue;
            }

            if (number < 0)
            {
                throw new RangeError("The value must be greater than or equal to 0.");
            }

            return (int)number;
        }

        internal static int NextFileDescriptor()
            => Interlocked.Increment(ref _nextFileDescriptor);

        internal static FsOpenSpec ResolveOpenSpec(object? flags, string defaultFlags = "r")
        {
            var normalized = flags == null || flags is JsNull
                ? defaultFlags
                : flags.ToString() ?? defaultFlags;

            return normalized switch
            {
                "r" => new FsOpenSpec(normalized, FileMode.Open, FileAccess.Read, false),
                "r+" => new FsOpenSpec(normalized, FileMode.Open, FileAccess.ReadWrite, false),
                "w" => new FsOpenSpec(normalized, FileMode.Create, FileAccess.Write, false),
                "w+" => new FsOpenSpec(normalized, FileMode.Create, FileAccess.ReadWrite, false),
                "a" => new FsOpenSpec(normalized, FileMode.OpenOrCreate, FileAccess.Write, true),
                "a+" => new FsOpenSpec(normalized, FileMode.OpenOrCreate, FileAccess.ReadWrite, true),
                _ => throw new NotSupportedException($"Unsupported fs flag '{normalized}'. Supported flags are r, r+, w, w+, a, and a+.")
            };
        }

        internal static FileStream OpenFileStream(string path, object? flags, string defaultFlags = "r", FileShare share = FileShare.ReadWrite)
        {
            var spec = ResolveOpenSpec(flags, defaultFlags);
            var stream = new FileStream(
                path,
                spec.Mode,
                spec.Access,
                share,
                bufferSize: 81920,
                options: FileOptions.Asynchronous);

            if (spec.Append)
            {
                stream.Seek(0, SeekOrigin.End);
            }

            return stream;
        }

        internal static FileHandle CreateFileHandle(string path, object? flags, IIOScheduler scheduler, string defaultFlags = "r")
        {
            var spec = ResolveOpenSpec(flags, defaultFlags);
            var stream = OpenFileStream(path, spec.NormalizedFlags, defaultFlags, FileShare.ReadWrite);
            return new FileHandle(path, spec.NormalizedFlags, stream, spec.Append, NextFileDescriptor(), scheduler);
        }

        internal static async Task CompleteOpenFileHandleAsync(IIOScheduler scheduler, string path, object? flags, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var handle = await Task.Run(() => CreateFileHandle(path, flags, scheduler)).ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, handle, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateOpenError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteReadFileTextAsync(IIOScheduler scheduler, string path, System.Text.Encoding textEncoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var content = await File.ReadAllTextAsync(path, textEncoding).ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, content, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateReadFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteReadFileBytesAsync(IIOScheduler scheduler, string path, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                var buffer = Buffer.FromBytes(bytes);
                scheduler.EndIo(promiseWithResolvers, buffer, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateReadFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteWriteFileBytesAsync(IIOScheduler scheduler, string path, byte[] bytes, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteWriteFileTextAsync(IIOScheduler scheduler, string path, string text, System.Text.Encoding encoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await File.WriteAllTextAsync(path, text, encoding).ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteAppendFileBytesAsync(IIOScheduler scheduler, string path, byte[] bytes, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await using var stream = OpenFileStream(path, "a", share: FileShare.ReadWrite);
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteAppendFileTextAsync(IIOScheduler scheduler, string path, string text, System.Text.Encoding encoding, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await using var stream = OpenFileStream(path, "a", share: FileShare.ReadWrite);
                var bytes = encoding.GetBytes(text);
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateWriteFileError(path, ex), isError: true);
            }
        }

        internal static async Task CompleteCopyFileAsync(IIOScheduler scheduler, string sourcePath, string destinationPath, PromiseWithResolvers promiseWithResolvers)
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

                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateCopyFileError(sourcePath, destinationPath, ex), isError: true);
            }
        }

        internal static async Task CompleteRenameAsync(IIOScheduler scheduler, string sourcePath, string destinationPath, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (File.Exists(sourcePath))
                    {
                        File.Move(sourcePath, destinationPath, overwrite: true);
                        return;
                    }

                    if (Directory.Exists(sourcePath))
                    {
                        if (Directory.Exists(destinationPath))
                        {
                            Directory.Delete(destinationPath, recursive: true);
                        }

                        Directory.Move(sourcePath, destinationPath);
                        return;
                    }

                    throw new FileNotFoundException(sourcePath);
                }).ConfigureAwait(false);

                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateRenameError(sourcePath, destinationPath, ex), isError: true);
            }
        }

        internal static async Task CompleteUnlinkAsync(IIOScheduler scheduler, string path, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(path))
                    {
                        throw new IOException("Path is a directory.");
                    }

                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException(path);
                    }

                    File.Delete(path);
                }).ConfigureAwait(false);

                scheduler.EndIo(promiseWithResolvers, null, isError: false);
            }
            catch (Exception ex)
            {
                scheduler.EndIo(promiseWithResolvers, TranslateUnlinkError(path, ex), isError: true);
            }
        }

        internal static Error TranslateCopyFileError(string sourcePath, string destinationPath, Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, copyfile '{sourcePath}' -> '{destinationPath}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, copyfile '{sourcePath}' -> '{destinationPath}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, copyfile '{sourcePath}' -> '{destinationPath}'", ex);
            }

            return new Error($"EIO: i/o error, copyfile '{sourcePath}' -> '{destinationPath}'", ex);
        }

        internal static Error TranslateOpenError(string path, Exception ex)
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
                return new Error($"EIO: i/o error, open '{path}'", ex);
            }

            return new Error($"EIO: i/o error, open '{path}'", ex);
        }

        internal static Error TranslateRenameError(string sourcePath, string destinationPath, Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, rename '{sourcePath}' -> '{destinationPath}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, rename '{sourcePath}' -> '{destinationPath}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, rename '{sourcePath}' -> '{destinationPath}'", ex);
            }

            return new Error($"EIO: i/o error, rename '{sourcePath}' -> '{destinationPath}'", ex);
        }

        internal static Error TranslateUnlinkError(string path, Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return new Error($"ENOENT: no such file or directory, unlink '{path}'", ex);
            }

            if (Directory.Exists(path))
            {
                return new Error($"EISDIR: illegal operation on a directory, unlink '{path}'", ex);
            }

            if (ex is UnauthorizedAccessException)
            {
                return new Error($"EACCES: permission denied, unlink '{path}'", ex);
            }

            if (ex is IOException)
            {
                return new Error($"EIO: i/o error, unlink '{path}'", ex);
            }

            return new Error($"EIO: i/o error, unlink '{path}'", ex);
        }

        internal static Error CreateBadFileDescriptorError(string operation)
            => new Error($"EBADF: bad file descriptor, {operation}");

        internal static Error TranslateReadFileError(string path, Exception ex)
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

        internal static Error TranslateWriteFileError(string path, Exception ex)
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

        internal static Error TranslateReaddirError(string path, Exception ex)
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

        internal readonly record struct FsOpenSpec(string NormalizedFlags, FileMode Mode, FileAccess Access, bool Append);
    }
}
