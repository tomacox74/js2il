namespace JavaScriptRuntime
{
    public sealed class ClassConstructorValue
    {
        public ClassConstructorValue(Type type, object[] scopes, int formalParameterCount = 0)
        {
            Type = type;
            Scopes = scopes;
            FormalParameterCount = formalParameterCount;
            PrototypeChain.SetPrototype(this, Function.Prototype);
        }

        public Type Type { get; }

        public object[] Scopes { get; }

        public int FormalParameterCount { get; }
    }
}
