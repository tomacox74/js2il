using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    internal static class FsCommon
    {
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
                scheduler.EndIo(promiseWithResolvers, new Error(ex.Message, ex), isError: true);
            }
        }

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
    }
}
