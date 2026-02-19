using System;
using System.Dynamic;
using System.IO;
using System.Text;
using JavaScriptRuntime;

namespace JavaScriptRuntime.Node
{
    [NodeModule("fs/promises")]
    public sealed class FSPromises
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
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
            try
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
                        var val = JavaScriptRuntime.Object.GetProperty(options, "withFileTypes");
                        withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                }
                catch { }

                var list = new JavaScriptRuntime.Array();

                foreach (var entry in Directory.EnumerateFileSystemEntries(path))
                {
                    var name = System.IO.Path.GetFileName(entry);

                    if (!withFileTypes)
                    {
                        list.Add(name);
                        continue;
                    }

                    bool isDir = false;
                    try { isDir = Directory.Exists(entry); } catch { }
                    list.Add(new FS.DirEnt(name, isDir));
                }

                return Promise.resolve(list);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
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
                        var val = JavaScriptRuntime.Object.GetProperty(options, "recursive");
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
            try
            {
                var s = src?.ToString() ?? string.Empty;
                var d = dest?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(d))
                {
                    return Promise.reject(new Error("src and dest must be non-empty strings"));
                }

                File.Copy(s, d, overwrite: true);
                return Promise.resolve(null);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        public object? readFile(object file, object? options = null)
        {
            try
            {
                var path = file?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    return Promise.reject(new Error("Path must be a non-empty string"));
                }

                if (TryGetTextEncoding(options, out var textEncoding))
                {
                    var content = File.ReadAllText(path, textEncoding!);
                    return Promise.resolve(content);
                }

                var buffer = Buffer.FromBytes(File.ReadAllBytes(path));
                return Promise.resolve(buffer);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        public object? writeFile(object file, object? content, object? options = null)
        {
            try
            {
                var path = file?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    return Promise.reject(new Error("Path must be a non-empty string"));
                }

                if (content is Buffer buffer)
                {
                    File.WriteAllBytes(path, buffer.ToByteArray());
                    return Promise.resolve(null);
                }

                if (content is byte[] bytes)
                {
                    File.WriteAllBytes(path, bytes);
                    return Promise.resolve(null);
                }

                var text = content?.ToString() ?? string.Empty;
                if (TryGetTextEncoding(options, out var textEncoding))
                {
                    File.WriteAllText(path, text, textEncoding!);
                    return Promise.resolve(null);
                }

                File.WriteAllText(path, text, Utf8NoBom);
                return Promise.resolve(null);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
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
            try
            {
                var path = file?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    return Promise.reject(new Error("Path must be a non-empty string"));
                }

                var fullPath = System.IO.Path.GetFullPath(path);
                return Promise.resolve(fullPath);
            }
            catch (Exception ex)
            {
                return Promise.reject(new Error(ex.Message, ex));
            }
        }

        private static bool TryGetTextEncoding(object? options, out Encoding? encoding)
        {
            encoding = null;
            if (options == null || options is JsNull)
            {
                return false;
            }

            if (options is string optionString)
            {
                return TryResolveTextEncoding(optionString, out encoding);
            }

            try
            {
                var encodingValue = JavaScriptRuntime.Object.GetProperty(options, "encoding");
                if (encodingValue == null || encodingValue is JsNull)
                {
                    return false;
                }

                return TryResolveTextEncoding(encodingValue.ToString() ?? string.Empty, out encoding);
            }
            catch
            {
                return false;
            }
        }

        private static bool TryResolveTextEncoding(string value, out Encoding? encoding)
        {
            encoding = null;
            if (IsUtf8(value))
            {
                encoding = Utf8NoBom;
                return true;
            }

            return false;
        }

        private static bool IsUtf8(string s)
        {
            return s.Equals("utf8", StringComparison.OrdinalIgnoreCase)
                || s.Equals("utf-8", StringComparison.OrdinalIgnoreCase);
        }
    }
}
