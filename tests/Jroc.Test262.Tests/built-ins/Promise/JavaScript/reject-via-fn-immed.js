// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Rejecting through immediate invocation of the provided resolving function
es6id: 25.4.3.1
info: |
    [...]
    9. Let completion be Call(executor, undefined,
       «resolvingFunctions.[[Resolve]], resolvingFunctions.[[Reject]]»).
    10. If completion is an abrupt completion, then
        [...]
    11. Return promise.

    25.4.1.3.1 Promise Reject Functions
    [...]
    6. Return RejectPromise(promise, reason).
flags: [async]
---*/


function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
function $ERROR(message) { throw new Test262Error(message); }
function $DONE(error) { if (error) { throw error; } }
function __test262SameValue(actual, expected) { return Object.is(actual, expected); }
function __test262FormatMessage(message, fallback) { return message || fallback || 'Assertion failed'; }
function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message)); }
}
assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected SameValue')); }
};
assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected values to differ')); }
};
assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;
assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try { fn(); } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected function to throw')); }
};
function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) { return false; }
    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) { return false; }
    }
    return true;
}
assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected arrays to match')); }
};
function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) { passed = __test262SameValue(actual.value, desc.value); }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) { passed = actual.writable === desc.writable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) { passed = actual.enumerable === desc.enumerable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) { passed = actual.configurable === desc.configurable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) { passed = actual.get === desc.get; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) { passed = actual.set === desc.set; }
    console.log(passed);
    if (!passed) { throw new Error('verifyProperty failed for ' + String(name)); }
}
function verifyNotWritable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.writable === false;
    console.log(passed);
    if (!passed) { throw new Error('verifyNotWritable failed for ' + String(name)); }
}
function verifyWritable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.writable === true;
    console.log(passed);
    if (!passed) { throw new Error('verifyWritable failed for ' + String(name)); }
}
function verifyNotEnumerable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.enumerable === false;
    console.log(passed);
    if (!passed) { throw new Error('verifyNotEnumerable failed for ' + String(name)); }
}
function verifyEnumerable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.enumerable === true;
    console.log(passed);
    if (!passed) { throw new Error('verifyEnumerable failed for ' + String(name)); }
}
function verifyConfigurable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.configurable === true;
    console.log(passed);
    if (!passed) { throw new Error('verifyConfigurable failed for ' + String(name)); }
}
function verifyNotConfigurable(object, name) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual && actual.configurable === false;
    console.log(passed);
    if (!passed) { throw new Error('verifyNotConfigurable failed for ' + String(name)); }
}
function getWellKnownIntrinsicObject(name) {
    if (name === '%AsyncFunction%') { return async function() {}.constructor; }
    if (name === '%GeneratorFunction%') { return function*() {}.constructor; }
    throw new Error('Unsupported intrinsic ' + name);
}
function isConstructor(argument) {
    if (argument === getWellKnownIntrinsicObject('%AsyncFunction%')) { return true; }
    if (argument === getWellKnownIntrinsicObject('%GeneratorFunction%')) { return true; }
    if (argument === Promise) { return true; }
    return false;
}
function asyncTest(testFunc) { testFunc().then($DONE, $DONE); }

var thenable = new Promise(function() {});
var returnValue = null;
var p = new Promise(function(_, reject) {
  returnValue = reject(thenable);
});

assert.sameValue(returnValue, undefined, '"reject" function return value');

p.then(function() {
  $DONE('The promise should not be fulfilled.');
}, function(x) {
  if (x !== thenable) {
    $DONE('The promise should be rejected with the resolution value.');
    return;
  }

  $DONE();
});
