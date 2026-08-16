using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime
{
    public static class IntrinsicObjectRegistry
    {
        private static readonly FrozenDictionary<string, IntrinsicObjectInfo> ByName =
            Build();

        public static Type? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return ByName.TryGetValue(name, out var info) ? info.Type : null;
        }

        public static IntrinsicObjectInfo? GetInfo(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return ByName.TryGetValue(name, out var info) ? info : null;
        }

        public static IReadOnlyCollection<IntrinsicObjectInfo> GetAll()
        {
            return ByName.Values;
        }

        private static FrozenDictionary<string, IntrinsicObjectInfo> Build()
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
            return dict.ToFrozenDictionary(StringComparer.Ordinal);
        }
    }

    public sealed record IntrinsicObjectInfo(string Name, Type Type, IntrinsicCallKind CallKind);
}
