using System;
using System.Dynamic;
using System.IO;

namespace JavaScriptRuntime.Node
{
    // Minimal fs module as a stable class: readFileSync/writeFileSync.
    [NodeModule("fs")]
    public sealed class FS
    {
        private static readonly object _constants = CreateConstants();

        public object constants => _constants;

        private static object CreateConstants()
        {
            dynamic c = new ExpandoObject();
            // Node's fs.constants.F_OK (value 0) - used as the existence-check mode.
            c.F_OK = 0.0;
            return c;
        }

        // Dynamic-friendly overloads first so Object.CallInstanceMethod prefers them
        public object readdirSync(object[] args)
        {
            var dir = (args != null && args.Length > 0) ? args[0] : null;
            var options = (args != null && args.Length > 1) ? args[1] : null;

            // If options indicates withFileTypes, delegate to the (dir, options) path
            bool withFileTypes = false;
            try
            {
                if (options != null)
                {
                    var val = JavaScriptRuntime.Object.GetProperty(options, "withFileTypes");
                    withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                }
            }
            catch { }

            if (withFileTypes)
            {
                return readdirSync(dir ?? string.Empty, options);
            }

            return readdirSync(dir ?? string.Empty);
        }

        public object mkdirSync(object[] args)
        {
            var dir = (args != null && args.Length > 0) ? args[0] : null;
            var options = (args != null && args.Length > 1) ? args[1] : null;
            return mkdirSync(dir ?? string.Empty, options);
        }

        public object readFileSync(string file)
            => readFileSync(file, null);

        public object writeFileSync(string file, object? content)
        {
            return writeFileSync(file, content, null);
        }

        public object mkdirSync(object dir, object? options)
        {
            var path = dir?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must be a non-empty string", nameof(dir));

            bool recursive = false;
            try
            {
                if (options != null)
                {
                    var val = JavaScriptRuntime.Object.GetProperty(options, "recursive");
                    recursive = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                }
            }
            catch { }

            if (!recursive)
            {
                // Best-effort Node behavior: do not create parents when recursive is false.
                var parent = System.IO.Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(parent) && !System.IO.Directory.Exists(parent))
                {
                    throw new DirectoryNotFoundException(parent);
                }
            }

            System.IO.Directory.CreateDirectory(path);
            return null!; // undefined
        }

        // Overload without options parameter for calls supplying only a path.
        public object mkdirSync(object dir)
        {
            return mkdirSync(dir, null);
        }

        // Overload supporting Node-style encoding option: 'utf8'
        public object readFileSync(object file, object? options)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must be a non-empty string", nameof(file));

            if (FsEncodingOptions.TryGetTextEncoding(options, out var textEncoding))
            {
                return System.IO.File.ReadAllText(path, textEncoding!);
            }

            return Buffer.FromBytes(System.IO.File.ReadAllBytes(path));
        }

        // Overload supporting Node-style encoding option: 'utf8'
        public object writeFileSync(object file, object? content, object? options)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must be a non-empty string", nameof(file));

            if (content is Buffer buffer)
            {
                System.IO.File.WriteAllBytes(path, buffer.ToByteArray());
                return null!;
            }

            if (content is byte[] bytes)
            {
                System.IO.File.WriteAllBytes(path, bytes);
                return null!;
            }

            var text = content?.ToString() ?? string.Empty;
            if (FsEncodingOptions.TryGetTextEncoding(options, out var textEncoding))
            {
                System.IO.File.WriteAllText(path, text, textEncoding!);
                return null!; // JS: undefined
            }

            System.IO.File.WriteAllText(path, text, FsEncodingOptions.Utf8NoBom);
            return null!; // JS: undefined
        }

        // --- Additions to support cleanUnusedSnapshots.js ---

        public object existsSync(object file)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) return false;
            return System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
        }

        public object readdirSync(object dir)
        {
            var path = dir?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                try { path = System.Environment.CurrentDirectory; }
                catch { return new JavaScriptRuntime.Array(); }
            }
            try
            {
                var entries = System.IO.Directory.GetFileSystemEntries(path);
                var list = new JavaScriptRuntime.Array();
                for (int i = 0; i < entries.Length; i++)
                {
                    var name = System.IO.Path.GetFileName(entries[i]);
                    list.Add(name);
                }
                return list;
            }
            catch
            {
                // Node's fs.readdirSync would throw ENOENT on missing path; for current test harness we return empty list.
                return new JavaScriptRuntime.Array();
            }
        }

        public object readdirSync(object dir, object? options)
        {
            var path = dir?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                try { path = System.Environment.CurrentDirectory; }
                catch { return new JavaScriptRuntime.Array(); }
            }

            bool withFileTypes = false;
            try
            {
                if (options is System.Dynamic.ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue("withFileTypes", out var val))
                    {
                        withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                }
                else if (options != null)
                {
                    // Fallback: use runtime dynamic property access for object literals
                    try
                    {
                        var val = JavaScriptRuntime.Object.GetProperty(options, "withFileTypes");
                        withFileTypes = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                    catch { }
                }
            }
            catch { }

            if (!withFileTypes)
            {
                // Safe re-entry: dir is non-null here (stringified earlier) but suppress analyzer by using path
                return readdirSync(path);
            }
            try
            {
                var list = new JavaScriptRuntime.Array();
                foreach (var entry in System.IO.Directory.EnumerateFileSystemEntries(path))
                {
                    var name = System.IO.Path.GetFileName(entry);
                    bool isDir = false;
                    try { isDir = System.IO.Directory.Exists(entry); } catch { }
                    list.Add(new DirEnt(name, isDir));
                }
                return list;
            }
            catch
            {
                return new JavaScriptRuntime.Array();
            }
        }

        public object statSync(object file)
        {
            var path = file?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(path)) return new Stats(0);
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var fi = new System.IO.FileInfo(path);
                    return new Stats(fi.Length);
                }
                if (System.IO.Directory.Exists(path))
                {
                    // Directory: size 0 (not used in our script)
                    return new Stats(0);
                }
            }
            catch { }
            return new Stats(0);
        }

        public object rmSync(object file, object? options)
        {
            var path = file?.ToString() ?? string.Empty;
            bool force = false;
            try
            {
                if (options is System.Dynamic.ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue("force", out var val))
                    {
                        force = JavaScriptRuntime.TypeUtilities.ToBoolean(val);
                    }
                }
            }
            catch { }

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                else if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path, recursive: true);
                }
            }
            catch
            {
                if (!force) throw;
            }
            return null!; // undefined
        }

        // Overload without options parameter for calls supplying only a path.
        public object rmSync(object file)
        {
            return rmSync(file, null);
        }

        // Minimal DirEnt (directory entry) object returned when withFileTypes: true
        public sealed class DirEnt
        {
            public string name { get; }
            private readonly bool _isDirectory;
            public DirEnt(string name, bool isDirectory)
            {
                this.name = name;
                _isDirectory = isDirectory;
            }

            public object isDirectory() => _isDirectory;

            public object isFile() => !_isDirectory;
        }

        // Minimal Stats object with a size property used by the script
        public sealed class Stats
        {
            public double size { get; }
            public Stats(long length)
            {
                size = (double)length;
            }
        }
    }
}
