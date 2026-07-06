/*---
description: Native C# test262 harness helpers are available as host globals
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

assert.throws(Test262Error, function() {
    $ERROR('native error');
}, '$ERROR should throw Test262Error');

var constructed = new Test262Error('constructed error');
assert.sameValue(constructed.name, 'Test262Error');
assert.sameValue(constructed.message, 'constructed error');

assert.throws(Test262Error, function() {
    $262.detachArrayBuffer();
}, 'unsupported $262 APIs should fail clearly');
