using System;
using System.IO;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    // Minimal path module as a stable class: join(...)
    [NodeModule("path")]
    public sealed class Path
    {
        public string sep => System.IO.Path.DirectorySeparatorChar.ToString();
        public string delimiter => System.IO.Path.PathSeparator.ToString();

        public string join(params object[] parts)
        {
            var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);

            if (strings.Length == 0)
            {
                // Node: path.join() with no args returns ".".
                return ".";
            }

            // Node's path.join concatenates and then normalizes '.', '..', and duplicate separators.
            // System.IO.Path.Combine does not normalize ".." segments, so we normalize after combining.
            var combined = strings[0] ?? string.Empty;
            for (int i = 1; i < strings.Length; i++)
            {
                combined = System.IO.Path.Combine(combined, strings[i] ?? string.Empty);
            }

            return Normalize(combined);
        }

        private static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ".";
            }

            char sep = System.IO.Path.DirectorySeparatorChar;
            char alt = System.IO.Path.AltDirectorySeparatorChar;

            // Normalize separators first so we can reliably split.
            var p = path.Replace(alt, sep);

            var root = System.IO.Path.GetPathRoot(p) ?? string.Empty;
            bool rooted = root.Length > 0;
            var rest = rooted ? p.Substring(root.Length) : p;

            var segments = rest.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries);
            var stack = new List<string>(segments.Length);

            foreach (var seg in segments)
            {
                if (seg == ".")
                {
                    continue;
                }

                if (seg == "..")
                {
                    if (stack.Count > 0 && stack[stack.Count - 1] != "..")
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                    else if (!rooted)
                    {
                        // For relative paths, preserve leading .. segments.
                        stack.Add("..");
                    }

                    continue;
                }

                stack.Add(seg);
            }

            var joined = string.Join(sep, stack);
            if (rooted)
            {
                return joined.Length == 0 ? root : root + joined;
            }

            return joined.Length == 0 ? "." : joined;
        }

        public string resolve(params object[] parts)
        {
            // Node's path.resolve resolves from right to left until an absolute path is constructed,
            // then normalizes. For our minimal needs in docs generation, combining and full-normalizing is sufficient.
            var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);
            if (strings.Length == 0)
            {
                return System.IO.Path.GetFullPath(".");
            }

            string combined = strings[0] ?? string.Empty;
            for (int i = 1; i < strings.Length; i++)
            {
                combined = System.IO.Path.Combine(combined, strings[i] ?? string.Empty);
            }

            // If the last segment is absolute, Path.Combine handles it by resetting the path.
            // GetFullPath will produce an absolute, normalized path.
            return System.IO.Path.GetFullPath(combined);
        }

        public string relative(object from, object to)
        {
            var fromStr = from?.ToString();
            var toStr = to?.ToString();

            if (string.IsNullOrWhiteSpace(fromStr)) fromStr = ".";
            if (string.IsNullOrWhiteSpace(toStr)) toStr = ".";

            // Use .NET's relative path util; platform-specific separators match Node's behavior.
            var relative = System.IO.Path.GetRelativePath(fromStr, toStr);
            if (relative == ".")
            {
                return string.Empty;
            }

            return relative;
        }

        public string basename(object path, object? ext = null)
        {
            var p = path?.ToString() ?? string.Empty;
            var name = System.IO.Path.GetFileName(p);
            if (ext is string es && es.Length > 0 && name.EndsWith(es, StringComparison.Ordinal))
            {
                return name.Substring(0, name.Length - es.Length);
            }
            return name;
        }

        // Overload without extension argument to support dynamic invocation sites that provide only one argument.
        public string basename(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return System.IO.Path.GetFileName(p);
        }

        public string dirname(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return System.IO.Path.GetDirectoryName(p) ?? string.Empty;
        }

        public string extname(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            var name = System.IO.Path.GetFileName(p);

            if (string.IsNullOrEmpty(name) || name == "." || name == "..")
            {
                return string.Empty;
            }

            var lastDot = name.LastIndexOf('.');

            // Node behavior: no extension when dot is absent or leading-only (e.g. ".bashrc").
            if (lastDot <= 0)
            {
                return string.Empty;
            }

            if (lastDot == name.Length - 1)
            {
                return ".";
            }

            return name.Substring(lastDot);
        }

        public bool isAbsolute(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return !string.IsNullOrEmpty(p) && System.IO.Path.IsPathRooted(p);
        }

        public string normalize(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return Normalize(p);
        }

        public string toNamespacedPath(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            return p;
        }

        public object parse(object path)
        {
            var p = path?.ToString() ?? string.Empty;
            var root = System.IO.Path.GetPathRoot(p) ?? string.Empty;
            var dir = System.IO.Path.GetDirectoryName(p) ?? string.Empty;
            var baseName = System.IO.Path.GetFileName(p);
            var ext = extname(p);
            var name = baseName;

            if (!string.IsNullOrEmpty(ext) && !string.IsNullOrEmpty(baseName) && baseName.EndsWith(ext, StringComparison.Ordinal))
            {
                name = baseName.Substring(0, baseName.Length - ext.Length);
            }

            var result = new System.Dynamic.ExpandoObject();
            var dict = (IDictionary<string, object?>)result;
            dict["root"] = root;
            dict["dir"] = dir;
            dict["base"] = baseName;
            dict["ext"] = ext;
            dict["name"] = name;
            return result;
        }

        public string format(object pathObject)
        {
            var dir = ReadPathProperty(pathObject, "dir");
            var root = ReadPathProperty(pathObject, "root");
            var baseName = ReadPathProperty(pathObject, "base");
            var name = ReadPathProperty(pathObject, "name");
            var ext = ReadPathProperty(pathObject, "ext");

            if (string.IsNullOrEmpty(baseName))
            {
                baseName = name + ext;
            }

            if (!string.IsNullOrEmpty(dir))
            {
                if (string.IsNullOrEmpty(baseName))
                {
                    return dir;
                }

                char sep = System.IO.Path.DirectorySeparatorChar;
                char alt = System.IO.Path.AltDirectorySeparatorChar;
                if (dir.EndsWith(sep) || dir.EndsWith(alt))
                {
                    return dir + baseName;
                }

                return dir + sep + baseName;
            }

            if (!string.IsNullOrEmpty(root))
            {
                if (string.IsNullOrEmpty(baseName))
                {
                    return root;
                }

                char sep = System.IO.Path.DirectorySeparatorChar;
                char alt = System.IO.Path.AltDirectorySeparatorChar;
                if (root.EndsWith(sep) || root.EndsWith(alt))
                {
                    return root + baseName;
                }

                return root + baseName;
            }

            return baseName;
        }

        private static string ReadPathProperty(object pathObject, string propertyName)
        {
            if (pathObject is null || pathObject is JsNull)
            {
                return string.Empty;
            }

            try
            {
                var value = JavaScriptRuntime.ObjectRuntime.GetItem(pathObject, propertyName);
                if (value is null || value is JsNull)
                {
                    return string.Empty;
                }

                return value.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
