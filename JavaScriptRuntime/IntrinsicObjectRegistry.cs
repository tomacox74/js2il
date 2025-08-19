using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime
{
    public static class IntrinsicObjectRegistry
    {
        private static readonly object _sync = new();
        private static volatile Dictionary<string, Type>? _byName;

        public static Type? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
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
            return map.TryGetValue(name, out var t) ? t : null;
        }

        private static Dictionary<string, Type> Build()
        {
            var dict = new Dictionary<string, Type>(StringComparer.Ordinal);
            var asm = typeof(IntrinsicObjectAttribute).Assembly;
            foreach (var t in asm.GetTypes())
            {
                var attr = (IntrinsicObjectAttribute?)t.GetCustomAttributes(typeof(IntrinsicObjectAttribute), inherit: false).FirstOrDefault();
                if (attr != null)
                {
                    dict[attr.Name] = t;
                }
            }
            return dict;
        }
    }
}
