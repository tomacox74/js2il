using System;

namespace JavaScriptRuntime.Node
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NodeModuleAttribute : Attribute
    {
        public string Name { get; }

        public NodeModuleAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Node module name must be a non-empty string.", nameof(name));
            Name = name;
        }
    }
}
