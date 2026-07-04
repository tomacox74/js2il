namespace JavaScriptRuntime
{
    public static class Atomics
    {
        public static string wait(object? typedArray, object? index, object? value)
            => wait(typedArray, index, value, 0d);

        public static string wait(object? typedArray, object? index, object? value, object? timeout)
        {
            if (typedArray is not Int32Array int32Array)
            {
                throw new TypeError("Atomics.wait requires an Int32Array");
            }

            if (int32Array.buffer is not SharedArrayBuffer)
            {
                throw new TypeError("Atomics.wait requires a SharedArrayBuffer-backed Int32Array");
            }

            var elementIndex = TypeUtilities.ToInt32(index);
            if (elementIndex < 0 || elementIndex >= (int)int32Array.length)
            {
                throw new RangeError("Invalid atomic index");
            }

            var expectedValue = TypeUtilities.ToInt32(value);
            var actualValue = TypeUtilities.ToInt32(int32Array[(double)elementIndex]);
            if (actualValue != expectedValue)
            {
                return "not-equal";
            }

            var timeoutMs = NormalizeTimeout(timeout);
            if (timeoutMs > 0)
            {
                global::System.Threading.Thread.Sleep(timeoutMs);
            }

            return "timed-out";
        }

        private static int NormalizeTimeout(object? timeout)
        {
            if (timeout is null || timeout is JsNull)
            {
                return 0;
            }

            var number = TypeUtilities.ToNumber(timeout);
            if (double.IsNaN(number) || number <= 0)
            {
                return 0;
            }

            if (double.IsPositiveInfinity(number) || number > int.MaxValue)
            {
                return int.MaxValue;
            }

            return (int)global::System.Math.Truncate(number);
        }
    }
}
