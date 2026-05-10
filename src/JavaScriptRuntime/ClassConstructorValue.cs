namespace JavaScriptRuntime
{
    public sealed class ClassConstructorValue
    {
        public ClassConstructorValue(Type type, object[] scopes)
        {
            Type = type;
            Scopes = scopes;
        }

        public Type Type { get; }

        public object[] Scopes { get; }
    }
}
