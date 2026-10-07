namespace JavaScriptRuntime
{
    [IntrinsicObject("Boolean", IntrinsicCallKind.ConstructorLike)]
    public sealed class Boolean : JsObject
    {
        private static readonly BuiltinFunction0 _booleanPrototypeToStringValue = static thisArgument =>
        {
            var booleanValue = JavaScriptRuntime.Boolean.ThisBooleanValue(thisArgument);
            return booleanValue ? "true" : "false";
        };

        private static readonly BuiltinFunction0 _booleanPrototypeValueOfValue = static thisArgument =>
            JavaScriptRuntime.Boolean.ThisBooleanValue(thisArgument);

        internal static void ConfigureIntrinsicSurface(object constructorValue, object prototypeValue, object objectPrototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PrototypeChain.SetPrototype(prototypeValue, objectPrototype);
            PropertyDescriptorStore.DefineOrUpdate(constructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = prototypeValue
            });
            JavaScriptRuntime.Function.MarkConstructible(
                constructorValue);
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, ObjectRuntime.PrimitiveValuePropertyName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = false
            });
            PropertyDescriptorStore.DefineOrUpdate(prototypeValue, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = constructorValue
            });
            GlobalThis.DefineBuiltinFunctionProperty(prototypeValue, "toString", _booleanPrototypeToStringValue, 0d);
            GlobalThis.DefineBuiltinFunctionProperty(prototypeValue, "valueOf", _booleanPrototypeValueOfValue, 0d);
            GlobalThis.DefineIntrinsicDataProperty(prototypeValue, global::JavaScriptRuntime.Symbol.toStringTag.DebugId, "Boolean");

            GlobalThis.ConfigureBuiltinFunctionObject(constructorValue);
        }

        private readonly bool _value;

        public Boolean()
        {
            _value = false;
            PrototypeChain.InitializePrototype(this, GlobalThis.BooleanPrototypeValue);
        }

        public Boolean(object? value)
        {
            _value = TypeUtilities.ToBoolean(value);
            PrototypeChain.InitializePrototype(this, GlobalThis.BooleanPrototypeValue);
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

            if (value is not null
                && PropertyDescriptorStore.TryGetOwn(
                    value,
                    ObjectRuntime.PrimitiveValuePropertyName,
                    out var descriptor)
                && descriptor.Kind == JsPropertyDescriptorKind.Data
                && descriptor.Value is bool wrapped)
            {
                return wrapped;
            }

            throw new TypeError("Boolean.prototype method called on incompatible receiver");
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
