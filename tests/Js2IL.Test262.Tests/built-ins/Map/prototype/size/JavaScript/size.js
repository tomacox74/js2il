function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
function __test262SameValue(actual, expected) { return Object.is(actual, expected); }
function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) { throw new Error(message || 'Assertion failed'); }
}
assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(message || 'Expected SameValue'); }
};
assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) { throw new Error(message || 'Expected values to differ'); }
};
assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try { fn(); } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) { throw new Error(message || 'Expected function to throw'); }
};
assert.compareArray = function(actual, expected, message) {
    var passed = !!actual && !!expected && actual.length === expected.length;
    if (passed) { for (var i = 0; i < actual.length; i++) { if (!__test262SameValue(actual[i], expected[i])) { passed = false; break; } } }
    console.log(passed);
    if (!passed) { throw new Error(message || 'Expected arrays to match'); }
};
function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) { passed = __test262SameValue(actual.value, desc.value); }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) { passed = actual.writable === desc.writable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) { passed = actual.enumerable === desc.enumerable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) { passed = actual.configurable === desc.configurable; }
    console.log(passed);
    if (!passed) { throw new Error('verifyProperty failed for ' + String(name)); }
}
function verifyNotEnumerable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.enumerable === false;
    console.log(passed);
    if (!passed) { throw new Error('verifyNotEnumerable failed for ' + String(name)); }
}
function verifyConfigurable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.configurable === true;
    console.log(passed);
    if (!passed) { throw new Error('verifyConfigurable failed for ' + String(name)); }
}
// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-get-map.prototype.size
description: >
  Property type and descriptor.
info: |
  get Map.prototype.size

  17 ECMAScript Standard Built-in Objects

includes: [propertyHelper.js]
---*/

var descriptor = Object.getOwnPropertyDescriptor(Map.prototype, 'size');

assert.sameValue(
  typeof descriptor.get,
  'function',
  'typeof descriptor.get is function'
);
assert.sameValue(
  typeof descriptor.set,
  'undefined',
  'typeof descriptor.set is undefined'
);

verifyNotEnumerable(Map.prototype, 'size');
verifyConfigurable(Map.prototype, 'size');

