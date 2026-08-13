/*---
description: Native C# test262 harness helpers are available as host globals
includes: [propertyHelper.js, testTypedArray.js, testAtomics.js, tcoHelper.js, decimalToHexString.js, nans.js, promiseHelper.js]
---*/

assert(true, 'assert should be callable');
assert.sameValue(1, 1, 'sameValue should use SameValue semantics');
assert.notSameValue(1, 2, 'notSameValue should use SameValue semantics');
assert.strictEqual('abc', 'abc', 'strictEqual should alias sameValue');
assert.notStrictEqual('abc', 'def', 'notStrictEqual should alias notSameValue');
assert.compareArray([1, 2, 3], [1, 2, 3], 'compareArray should compare indexed elements');

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

assert.throws(Test262Error, function() {
    $262.detachArrayBuffer();
}, 'unsupported $262 APIs should fail clearly');
