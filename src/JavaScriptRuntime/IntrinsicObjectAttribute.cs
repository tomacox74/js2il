using System;

namespace JavaScriptRuntime
{
    public enum IntrinsicCallKind
    {
        None = 0,
        ConstructorLike,
        BuiltInError,
        ArrayConstruct,
        ObjectConstruct,
        DateToString
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IntrinsicObjectAttribute : Attribute
    {
        public string Name { get; }
        public IntrinsicCallKind CallKind { get; }

        public IntrinsicObjectAttribute(string name, IntrinsicCallKind callKind = IntrinsicCallKind.None)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CallKind = callKind;
        }
    }
}
