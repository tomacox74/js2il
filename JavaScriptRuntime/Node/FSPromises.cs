using System;
using System.Dynamic;
using System.IO;
using JavaScriptRuntime;

namespace JavaScriptRuntime.Node
{
    [NodeModule("fs/promises")]
    public sealed class FSPromises
    {
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
    }
}
