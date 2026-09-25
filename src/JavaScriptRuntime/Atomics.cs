using System;
using System.Numerics;

namespace JavaScriptRuntime
{
    public static class Atomics
    {
        private enum Operation { Add, And, CompareExchange, Exchange, Load, Or, Store, Sub, Xor }

        public static object add(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Add);

        public static object and(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.And);

        public static object compareExchange(object? array, object? index, object? expected, object? replacement)
            => Apply(array, index, expected, replacement, Operation.CompareExchange);

        public static object exchange(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Exchange);

        public static object load(object? array, object? index)
            => Apply(array, index, null, null, Operation.Load);

        public static object or(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Or);

        public static object store(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Store);

        public static object sub(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Sub);

        public static object xor(object? array, object? index, object? value)
            => Apply(array, index, value, null, Operation.Xor);

        public static bool isLockFree(object? size)
        {
            var number = TypeUtilities.ToNumber(size);
            var integer = double.IsNaN(number) ? 0d : global::System.Math.Truncate(number);
            return integer is 1d or 2d or 4d or 8d;
        }

        public static object? pause(object? hint)
        {
            if (hint is not null
                && (hint is not double and not int
                    || hint is double number
                    && (!double.IsFinite(number) || global::System.Math.Truncate(number) != number)))
            {
                throw new TypeError("Atomics.pause hint must be a Number");
            }

            return null;
        }

        private static object Apply(object? array, object? index, object? value, object? replacement, Operation operation)
        {
            var view = array as TypedArrayBase;
            if (view is not (Int8Array or Uint8Array or Int16Array or Uint16Array or Int32Array
                or Uint32Array or BigInt64Array or BigUint64Array) || view.buffer.IsDetached)
            {
                throw new TypeError("Atomics requires an integer typed array");
            }

            var numericIndex = TypeUtilities.ToNumber(index);
            var integerIndex = double.IsNaN(numericIndex) ? 0d : global::System.Math.Truncate(numericIndex);
            if (integerIndex < 0 || integerIndex >= view.length)
            {
                throw new RangeError("Invalid atomic index");
            }

            var elementIndex = (int)integerIndex;
            var isBigInt = view is BigInt64Array or BigUint64Array;
            if (operation == Operation.Load)
            {
                lock (SynchronizationRoot(view))
                {
                    return view.ReadElementObject(elementIndex)!;
                }
            }

            var operand = ConvertOperand(value, isBigInt);
            var nextOperand = operation == Operation.CompareExchange
                ? ConvertOperand(replacement, isBigInt)
                : null;
            lock (SynchronizationRoot(view))
            {
                var previous = view.ReadElementObject(elementIndex)!;
                if (operation == Operation.Store)
                {
                    view.WriteElementObject(elementIndex, operand);
                    return operand;
                }

                if (operation == Operation.CompareExchange)
                {
                    if (Equals(previous, NormalizeOperand(view, operand)))
                    {
                        view.WriteElementObject(elementIndex, nextOperand);
                    }
                    return previous;
                }

                if (operation == Operation.Exchange)
                {
                    view.WriteElementObject(elementIndex, operand);
                    return previous;
                }

                object result = isBigInt
                    ? ModifyBigInt((BigInteger)previous, (BigInteger)operand, operation)
                    : ModifyNumber((double)previous, (double)operand, operation);
                view.WriteElementObject(elementIndex, result);
                return previous;
            }
        }

        private static object ConvertOperand(object? value, bool bigInt)
        {
            if (bigInt)
            {
                return BigInt.ToBigIntForTypedArray(value);
            }

            var number = TypeUtilities.ToNumber(value);
            return double.IsNaN(number) || number == 0d
                ? 0d
                : global::System.Math.Truncate(number);
        }

        private static object SynchronizationRoot(TypedArrayBase view)
            => view.buffer is SharedArrayBuffer shared ? shared.BackingStore : view.buffer;

        private static object NormalizeOperand(TypedArrayBase view, object operand)
        {
            if (operand is BigInteger bigInt)
            {
                var bits = BigInt.AsUintN(64d, bigInt);
                return view is BigInt64Array ? BigInt.AsIntN(64d, bits) : bits;
            }

            var number = (double)operand;
            var bits32 = TypeUtilities.ToUint32(number);
            return view switch
            {
                Int8Array => (double)unchecked((sbyte)bits32),
                Uint8Array => (double)(byte)bits32,
                Int16Array => (double)unchecked((short)bits32),
                Uint16Array => (double)(ushort)bits32,
                Int32Array => (double)unchecked((int)bits32),
                _ => (double)bits32
            };
        }

        private static BigInteger ModifyBigInt(BigInteger old, BigInteger value, Operation operation)
            => operation switch
            {
                Operation.Add => old + value,
                Operation.And => old & value,
                Operation.Or => old | value,
                Operation.Sub => old - value,
                Operation.Xor => old ^ value,
                _ => throw new InvalidOperationException()
            };

        private static double ModifyNumber(double old, double value, Operation operation)
            => operation switch
            {
                Operation.Add => old + value,
                Operation.And => TypeUtilities.ToInt32(old) & TypeUtilities.ToInt32(value),
                Operation.Or => TypeUtilities.ToInt32(old) | TypeUtilities.ToInt32(value),
                Operation.Sub => old - value,
                Operation.Xor => TypeUtilities.ToInt32(old) ^ TypeUtilities.ToInt32(value),
                _ => throw new InvalidOperationException()
            };

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

            var elementIndexNumber = TypeUtilities.ToNumber(index);
            if (double.IsInfinity(elementIndexNumber))
            {
                throw new RangeError("Invalid atomic index");
            }

            var elementIndexInteger = double.IsNaN(elementIndexNumber)
                ? 0d
                : global::System.Math.Truncate(elementIndexNumber);
            if (elementIndexInteger < 0
                || elementIndexInteger > int.MaxValue
                || elementIndexInteger >= int32Array.length)
            {
                throw new RangeError("Invalid atomic index");
            }

            var elementIndex = (int)elementIndexInteger;
            var expectedValue = TypeUtilities.ToInt32(value);
            var timeoutMs = NormalizeTimeout(timeout);
            var context = RuntimeExecutionContext.CurrentOrOverride;
            var backingStore = ((SharedArrayBuffer)int32Array.buffer).BackingStore;
            if (context != null
                && ReferenceEquals(
                    backingStore.Owner,
                    context.Agent.Cluster.SharedServices))
            {
                return context.Agent.Cluster.SharedServices.Atomics.Wait(
                    context.Agent,
                    backingStore,
                    checked((int)int32Array.byteOffset + (elementIndex * sizeof(int))),
                    expectedValue,
                    timeoutMs) switch
                {
                    RuntimeAtomicsWaitResult.NotEqual => "not-equal",
                    RuntimeAtomicsWaitResult.Notified => "ok",
                    _ => "timed-out"
                };
            }

            var actualValue = TypeUtilities.ToInt32(int32Array[(double)elementIndex]);
            if (actualValue != expectedValue)
            {
                return "not-equal";
            }

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
