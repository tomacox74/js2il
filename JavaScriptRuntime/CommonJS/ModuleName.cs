using System.Text;

namespace JavaScriptRuntime.CommonJS
{
    public static class ModuleName
    {
        public static string GetModuleIdFromSpecifier(string specifier)
        {
            if (specifier == null)
            {
                throw new ArgumentNullException(nameof(specifier));
            }

            var s = specifier.Trim();
            s = s.Replace('\\', '/');

            // Accept both 'node:fs' and 'fs' by stripping optional node: prefix
            if (s.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
            {
                s = s.Substring("node:".Length);
            }

            // Normalize common local-specifier prefix so "./x" maps to the same id as "x".
            if (s.StartsWith("./", StringComparison.Ordinal))
            {
                s = s.Substring(2);
            }

            if (s.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            {
                s = s.Substring(0, s.Length - 3);
            }

            return SanitizeModuleId(s);
        }

        public static string GetModuleIdFromPath(string modulePath, string rootModulePath)
        {
            if (modulePath == null)
            {
                throw new ArgumentNullException(nameof(modulePath));
            }

            if (rootModulePath == null)
            {
                throw new ArgumentNullException(nameof(rootModulePath));
            }

            var rootFullPath = Path.GetFullPath(rootModulePath);
            var rootDirectory = Path.GetDirectoryName(rootFullPath) ?? ".";

            var moduleFullPath = Path.GetFullPath(modulePath);
            var relative = Path.GetRelativePath(rootDirectory, moduleFullPath);
            relative = relative.Replace('\\', '/');

            if (relative.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            {
                relative = relative.Substring(0, relative.Length - 3);
            }
            else
            {
                // Path.ChangeExtension handles removing extension, but keep forward slashes.
                relative = Path.ChangeExtension(relative.Replace('/', Path.DirectorySeparatorChar), null) ?? relative;
                relative = relative.Replace('\\', '/');
            }

            return SanitizeModuleId(relative);
        }

        public static string GetModuleIdForManifestFromPath(string modulePath, string rootModulePath)
        {
            if (modulePath == null)
            {
                throw new ArgumentNullException(nameof(modulePath));
            }

            if (rootModulePath == null)
            {
                throw new ArgumentNullException(nameof(rootModulePath));
            }

            // Mirror GetModuleIdFromPath but stop BEFORE sanitization.
            // This preserves path-like module ids for host-facing discovery (e.g. "calculator/index").
            var rootFullPath = Path.GetFullPath(rootModulePath);
            var rootDirectory = Path.GetDirectoryName(rootFullPath) ?? ".";

            var moduleFullPath = Path.GetFullPath(modulePath);
            var relative = Path.GetRelativePath(rootDirectory, moduleFullPath);
            relative = relative.Replace('\\', '/');

            if (relative.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            {
                relative = relative.Substring(0, relative.Length - 3);
            }
            else
            {
                // Path.ChangeExtension handles removing extension, but keep forward slashes.
                relative = Path.ChangeExtension(relative.Replace('/', Path.DirectorySeparatorChar), null) ?? relative;
                relative = relative.Replace('\\', '/');
            }

            if (string.IsNullOrWhiteSpace(relative))
            {
                throw new InvalidOperationException(
                    $"Computed an invalid module ID from path '{modulePath}' relative to root module '{rootModulePath}'.");
            }

            return relative;
        }

        private static string SanitizeModuleId(string moduleId)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
            {
                return "_";
            }

            var sb = new StringBuilder(moduleId.Length);
            foreach (var c in moduleId)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('_');
                }
            }

            if (sb.Length == 0)
            {
                return "_";
            }

            if (char.IsDigit(sb[0]))
            {
                sb.Insert(0, '_');
            }

            return sb.ToString();
        }
    }
}
