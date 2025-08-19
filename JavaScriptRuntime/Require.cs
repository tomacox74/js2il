using System;
using System.Dynamic;

namespace JavaScriptRuntime
{
    public static class Require
    {
        // Minimal placeholder for require("module") returning an object.
        // For now, we only support Node core modules via a whitelist.
        // Unknown specifiers throw a ReferenceError.
        public static object require(string specifier)
        {
            if (string.IsNullOrWhiteSpace(specifier))
                throw new ReferenceError("require specifier must be a non-empty string");

            var key = Normalize(specifier);
            switch (key)
            {
                case "node:fs":
                case "fs":
                    return Node.FS.CreateModule();
                case "node:path":
                case "path":
                    return Node.Path.CreateModule();
                default:
                    throw new ReferenceError($"Cannot find module '{specifier}'");
            }
        }

        private static string Normalize(string s)
            => s.Trim();
    }
}
