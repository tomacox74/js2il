/*---
description: Native C# test262 harness helpers are available as host globals
includes: [propertyHelper.js, testTypedArray.js, testAtomics.js, tcoHelper.js, decimalToHexString.js, nans.js, promiseHelper.js, compareIterator.js, regExpUtils.js, detachArrayBuffer.js, proxyTrapsHelper.js, byteConversionValues.js, deepEqual.js]
---*/

assert(true, 'assert should be callable');
assert.sameValue(1, 1, 'sameValue should use SameValue semantics');
assert.notSameValue(1, 2, 'notSameValue should use SameValue semantics');
assert.strictEqual('abc', 'abc', 'strictEqual should alias sameValue');
assert.notStrictEqual('abc', 'def', 'notStrictEqual should alias notSameValue');
assert.compareArray([1, 2, 3], [1, 2, 3], 'compareArray should compare indexed elements');
assert.deepEqual([[1, 2], [3, undefined]], [[1, 2], [3, undefined]], 'deepEqual should compare nested arrays');

var expectedMatch = ['a'];
expectedMatch.index = 0;
expectedMatch.input = 'a';
var iteratorDone = false;
assert.compareIterator({
    next: function() {
        if (iteratorDone) {
            return { value: undefined, done: true };
        }
        iteratorDone = true;
        return { value: expectedMatch, done: false };
    }
}, [matchValidator(['a'], 0, 'a')], 'RegExp helper validators should be available');

var value = {};
Object.defineProperty(value, 'answer', {
    value: 42,
    writable: false,
    enumerable: true,
    configurable: false
});

verifyProperty(value, 'answer', {
    value: 42,
    writable: false,
    enumerable: true,
    configurable: false
});
verifyNotWritable(value, 'answer');
verifyEnumerable(value, 'answer');
verifyNotConfigurable(value, 'answer');
verifyEqualTo({ answer: 42 }, 'answer', 42);

assert.sameValue(decimalToHexString(255), '00FF');
assert.sameValue(decimalToPercentHexString(255), '%FF');
assert.sameValue($MAX_ITERATIONS, 100000);
assert.sameValue(NaNs.length, 9);
assert(NaNs.every(Number.isNaN));
assert(checkSequence([1, 2, 3]));
assert.sameValue(byteConversionValues.values.length, 56);
assert.sameValue(byteConversionValues.values[0], 127);
assert.sameValue(byteConversionValues.values[20], undefined);
assert.sameValue(byteConversionValues.values[38], Infinity);

var typedArrayRuns = 0;
testWithTypedArrayConstructors(function(TA, makeCtorArg) {
    var typed = new TA(makeCtorArg([1, 2]));
    assert.compareArray(typed, [1, 2]);
    typedArrayRuns++;
}, null, ['passthrough']);
assert.sameValue(typedArrayRuns, 9);

var outOfBoundsIndexRuns = 0;
testWithAtomicsOutOfBoundsIndices(function(indexGenerator) {
    indexGenerator({ length: 4 });
    outOfBoundsIndexRuns++;
});
assert.sameValue(outOfBoundsIndexRuns, 7);

var nonViewRuns = 0;
testWithAtomicsNonViewValues(function() {
    nonViewRuns++;
});
assert.sameValue(nonViewRuns, 24);

assert.throws(Test262Error, function() {
    $ERROR('native error');
}, '$ERROR should throw Test262Error');

var constructed = new Test262Error('constructed error');
assert.sameValue(constructed.name, 'Test262Error');
assert.sameValue(constructed.message, 'constructed error');

var detachableBuffer = new ArrayBuffer(4);
var detachableView = new Uint8Array(detachableBuffer);
$DETACHBUFFER(detachableBuffer);
assert.sameValue(detachableBuffer.detached, true);
assert.sameValue(detachableBuffer.byteLength, 0);
assert.sameValue(detachableView.length, 0);
assert.sameValue(detachableView[0], undefined);

var detachedDataViewBuffer = new ArrayBuffer(4);
var detachedDataView = new DataView(detachedDataViewBuffer, 0, 4);
$DETACHBUFFER(detachedDataViewBuffer);
assert.throws(RangeError, function() {
    detachedDataView.getInt8(Infinity);
});

var coercionDataViewBuffer = new ArrayBuffer(4);
var coercionDataView = new DataView(coercionDataViewBuffer, 0, 4);
assert.throws(TypeError, function() {
    coercionDataView.getInt8({
        valueOf: function() {
            $DETACHBUFFER(coercionDataViewBuffer);
            return 0;
        }
    });
});

var constructorBuffer = new ArrayBuffer(4);
assert.throws(TypeError, function() {
    new Uint8Array(constructorBuffer, 0, {
        valueOf: function() {
            $DETACHBUFFER(constructorBuffer);
            return 1;
        }
    });
});

var allowedProxyTrapCalls = 0;
var allowedProxyTraps = allowProxyTraps({
    get: function() {
        allowedProxyTrapCalls++;
        return 'allowed';
    }
});
assert.sameValue(allowedProxyTraps.get(), 'allowed');
assert.sameValue(allowedProxyTrapCalls, 1);
assert.throws(Test262Error, function() {
    allowedProxyTraps.set();
});
