using JavaScriptRuntime;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

internal static class Test262AsyncHelpers
{
    internal static object ThrowsAsync(object? expectedConstructor, object? callback, object? message)
        => new Promise(CreateFunction((Action<object?, object?>)((resolve, _) =>
        {
            void Fail(string detail)
            {
                var error = CreateTest262Error(message is null
                    ? detail
                    : DotNet2JSConversions.ToString(message) + " " + detail);
                var constructor = ObjectRuntime.GetItem(GlobalThis.globalThis, "Test262Error");
                JavaScriptRuntime.Object.setPrototypeOf(error, ObjectRuntime.GetItem(constructor!, "prototype"));
                throw error;
            }

            if (!CallableOperations.IsCallable(expectedConstructor))
            {
                Fail("assert.throwsAsync called with an argument that is not an error constructor");
            }
            if (!CallableOperations.IsCallable(callback))
            {
                Fail("assert.throwsAsync called with an argument that is not a function");
            }

            var expectedName = ObjectRuntime.GetItem(expectedConstructor!, "name");
            var expectation = $"Expected a {ToMessage(expectedName)} to be thrown asynchronously";
            object? result = null;
            try
            {
                result = Invoke(callback);
            }
            catch (Exception)
            {
                Fail(expectation + " but the function threw synchronously");
            }
            if (!IsObject(result) || !CallableOperations.IsCallable(ObjectRuntime.GetItem(result!, "then")))
            {
                Fail(expectation + " but result was not a thenable");
            }

            object? onFulfilled = null;
            object? onRejected = null;
            var settlement = new Promise(CreateFunction(
                (Action<object?, object?>)((fulfill, reject) =>
                {
                    onFulfilled = fulfill;
                    onRejected = reject;
                }), "", 2));
            try
            {
                ObjectRuntime.CallMember(result!, "then", new object[] { onFulfilled!, onRejected! });
            }
            catch (Exception)
            {
                Fail(expectation + " but .then threw synchronously");
            }
            var checkedSettlement = settlement.then(
                CreateFunction((Action)(() => Fail(expectation + " but no exception was thrown at all")), "", 0),
                CreateFunction((Action<object?>)(thrown =>
                {
                    if (!IsObject(thrown))
                    {
                        Fail(expectation + " but thrown value was not an object");
                    }
                    var actualConstructor = ObjectRuntime.GetItem(thrown!, "constructor");
                    if (!JavaScriptRuntime.Object.@is(actualConstructor, expectedConstructor))
                    {
                        var actualName = ObjectRuntime.GetItem(actualConstructor!, "name");
                        Fail(JavaScriptRuntime.Object.@is(expectedName, actualName)
                            ? expectation + " but got a different error constructor with the same name"
                            : expectation + " but got a " + ToMessage(actualName));
                    }
                }), "", 1));
            Invoke(resolve, checkedSettlement);
        }), "", 2));

    private static bool IsObject(object? value)
        => value is not null and not JsNull && TypeUtilities.Typeof(value) == "object";
}
