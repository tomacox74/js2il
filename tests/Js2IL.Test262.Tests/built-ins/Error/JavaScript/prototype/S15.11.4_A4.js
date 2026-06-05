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
// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Since Error prototype object is not function it has no [[Construct]] method
es5id: 15.11.4_A4
description: Checking if creating "new Error.prototype" fails
---*/

assert.throws(TypeError, () => {
  new Error.prototype();
  throw new Test262Error();
});

