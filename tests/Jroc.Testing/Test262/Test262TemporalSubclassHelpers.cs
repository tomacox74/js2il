using JavaScriptRuntime;
using System.Numerics;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// Native equivalents of the subclass matrices in the pinned temporalHelpers.js.
internal static partial class Test262TemporalHelpers
{
    private static void RegisterSubclassHelpers(JsObject helpers)
    {
        string[] instanceChecks =
        [
            "checkSubclassConstructorNotObject", "checkSubclassConstructorUndefined",
            "checkSubclassConstructorThrows", "checkSubclassConstructorNotCalled",
            "checkSubclassSpeciesInvalidResult", "checkSubclassSpeciesNotAConstructor",
            "checkSubclassSpeciesNull", "checkSubclassSpeciesUndefined", "checkSubclassSpeciesThrows"
        ];
        string[] staticChecks =
            ["checkStaticInvalidReceiver", "checkStaticReceiverNotCalled", "checkThisValueNotCalled"];
        foreach (var name in instanceChecks)
        {
            ObjectRuntime.SetItem(helpers, name, CreateFunction(
                (BuiltinFunctionVariadic)((object? _, in JsCallArguments args) =>
                {
                    CheckSubclass(name, args.ToArray());
                    return null;
                }), name, 5));
        }
        foreach (var name in staticChecks)
        {
            ObjectRuntime.SetItem(helpers, name, CreateFunction(
                (BuiltinFunctionVariadic)((object? _, in JsCallArguments args) =>
                {
                    CheckStaticSubclass(name, args.ToArray());
                    return null;
                }), name, 4));
        }
        ObjectRuntime.SetItem(helpers, "checkSubclassingIgnored", CreateFunction(
            (BuiltinFunctionVariadic)((object? receiver, in JsCallArguments args) =>
            {
                foreach (var name in instanceChecks)
                {
                    ObjectRuntime.CallMember(receiver!, name, args.ToArray()!);
                }
                return null;
            }), "checkSubclassingIgnored", 0));
        ObjectRuntime.SetItem(helpers, "checkSubclassingIgnoredStatic", CreateFunction(
            (BuiltinFunctionVariadic)((object? receiver, in JsCallArguments args) =>
            {
                foreach (var name in staticChecks)
                {
                    ObjectRuntime.CallMember(receiver!, name, args.ToArray()!);
                }
                return null;
            }), "checkSubclassingIgnoredStatic", 0));
    }

    private static object?[] SpreadArguments(object? iterable)
    {
        var iterator = CallableOperations.Call(ObjectRuntime.GetItem(iterable!, Symbol.iterator), iterable, []);
        var next = ObjectRuntime.GetItem(iterator!, "next");
        var values = new List<object?>();
        while (true)
        {
            var result = CallableOperations.Call(next, iterator, []);
            if (TypeUtilities.IsPrimitive(result))
            {
                throw new TypeError("Iterator result must be an object");
            }
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(result!, "done")))
            {
                return values.ToArray();
            }
            values.Add(ObjectRuntime.GetItem(result!, "value"));
        }
    }

    private static (object? Value, string Description)[] SubclassValues(bool includeUndefinedAndObject)
    {
        (object?, string)[] primitives =
        [
            (JsNull.Null, "null"), (true, "true"), ("test", "string"),
            (new Symbol(), "Symbol"), (7d, "number"), (new BigInteger(7), "bigint")
        ];
        return includeUndefinedAndObject
            ? [(null, "undefined"), .. primitives, (PlainObject(), "plain object")]
            : primitives;
    }

    private static object ThrowingSubclassGetter()
        => CreateFunction((Func<object?>)(() =>
            throw CreateTest262Error("Subclass constructor/species must not be read")), "get", 0);

    private static void DefineThrowingSubclassProperty(object target, object key)
    {
        var descriptor = new JsObject();
        ObjectRuntime.SetItem(descriptor, "get", ThrowingSubclassGetter());
        JavaScriptRuntime.Object.defineProperty(target, key, descriptor);
    }

    private static object SpeciesHolder(object? species)
    {
        var holder = PlainObject();
        ObjectRuntime.SetItem(holder, Symbol.species, species);
        return holder;
    }

    private static void AssertSubclassResult(object? result, object? construct,
        object? assertions, string description = "", bool callAssertions = true)
    {
        Assert.strictEqual(JavaScriptRuntime.Object.getPrototypeOf(result!),
            ObjectRuntime.GetItem(construct!, "prototype"), description);
        if (callAssertions)
        {
            Call(assertions, result);
        }
    }

    private static void CheckSubclass(string check, object?[] args)
    {
        var construct = args.ElementAtOrDefault(0);
        var constructArgs = args.ElementAtOrDefault(1);
        var method = StringValue(args.ElementAtOrDefault(2));
        var methodArgs = args.ElementAtOrDefault(3);
        var assertions = args.ElementAtOrDefault(4);
        object Instance() => CallableOperations.Construct(construct, SpreadArguments(constructArgs))!;
        object? Run(object instance)
        {
            var callable = ObjectRuntime.GetItem(instance, method);
            return CallableOperations.Call(callable, instance, SpreadArguments(methodArgs));
        }
        if (check is "checkSubclassConstructorNotObject"
            or "checkSubclassSpeciesInvalidResult" or "checkSubclassSpeciesNotAConstructor")
        {
            var values = SubclassValues(check == "checkSubclassSpeciesInvalidResult");
            if (check == "checkSubclassSpeciesNotAConstructor")
            {
                values = [.. values.Where(entry => entry.Value is not JsNull), (PlainObject(), "plain object")];
            }
            foreach (var (value, description) in values)
            {
                var instance = Instance();
                if (check == "checkSubclassConstructorNotObject")
                {
                    ObjectRuntime.SetItem(instance, "constructor", value);
                }
                else
                {
                    object? species = value;
                    if (check == "checkSubclassSpeciesInvalidResult")
                    {
                        var function = CreateFunction((Func<object?>)(() => value), "", 0);
                        Function.MarkConstructible(function);
                        species = function;
                    }
                    ObjectRuntime.SetItem(instance, "constructor", SpeciesHolder(species));
                }
                AssertSubclassResult(Run(instance), construct, assertions, description);
            }
            return;
        }
        if (check is "checkSubclassConstructorThrows" or "checkSubclassSpeciesThrows")
        {
            var instance = Instance();
            if (check == "checkSubclassConstructorThrows")
            {
                DefineThrowingSubclassProperty(instance, "constructor");
            }
            else
            {
                var constructor = PlainObject();
                DefineThrowingSubclassProperty(constructor, Symbol.species);
                ObjectRuntime.SetItem(instance, "constructor", constructor);
            }
            AssertSubclassResult(Run(instance), construct, assertions,
                callAssertions: check != "checkSubclassSpeciesThrows");
            return;
        }
        var subclass = new TemporalHelperSubclass(construct, constructArgs);
        var subclassInstance = CallableOperations.Construct(subclass, [])!;
        Assert.strictEqual((double)subclass.Called, 1d);
        var prototype = ObjectRuntime.GetItem(subclass, "prototype")!;
        if (check == "checkSubclassConstructorUndefined")
        {
            ObjectRuntime.SetItem(prototype, "constructor", null);
        }
        else if (check is "checkSubclassSpeciesNull" or "checkSubclassSpeciesUndefined")
        {
            ObjectRuntime.SetItem(prototype, "constructor",
                SpeciesHolder(check == "checkSubclassSpeciesNull" ? JsNull.Null : null));
        }
        var result = Run(subclassInstance);
        Assert.strictEqual((double)subclass.Called, 1d);
        AssertSubclassResult(result, construct, assertions);
    }

    private static void CheckStaticSubclass(string check, object?[] args)
    {
        var construct = args.ElementAtOrDefault(0);
        var method = StringValue(args.ElementAtOrDefault(1));
        var methodArgs = args.ElementAtOrDefault(2);
        var assertions = args.ElementAtOrDefault(3);
        if (check == "checkThisValueNotCalled")
        {
            var subclass = new TemporalHelperSubclass(construct, null);
            var callable = ObjectRuntime.GetItem(subclass, method);
            var result = CallableOperations.Call(callable, subclass, SpreadArguments(methodArgs));
            Assert.strictEqual((double)subclass.Called, 0d);
            AssertSubclassResult(result, construct, assertions);
            return;
        }
        foreach (var (value, _) in SubclassValues(true))
        {
            var receiver = check == "checkStaticInvalidReceiver" ? value
                : CreateFunction((Func<object?>)(() => value), "receiver", 0);
            if (check == "checkStaticReceiverNotCalled")
            {
                Function.MarkConstructible(receiver!);
            }
            var methodValue = ObjectRuntime.GetItem(construct!, method);
            // Upstream uses .apply(), which consumes an array-like argument list.
            var result = ObjectRuntime.CallMember(methodValue!, "apply", [receiver!, methodArgs!]);
            AssertSubclassResult(result, construct, assertions);
        }
    }

    private sealed class TemporalHelperSubclass : JsFunctionObject
    {
        private readonly object? _baseConstructor;
        private readonly object? _constructArgs;
        internal int Called { get; private set; }
        public override bool IsConstructor => true;

        internal TemporalHelperSubclass(object? baseConstructor, object? constructArgs)
        {
            _baseConstructor = baseConstructor;
            _constructArgs = constructArgs;
            Function.InitializeFunctionInstance(this, 0d, "MySubclass", false);
            JavaScriptRuntime.Object.setPrototypeOf(this, baseConstructor!);
            var prototype = PlainObject();
            JavaScriptRuntime.Object.setPrototypeOf(prototype, ObjectRuntime.GetItem(baseConstructor!, "prototype")!);
            PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = this,
                Writable = true,
                Configurable = true,
                Enumerable = false
            });
            PropertyDescriptorStore.DefineOrUpdate(this, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = prototype,
                Writable = false,
                Configurable = false,
                Enumerable = false
            });
        }

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
            => throw new TypeError("Class constructor requires new");

        protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        {
            Called++;
            var forwarded = _constructArgs is null ? arguments.ToArray() : SpreadArguments(_constructArgs);
            return CallableOperations.Construct(_baseConstructor, forwarded, newTarget ?? this);
        }
    }
}
