namespace JavaScriptRuntime
{
    [IntrinsicObject("Boolean", IntrinsicCallKind.ConstructorLike)]
    public sealed class Boolean
    {
        private readonly bool _value;

        public Boolean()
        {
            _value = false;
            PrototypeChain.SetPrototype(this, GlobalThis.BooleanPrototypeValue);
        }

        public Boolean(object? value)
        {
            _value = TypeUtilities.ToBoolean(value);
            PrototypeChain.SetPrototype(this, GlobalThis.BooleanPrototypeValue);
        }

        public string toString()
        {
            return _value ? "true" : "false";
        }

        public bool valueOf()
        {
            return _value;
        }

        internal static bool ThisBooleanValue(object? value)
        {
            if (value is bool primitive)
            {
                return primitive;
            }

            if (value is JavaScriptRuntime.Boolean wrapper)
            {
                return wrapper._value;
            }

            throw new TypeError("Boolean.prototype method called on incompatible receiver");
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
