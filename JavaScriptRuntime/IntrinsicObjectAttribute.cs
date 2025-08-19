using System;

namespace JavaScriptRuntime
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IntrinsicObjectAttribute : Attribute
    {
        public string Name { get; }
        public IntrinsicObjectAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
