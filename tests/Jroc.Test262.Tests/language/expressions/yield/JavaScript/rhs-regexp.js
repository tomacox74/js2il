// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-generator-function-definitions
es6id: 14.4
description: >
  YieldExpression accepts a regular expression literal as its right-hand side
info: |
  The syntactic context immediately following yield requires use of the
  InputElementRegExpOrTemplateTail lexical goal.
features: [generators]
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

var complete = false;
var sent = {};
var iter, iterResult, received;
// Unused variables declared to improve error messages in incorrect parsing
// scenarios.
var abc, i;
function* g() {
  received = yield/abc/i;
  complete = true;
}

iter = g();

assert.sameValue(complete, false, 'generator initially paused');
assert.sameValue(received, undefined, 'first statement no executed');

iterResult = iter.next();

assert.sameValue(complete, false, 'generator paused following expression');
assert.sameValue(received, undefined, 'first statement not executed');

assert.sameValue(iterResult.done, false, 'iteration not complete');
assert.sameValue(iterResult.value.test('ABC'), true, 'first iterated value');

iterResult = iter.next(sent);

assert.sameValue(received, sent, 'YieldExpression value');
assert.sameValue(complete, true, 'generator correctly re-started');
assert.sameValue(iterResult.done, true, 'iteration complete');
assert.sameValue(iterResult.value, undefined, 'second iterated value');
