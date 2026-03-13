using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime
{
    public static class IntrinsicObjectRegistry
    {
        private static readonly object _sync = new();
        private static volatile Dictionary<string, IntrinsicObjectInfo>? _byName;

        public static Type? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var map = EnsureMap();
            return map.TryGetValue(name, out var info) ? info.Type : null;
        }

        public static IntrinsicObjectInfo? GetInfo(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var map = EnsureMap();
            return map.TryGetValue(name, out var info) ? info : null;
        }

        private static Dictionary<string, IntrinsicObjectInfo> EnsureMap()
        {
            var map = _byName;
            if (map == null)
            {
                lock (_sync)
                {
                    map = _byName;
                    if (map == null)
                    {
                        map = Build();
                        _byName = map;
                    }
                }
            }
            return map;
        }

        private static Dictionary<string, IntrinsicObjectInfo> Build()
        {
            var dict = new Dictionary<string, IntrinsicObjectInfo>(StringComparer.Ordinal);
            var asm = typeof(IntrinsicObjectAttribute).Assembly;
            foreach (var t in asm.GetTypes())
            {
                var attr = (IntrinsicObjectAttribute?)t.GetCustomAttributes(typeof(IntrinsicObjectAttribute), inherit: false).FirstOrDefault();
                if (attr != null)
                {
                    dict[attr.Name] = new IntrinsicObjectInfo(attr.Name, t, attr.CallKind);
                }
            }
            return dict;
        }
    }

    public sealed record IntrinsicObjectInfo(string Name, Type Type, IntrinsicCallKind CallKind);
}
