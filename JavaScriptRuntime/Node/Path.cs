using System;
using System.IO;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    // Minimal path module as a stable class: join(...)
    [NodeModule("path")]
    public sealed class Path
    {
        private static readonly VariantPath _posix = VariantPath.Posix;
        private static readonly VariantPath _win32 = VariantPath.Win32;

        // Node: path.posix and path.win32 are always present (even on Windows).
        public object posix => _posix;
        public object win32 => _win32;

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

        // NOTE: VariantPath currently duplicates some of the outer Path surface to keep variant semantics isolated.
        // TODO: Consider delegating the host Path implementation to VariantPath to avoid duplication.
        private sealed class VariantPath
        {
            public static readonly VariantPath Posix = new VariantPath(isWin32: false);
            public static readonly VariantPath Win32 = new VariantPath(isWin32: true);

            private readonly bool _isWin32;
            private readonly char _sep;
            private readonly char _altSep;
            private readonly string _delimiter;

            private VariantPath(bool isWin32)
            {
                _isWin32 = isWin32;
                _sep = isWin32 ? '\\' : '/';
                _altSep = isWin32 ? '/' : _sep; // POSIX treats '\\' as a valid filename character.
                _delimiter = isWin32 ? ";" : ":";
            }

            public string sep => _sep.ToString();
            public string delimiter => _delimiter;

            public string join(params object[] parts)
            {
                var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);

                if (strings.Length == 0)
                {
                    return ".";
                }

                string combined = string.Empty;
                for (int i = 0; i < strings.Length; i++)
                {
                    var part = strings[i] ?? string.Empty;
                    if (part.Length == 0)
                    {
                        continue;
                    }

                    part = NormalizeSeparators(part);

                    if (combined.Length == 0)
                    {
                        combined = part;
                        continue;
                    }

                    combined = combined.TrimEnd(_sep) + _sep + part.TrimStart(_sep);
                }

                if (combined.Length == 0)
                {
                    return ".";
                }

                return NormalizeInternal(combined);
            }

            public string resolve(params object[] parts)
            {
                var strings = System.Array.ConvertAll(parts ?? System.Array.Empty<object>(), p => p?.ToString() ?? string.Empty);
                if (strings.Length == 0)
                {
                    return NormalizeInternal(GetCwd());
                }

                string? resolved = null;

                // Resolve from right to left until we hit an absolute segment.
                for (int i = strings.Length - 1; i >= 0; i--)
                {
                    var part = strings[i] ?? string.Empty;
                    if (part.Length == 0)
                    {
                        continue;
                    }

                    part = NormalizeSeparators(part);

                    if (resolved is null)
                    {
                        resolved = part;
                    }
                    else
                    {
                        resolved = part.TrimEnd(_sep) + _sep + resolved.TrimStart(_sep);
                    }

                    if (IsAbsoluteInternal(part))
                    {
                        break;
                    }
                }

                resolved ??= string.Empty;
                if (!IsAbsoluteInternal(resolved))
                {
                    resolved = GetCwd().TrimEnd(_sep) + _sep + resolved.TrimStart(_sep);
                }

                return NormalizeInternal(resolved);
            }

            public string relative(object from, object to)
            {
                var fromStr = from?.ToString();
                var toStr = to?.ToString();

                if (string.IsNullOrWhiteSpace(fromStr)) fromStr = ".";
                if (string.IsNullOrWhiteSpace(toStr)) toStr = ".";

                var fromNorm = NormalizeInternal(NormalizeSeparators(fromStr));
                var toNorm = NormalizeInternal(NormalizeSeparators(toStr));

                if (fromNorm == ".") fromNorm = string.Empty;
                if (toNorm == ".") toNorm = string.Empty;

                // If roots differ (e.g. different drives/UNC), return target as Node does.
                var (fromRoot, fromRemainder, fromRooted) = SplitRoot(fromNorm);
                var (toRoot, toRemainder, toRooted) = SplitRoot(toNorm);

                if (fromRooted != toRooted)
                {
                    return toNorm;
                }

                if (!string.Equals(fromRoot, toRoot, _isWin32 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return toNorm;
                }

                var fromParts = SplitSegments(fromRemainder);
                var toParts = SplitSegments(toRemainder);

                int common = 0;
                while (common < fromParts.Length && common < toParts.Length)
                {
                    if (!string.Equals(fromParts[common], toParts[common], _isWin32 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        break;
                    }
                    common++;
                }

                var relParts = new List<string>();
                for (int i = common; i < fromParts.Length; i++)
                {
                    if (fromParts[i].Length > 0)
                    {
                        relParts.Add("..");
                    }
                }

                for (int i = common; i < toParts.Length; i++)
                {
                    if (toParts[i].Length > 0)
                    {
                        relParts.Add(toParts[i]);
                    }
                }

                if (relParts.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(_sep, relParts);
            }

            public string basename(object path, object? ext = null)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);
                var name = GetFileName(p);
                if (ext is string es && es.Length > 0 && name.EndsWith(es, StringComparison.Ordinal))
                {
                    return name.Substring(0, name.Length - es.Length);
                }
                return name;
            }

            public string basename(object path)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);
                return GetFileName(p);
            }

            public string dirname(object path)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);
                if (string.IsNullOrEmpty(p))
                {
                    return ".";
                }

                var (root, _, rooted) = SplitRoot(p);
                var lastSep = p.LastIndexOf(_sep);
                if (lastSep < 0)
                {
                    return ".";
                }

                if (rooted && lastSep < root.Length)
                {
                    return root;
                }

                if (lastSep == 0)
                {
                    return root.Length > 0 ? root : _sep.ToString();
                }

                return p.Substring(0, lastSep);
            }

            public string extname(object path)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);
                var name = GetFileName(p);

                if (string.IsNullOrEmpty(name) || name == "." || name == "..")
                {
                    return string.Empty;
                }

                var lastDot = name.LastIndexOf('.');
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
                return IsAbsoluteInternal(p);
            }

            public string normalize(object path)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);
                return NormalizeInternal(p);
            }

            public string toNamespacedPath(object path)
            {
                var p = path?.ToString() ?? string.Empty;
                return p;
            }

            public object parse(object path)
            {
                var p = NormalizeSeparators(path?.ToString() ?? string.Empty);

                var (root, _, _) = SplitRoot(p);
                var dir = dirname(p);
                if (dir == "." && p.IndexOf(_sep) < 0)
                {
                    dir = string.Empty;
                }
                var baseName = GetFileName(p);
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

                dir = NormalizeSeparators(dir);
                root = NormalizeSeparators(root);

                if (!string.IsNullOrEmpty(dir))
                {
                    if (string.IsNullOrEmpty(baseName))
                    {
                        return dir;
                    }

                    if (dir.EndsWith(_sep))
                    {
                        return dir + baseName;
                    }

                    return dir + _sep + baseName;
                }

                if (!string.IsNullOrEmpty(root))
                {
                    if (string.IsNullOrEmpty(baseName))
                    {
                        return root;
                    }

                    if (root.EndsWith(_sep))
                    {
                        return root + baseName;
                    }

                    return root + baseName;
                }

                return baseName;
            }

            private string GetCwd()
            {
                return NormalizeSeparators(Environment.CurrentDirectory ?? ".");
            }

            private string NormalizeSeparators(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return string.Empty;
                }

                if (_altSep == _sep)
                {
                    return path;
                }

                // For the win32 variant, accept both separators but always output the variant separator.
                return path.Replace(_altSep, _sep);
            }

            private string GetFileName(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return string.Empty;
                }

                var p = NormalizeSeparators(path);
                var lastSep = p.LastIndexOf(_sep);
                if (lastSep < 0)
                {
                    return p;
                }

                if (lastSep == p.Length - 1)
                {
                    // Trailing separator => empty basename.
                    return string.Empty;
                }

                return p.Substring(lastSep + 1);
            }

            private bool IsAbsoluteInternal(string path)
            {
                // Node.js does not trim whitespace before absolute checks.
                var p = NormalizeSeparators(path ?? string.Empty);
                if (p.Length == 0)
                {
                    return false;
                }

                if (!_isWin32)
                {
                    return p.StartsWith("/", StringComparison.Ordinal);
                }

                if (p.Length >= 2 && p[0] == _sep && p[1] == _sep)
                {
                    return true; // UNC
                }

                if (p[0] == _sep)
                {
                    return true;
                }

                if (p.Length >= 3 && char.IsLetter(p[0]) && p[1] == ':' && p[2] == _sep)
                {
                    return true;
                }

                return false;
            }

            private string NormalizeInternal(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return ".";
                }

                var p = NormalizeSeparators(path);

                var (root, remainder, rooted) = SplitRoot(p);

                var segments = remainder.Split(new[] { _sep }, StringSplitOptions.RemoveEmptyEntries);
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
                            stack.Add("..");
                        }
                        continue;
                    }

                    stack.Add(seg);
                }

                var joined = string.Join(_sep, stack);
                if (rooted)
                {
                    return joined.Length == 0 ? root : root + joined;
                }

                return joined.Length == 0 ? "." : joined;
            }

            private (string Root, string Remainder, bool Rooted) SplitRoot(string normalized)
            {
                var p = NormalizeSeparators(normalized);

                if (!_isWin32)
                {
                    if (p.StartsWith(_sep))
                    {
                        return ("/", p.Substring(1), true);
                    }
                    return (string.Empty, p, false);
                }

                if (p.Length >= 2 && p[0] == _sep && p[1] == _sep)
                {
                    // UNC: \\server\share\...
                    var withoutPrefix = p.Substring(2);
                    var parts = withoutPrefix.Split(new[] { _sep }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        var root = new string(_sep, 2) + parts[0] + _sep + parts[1] + _sep;
                        var rest = parts.Length > 2 ? string.Join(_sep, parts, 2, parts.Length - 2) : string.Empty;
                        return (root, rest, true);
                    }

                    return (new string(_sep, 2), withoutPrefix.TrimStart(_sep), true);
                }

                if (p.Length >= 3 && char.IsLetter(p[0]) && p[1] == ':' && p[2] == _sep)
                {
                    return (p.Substring(0, 3), p.Substring(3), true);
                }

                if (p.StartsWith(_sep))
                {
                    return (_sep.ToString(), p.Substring(1), true);
                }

                return (string.Empty, p, false);
            }

            private string[] SplitSegments(string path)
            {
                var p = NormalizeSeparators(path);
                return p.Split(new[] { _sep }, StringSplitOptions.RemoveEmptyEntries);
            }
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
